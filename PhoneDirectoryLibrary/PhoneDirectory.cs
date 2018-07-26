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
        // This HashSet is set to only look at PiD when hashing/comparing
        private HashSet<Contact> contacts;
        public string DataFilePath;

        public PhoneDirectory()
        {
            contacts = new HashSet<Contact>();
            DataFilePath = Path.ChangeExtension(Path.Combine("C:\\Dev", "directory"), "json");
        }

        public PhoneDirectory(HashSet<Contact> contacts)
        {
            try
            {
                this.contacts = contacts ?? throw new ArgumentNullException(nameof(contacts));
                DataFilePath = Path.ChangeExtension(Path.Combine("C:\\Dev", "directory"), "json");
            }
            catch(Exception e)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
            }
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

        public int Count()
        {
            return contacts.Count();
        }

        public void Add(Contact contact)
        {
            try
            {
                if(contact == null)
                {
                    throw new ArgumentNullException("Cannot add a null contact.");
                }
                contacts.Add(contact);
                Save();
            }            
            catch(Exception e)
            {
                var logger = NLog.LogManager.GetCurrentClassLogger();
                logger.Error(e.Message);
            }
        }

        //Returns true if the item was deleted, false otherwise
        public bool Delete(Contact contact)
        {
            bool result = contacts.Remove(contact);
            if (result) { Save(); };
            return result;
        }


        // Replace an existing contact with an updated version in the collection
        // Returns true if an update was made, false if object not found
        // Also updates the instance in the DB
        public bool Update(Contact contact)
        {
            bool result = false;
            Contact oldContact;

            // Returns true if the update worked and false otherwise
            if (contacts.TryGetValue(contact, out oldContact))
            {
                if (contacts.Remove(oldContact))
                {
                    // Add to collection and save the JSON file
                    result = contacts.Add(contact);
                    if (result) { Save(); };

                    // Update the DB with the changed fields
                    Dictionary<string, string> addressToChange = new Dictionary<string, string>();

                    if (oldContact.AddressID.Pid != contact.AddressID.Pid) { addressToChange.Add("Pid", contact.AddressID.Pid); };

                    //UpdateInDB(contact)
                }
            }

            return result;
        }

        private bool UpdateInDB(Contact contact, Dictionary<string, string> contactUpdate, Dictionary<string, string> addressUpdate)
        {
            Regex justDigits = new Regex(@"[^\d]");

            string addressCommandString = "UPDATE DirectoryAddress SET ";

            foreach (var item in addressUpdate)
            {
                // If it's a number don't put quotes, otherwise do
                if (justDigits.IsMatch(item.Value))
                {
                    addressCommandString += item.Key + " = " + item.Value + ", ";

                }
                else
                {
                    addressCommandString += item.Key + " = '" + item.Value + "', ";
                }
            }

            addressCommandString = addressCommandString.Remove(addressCommandString.Length - 1, 2);
            addressCommandString += $" WHERE PersonID = {contact.Pid}";

            string contactCommandString = "UPDATE Contact SET ";

            foreach (var item in contactUpdate)
            {
                // If it's a number don't put quotes, otherwise do
                if (justDigits.IsMatch(item.Value))
                {
                    contactCommandString += item.Key + " = " + item.Value + ", ";

                }
                else
                {
                    contactCommandString += item.Key + " = '" + item.Value + "', ";
                }
            }

            contactCommandString = contactCommandString.Remove(contactCommandString.Length - 1, 2);
            contactCommandString += $" WHERE Pid = {contact.Pid}";

            var logger = NLog.LogManager.GetCurrentClassLogger();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand addressCommand = new SqlCommand(addressCommandString, connection);
                    SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

                    if (addressCommand.ExecuteNonQuery() != 0 && contactCommand.ExecuteNonQuery() != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
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

        /// <summary>
        /// Searchs for contacts matching the search term and returns the first result
        /// </summary>
        /// <param name="type">The type of search to perform, based on an Enum</param>
        /// <param name="searchTerm">The string to search for, the meaning of which is based on the "type" parameter</param>
        /// <returns></returns>
        public Contact SearchOne(SearchType type, string searchTerm)
        {
            switch (type)
            {
                case SearchType.firstName:
                    return contacts.Where(query => (query.FirstName).Contains(searchTerm)).First();
                case SearchType.lastName:
                    return contacts.Where(query => (query.LastName).Contains(searchTerm)).First();
                case SearchType.zip:
                    return contacts.Where(query => (query.AddressID.Zip).Contains(searchTerm)).First();
                case SearchType.city:
                    return contacts.Where(query => (query.AddressID.City).Contains(searchTerm)).First();
                case SearchType.phone:
                    return contacts.Where(query => (query.Phone).Contains(searchTerm)).First();
                default:
                    throw new InvalidSearchTermException($"{type.ToString()} is not a valid search term.");
            }
        }

        /// <summary>
        /// Searchs for contacts matching the search term and returns all results
        /// </summary>
        /// <param name="type">The type of search to perform, based on an Enum</param>
        /// <param name="searchTerm">The string to search for, the meaning of which is based on the "type" parameter</param>
        /// <returns></returns>
        public IEnumerable<Contact> Search(SearchType type, string searchTerm)
        {
            switch (type)
            {
                case SearchType.firstName:
                    return contacts.Where(query => (query.FirstName).Contains(searchTerm));
                case SearchType.lastName:
                    return contacts.Where(query => (query.LastName).Contains(searchTerm));
                case SearchType.zip:
                    return contacts.Where(query => (query.AddressID.Zip).Contains(searchTerm));
                case SearchType.city:
                    return contacts.Where(query => (query.AddressID.City).Contains(searchTerm));
                case SearchType.phone:
                    return contacts.Where(query => (query.Phone).Contains(searchTerm));
                default:
                    throw new InvalidSearchTermException($"{type.ToString()} is not a valid search term.");
            }
        }

        public void Save()
        {
            string jsonData = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            File.WriteAllText(DataPath(),jsonData);
        }

        public void Load()
        {
            string jsonData = File.ReadAllText(DataPath());
            contacts.Clear();
            foreach (Contact contact in JsonConvert.DeserializeObject<HashSet<Contact>>(jsonData))
            {
                contacts.Add(contact);
            }
        }
        
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

            foreach (var header in contacts.First<Contact>().ToRow(MaxWidths(contacts)).Keys)
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

        /// <summary>
        /// Returns a pretty-printed string representing all contacts in the collection on the console
        /// </summary>
        /// <returns></returns>
        public string Read(bool addId = false)
        {
            return Read(this.contacts.ToList<Contact>(), addId);
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
            if(contactList.Count > 0)
            {
                return Read(contactList, addId);
            }
            else
            {
                contactList = this.contacts.ToList<Contact>();
                return Read(contactList, addId);
            }
            
        }

        //public Contact GetContactFromDB(Contact contact, SqlConnection connection)
        //{
        //    string query = "SELECT * FROM Contact WHERE Pid = @id";
        //    SqlCommand sqlCommand = new SqlCommand(query, connection);
        //    sqlCommand.Parameters.AddWithValue("@id", query);
        //    SqlDataReader reader = sqlCommand.ExecuteReader();

        //    reader.

        //    Contact dbContact = new Contact
        //        (
        //            firstName = reader.
        //        )
        //}

        public List<Contact> GetAll()
        {
            return contacts.ToList<Contact>();
        }

        public static bool ContactExists(Contact contact, SqlConnection connection)
        {
            string query = "SELECT * FROM Contact WHERE Pid = @id";
            SqlCommand sqlCommand = new SqlCommand(query, connection);
            sqlCommand.Parameters.AddWithValue("@id", query);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            return reader.HasRows;
        }

        public static void UpdateContact(Contact contact, SqlConnection connection)
        {
            if(ContactExists(contact, connection))
            {
                    
            }
            else
            {
                throw new DatabaseCommandException($"Cannot insert contact with id {contact.Pid}. ID does not exist in database.");
            }
        }

        public static void InsertContact(Contact contact, SqlConnection connection)
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

            if (addressCommand.ExecuteNonQuery() != 0)
            {
                throw new DatabaseCommandException($"Failed to insert address '{contact.AddressID.ToString()}'");
            }

            if (contactCommand.ExecuteNonQuery() != 0)
            {
                throw new DatabaseCommandException($"Failed to insert contact '{contact.FirstName} {contact.LastName}'");
            }
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
                    if(field.Value > maxWidths[field.Key])
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
            phone,
            id
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
