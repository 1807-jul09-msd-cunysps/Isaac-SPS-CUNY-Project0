using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public struct Phone
    {
        public Guid Pid { get; set; }
        public string AreaCode { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public Country CountryCode { get; set; }
        public Guid ContactID { get; set; }

        public Phone(Guid Pid, string areaCode, string number, string extension, Country countryCode, Guid contactID) : this(areaCode, number, extension, countryCode, contactID)
        {
            this.Pid = Pid;
        }

        public Phone(string areaCode, string number, string extension, Country countryCode, Guid contactID)
        {
            Pid = Guid.NewGuid();
            AreaCode = areaCode ?? throw new ArgumentNullException(nameof(areaCode));
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            CountryCode = countryCode;
            ContactID = contactID;
        }
    }
}
