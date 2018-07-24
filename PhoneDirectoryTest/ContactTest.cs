using System;
using System.IO;
using PhoneDirectoryLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PhoneDirectoryLibrary.Seeders;

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
            Assert.AreEqual("12345", phoneDirectory.SearchOne(PhoneDirectory.SearchType.zip, "12345").Address.Zip);
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

            address = contact.Address;

            //Ensure adding worked
            Assert.AreEqual("John", contact.FirstName);

            //Try updating the result
            contact.FirstName = "Jane";
            address.City = "Old City";
            contact.Address = address;

            //Ensure the update worked
            Assert.AreEqual("Jane", phoneDirectory.SearchOne(PhoneDirectory.SearchType.lastName, "Smith").FirstName);
            Assert.AreEqual("Old City", phoneDirectory.SearchOne(PhoneDirectory.SearchType.lastName, "Smith").Address.City);
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
        public void InsertIntoDB()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.Add(contact);

            ContactSeeder.Seed(ref phoneDirectory, 10);
        }
    }
}
