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

            phoneDirectory.LoadFromDB();

            UserInterfaceFunctions.UserDisplayDashboard(ref phoneDirectory);
        }
    }
}
