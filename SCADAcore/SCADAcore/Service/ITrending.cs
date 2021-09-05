using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITrending" in both code and config file together.
    [ServiceContract(CallbackContract = typeof(ITrendingCallback))]
    public interface ITrending
    {
        [OperationContract(IsOneWay = true)]
        void InitTrendingService();
    }

    public interface ITrendingCallback
    {
        [OperationContract(IsOneWay = true)]
        void WriteToConsole(string tagstr);
    }
}
