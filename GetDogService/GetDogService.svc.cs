using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GetDogService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class GetDogService : IGetDogService
    {
        public string GetDog()
        {
            return "Rover is a dog. Rover is a good boy.";
        }

        public string GetDogs(int count)
        {
            string output = "";

            string[] names = { "Rover", "Burke", "Bailey", "Spot", "Mr. Peanut Butter" };

            Random random = new Random();

            for(int i = 0; i < count; i++)
            {
                string name = names[random.Next(0, names.Length - 1)];
                output += $"{name} is a dog. {name} is a good boy.{Environment.NewLine}";
            }

            return output;
        }
    }
}
