using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace PhoneDirectoryLibrary
{
    public class PhoneDirectory
    {
        public string DataFilePath;
        private const string CONNECTION_STRING = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

        public PhoneDirectory()
        {
            DataFilePath = Path.ChangeExtension(Path.Combine("C:\\Dev", "directory"), "json");
            this.LoadFromDB();
        }

        public string DataPath(string newDirectory)
        {
            Directory.CreateDirectory(newDirectory);
            DataFilePath = Path.ChangeExtension(Path.Combine(newDirectory, "directory"), "json");
            return DataFilePath;
        }

        public string DataPath()
        {
            return DataFilePath;
        }

        public int Count(SqlConnection connection)
        {
            string countCommandString = "SELECT COUNT(*) FROM Contact";
            var logger = NLog.LogManager.GetCurrentClassLogger();

            SqlCommand sqlCommand = new SqlCommand(countCommandString);
            try
            {
                return sqlCommand.ExecuteReader().GetInt32(0);
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
                return 0;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return 0;
            }            
        }

        public int Count()
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);

            using (connection)
            {
                connection.Open();
                return Count(connection);
            }
        }

        public void Add(IEnumerable<Contact> contacts, SqlConnection connection = null)
        {
            foreach (Contact contact in contacts)
            {
                Add(contact);
            }
        }

        /// <summary>
        /// Persist the contact with the rest of our data
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="connection"></param>
        public void Add(Contact contact, SqlConnection connection = null)
        {
            try
            {
                if (contact == null)
                {
                    throw new ArgumentNullException("Cannot add a null contact.");
                }

                // Insert into DB, using the existing DB connection if there is one
                if (connection == null)
                {
                    InsertContact(contact);
                }
                else
                {
                    InsertContact(contact, connection);
                }
            }
            catch (Exception e)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
            }
        }

        //Returns true if the item was deleted, false otherwise
        public bool Delete(Contact contact)
        {
            if(ContactExistsInDB(contact))
            {
                return DeleteContactFromDB(contact);
            }

            return false;
        }


        /// <summary>
        /// Replace an existing contact with an updated version in the collection
        /// Returns true if an update was made, false if object not found
        /// Also updates the instance in the DB
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public bool Update(Contact contact)
        {
            bool result = false;

            // Returns true if the update worked and false otherwise
            if (ContactExistsInDB(contact))
            {
                return UpdateInDB(contact);
            }

            return result;
        }

        // @TODO: Replace LINQ with calls to SearchWild and SearchExact
        /// <summary>
        /// Searchs for contacts matching the search term and returns all results
        /// </summary>
        /// <param name="type">The type of search to perform, based on an Enum</param>
        /// <param name="searchTerm">The string to search for, the meaning of which is based on the "type" parameter</param>
        /// <returns></returns>
        public List<Contact> Search(SearchType type, string searchTerm)
        {
            // If the search contained a wildcard, do a wildcard search
            if (searchTerm.Contains('*'))
            {
                return Search(type, searchTerm, true).ToList<Contact>();
            }
            // Otherwise search for the exact, case insensitive, value
            else
            {
                return Search(type, searchTerm, false).ToList<Contact>();
            }
        }

        /// <summary>
        /// Searchs the DB by the term specified on the field specified, using wildcards if specified
        /// </summary>
        /// <param name="searchType"></param>
        /// <param name="searchTerm"></param>
        /// <param name="wild"></param>
        /// <returns></returns>
        private IEnumerable<Contact> Search(SearchType searchType, string searchTerm, bool wild = false)
        {
            List<Contact> contacts = new List<Contact>();

            string searchQuery;

            if (wild)
            {
                searchQuery = @"
                    SELECT
	                c.Pid
                    FROM Contact as c 
                INNER JOIN DirectoryAddress as d on c.AddressID = d.Pid
                INNER JOIN StateLookup as s on d.StateCode = s.StateCode
                INNER JOIN Country as n on n.CountryCode = d.CountryCode
                WHERE [searchType] LIKE @searchTerm";

                searchTerm = searchTerm.Replace('*', '%');
            }
            else
            {
                searchQuery = @"
                    SELECT
	                c.Pid
                    FROM Contact as c 
                INNER JOIN DirectoryAddress as d on c.AddressID = d.Pid
                INNER JOIN StateLookup as s on d.StateCode = s.StateCode
                INNER JOIN Country as n on n.CountryCode = d.CountryCode
                WHERE [searchType] = @searchTerm";
            }

            switch (searchType)
            {
                case SearchType.firstName:
                    searchQuery = searchQuery.Replace("[searchType]", "c.FirstName");
                    break;
                case SearchType.lastName:
                    searchQuery = searchQuery.Replace("[searchType]", "c.LastName");
                    break;
                case SearchType.phone:
                    searchQuery = searchQuery.Replace("[searchType]", "c.Phone");
                    break;
                case SearchType.zip:
                    searchQuery = searchQuery.Replace("[searchType]", "d.Zip");
                    break;
                case SearchType.city:
                    searchQuery = searchQuery.Replace("[searchType]", "d.City");
                    break;
                default:
                    throw new InvalidSearchTermException($"Invalid search type: {searchType}");
            }

            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                SqlCommand searchCommand = new SqlCommand(searchQuery, connection);
                searchCommand.Parameters.AddWithValue("@searchTerm", searchTerm);

                var contactReader = searchCommand.ExecuteReader();

                List<Guid> contactIDs = new List<Guid>();

                using (contactReader)
                {
                    while (contactReader.Read())
                    {
                        contactIDs.Add(contactReader.GetGuid(0));

                    }
                }

                contacts.AddRange(GetContactsFromDB(contactIDs, connection));

                return contacts;
            }
        }

        public void Save()
        {
            var contacts = GetAll();

            string jsonData = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            File.WriteAllText(DataPath(), jsonData);
        }

        // @TODO: Either fix this or remove it
        //public void LoadFromText()
        //{
        //    string jsonData = File.ReadAllText(DataPath());
        //    contacts.Clear();
        //    foreach (Contact contact in JsonConvert.DeserializeObject<HashSet<Contact>>(jsonData))
        //    {
        //        contacts.Add(contact);
        //    }
        //}

        /// <summary>
        /// Returns a pretty printed string representing the specified contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public string Read(Contact contact)
        {
            string headers = "|";
            string columns = "|";
            var contactRow = contact.ToRow(MaxWidths(new List<Contact> { contact }));

            var contacts = GetAll();

            foreach (var header in contact.ToRow(MaxWidths(contacts)).Keys)
            {
                headers += header + '|';
            }

            headers += Environment.NewLine + new string('-', headers.Length) + Environment.NewLine;

            foreach (var value in contactRow.Values)
            {
                columns += value + '|';
            }

            columns += Environment.NewLine;

            return headers + columns;
        }

        /// <summary>
        /// Returns a pretty printed string representing the specified contacts
        /// </summary>
        /// <param name="contact">The contacts to print</param>
        /// <param name="addId">True: Add dynamic ids (not Pid) to output</param>
        /// <returns></returns>
        public string Read(IEnumerable<Contact> contacts, bool addId = false)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                string headers = "|";
                string idHeader = "Selection ID";

                if (addId)
                {
                    headers += $"{idHeader}|";
                }

                foreach (var header in contacts.First<Contact>().ToRow(MaxWidths(contacts)).Keys)
                {
                    headers += header + '|';
                }

                // Adds a newline and a border under the headers
                headers += Environment.NewLine + new string('-', headers.Length) + Environment.NewLine;

                string columns = "";

                int count = 1;

                foreach (Contact contact in contacts)
                {
                    columns += '|';

                    if (addId)
                    {
                        columns += $"{count++.ToString().PadRight(idHeader.Length)}|";
                    }

                    foreach (var value in contact.ToRow(MaxWidths(contacts)).Values)
                    {
                        columns += value + '|';
                    }

                    columns += Environment.NewLine;
                }

                return headers + columns;
            }
            catch (InvalidOperationException e)
            {
                if(e.Message == "Sequence contains no elements")
                {
                    Console.SetCursorPosition(1, Console.WindowHeight / 2);
                    return UserInterfaceFunctions.Center("There are no contacts stored.");
                }
                else
                {
                    logger.Error(e.Message);
                    return "There was an error.";
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                return "There was an error.";
            }
        }

        /// <summary>
        /// Returns a pretty-printed string representing all contacts in the collection on the console
        /// </summary>
        /// <returns></returns>
        public string Read(bool addId = false)
        {
            return Read(GetAll(), addId);
        }

        /// <summary>
        /// Returns a pretty-printed string representing all contacts in the collection on the console
        /// If a populated list is passed, we use that list instead
        /// </summary>
        /// <param name="addId"></param>
        /// <param name="contactList">The variable to save the generated list to</param>
        /// <returns></returns>
        public string Read(ref List<Contact> contactList, bool addId = false)
        {
            if (contactList.Count > 0)
            {
                return Read(contactList, addId);
            }
            else
            {
                contactList = GetAll().ToList<Contact>();
                return Read(contactList, addId);
            }

        }

        /// <summary>
        /// Delete the specified contact from the database if they exist
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public bool DeleteContactFromDB(Contact contact)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);

            using (connection)
            {
                connection.Open();
                return DeleteContactFromDB(contact, connection);
            }
        }

        /// <summary>
        /// Delete the specified contact from the database if they exist
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool DeleteContactFromDB(Contact contact, SqlConnection connection)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                string deleteAddressCommandString = "DELETE FROM DirectoryAddress WHERE Pid = @Pid";
                string deleteContactCommandString = "DELETE FROM Contact WHERE Pid = @Pid";

                SqlCommand addressCommand = new SqlCommand(deleteAddressCommandString, connection);
                SqlCommand contactCommand = new SqlCommand(deleteContactCommandString, connection);

                addressCommand.Parameters.AddWithValue("@Pid", contact.AddressID.Pid);
                contactCommand.Parameters.AddWithValue("@Pid", contact.Pid);

                return (contactCommand.ExecuteNonQuery() != 0 && addressCommand.ExecuteNonQuery() != 0);
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
        }

        public Contact GetContactFromDB(Guid contactID)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);

            using (connection)
            {
                connection.Open();
                return GetContactFromDB(contactID, connection);
            }
        }

        /// <summary>
        /// Looks up a given contact (by Pid) in the database and returns it
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public Contact GetContactFromDB(Contact contact)
        {
            SqlConnection connection = new SqlConnection(CONNECTION_STRING);

            using (connection)
            {
                connection.Open();
                return GetContactFromDB(contact, connection);
            }
        }

        public Contact GetContactFromDB(Contact contact, SqlConnection connection)
        {
            return GetContactFromDB(contact.Pid, connection);
        }

        public Contact GetContactFromDB(Guid contactID, SqlConnection connection)
        {
            return GetContactsFromDB(new List<Guid>() { contactID }, connection).First();
        }

        /// <summary>
        /// Gets all the contacts for the passed list of contact IDs
        /// </summary>
        /// <param name="contactIDs"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IEnumerable<Contact> GetContactsFromDB(IEnumerable<Guid> contactIDs, SqlConnection connection)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            if(contactIDs == null || contactIDs.Count() == 0)
            {
                throw new ArgumentException("List of contacts to return was empty.");
            }

            if (connection == null)
            {
                connection = new SqlConnection(CONNECTION_STRING);
                connection.Open();
            }

            List<Contact> contacts = new List<Contact>();

            try
            {
                string contactCommandString = @"
                SELECT
                    c.Pid,
	                c.FirstName, 
	                c.LastName, 
	                c.Phone, 
	                d.Pid ,
	                d.HouseNum, 
	                d.Street, 
	                d.City, 
	                d.Zip,
	                n.CountryCode,
                    s.StateCode FROM Contact as c
                INNER JOIN DirectoryAddress as d on c.AddressID = d.Pid
                INNER JOIN StateLookup as s on d.StateCode = s.StateCode
                INNER JOIN Country as n on n.CountryCode = d.CountryCode
                WHERE c.Pid = @ContactID";

                foreach (Guid contactID in contactIDs)
                {
                    try
                    {
                        SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

                        contactCommand.Parameters.AddWithValue("@ContactID", contactID);

                        var contactReader = contactCommand.ExecuteReader();

                        using (contactReader)
                        {
                            while (contactReader.Read())
                            {
                                contacts.Add(
                                    new Contact(
                                        contactReader.GetGuid(0),
                                        contactReader.GetString(1),
                                        contactReader.GetString(2),
                                        new Address(
                                            contactReader.GetGuid(4),
                                            contactReader.GetString(6),
                                            contactReader.GetString(5),
                                            contactReader.GetString(7),
                                            contactReader.GetString(8),
                                            (Country)Enum.Parse(typeof(Country), contactReader.GetInt32(9).ToString()),
                                            (State)Enum.Parse(typeof(State), contactReader.GetString(10))),
                                            contactReader.GetString(3)
                                        )
                                    );
                            }

                            return contacts;
                        }
                    }
                    catch (SqlException e)
                    {
                        logger.Error(e);
                        return null;
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (SqlException e)
            {
                logger.Error(e);
                return null;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return null;
            }
            finally
            {
                connection.Close();
            }

            return contacts;
        }

        public IEnumerable<Contact> GetAll()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                using (var connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string contactCommandString =
                                @"
                    SELECT
                        c.Pid as 'ContactID',
	                    c.FirstName, 
	                    c.LastName, 
	                    c.Phone, 
	                    d.Pid as 'AddressID',
	                    d.HouseNum, 
	                    d.Street, 
	                    d.City, 
	                    d.Zip,
	                    n.CountryCode,
                        s.StateCode FROM Contact as c
                    INNER JOIN DirectoryAddress as d on c.AddressID = d.Pid
                    INNER JOIN StateLookup as s on d.StateCode = s.StateCode
                    INNER JOIN Country as n on n.CountryCode = d.CountryCode";

                    SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

                    var contactReader = contactCommand.ExecuteReader();

                    List<Contact> contacts = new List<Contact>();

                    using (contactReader)
                    {
                        while (contactReader.Read())
                        {
                            var state = (State)Enum.Parse(typeof(State), contactReader.GetString(10));
                            var country = (Country)Enum.Parse(typeof(Country), contactReader.GetInt32(9).ToString());

                            contacts.Add(
                                new Contact(
                                    contactReader.GetGuid(0),
                                    contactReader.GetString(1),
                                    contactReader.GetString(2),
                                    new Address(
                                        contactReader.GetGuid(4),
                                        contactReader.GetString(6),
                                        contactReader.GetString(5),
                                        contactReader.GetString(7),
                                        contactReader.GetString(8),
                                        (Country)Enum.Parse(typeof(Country), contactReader.GetInt32(9).ToString()),
                                        (State)Enum.Parse(typeof(State), contactReader.GetString(10))),
                                    contactReader.GetString(3)
                                    )
                                );
                        }

                        return contacts;
                    }
                }
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
                return new List<Contact>();
            }
            catch(Exception e)
            {
                logger.Error(e.Message);
                return new List<Contact>();
            }  
        }

        public bool ContactExistsInDB(Contact contact)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();
                return ContactExistsInDB(contact, connection);
            }
        }

        public bool ContactExistsInDB(Contact contact, SqlConnection connection)
        {
            string query = "SELECT * FROM Contact WHERE Pid = @id";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("@id", contact.Pid);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            using (reader)
            {
                return reader.HasRows;
            }
        }

        public bool UpdateInDB(Contact contact)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                if (ContactExistsInDB(contact, connection))
                {
                    string deleteContactCommandString = "DELETE FROM Contact WHERE Pid = @Pid";
                    string deleteAddressCommandString = "DELETE FROM DirectoryAddress WHERE Pid = @Pid";

                    SqlCommand deleteContactCommand = new SqlCommand(deleteContactCommandString, connection);
                    SqlCommand deleteAddressCommand = new SqlCommand(deleteAddressCommandString, connection);

                    deleteContactCommand.Parameters.AddWithValue("@Pid", contact.Pid);
                    deleteAddressCommand.Parameters.AddWithValue("@Pid", contact.AddressID.Pid);

                    if (deleteContactCommand.ExecuteNonQuery() != 0 && deleteAddressCommand.ExecuteNonQuery() != 0)
                    {
                        return InsertContact(contact, connection);
                    }
                    else
                    {
                        throw new DatabaseCommandException($"Could not delete contact with ID {contact.Pid}.");
                    }
                }
                else
                {
                    throw new DatabaseCommandException($"Cannot update contact with id {contact.Pid}. ID does not exist in database.");
                }
            }
        }

        /// <summary>
        /// Replaces the current contact directory contained within the passed PhoneDirectory with whatever is in the DB
        /// </summary>
        /// <param name="phoneDirectory"></param>
        public void LoadFromDB()
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            try
            {
                using (var connection = new SqlConnection(CONNECTION_STRING))
                {
                    connection.Open();

                    string addressCommandString = "SELECT * FROM DirectoryAddress";
                    string contactCommandString = "SELECT * FROM Contact";

                    SqlCommand addressCommand = new SqlCommand(addressCommandString, connection);
                    SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

                    var addressReader = addressCommand.ExecuteReader();

                    Dictionary<Guid, Address> addresses = new Dictionary<Guid, Address>();
                    List<Contact> contacts = new List<Contact>();

                    using (addressReader)
                    {
                        while (addressReader.Read())
                        {
                            Address tempAddress = new Address();

                            try
                            {
                                tempAddress.Pid = addressReader.GetGuid(0);
                                tempAddress.Street = addressReader.GetString(1);
                                tempAddress.HouseNum = addressReader.GetString(2);
                                tempAddress.City = addressReader.GetString(3);
                                tempAddress.Zip = addressReader.GetString(4);
                                tempAddress.StateCode = Lookups.GetStateByCode(addressReader.GetString(5));
                                tempAddress.CountryCode = (Country)Enum.Parse(typeof(Country), addressReader.GetSqlInt32(6).ToString());
                            }
                            catch (Exception e)
                            {
                                logger.Error($"Could not read address from DB. Error: {e.Message}");

                            }

                            addresses.Add(tempAddress.Pid, tempAddress);
                        }
                    }
                    

                    var contactReader = contactCommand.ExecuteReader();

                    using (contactReader)
                    {
                        while (contactReader.Read())
                        {
                            Contact tempContact = new Contact(
                                contactReader.GetGuid(0),
                                contactReader.GetString(1),
                                contactReader.GetString(2),
                                addresses[contactReader.GetGuid(4)],
                                contactReader.GetString(3));

                            contacts.Add(tempContact);
                        }
                    }

                    Add(contacts);
                }
            }
            catch (SqlException e)
            {
                logger.Error(e);
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        /// <summary>
        /// Inserts the specified contact into the database using a new connection
        /// </summary>
        /// <param name="contact"></param>
        public void InsertContact(Contact contact)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                InsertContact(contact, connection);
            }
        }

        /// <summary>
        /// Inserts all the contacts in the collection into the database using a new connection
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="connection"></param>
        public void InsertContacts(IEnumerable<Contact> contacts)
        {
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                connection.Open();

                InsertContacts(contacts, connection);
            }
        }

        /// <summary>
        /// Inserts all the contacts in the collection into the database using the passed connection
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="connection"></param>
        public void InsertContacts(IEnumerable<Contact> contacts, SqlConnection connection)
        {
            foreach (Contact contact in contacts)
            {
                InsertContact(contact, connection);
            }
        }

        /// <summary>
        /// Inserts the specified contact into the database using the passed connection
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="connection"></param>
        public bool InsertContact(Contact contact, SqlConnection connection)
        {
            // Only insert the contact if they don't yet exist
            if (!ContactExistsInDB(contact, connection))
            {

                string addressCommandString = "INSERT INTO DirectoryAddress values(@id, @street, @housenum, @city, @zip, @state, @country)";
                string contactCommandString = "INSERT INTO Contact values(@id, @firstname, @lastname, @phone, @address)";

                SqlCommand addressCommand = new SqlCommand(addressCommandString, connection);
                SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

                // Add values for the address
                addressCommand.Parameters.AddWithValue("@id", contact.AddressID.Pid);
                addressCommand.Parameters.AddWithValue("@street", contact.AddressID.Street);
                addressCommand.Parameters.AddWithValue("@housenum", contact.AddressID.HouseNum);
                addressCommand.Parameters.AddWithValue("@city", contact.AddressID.City);
                addressCommand.Parameters.AddWithValue("@zip", contact.AddressID.Zip);
                addressCommand.Parameters.AddWithValue("@state", contact.AddressID.StateCode.ToString());
                addressCommand.Parameters.AddWithValue("@country", (int)contact.AddressID.CountryCode);

                // Add values for the contact
                contactCommand.Parameters.AddWithValue("@id", contact.Pid);
                contactCommand.Parameters.AddWithValue("@firstname", contact.FirstName);
                contactCommand.Parameters.AddWithValue("@lastname", contact.LastName);
                contactCommand.Parameters.AddWithValue("@phone", contact.Phone);
                contactCommand.Parameters.AddWithValue("@address", contact.AddressID.Pid);

                if (addressCommand.ExecuteNonQuery() == 0)
                {
                    throw new DatabaseCommandException($"Failed to insert address '{contact.AddressID.ToString()}'");
                }

                if (contactCommand.ExecuteNonQuery() == 0)
                {
                    throw new DatabaseCommandException($"Failed to insert contact '{contact.FirstName} {contact.LastName}'");
                }

                return true;
            }

            return false;
        }

        private Dictionary<string, int> MaxWidths(IEnumerable<Contact> contacts)
        {
            Dictionary<string, int> maxWidths = new Dictionary<string, int>();

            // Default the widths to the header widths in case headers are the widest
            foreach (var header in contacts.First().ToRow().Keys)
            {
                maxWidths.Add(header, header.Length);
            }

            // Loop through contacts to get the maximum width for each column
            foreach (Contact contact in contacts)
            {
                foreach (var field in contact.ColumnWidths())
                {
                    if (field.Value > maxWidths[field.Key])
                    {
                        maxWidths[field.Key] = field.Value;
                    }
                }
            }

            return maxWidths;
        }

        public enum SearchType
        {
            firstName,
            lastName,
            zip,
            city,
            phone
        }

        public class InvalidSearchTermException : Exception
        {
            public InvalidSearchTermException(string message) : base(message) { }

            public InvalidSearchTermException() : base() { }
        }

        public class DatabaseCommandException : Exception
        {
            public DatabaseCommandException(string message) : base(message) { }

            public DatabaseCommandException() : base() { }
        }
    }
}
