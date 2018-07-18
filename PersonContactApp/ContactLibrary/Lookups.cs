﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactLibrary
{
    public static class Lookups
    {
        public static string listCountries()
        {
            string list = "";
            foreach(string country in Enum.GetNames(typeof(Country)))
            {
                list += Enum.GetValues(typeof(Country)) + Environment.NewLine;
            }

            return list;
        }

        public static Dictionary<State, string> StateNames = new Dictionary<State, string>()
        {
            {State.NA, "Not in United States" }, {State.AL, "ALABAMA"}, {State.AK, "ALASKA"}, {State.AZ, "ARIZONA"}, {State.AR, "ARKANSAS"}, {State.CA, "CALIFORNIA"}, {State.CO, "COLORADO"}, {State.CT, "CONNECTICUT"}, {State.DE, "DELAWARE"}, {State.FL, "FLORIDA"}, {State.GA, "GEORGIA"}, {State.HI, "HAWAII"}, {State.ID, "IDAHO"}, {State.IL, "ILLINOIS"}, {State.IN, "INDIANA"}, {State.IA, "IOWA"}, {State.KS, "KANSAS"}, {State.KY, "KENTUCKY"}, {State.LA, "LOUISIANA"}, {State.ME, "MAINE"}, {State.MD, "MARYLAND"}, {State.MA, "MASSACHUSETTS"}, {State.MI, "MICHIGAN"}, {State.MN, "MINNESOTA"}, {State.MS, "MISSISSIPPI"}, {State.MO, "MISSOURI"}, {State.MT, "MONTANA"}, {State.NE, "NEBRASKA"}, {State.NV, "NEVADA"}, {State.NH, "NEW HAMPSHIRE"}, {State.NJ, "NEW JERSEY"}, {State.NM, "NEW MEXICO"}, {State.NY, "NEW YORK"}, {State.NC, "NORTH CAROLINA"}, {State.ND, "NORTH DAKOTA"}, {State.OH, "OHIO"}, {State.OK, "OKLAHOMA"}, {State.OR, "OREGON"}, {State.PA, "PENNSYLVANIA"}, {State.RI, "RHODE ISLAND"}, {State.SC, "SOUTH CAROLINA"}, {State.SD, "SOUTH DAKOTA"}, {State.TN, "TENNESSEE"}, {State.TX, "TEXAS"}, {State.UT, "UTAH"}, {State.VT, "VERMONT"}, {State.VA, "VIRGINIA"}, {State.WA, "WASHINGTON"}, {State.WV, "WEST VIRGINIA"}, {State.WI, "WISCONSIN"}, {State.WY, "WYOMING"}
        }; 
    }

    public enum State
    {
        NA, AL, AK, AZ, AR, CA, CO, CT, DE, FL, GA, HI, ID, IL, IN, IA, KS, KY, LA, ME, MD, MA, MI, MN, MS, MO, MT, NE, NV, NH, NJ, NM, NY, NC, ND, OH, OK, OR, PA, RI, SC, SD, TN, TX, UT, VT, VA, WA, WV, WI, WY
    }
    public enum Country
    {
        Andorra = 1, Afghanistan = 2, Antigua_and_Barbuda = 3, Anguilla = 4, Albania = 5, Armenia = 6, Angola = 7, Antarctica = 8, Argentina = 9, American_Samoa = 10, Australia = 11, Aruba = 12, Azerbaijan = 14, Austria = 15, Algeria = 16, Bahamas = 17, Bahrain = 18, Bangladesh = 19, Barbados = 20, Belarus = 21, Belgium = 22, Belize = 23, Benin = 24, Bermuda = 25, Bhutan = 26, Plurinational_State_of_Bolivia = 27, Sint_Eustatius_and_Saba_Bonaire = 28, Bosnia_and_Herzegowina = 29, Botswana = 30, Bouvet_Island = 31, Brazil = 32, British_Indian_Ocean_Territory = 33, Brunei_Darussalam = 34, Bulgaria = 35, Burkina_Faso = 36, Burundi = 37, Cambodia = 38, Cameroon = 39, Canada = 40, Cape_Verde = 41, Cayman_Islands = 42, Central_African_Republic = 43, Chad = 44, Chile = 45, China = 46, Christmas_Island = 47, Cocos__Islands = 48, Colombia = 49, Comoros = 50, Congo = 51, The_Democratic_Republic_of_The_Congo = 52, Cook_Islands = 53, Costa_Rica = 54, CÃŽte_dIvoire = 55, Croatia_ = 56, Cuba = 57, Curacao = 58, Cyprus = 59, Czech_Republic = 60, Denmark = 61, Djibouti = 62, Dominica = 63, Dominican_Republic = 64, Ecuador = 65, Egypt = 66, El_Salvador = 67, Equatorial_Guinea = 68, Eritrea = 69, Estonia = 70, Ethiopia = 71, Falkland_Islands_ = 72, Faroe_Islands = 73, Fiji = 74, Finland = 75, France = 76, French_Guiana = 77, French_Polynesia = 78, French_Southern_Territories = 79, Gabon = 80, Gambia = 81, Georgia = 82, Germany = 83, Ghana = 84, Gibraltar = 85, Greece = 86, Greenland = 87, Grenada = 88, Guadeloupe = 89, Guam = 90, Guatemala = 91, Guernsey = 92, Guinea = 93, Guinea_bissau = 94, Guyana = 95, Haiti = 96, Heard_and_McDonald_Islands = 97, Holy_See_ = 98, Honduras = 99, Hong_Kong = 100, Hungary = 101, Iceland = 102, India = 103, Indonesia = 104, Iran_ = 105, Iraq = 106, Ireland = 107, Isle_of_Man = 108, Israel = 109, Italy = 110, Jamaica = 111, Japan = 112, Jersey = 113, Jordan = 114, Kazakhstan = 115, Kenya = 116, Kiribati = 117, Democratic_Peoples_Republic_of_Korea = 118, Republic_of_Korea = 119, Kuwait = 120, Kyrgyzstan = 121, Lao_Peoples_Democratic_Republic = 122, Latvia = 123, Lebanon = 124, Lesotho = 125, Liberia = 126, Libya = 127, Liechtenstein = 128, Lithuania = 129, Luxembourg = 130, Macao = 131, The_Former_Yugoslav_Republic_of_Macedonia = 132, Madagascar = 133, Malawi = 134, Malaysia = 135, Maldives = 136, Mali = 137, Malta = 138, Marshall_Islands = 139, Martinique = 140, Mauritania = 141, Mauritius = 142, Mayotte = 143, Mexico = 144, Federated_States_of_Micronesia = 145, Republic_of_Moldova = 146, Monaco = 147, Mongolia = 148, Montenegro = 149, Montserrat = 150, Morocco = 151, Mozambique = 152, Myanmar = 153, Namibia = 154, Nauru = 155, Nepal = 156, Netherlands = 157, New_Caledonia = 158, New_Zealand = 159, Nicaragua = 160, Niger = 161, Nigeria = 162, Niue = 163, Norfolk_Island = 164, Northern_Mariana_Islands = 165, Norway = 166, Oman = 167, Pakistan = 168, Palau = 169, State_of_Palestine = 170, Panama = 171, Papua_New_Guinea = 172, Paraguay = 173, Peru = 174, Philippines = 175, Pitcairn = 176, Poland = 177, Portugal = 178, Puerto_Rico = 179, Qatar = 180, Reunion = 181, Romania = 182, Russian_Federation = 183, Rwanda = 184, Ascension_and_Tristan_Da_Cunha_Saint_Helena = 185, Saint_Barthalemy = 186, Saint_Kitts_and_Nevis = 187, Saint_Lucia = 188, Saint_Pierre_and_Miquelon = 189, Saint_Vincent_and_The_Grenadines = 190, Samoa = 191, San_Marino = 192, Sao_Tome_and_Principe = 193, Saudi_Arabia = 194, Senegal = 195, Serbia = 196, Seychelles = 197, Sierra_Leone = 198, Singapore = 199, Sint_Maarten_ = 200, Slovakia = 201, Slovenia = 202, Solomon_Islands = 203, Somalia = 204, South_Africa = 205, South_Georgia_and_The_South_Sandwich_Islands = 206, South_Sudan = 207, Spain = 208, Sri_Lanka = 209, Sudan = 210, Suriname = 211, Svalbard_and_Jan_Mayen_Islands = 212, Swaziland = 213, Sweden = 214, Switzerland = 215, Syrian_Arab_Republic = 216, Province_of_China_Taiwan = 217, Tajikistan = 218, United_Republic_of_Tanzania = 219, Thailand = 220, Timor_leste = 221, Togo = 222, Tokelau = 223, Tonga = 224, Trinidad_and_Tobago = 225, Tunisia = 226, Turkey = 227, Turkmenistan = 228, Turks_and_Caicos_Islands = 229, Tuvalu = 230, Uganda = 231, Ukraine = 232, United_Arab_Emirates = 233, United_Kingdom = 234, United_States = 235, United_States_Minor_Outlying_Islands = 236, Uruguay = 237, Uzbekistan = 238, Vanuatu = 239, Bolivarian_Republic_of_Venezuela = 240, Vietnam = 241, Virgin_Islands_British = 242, Virgin_Islands_US = 243, Wallis_and_Futuna_Islands = 244, Western_Sahara = 245, Yemen = 246, Zambia = 247, Zimbabwe = 248
    }

}