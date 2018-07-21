using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary.Seeders
{
    class SeederException : Exception
    {
        public SeederException(string message) : base(message) { }

        public SeederException() : base() { }
    }
}
