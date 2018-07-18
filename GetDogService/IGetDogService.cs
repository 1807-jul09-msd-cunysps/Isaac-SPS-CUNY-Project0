using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GetDogService
{
    /// <summary>
    /// This service returns one or more random dogs.
    /// </summary>
    [ServiceContract]
    public interface IGetDogService
    {
        [OperationContract]
        string GetDog();

        [OperationContract]
        string GetDogs(int count);
    }
}
