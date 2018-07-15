using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactLibrary
{
    public class Contact
    {
        private string firstName;
        private string lastName;
        private readonly List<Address> addresses;
        private Phone phone;

        public Contact(string firstName, string lastName, string phoneNumber, Country countryCode , string areaCode = "", string phoneSuffix = "", params Address[] addresses) : this(firstName, lastName, new Phone(phoneNumber, countryCode, areaCode, phoneSuffix), addresses)
        {
            //
        }

        public Contact(string firstName, string lastName, Phone phone, params Address[] addresses)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                throw new InvalidNameException($"Either the first name '{firstName}' or the last name '{lastName}' is empty. Both are required.");
            }

            // Convert our params to a list so we can collect all submitted addresses
            List<Address> addressList = new List<Address>();
            addressList.AddRange(addresses);
            RequireAddress(addressList);

            this.phone = phone;

            this.firstName = firstName;
            this.lastName = lastName;

            this.addresses = addressList;
        }

        private void AddAddresses(List<Address> addresses)
        {
            // Check whether we actually have addresses
            RequireAddress(addresses);

            this.addresses.AddRange(addresses);
        }

        private void RequireAddress(List<Address> addresses)
        {
            if (addresses.Count < 1)
            {
                throw new ArgumentException("Attempting to add an Address, but list of addresses was empty.");
            }
        }

        public class InvalidNameException : ArgumentException
        {
            public InvalidNameException(string message) : base(message)
            {
                //
            }
        }
    }
}
