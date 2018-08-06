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
        public static IEnumerable<Contact> Seed(ref PhoneDirectory phoneDirectory, int count=1)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            List<Contact> contacts = new List<Contact>();

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
                        contacts.Add(contact);
                    }
                }

                return contacts;
            }
            catch (SeederException e)
            {
                logger.Error(e.Message);
                return contacts;
            }
            catch (SqlException e)
            {
                logger.Error(e.Message);
                return contacts;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return contacts;
            }
        }

        /// <summary>
        /// Generates a random address using the Bogus library (a port of Faker.js)
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Address> RandomAddresses()
        {
            List<Address> addresses = new List<Address>();

            var random = new Random();

            for (int i = 0; i < random.Next(1,5); i++)
            {
                var bogusAddress = new Bogus.DataSets.Address();


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

                if (country == Country.United_States)
                {
                    state = (State)states.GetValue(random.Next(states.Length));
                }
                else
                {
                    state = State.NA;
                }

                addresses.Add(new Address()
                {
                    HouseNum = bogusAddress.BuildingNumber(),
                    Street = bogusAddress.StreetName() + (random.Next() % 2 == 0 ? bogusAddress.StreetSuffix() : ""),
                    City = bogusAddress.City(),
                    CountryCode = country,
                    StateCode = state,
                    Zip = bogusAddress.ZipCode(),
                    Pid = System.Guid.NewGuid()
                });   
            }

            return addresses;
        }

        private static IEnumerable<Phone> RandomPhones(Guid contactID)
        {
            Random random = new Random();

            List<Phone> phones = new List<Phone>();

            for (int i = 0; i < random.Next(1,5); i++)
            {
                phones.Add(new Phone(
                    random.Next(100,999).ToString(),
                    random.Next(1111111,9999999).ToString(),
                    "x" + random.Next(1,999).ToString(),
                    (short)random.Next(1,855),
                    contactID
                    ));
            }

            return phones;
        }

        /// <summary>
        /// Generates a random contact for testing purposes
        /// </summary>
        /// <returns></returns>
        private static Contact RandomContact()
        {
            var person = new Bogus.Person();
            Random random = new Random();
            List<Address> addresses = RandomAddresses().ToList<Address>();            

            Contact contact = new Contact
                (
                    FirstName: person.FirstName,
                    LastName: person.LastName,
                    Addresses: addresses,
                    GenderID: random.Next(0, Lookups.Genders.Count() - 1),
                    Emails: new List<Email>(),
                    Phones: new List<Phone>()
                );

            contact.Phones = RandomPhones(contact.Pid).ToList<Phone>();
            contact.Emails.Add(new Email(person.Email, contact.Pid));

            return contact;
        }
    }
}
