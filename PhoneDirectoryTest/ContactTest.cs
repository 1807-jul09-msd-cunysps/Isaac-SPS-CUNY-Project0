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

            Contact contact = new Contact("John", "Smith", new List<Address>(), 0, new List<Email>(), new List<Phone>());

            contact.Addresses.Add(new Address(contact.Pid, "Main Street", "123", "New City", "12345", Country.United_States, State.NY));

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact));
        }

        [TestMethod]
        public void DeleteContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Contact contact = new Contact("John", "Smith", new List<Address>(), 0, new List<Email>(), new List<Phone>());

            contact.Addresses.Add(new Address(contact.Pid, "Main Street", "123", "New City", "12345", Country.United_States, State.NY));

            phoneDirectory.Add(contact);

            Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact));
            phoneDirectory.Delete(contact);
            Assert.IsFalse(phoneDirectory.ContactExistsInDB(contact));
        }

        [TestMethod]
        public void SearchContactsTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Contact contact;

            using(var connection = new SqlConnection("Data Source=robodex.database.windows.net;Initial Catalog=RoboDex;Persist Security Info=True;User ID=isaac;Password=qe%8KQ^mrjJe^zq75JmPe$xa2tWFxH"))
            {
                connection.Open();

                for (int i = 0; i < 1; i++)
                {
                    // We create a new contact to get a new GUID
                    contact = new Contact("John", "Smith", new List<Address>(), 0, new List<Email>(), new List<Phone>());

                    contact.Addresses.Add(new Address(contact.Pid, "Main Street", "123", "New City", "12345", Country.United_States, State.NY));

                    phoneDirectory.Add(contact);

                    Assert.IsTrue(phoneDirectory.ContactExistsInDB(contact, connection));
                }

                List<Contact> contactsByName = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.firstName, "John"));
                List<Contact> contactsByZip = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.zip, "12345"));
                List<Contact> contactsByWildCity = new List<Contact>(phoneDirectory.Search(PhoneDirectory.SearchType.city, "*W*"));

                Assert.IsTrue(contactsByName.Count >= 1);
                Assert.IsTrue(contactsByZip.Count >= 1);
                Assert.IsTrue(contactsByWildCity.Count >= 1);
            }            
        }

        [TestMethod]
        public void UpdateContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Contact contactInMemory = new Contact("John", "Smith", new List<Address>(), 0, new List<Email>(), new List<Phone>());

            Address address = new Address(contactInMemory.Pid, "Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            contactInMemory.Addresses.Add(address);

            //Add a contact
            phoneDirectory.Add(contactInMemory);

            contactInMemory = phoneDirectory.Search(PhoneDirectory.SearchType.firstName, "John")[0];

            address = contactInMemory.Addresses[0];

            //Ensure adding worked
            Assert.AreEqual("John", contactInMemory.FirstName);

            //Try updating the result
            contactInMemory.FirstName = "Jane";
            address.City = "Old City";
            contactInMemory.Addresses[0] = address;

            phoneDirectory.Update(contactInMemory);

            //Ensure the update worked
            Contact contactInDB = phoneDirectory.GetContactFromDB(contactInMemory.Pid);

            if (contactInDB == contactInMemory)
            {
                // If they are equal on a shallow compare (by Pid only), ensure they are equal on a deep comparison
                Assert.IsTrue(contactInMemory.Equals(contactInDB, true));
            }

            else
            {
                throw new AssertFailedException("Contact Pids did not match.");
            }
        }

        [TestMethod]
        public void SaveDirectoryTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Contact contact;

            for (int i = 0; i < 200; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", new List<Address>(), 0, new List<Email>(), new List<Phone>());
                contact.Addresses.Add(new Address(contact.Pid, "Main Street", "123", "New City", "12345", Country.United_States, State.NY));
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
    }
}
