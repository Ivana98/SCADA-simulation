using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ReportManager" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select ReportManager.svc or ReportManager.svc.cs at the Solution Explorer and start debugging.
    public class ReportManager : IReportManager
    {
        private readonly Context db = new Context();
        public string AllAlarmsByTime(DateTime startDate, DateTime endDate)
        {
            lock (db)
            {
                var filteredResult = (from alarm in db.Alarms
                                     where alarm.ActivationTime != null && alarm.ActivationTime > startDate && alarm.ActivationTime < endDate
                                     orderby alarm.ActivationTime, (int)alarm.Priority descending
                                     select alarm).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }          
        }
        public string AllAlarmsByPriority(int priority)
        {
            lock (db)
            {
                var filteredResult = (from alarm in db.Alarms
                                     where (int)alarm.Priority == priority
                                     orderby alarm.ActivationTime descending
                                     select alarm).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string AllTagsByTime(DateTime startDate, DateTime endDate)
        {
            lock (db)
            {
                var filteredResult = (from tag in db.TagValues
                                      where tag.ModificationTime != null && tag.ModificationTime > startDate && tag.ModificationTime < endDate
                                      orderby tag.ModificationTime descending
                                      select tag).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string AllAI()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.TagValues
                                      where tag.TagType.Equals(TagType.AI)
                                      orderby tag.ModificationTime descending
                                      select tag).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string AllDI()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.TagValues
                                      where tag.TagType.Equals(TagType.DI)
                                      orderby tag.ModificationTime descending
                                      select tag).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string AllTagById(string id)
        {
            lock (db)
            {
                var filteredResult = (from tag in db.TagValues
                                      where tag.TagName.Equals(id)
                                      orderby tag.Value descending
                                      select tag).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
    }
}
