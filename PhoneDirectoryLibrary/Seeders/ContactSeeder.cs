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
        public static void Seed(int count=1)
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
                    InsertContact(RandomContact(), connection);
                }
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
                Zip = bogusAddress.ZipCode()
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

        private static void InsertContact(Contact contact, SqlConnection connection)
        {
            string addressCommandString = "INSERT INTO DirectoryAddress values(@p1, @p2, @p3, @p4, @p5, @p6, @p7)";
            string contactCommandString = "INSERT INTO Contact values(@p1, @p2, @p3, @p4, @p5)";

            SqlCommand addressCommand = new SqlCommand(addressCommandString, connection);
            SqlCommand contactCommand = new SqlCommand(contactCommandString, connection);

            // Add values for the address
            addressCommand.Parameters.AddWithValue("@p1", contact.Address.Pid);
            addressCommand.Parameters.AddWithValue("@p2", contact.Address.Street);
            addressCommand.Parameters.AddWithValue("@p3", contact.Address.HouseNum);
            addressCommand.Parameters.AddWithValue("@p4", contact.Address.City);
            addressCommand.Parameters.AddWithValue("@p5", contact.Address.Zip);
            addressCommand.Parameters.AddWithValue("@p6", contact.Address.State);
            addressCommand.Parameters.AddWithValue("@p7", (int)contact.Address.Country);

            // Add values for the contact
            contactCommand.Parameters.AddWithValue("@p1", contact.Pid);
            contactCommand.Parameters.AddWithValue("@p2", contact.FirstName);
            contactCommand.Parameters.AddWithValue("@p3", contact.LastName);
            contactCommand.Parameters.AddWithValue("@p4", contact.Phone);
            contactCommand.Parameters.AddWithValue("@p5", contact.Address.Pid);
                                   

            if(addressCommand.ExecuteNonQuery() != 1)
            {
                throw new SeederException($"Failed to insert address '{contact.Address.ToString()}'");
            }

            if(contactCommand.ExecuteNonQuery() != 1)
            {
                throw new SeederException($"Failed to insert contact '{contact.FirstName} {contact.LastName}'");
            }
        }
    }
}
