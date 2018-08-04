using System;
using System.Collections.Generic;
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
    }

    public enum State
    {
        NA, AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY
    }

    public enum Country
    {
        Afghanistan = 4, Albania = 8, Algeria = 12, American_Samoa = 16, Andorra = 20, Angola = 24, Anguilla = 660, Antarctica = 10, Antigua_and_Barbuda = 28, Argentina = 32, Armenia = 51, Aruba = 533, Australia = 36, Austria = 40, Azerbaijan = 31, Bahamas = 44, Bahrain = 48, Bangladesh = 50, Barbados = 52, Belarus = 112, Belgium = 56, Belize = 84, Benin = 204, Bermuda = 60, Bhutan = 64, Bolivia = 68, Bosnia_and_Herzegovina = 70, Botswana = 72, Brazil = 76, British_Indian_Ocean_Territory = 86, British_Virgin_Islands = 92, Bulgaria = 100, Burkina_Faso = 854, Burundi = 108, Cambodia = 116, Cameroon = 120, Canada = 124, Cape_Verde = 132, Cayman_Islands = 136, Central_African_Republic = 140, Chad = 148, Chile = 152, China = 156, Christmas_Island = 162, Colombia = 170, Cook_Islands = 174, Costa_Rica = 184, Croatia = 188, Cuba = 191, Curacao = 192, Czech_Republic = 203, Democratic_Republic_of_the_Congo = 178, Denmark = 208, Djibouti = 262, Dominica = 212, Dominican_Republic = 214, East_Timor = 626, Ecuador = 218, Egypt = 818, El_Salvador = 222, Equatorial_Guinea = 226, Eritrea = 232, Estonia = 233, Ethiopia = 231, Falkland_Islands = 238, Faroe_Islands = 234, Fiji = 242, Finland = 246, France = 250, French_Polynesia = 258, Gabon = 266, Gambia = 270, Georgia = 268, Germany = 276, Ghana = 288, Gibraltar = 292, Greece = 300, Greenland = 304, Grenada = 308, Guam = 316, Guatemala = 320, Guernsey = 831, Guinea = 324, Guinea_Bissau = 624, Guyana = 328, Haiti = 332, Honduras = 340, Hong_Kong = 344, Hungary = 348, Iceland = 352, India = 356, Indonesia = 360, Iran = 364, Iraq = 368, Ireland = 372, Isle_of_Man = 833, Israel = 376, Italy = 380, Ivory_Coast = 384, Jamaica = 388, Japan = 392, Jersey = 832, Jordan = 400, Kazakhstan = 398, Kenya = 404, Kiribati = 296, Kosovo = 999, Kuwait = 414, Kyrgyzstan = 417, Laos = 418, Latvia = 428, Lebanon = 422, Lesotho = 426, Liberia = 430, Libya = 434, Liechtenstein = 438, Lithuania = 440, Luxembourg = 442, Macau = 446, Macedonia = 807, Madagascar = 450, Malawi = 454, Malaysia = 458, Maldives = 462, Mali = 466, Malta = 470, Marshall_Islands = 584, Mauritania = 478, Mauritius = 480, Mayotte = 175, Mexico = 484, Micronesia = 583, Moldova = 498, Monaco = 492, Mongolia = 496, Montenegro = 499, Montserrat = 500, Morocco = 504, Mozambique = 508, Myanmar = 104, Namibia = 516, Nauru = 520, Nepal = 524, Netherlands = 528, Netherlands_Antilles = 530, New_Caledonia = 540, New_Zealand = 554, Nicaragua = 558, Niger = 562, Nigeria = 566, Niue = 570, North_Korea = 408, Northern_Mariana_Islands = 580, Norway = 578, Oman = 512, Pakistan = 586, Palau = 585, Palestine = 275, Panama = 591, Papua_New_Guinea = 598, Paraguay = 600, Peru = 604, Philippines = 608, Pitcairn = 612, Poland = 616, Portugal = 620, Puerto_Rico = 630, Qatar = 634, Republic_of_the_Congo = 180, Reunion = 638, Romania = 642, Russia = 643, Rwanda = 646, Saint_Barthelemy = 652, Saint_Helena = 654, Saint_Kitts_and_Nevis = 659, Saint_Lucia = 662, Saint_Martin = 663, Saint_Pierre_and_Miquelon = 666, Saint_Vincent_and_the_Grenadines = 670, Samoa = 882, San_Marino = 674, Sao_Tome_and_Principe = 678, Saudi_Arabia = 682, Senegal = 686, Serbia = 688, Seychelles = 690, Sierra_Leone = 694, Singapore = 702, Slovakia = 703, Solomon_Islands = 705, Somalia = 90, South_Africa = 706, South_Korea = 710, South_Sudan = 410, Spain = 728, Sri_Lanka = 724, Sudan = 144, Suriname = 736, Svalbard_and_Jan_Mayen = 740, Sweden = 748, Switzerland = 752, Syria = 756, Taiwan = 158, Tanzania = 762, Thailand = 764, Tokelau = 768, Tonga = 772, Trinidad_and_Tobago = 776, Tunisia = 780, Turkey = 788, Turkmenistan = 792, Turks_and_Caicos_Islands = 795, Tuvalu = 796, US_Virgin_Islands = 798, Uganda = 850, Ukraine = 800, United_Arab_Emirates = 804, United_Kingdom = 784, United_States = 826, Uruguay = 840, Uzbekistan = 858, Vanuatu = 860, Vatican = 548, Venezuela = 336, Vietnam = 862, Wallis_and_Futuna = 704, Western_Sahara = 876, Yemen = 732, Zambia = 887, Zimbabwe = 894
    }
}
