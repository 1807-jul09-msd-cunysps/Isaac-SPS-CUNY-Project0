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

            Assert.IsTrue(phoneDirectory.Count() > 0);
        }

        [TestMethod]
        public void DeleteContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.Count() > 0);
            phoneDirectory.Delete(contact);
            Assert.AreEqual(0, phoneDirectory.Count());
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

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact;

            for (int i = 0; i < 200; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", address, "12345678");
                phoneDirectory.Add(contact);
            }

            List<Contact> contactsByName = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.firstName, "John"));
            List<Contact> contactsByZip = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.zip, "12345"));

            Assert.AreEqual(200, contactsByName.Count);
            Assert.AreEqual(200, contactsByZip.Count);
        }

        [TestMethod]
        public void UpdateContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            //Add a contact
            phoneDirectory.Add(contact);

            contact = phoneDirectory.SearchOne(PhoneDirectory.SearchType.firstName, "John");

            address = contact.AddressID;

            //Ensure adding worked
            Assert.AreEqual("John", contact.FirstName);

            //Try updating the result
            contact.FirstName = "Jane";
            address.City = "Old City";
            contact.AddressID = address;

            //Ensure the update worked
            Assert.AreEqual("Jane", phoneDirectory.SearchOne(PhoneDirectory.SearchType.lastName, "Smith").FirstName);
            Assert.AreEqual("Old City", phoneDirectory.SearchOne(PhoneDirectory.SearchType.lastName, "Smith").AddressID.City);

            string connectionString = "Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                Assert.IsTrue(PhoneDirectory.ContactExists(contact,connection));
            }                
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

        [TestMethod]
        public void CountDirectoryTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.Count() == 1);

            for(int i = 0; i < 200; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", address, "12345678");
                phoneDirectory.Add(contact);
            }

            Assert.IsTrue(phoneDirectory.Count() == 201);
        }

        [TestMethod]
        public void ReadContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.Read(contact).Contains("|John"));
        }

        [TestMethod]
        public void DeserializeContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            phoneDirectory.Save();

            PhoneDirectory phoneDirectory2 = new PhoneDirectory();

            phoneDirectory2.LoadFromText();

            Assert.IsTrue(phoneDirectory2.GetAll().Count > 0);
        }

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
