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
            PhoneDirectory phoneDirectory = new PhoneDirectory();

            //StateSeeder.Seed();
            //CountrySeeder.Seed();

            phoneDirectory.Load();

            //for (int i = 0; i < 200; i++)
            //{
            //    // We create a new contact to get a new GUID
            //    contact = new Contact("John", "Smith", address, "12345678");
            //    phoneDirectory.Add(contact);
            //}

            UserInterfaceFunctions.UserDisplayDashboard(ref phoneDirectory);

            //ContactSeeder.Seed(50);
        }
    }
}
