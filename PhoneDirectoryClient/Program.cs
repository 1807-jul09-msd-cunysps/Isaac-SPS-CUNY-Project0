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

            UserInterfaceFunctions.UserDisplayDashboard(ref phoneDirectory);
        }
    }
}
