using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using PhoneDirectoryLibrary;
using System.Web.Http.Cors;
using System.Web.Http.Results;

namespace RoboDexApi.Controllers
{
    [EnableCors("*", "*", "*")]
    public class ContactController : ApiController
    {
        PhoneDirectory phoneDirectory = new PhoneDirectory();

        [HttpGet]
        public JsonResult<IEnumerable<Contact>> Get(string contactId = "")
        {
            if (contactId.Length == 0)
            {
                return Json<IEnumerable<Contact>>(phoneDirectory.GetAll());
            }
            else
            {
                Guid contactGuid;

                if (Guid.TryParse(contactId, out contactGuid))
                {
                    List<Contact> contacts = new List<Contact>();
                    contacts.Add(phoneDirectory.GetContactFromDB(contactGuid));

                    return Json<IEnumerable<Contact>>(contacts);
                }

                else
                {
                    return null;
                }
            }
        }
    }
}
