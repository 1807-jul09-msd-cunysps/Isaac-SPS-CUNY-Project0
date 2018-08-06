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
    public class MessageController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string id = "")
        {
            if(id.Length == 0)
            {
                return Json<Dictionary<Contact, List<Message>>>(Message.Get());
            }
            else
            {
                Guid messageID;

                if (Guid.TryParse(id, out messageID))
                {
                    return Json(Message.Get(messageID));
                }
                else
                {
                    return BadRequest("No contacts with that id.");
                }
            }
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody]Tuple<Contact, Message> id)
        {
            if (id == null || id.Item1 == null || id.Item2 == null)
            {
                return BadRequest("No data supplied.");
            }
            else
            {
                PhoneDirectory phoneDirectory = new PhoneDirectory();

                if (!phoneDirectory.ContactExistsInDB(id.Item1))
                {
                    phoneDirectory.Add(id.Item1);
                }

                return Json(id.Item2.InsertMessage());
            }
        }
    }
}
