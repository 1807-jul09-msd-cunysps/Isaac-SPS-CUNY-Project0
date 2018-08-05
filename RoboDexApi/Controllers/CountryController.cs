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
        public IHttpActionResult Get(string id = "")
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            Regex regex = new Regex("[0-9]+");

            try
            {
                // Get all the countries
                if (id.Length == 0)
                {
                    return Json(Lookups.CountryKeys());
                }
                // Lookup by country code
                else if (regex.IsMatch(id))
                {
                    return Json<string>(Lookups.CountryKeys()[Convert.ToInt32(id)]);
                }
                else
                {
                    return Json<int>((int)(Country)Enum.Parse(typeof(Country), id));
                }
            }
            catch (KeyNotFoundException e)
            {
                logger.Error(e.Message);
                return BadRequest("Lookup failed.");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}
