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
        public static void Seed(ref PhoneDirectory phoneDirectory, int count=1)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    for (int i = 0; i < count; i++)
                    {
                        var contact = RandomContact();
                        phoneDirectory.InsertContact(contact, connection);
                        phoneDirectory.Add(contact);
                    }
                }

                //Save to file
                phoneDirectory.Save();
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
                CountryCode = country,
                StateCode = state,
                Zip = bogusAddress.ZipCode(),
                Pid = System.Guid.NewGuid()
            };
        }

        /// <summary>
        /// Generates a random contact for testing purposes
        /// </summary>
        /// <returns></returns>
        private static Contact RandomContact()
        {
            var person = new Bogus.Person();
            Random random = new Random();
            Address address = RandomAddress();

            Contact contact = new Contact
                (
                    FirstName: person.FirstName,
                    LastName: person.LastName,
                    Address: address,
                    Phone: person.Phone
                );

            return contact;
        }
    }
}
