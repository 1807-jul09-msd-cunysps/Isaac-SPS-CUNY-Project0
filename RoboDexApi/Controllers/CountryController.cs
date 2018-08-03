using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Cors;
using PhoneDirectoryLibrary;

namespace RoboDexApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class CountryController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string countryLookup = "")
        {
            Regex regex = new Regex("[0-9]+");

            // Get all the countries
            if (countryLookup.Length == 0)
            {
                return Json(Lookups.CountryKeys());
            }
            // Lookup by country code
            else if (regex.IsMatch(countryLookup))
            {
                return Json<string>(Lookups.CountryKeys()[Convert.ToInt32(countryLookup)]);
            }
            else
            {
                return Json<int>((int)(Country)Enum.Parse(typeof(Country), countryLookup));
            }
        }
    }
}
