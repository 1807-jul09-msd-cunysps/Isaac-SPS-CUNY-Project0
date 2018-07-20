using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public struct Address
    {
        public string street { get; set; }
        public string houseNum { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string Pid;

        public Address(string street, string houseNum, string city, string zip) : this()
        {
            this.street = street ?? throw new ArgumentNullException(nameof(street));
            this.houseNum = houseNum ?? throw new ArgumentNullException(nameof(houseNum));
            this.city = city ?? throw new ArgumentNullException(nameof(city));
            this.zip = zip ?? throw new ArgumentNullException(nameof(zip));
            Pid = System.Guid.NewGuid().ToString();
        }
    }
}
