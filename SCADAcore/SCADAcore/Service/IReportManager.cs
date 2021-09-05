using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IReportManager" in both code and config file together.
    [ServiceContract]
    public interface IReportManager
    {
        [OperationContract]
        string AllAlarmsByTime(DateTime startDate, DateTime endDate);   // sort by time, priority
        [OperationContract]
        string AllAlarmsByPriority(int priority);   // sort by time
        [OperationContract]
        string AllTagsByTime(DateTime startDate, DateTime endDate); // sort by time
        [OperationContract]
        string AllAI();     // sort by time
        [OperationContract]
        string AllDI();     // sort by time
        [OperationContract]
        string AllTagById(string id);   // sort by value
    }
}
