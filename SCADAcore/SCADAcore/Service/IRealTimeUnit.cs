using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRealTimeUnit" in both code and config file together.
    [ServiceContract]
    public interface IRealTimeUnit
    {
        [OperationContract(IsOneWay = true)]
        void InitService(string keyPath);

        [OperationContract(IsOneWay = true)]
        void SendData(string address, string datastr, byte[] signature);
    }
}
