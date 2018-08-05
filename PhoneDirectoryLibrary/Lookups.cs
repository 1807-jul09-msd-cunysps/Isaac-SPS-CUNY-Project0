using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneDirectoryLibrary
{
    public static class Lookups
    {
        const int MAX_COUNTRY_NAME_WIDTH = 30;
        const int MAX_COUNTRY_ID_WIDTH = 3;

        public static Dictionary<int, string> Genders = new Dictionary<int, string>()
        {
            {0, "Male" },
            {1, "Female" },
            {3, "Other" }
        };

        public static Dictionary<State, string> StateNames = new Dictionary<State, string>()
        {
            {State.NA, "Not in United States" }, {State.AL, "ALABAMA"}, {State.AK, "ALASKA"}, {State.AZ, "ARIZONA"}, {State.AR, "ARKANSAS"}, {State.CA, "CALIFORNIA"}, {State.CO, "COLORADO"}, {State.CT, "CONNECTICUT"}, {State.DE, "DELAWARE"}, {State.FL, "FLORIDA"}, {State.GA, "GEORGIA"}, {State.HI, "HAWAII"}, {State.ID, "IDAHO"}, {State.IL, "ILLINOIS"}, {State.IN, "INDIANA"}, {State.IA, "IOWA"}, {State.KS, "KANSAS"}, {State.KY, "KENTUCKY"}, {State.LA, "LOUISIANA"}, {State.ME, "MAINE"}, {State.MD, "MARYLAND"}, {State.MA, "MASSACHUSETTS"}, {State.MI, "MICHIGAN"}, {State.MN, "MINNESOTA"}, {State.MS, "MISSISSIPPI"}, {State.MO, "MISSOURI"}, {State.MT, "MONTANA"}, {State.NE, "NEBRASKA"}, {State.NV, "NEVADA"}, {State.NH, "NEW HAMPSHIRE"}, {State.NJ, "NEW JERSEY"}, {State.NM, "NEW MEXICO"}, {State.NY, "NEW YORK"}, {State.NC, "NORTH CAROLINA"}, {State.ND, "NORTH DAKOTA"}, {State.OH, "OHIO"}, {State.OK, "OKLAHOMA"}, {State.OR, "OREGON"}, {State.PA, "PENNSYLVANIA"}, {State.RI, "RHODE ISLAND"}, {State.SC, "SOUTH CAROLINA"}, {State.SD, "SOUTH DAKOTA"}, {State.TN, "TENNESSEE"}, {State.TX, "TEXAS"}, {State.UT, "UTAH"}, {State.VT, "VERMONT"}, {State.VA, "VIRGINIA"}, {State.WA, "WASHINGTON"}, {State.WV, "WEST VIRGINIA"}, {State.WI, "WISCONSIN"}, {State.WY, "WYOMING"}
        };

        /// <summary>
        /// Makes a three column display of all the possible countries, along with their IDs
        /// </summary>
        /// <returns>The formatted string containing the list of countries</returns>
        public static string ListCountryOptions()
        {
            string finalList = "";

            Queue<string> columnOne = new Queue<string>();
            Queue<string> columnTwo = new Queue<string>();
            Queue<string> columnThree = new Queue<string>();

            var keyList = CountryKeys();

            //Divide the list into three columns so it's easier to read
            int firstAndSecondThirdSize = keyList.Count() / 3;
            int remainderSize = firstAndSecondThirdSize % keyList.Count;

            // Put the first column on its stack as strings
            for (int i = 0; i < firstAndSecondThirdSize; i++)
            {
                EnqueueAndPadCountryOption(ref columnOne, keyList.ElementAt(i));
            }

            // Put the second column on its stack as strings
            for (int i = firstAndSecondThirdSize; i < firstAndSecondThirdSize * 2; i++)
            {
                EnqueueAndPadCountryOption(ref columnTwo, keyList.ElementAt(i));
            }

            // Put the third column on its stack as strings
            for (int i = keyList.Count - remainderSize; i < keyList.Count; i++)
            {
                EnqueueAndPadCountryOption(ref columnThree, keyList.ElementAt(i));
            }

            while (columnOne.Count + columnTwo.Count + columnThree.Count != 0)
            {
                if (columnOne.Count != 0)
                {
                    finalList += columnOne.Dequeue() + "\t";
                }
                if (columnTwo.Count != 0)
                {
                    finalList += columnTwo.Dequeue() + "\t";
                }
                if (columnThree.Count != 0)
                {
                    finalList += columnThree.Dequeue();
                }

                finalList += Environment.NewLine;
            }

            return finalList;
        }

        /// <summary>
        /// Adds the given country to the given queue object, essentially just calling FormatCountryOption
        /// </summary>
        /// <param name="queue">The queue into which to insert</param>
        /// <param name="country">The key-value pair to format</param>
        private static void EnqueueAndPadCountryOption(ref Queue<string> queue, KeyValuePair<int, string> country)
        {
            string countryOption = FormatCountryOption(country.Value, country.Key);

            queue.Enqueue(countryOption);
        }

        /// <summary>
        /// Converts the Country Enum into a Dictionary object
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> CountryKeys()
        {
            var countryDict = new Dictionary<int, string>();

            foreach (var name in Enum.GetNames(typeof(Country)))
            {
                countryDict.Add((int)Enum.Parse(typeof(Country), name), name);
            }

            return countryDict;
        }

        /// <summary>
        /// Trims or pads country names and IDs as needed to make them consistent
        /// </summary>
        /// <param name="name">The name of the country</param>
        /// <param name="id">The ID number of the country</param>
        /// <param name="trim">Whether or not we should trim or just naively concatenate, defaults to yes</param>
        /// <returns></returns>
        private static string FormatCountryOption(string name, int id, bool trim=true)
        {           
            if (trim)
            {
                // Trim the name to fit in the allowed space
                name = name.Length > MAX_COUNTRY_NAME_WIDTH ? name.Substring(0, MAX_COUNTRY_NAME_WIDTH) : name.PadRight(MAX_COUNTRY_NAME_WIDTH,'.');
            }            

            return name + "." + Convert.ToString(id).PadLeft(3,'.');
        }

        public static State GetStateByName(string stateName)
        {
            foreach (var state in StateNames)
            {
                if(state.Value == stateName.ToUpper())
                {
                    return state.Key;
                }
            }

            throw new ArgumentException($"{stateName} is not one of the fifty states in the United States of America!");
        }

        public static State GetStateByCode(string stateCode)
        {
            foreach(var state in StateNames)
            {
                if(state.Key.ToString() == stateCode.ToUpper())
                {
                    return state.Key;
                }
            }

            throw new ArgumentException($"{stateCode} is not one of the fifty states in the United States of America!");
        }

        public static Country GetCountryByTwoLetterCode(string twoLetterCode)
        {
            if (twoLetterCode.Length == 2)
            {
                string countryCommandString = "SELECT CountryCode, TwoLetterCode FROM Country WHERE TwoLetterCode = @code";
                SqlCommand sqlCommand = new SqlCommand(countryCommandString);

                SqlConnection connection = new SqlConnection(PhoneDirectory.CONNECTION_STRING);

                using (connection)
                {
                    connection.Open();

                    var reader = sqlCommand.ExecuteReader();

                    using (reader)
                    {
                        return (Country)reader.GetInt32(0);
                    }
                }
            }

            else
            {
                throw new ArgumentException("Could not find reuqested country.");
            }
        }
    }

    public enum State
    {
        NA, AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY
    }

    public enum Country
    {
        Afghanistan = 4, Albania = 8, Antarctica = 10, Algeria = 12, American_Samoa = 16, Andorra = 20, Angola = 24, Antigua_and_Barbuda = 28, Azerbaijan = 31, Argentina = 32, Australia = 36, Austria = 40, Bahamas = 44, Bahrain = 48, Bangladesh = 50, Armenia = 51, Barbados = 52, Belgium = 56, Bermuda = 60, Bhutan = 64, Bolivia = 68, Bosnia_and_Herzegovina = 70, Botswana = 72, Bouvet_Island = 74, Brazil = 76, Belize = 84, British_Indian_Ocean_Territory = 86, Solomon_Islands = 90, British_Virgin_Islands = 92, Brunei_Darussalam = 96, Bulgaria = 100, Myanmar = 104, Burundi = 108, Belarus = 112, Cambodia = 116, Cameroon = 120, Canada = 124, Cape_Verde = 132, Cayman_Islands = 136, Central_African_Republic = 140, Sri_Lanka = 144, Chad = 148, Chile = 152, China = 156, Taiwan = 158, Christmas_Island = 162, Cocos_Islands = 166, Colombia = 170, Comoros = 174, Mayotte = 175, Republic_of_the_Congo = 178, Democratic_Republic_of_the_Congo = 180, Cook_Islands = 184, Costa_Rica = 188, Croatia = 191, Cuba = 192, Cyprus = 196, Czech_Republic = 203, Benin = 204, Denmark = 208, Dominica = 212, Dominican_Republic = 214, Ecuador = 218, El_Salvador = 222, Equatorial_Guinea = 226, Ethiopia = 231, Eritrea = 232, Estonia = 233, Faroe_Islands = 234, Falkland_Islands = 238, South_Georgia_and_the_South_Sandwich_Islands = 239, Fiji = 242, Finland = 246, France = 250, French_Guiana = 254, French_Polynesia = 258, French_Southern_Territories = 260, Djibouti = 262, Gabon = 266, Georgia = 268, Gambia = 270, Palestine = 275, Germany = 276, Ghana = 288, Gibraltar = 292, Kiribati = 296, Greece = 300, Greenland = 304, Grenada = 308, Guadeloupe = 312, Guam = 316, Guatemala = 320, Guinea = 324, Guyana = 328, Haiti = 332, Heard_and_Mcdonald_Islands = 334, Vatican = 336, Honduras = 340, Hong_Kong, SAR_China = 344, Hungary = 348, Iceland = 352, India = 356, Indonesia = 360, Iran = 364, Iraq = 368, Ireland = 372, Israel = 376, Italy = 380, Ivory_Coast = 384, Jamaica = 388, Japan = 392, Kazakhstan = 398, Jordan = 400, Kenya = 404, North_Korea = 408, South_Korea = 410, Kuwait = 414, Kyrgyzstan = 417, Laos = 418, Lebanon = 422, Lesotho = 426, Latvia = 428, Liberia = 430, Libya = 434, Liechtenstein = 438, Lithuania = 440, Luxembourg = 442, Macao, _SAR_China = 446, Madagascar = 450, Malawi = 454, Malaysia = 458, Maldives = 462, Mali = 466, Malta = 470, Martinique = 474, Mauritania = 478, Mauritius = 480, Mexico = 484, Monaco = 492, Mongolia = 496, Moldova = 498, Montenegro = 499, Montserrat = 500, Morocco = 504, Mozambique = 508, Oman = 512, Namibia = 516, Nauru = 520, Nepal = 524, Netherlands = 528, Netherlands_Antilles = 530, Aruba = 533, New_Caledonia = 540, Vanuatu = 548, New_Zealand = 554, Nicaragua = 558, Niger = 562, Nigeria = 566, Niue = 570, Norfolk_Island = 574, Norway = 578, Northern_Mariana_Islands = 580, US_Minor_Outlying_Islands = 581, Micronesia = 583, Marshall_Islands = 584, Palau = 585, Pakistan = 586, Panama = 591, Papua_New_Guinea = 598, Paraguay = 600, Peru = 604, Philippines = 608, Pitcairn = 612, Poland = 616, Portugal = 620, Guinea_Bissau = 624, Timor_Leste = 626, Puerto_Rico = 630, Qatar = 634, Réunion = 638, Romania = 642, Russia = 643, Rwanda = 646, Saint_Barthelemy = 652, Saint_Helena = 654, Saint_Kitts_and_Nevis = 659, Anguilla = 660, Saint_Lucia = 662, Saint_Martin = 663, Saint_Pierre_and_Miquelon = 666, Saint_Vincent_and_Grenadines = 670, San_Marino = 674, Sao_Tome_and_Principe = 678, Saudi_Arabia = 682, Senegal = 686, Serbia = 688, Seychelles = 690, Sierra_Leone = 694, Singapore = 702, Slovakia = 703, Vietnam = 704, Slovenia = 705, Somalia = 706, South_Africa = 710, Zimbabwe = 716, Spain = 724, South_Sudan = 728, Western_Sahara = 732, Sudan = 736, Suriname = 740, Svalbard_and_Jan_Mayen_Islands = 744, Swaziland = 748, Sweden = 752, Switzerland = 756, Syria = 760, Tajikistan = 762, Thailand = 764, Togo = 768, Tokelau = 772, Tonga = 776, Trinidad_and_Tobago = 780, United_Arab_Emirates = 784, Tunisia = 788, Turkey = 792, Turkmenistan = 795, Turks_and_Caicos_Islands = 796, Tuvalu = 798, Uganda = 800, Ukraine = 804, Macedonia = 807, Egypt = 818, United_Kingdom = 826, Guernsey = 831, Jersey = 832, Isle_of_Man = 833, Tanzania = 834, United_States = 840, Virgin_Islands = 850, Burkina_Faso = 854, Uruguay = 858, Uzbekistan = 860, Venezuela = 862, Wallis_and_Futuna_Islands = 876, Samoa = 882, Yemen = 887, Zambia = 894
    }
}
