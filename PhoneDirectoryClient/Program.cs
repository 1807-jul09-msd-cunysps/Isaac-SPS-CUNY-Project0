using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhoneDirectoryLibrary;
using PhoneDirectoryLibrary.Seeders;

namespace PhoneDirectoryClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //PhoneDirectory phoneDirectory = new PhoneDirectory();

            //Address address = new Address("Main Street", "123", "New City", "12345", Country.United_States, State.NY);

            //Contact contact = new Contact("John", "Smith", address, "12345678");

            //phoneDirectory.Add(contact);

            //for (int i = 0; i < 200; i++)
            //{
            //    // We create a new contact to get a new GUID
            //    contact = new Contact("John", "Smith", address, "12345678");
            //    phoneDirectory.Add(contact);
            //}

            //UserInterfaceFunctions.UserDisplayDashboard(ref phoneDirectory);

            StateSeeder.Seed();

            //@TODO Add update function to UI, create seeders or just read from disk, figure out why listing doesn't seem to work, create and write to DB
        }
    }
}
