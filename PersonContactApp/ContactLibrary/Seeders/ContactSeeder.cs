using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactLibrary.Seeders
{
    public static class ContactSeeder
    {
        public static string SingleName => "Contact";
        public static string PluralName => "Contacts";

        public static List<Contact> seed(ContactDirectory contactDirectory, int seedCount = 1, bool usaOnly = false)
        {
            string firstName;
            string lastName;
            Address address;
            Phone phone;
            List<Contact> contacts = new List<Contact>();

            for(int i = 0; i < seedCount; i++)
            {
                firstName = "John";
                lastName = "Smith";
                address = AddressSeeder.seed(1)[0];
                phone = new Phone("123456789", Country.United_States, "1234", "1234");

                Contact contact = new Contact(firstName, lastName, phone, address);

                contacts.Add(contact);
            }
            return contacts;
    }
    }
}
