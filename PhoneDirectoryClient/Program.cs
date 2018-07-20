using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryLibrary;

namespace PhoneDirectoryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            Address address = new Address("Main Street", "123", "New City", "12345",Country.United_States,State.AK);

            Contact contact = new Contact("John", "Smith", address, "12345678");

            phoneDirectory.add(contact);

            for (int i = 0; i < 10; i++)
            {
                // We create a new contact to get a new GUID
                contact = new Contact("John", "Smith", address, "12345678");
                phoneDirectory.add(contact);
            }

            Console.WriteLine(phoneDirectory.read());

            UserInterfaceFunctions.UserInsertContact(ref phoneDirectory);

            Console.WriteLine(phoneDirectory.read());

            phoneDirectory.save();

            Console.Read();
        }
    }
}
