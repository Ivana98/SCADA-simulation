using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IAlarmDisplay" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(IAlarmCallback))]
    public interface IAlarmDisplay
    {
        [OperationContract(IsOneWay = true)]
        void InitAlarmService();
    }

    public interface IAlarmCallback
    {
        [OperationContract(IsOneWay = true)]
        void WriteToConsole(string tagstr);
    }
}
