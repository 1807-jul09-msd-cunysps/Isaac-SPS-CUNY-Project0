using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactLibrary.Seeders
{
    public static class AddressSeeder
    {
        public static string SingleName => "Address";
        public static string PluralName => "Addreses";

        public static void seed(int seedCount = 1, bool usaOnly=false)
        {
            string title;
            Country country;
            State state;
            string houseNum;
            string street;
            string city;
            string zip;

            Address tempAddress;
            string[] titleOptions = { "Home", "Work", "Other", "" };
            Array countries = Enum.GetValues(typeof(Country));
            Array states = Enum.GetValues(typeof(State));

            for (int i = 0; i < seedCount; i++)
            {
                Random rnd = new Random();

                // Get a random title
                title = titleOptions[rnd.Next(titleOptions.Length)];

                // Random country, unless usaOnly is set to true
                country = usaOnly ? Country.United_States : (Country)countries.GetValue(rnd.Next(countries.Length));

                // Only get a state if the country is USA
                if (country.Equals(Country.United_States) || usaOnly == true)
                {
                    state = (State)states.GetValue(rnd.Next(states.Length));
                }
                else
                {
                    state = State.NA;
                }

                houseNum = rnd.Next(9999).ToString();
                // *Everyone* lives on Main Street
                street = "Main Street";
                city = "Springfield";
                zip = rnd.Next(99999).ToString().PadLeft(5, '0');

                // Finally create the actual address
                tempAddress = new Address(country, houseNum, street, city, zip, state, title);

                tempAddress.save();
            }
        }
    }
}
