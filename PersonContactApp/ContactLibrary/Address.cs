using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ContactLibrary
{
    public struct Address
    {
        public string Pid { get; }
        public string title { get; }
        public Country countryCode { get; }
        public State stateCode { get; }
        public string houseNum { get; }
        public string street { get; }
        public string city { get; }
        public string zip { get; }

        public Address(Country countryCode, string houseNum, string street, string city, string zip, State stateCode = State.NA, string title = "")
        {
            if (string.IsNullOrWhiteSpace(houseNum) || string.IsNullOrWhiteSpace(street) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(zip))
            {
                //@TODO Switch country over to enum of country codes and lookup the name
                throw new InvalidAddressFieldException(
                    "One of the address fields is empty. A value is required." +
                    $"Country:{countryCode}" + Environment.NewLine +
                    $"House Number:{houseNum}" + Environment.NewLine +
                    $"Street:{street}" + Environment.NewLine +
                    $"City:{city}" + Environment.NewLine +
                    $"ZIP:{zip}");
            }

            if(stateCode != State.NA && countryCode != Country.United_States)
            {
                throw new InvalidAddressFieldException($"A state must be supplied if the address is in the United States. Received: {countryCode}.");
            }

            this.countryCode = countryCode;
            this.stateCode = stateCode;
            this.title = title;
            this.houseNum = houseNum;
            this.street = street;
            this.city = city;
            this.zip = zip;

            this.Pid = System.Guid.NewGuid().ToString();
        }

        public void save()
        {
            // Writes a serialized version of this object to a file
            // Overwrites file if it exists
            // The file name is the UUID for this object
            string json = JsonConvert.SerializeObject(this);
            string savePath = Path.Combine(ContactDirectory.rootPath, this.Pid);
            savePath = Path.ChangeExtension(savePath, "json");
            Console.WriteLine($"Saving to {savePath}");
            File.WriteAllText(savePath, json);
        }

        public override string ToString()
        {
            string output = "";

            if (title != "")
            {
                output += $"Description: {title}" + Environment.NewLine;
            }

            output += $"{street}, {city} {zip}";

            // @TODO lookup country by code
            //output += Environment.NewLine + country;

            return output;
        }

        internal class InvalidAddressFieldException : ArgumentException
        {
            public InvalidAddressFieldException()
            {
                //
            }

            public InvalidAddressFieldException(string message) : base(message)
            {
                //
            }
        }
    }
}
