using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "AlarmDisplay" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select AlarmDisplay.svc or AlarmDisplay.svc.cs at the Solution Explorer and start debugging.
    public class AlarmDisplay : IAlarmDisplay
    {
        delegate void alarmDelegate(string message);
        static event alarmDelegate OnAlarm;
        IAlarmCallback proxy;

        public void InitAlarmService()
        {
            proxy = OperationContext.Current.GetCallbackChannel<IAlarmCallback>();
            OnAlarm += proxy.WriteToConsole;
            TagProcessing.alarmService = this;
        }

        public void Write(string message)
        {
            try
            {
                if (OnAlarm != null)
                {
                    OnAlarm.Invoke(message);
                }
            }
            catch
            {
                return;
            }
        }

    }

}
