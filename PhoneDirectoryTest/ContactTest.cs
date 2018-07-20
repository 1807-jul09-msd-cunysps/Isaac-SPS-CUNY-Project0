using System;
using System.IO;
using PhoneDirectoryLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhoneDirectoryTest
{
    [TestClass]
    public class PhoneDirectoryTest
    {
        [TestMethod]
        public void AddContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            Assert.IsTrue(phoneDirectory.count() > 0);
        }

        [TestMethod]
        public void DeleteContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            Assert.IsTrue(phoneDirectory.count() > 0);
            phoneDirectory.delete(contact.Pid);
            Assert.AreEqual(0, phoneDirectory.count());
        }

        [TestMethod]
        public void SearchContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            Assert.AreEqual("John", phoneDirectory.search(PhoneDirectory.SearchType.firstName, "John").firstName);
        }

        [TestMethod]
        public void UpdateContactTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            //Add a contact
            phoneDirectory.add(contact);

            contact = phoneDirectory.search(PhoneDirectory.SearchType.firstName, "John");

            address = contact.address;

            //Ensure adding worked
            Assert.AreEqual("John", contact.firstName);

            //Try updating the result
            contact.firstName = "Jane";
            address.city = "Old City";
            contact.address = address;

            //Ensure the update worked
            Assert.AreEqual("Jane", phoneDirectory.search(PhoneDirectory.SearchType.lastName, "Smith").firstName);
            Assert.AreEqual("Old City", phoneDirectory.search(PhoneDirectory.SearchType.lastName, "Smith").address.city);
        }

        [TestMethod]
        public void SaveDirectoryTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            phoneDirectory.save();

            string fileContents = File.ReadAllText(phoneDirectory.DataPath());

            Assert.IsTrue(fileContents.Contains("John"));
        }

        [TestMethod]
        public void CountDirectoryTest()
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345");

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            Assert.IsTrue(phoneDirectory.count() == 1);

            for(int i = 0; i < 200; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", address, "12345678");
                phoneDirectory.add(contact);
            }

            Assert.IsTrue(phoneDirectory.count() == 201);
        }
    }
}
