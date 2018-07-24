using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary.Seeders
{
    public static class ContactSeeder
    {
        public static void Seed(ref PhoneDirectory phoneDirectiory, int count=1)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            SqlConnection connection = null;
            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                for(int i = 0; i < count; i++)
                {
                    var contact = RandomContact();
                    InsertContact(contact, connection);
                    phoneDirectiory.Add(contact);
                }

                //Save to file
                phoneDirectiory.Save();
            }
            catch (SeederException e)
            {
                logger.Error(e.Message);
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Generates a random address using the Bogus library (a port of Faker.js)
        /// </summary>
        /// <returns></returns>
        private static Address RandomAddress()
        {
            var bogusAddress = new Bogus.DataSets.Address();
            var random = new Random();

            Country country;
            State state;

            var countries = Enum.GetValues(typeof(Country));
            var states = Enum.GetValues(typeof(State));

            if (random.Next(1, 100) > 75)
            {
                country = (Country)countries.GetValue(random.Next(countries.Length));
            }
            else
            {
                country = Country.United_States;
            }

            if(country == Country.United_States)
            {
                state = (State)states.GetValue(random.Next(states.Length));
            }
            else
            {
                state = State.NA;
            }

            return new Address()
            {
                HouseNum = bogusAddress.BuildingNumber(),
                Street = bogusAddress.StreetName() + (random.Next() % 2 == 0 ? bogusAddress.StreetSuffix() : ""),
                City = bogusAddress.City(),
                Country = country,
                State = state,
                Zip = bogusAddress.ZipCode(),
                Pid = System.Guid.NewGuid().ToString()
            };
        }

        private static Contact RandomContact()
        {
            var person = new Bogus.Person();
            Random random = new Random();
            Address address = RandomAddress();

            Contact contact = new Contact
                (
                    firstName: person.FirstName,
                    lastName: person.LastName,
                    address: address,
                    phone: person.Phone
                );

            return contact;
        }

        public static void InsertContact(Contact contact, SqlConnection connection)
        {
            string addressCommandString = "INSERT INTO DirectoryAddress values(@id, @street, @housenum, @city, @zip, @state, @country)";
            string contactCommandString = "INSERT INTO Contact values(@id, @firstname, @lastname, @phone, @address)";

            SqlCommand addressCommand = new SqlCommand(addressCommandString, connection);
            SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

            // Add values for the address
            addressCommand.Parameters.AddWithValue("@id", contact.Address.Pid);
            addressCommand.Parameters.AddWithValue("@street", contact.Address.Street);
            addressCommand.Parameters.AddWithValue("@housenum", contact.Address.HouseNum);
            addressCommand.Parameters.AddWithValue("@city", contact.Address.City);
            addressCommand.Parameters.AddWithValue("@zip", contact.Address.Zip);
            addressCommand.Parameters.AddWithValue("@state", contact.Address.State.ToString());
            addressCommand.Parameters.AddWithValue("@country", (int)contact.Address.Country);

            // Add values for the contact
            contactCommand.Parameters.AddWithValue("@id", contact.Pid);
            contactCommand.Parameters.AddWithValue("@firstname", contact.FirstName);
            contactCommand.Parameters.AddWithValue("@lastname", contact.LastName);
            contactCommand.Parameters.AddWithValue("@phone", contact.Phone);
            contactCommand.Parameters.AddWithValue("@address", contact.Address.Pid);

            using (connection)
            {
                if (addressCommand.ExecuteNonQuery() != 1)
                {
                    throw new SeederException($"Failed to insert address '{contact.Address.ToString()}'");
                }

                if (contactCommand.ExecuteNonQuery() != 1)
                {
                    throw new SeederException($"Failed to insert contact '{contact.FirstName} {contact.LastName}'");
                }
            }  
        }
    }
}
