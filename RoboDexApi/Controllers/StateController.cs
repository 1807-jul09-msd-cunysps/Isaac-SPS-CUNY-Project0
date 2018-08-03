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
    public class StateController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string stateLookup = "")
        {
            try
            {
                Regex regex = new Regex("[0-9]+");

                // Get all states
                if (stateLookup.Length == 0)
                {
                    return Json<Dictionary<State, string>>(Lookups.StateNames);
                }
                // Lookup by numeric code
                else if (regex.IsMatch(stateLookup))
                {
                    return Json<KeyValuePair<string, string>>(
                        new KeyValuePair<string, string>(
                            Enum.Parse(typeof(State), stateLookup).ToString(),
                            Lookups.StateNames[Lookups.GetStateByCode(
                                Enum.Parse(typeof(State), stateLookup).ToString())]
                                ));
                }
                // Lookup by two-letter code
                else if (stateLookup.Length == 2)
                {
                    return Json<State>(Lookups.GetStateByCode(stateLookup));
                }
                // Lookup by name
                else
                {
                    return Json<State>(Lookups.GetStateByName(stateLookup));
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
