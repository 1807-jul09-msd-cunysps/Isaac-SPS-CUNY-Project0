using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public class Contact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public Address address { get; set; }
        public string phone;
        public string Pid { get; }

        public Contact(string firstName, string lastName, Address address, string phone)
        {
            this.firstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            this.lastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            this.address = address;
            this.phone = phone ?? throw new ArgumentNullException(nameof(phone));
            this.Pid = System.Guid.NewGuid().ToString();
        }
    }
}
