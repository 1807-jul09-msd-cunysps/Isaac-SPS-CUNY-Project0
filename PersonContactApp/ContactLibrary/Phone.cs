using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ContactLibrary
{
    public class Phone
    {
        public long Pid { get; }
        Country countryCode { get; set; }
        string number { get; set; }
        string areaCode { get; set; }
        string extension { get; set; }

        // Storing and validating phone numbers per: https://stackoverflow.com/a/4729239/3561626
        public Phone(string number, Country countryCode = Country.United_States, string areaCode = "", string extension = "")
        {
            // Remove all non-digits from input
            number = CleanToDigits(number);
            areaCode = CleanToDigits(areaCode);
            extension = CleanToDigits(extension);

            // Validate main phone number
            if (string.IsNullOrWhiteSpace(number))
            {
                throw new ArgumentException("Number is required when creating a phone number.");
            }

            if (number.Length > 15)
            {
                throw new ArgumentException($"Phone number must not be longer than 15 digits. Received: {number}");
            }

            // Validate areaCode
            if (areaCode != "" && areaCode.Length > 5)
            {
                throw new ArgumentException($"Area code must not be longer than 5 digits. Received: {areaCode}");
            }

            // Validate extension
            if (extension != "" && extension.Length > 11)
            {
                throw new ArgumentException($"Extension must not be longer than 5 digits. Received: {extension}");
            }

            this.number = number;
            this.areaCode = areaCode;
            this.extension = extension;
            this.countryCode = countryCode;
        }

        private static string CleanToDigits(string text)
        {
            Regex justDigits = new Regex(@"[^\d]");
            return justDigits.Replace(text, "");
        }
    }
}
