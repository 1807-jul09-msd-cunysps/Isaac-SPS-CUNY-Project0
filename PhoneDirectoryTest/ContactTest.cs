using System;
using System.IO;
using PhoneDirectoryLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PhoneDirectoryLibrary.Seeders;
using System.Data.SqlClient;

namespace PhoneDirectoryTest
{
    [TestClass]
    public class PhoneDirectoryTest
    {
        [TestMethod]
        public void AddContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact));
        }

        [TestMethod]
        public void DeleteContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact));
            phoneDirectory.Delete(contact);
            Assert.IsFalse(phoneDirectory.ContactExistsInDB(contact));
        }

        [TestMethod]
        public void SearchOneContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.AreEqual("John", phoneDirectory.SearchOne(PhoneDirectory.SearchType.firstName, "John").FirstName);
            Assert.AreEqual("12345", phoneDirectory.SearchOne(PhoneDirectory.SearchType.zip, "12345").AddressID.Zip);
        }

        [TestMethod]
        public void SearchContactsTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Contact contact;

            using(var connection = new SqlConnection("Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH"))
            {
                connection.Open();

                for (int i = 0; i < 10; i++)
                {
                    // We create a new contact to get a new GUID
                    Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);
                    contact = new Contact("John", "Smith", address, "12345678");

                    phoneDirectory.Add(contact, connection);

                    Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact, connection));
                }

                List<Contact> contactsByName = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.firstName, "John"));
                List<Contact> contactsByZip = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.zip, "12345"));
                List<Contact> contactsByWildCity = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.city, "*W*"));

                Assert.AreEqual(10, contactsByName.Count);
                Assert.AreEqual(10, contactsByZip.Count);
                Assert.AreEqual(10, contactsByWildCity.Count);
            }            
        }

        [TestMethod]
        public void UpdateContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contactInMemory = new Contact("John", "Smith", address, "12345678");

            //Add a contact
            phoneDirectory.Add(contactInMemory);

            contactInMemory = phoneDirectory.SearchOne(PhoneDirectory.SearchType.firstName, "John");

            address = contactInMemory.AddressID;

            //Ensure adding worked
            Assert.AreEqual("John", contactInMemory.FirstName);

            //Try updating the result
            contactInMemory.FirstName = "Jane";
            address.City = "Old City";
            contactInMemory.AddressID = address;

            phoneDirectory.Update(contactInMemory);

            //Ensure the update worked
            foreach (Contact contact in phoneDirectory.GetAll())
            {
                if(contact == contactInMemory)
                {
                    // If they are equal on a shallow compare (by Pid only), ensure they are equal on a deep comparison
                    Assert.IsTrue(contactInMemory.Equals(contact, true));
                }
            }

            Contact contactFromDB = phoneDirectory.GetContactFromDB(contactInMemory);

            Assert.IsTrue(contactInMemory.Equals(contactFromDB, true));
        }

        [TestMethod]
        public void SaveDirectoryTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact;

            for (int i = 0; i < 200; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", address, "12345678");
                phoneDirectory.Add(contact);
            }

            phoneDirectory.Save();

            string fileContents = File.ReadAllText(phoneDirectory.DataPath());

            Assert.IsTrue(fileContents.Contains("John"));
        }

        //[TestMethod]
        //public void CountDirectoryTest()
        //{
        //    PhoneDirectory phoneDirectory = new PhoneDirectory();

        //    Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

        //    Contact contact = new Contact("John", "Smith", address, "12345678");

        //    phoneDirectory.Add(contact);

        //    Assert.IsTrue(phoneDirectory.Count() == 1);

        //    for(int i = 0; i < 200; i++)
        //    {
        //        // We create a new contact to get a new GUID
        //        contact = new Contact("John", "Smith", address, "12345678");
        //        phoneDirectory.Add(contact);
        //    }

        //    Assert.IsTrue(phoneDirectory.Count() == 201);
        //}

        [TestMethod]
        public void ReadContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.Read(contact).Contains("|John"));
        }

        //[TestMethod]
        //public void DeserializeContactTest()
        //{
        //    PhoneDirectory phoneDirectory = new PhoneDirectory();

        //    Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

        //    Contact contact = new Contact("John", "Smith", address, "12345678");

        //    phoneDirectory.Add(contact);

        //    phoneDirectory.Save();

        //    PhoneDirectory phoneDirectory2 = new PhoneDirectory();

        //    phoneDirectory2.LoadFromText();

        //    Assert.IsTrue(phoneDirectory2.GetAll().Count > 0);
        //}

        [TestMethod]
        public void SeedStatesTest()
        {
            StateSeeder.Seed();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkStateCommandString = "SELECT * FROM StateLookup";

                SqlCommand sqlCommand = new SqlCommand(checkStateCommandString, connection);

                Assert.IsTrue(sqlCommand.ExecuteReader().HasRows);
            }
        }

        [TestMethod]
        public void SeedCountriesTest()
        {
            CountrySeeder.Seed();

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkCountryCommandString = "SELECT * FROM Country";

                SqlCommand sqlCommand = new SqlCommand(checkCountryCommandString, connection);

                Assert.IsTrue(sqlCommand.ExecuteReader().HasRows);
            }
        }
    }
}
