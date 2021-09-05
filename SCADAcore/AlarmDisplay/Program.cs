using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace AlarmDisplay
{
    public class Callback : AlarmServiceReference.IAlarmDisplayCallback
    { 
        public void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static AlarmServiceReference.AlarmDisplayClient service;
        static void Main(string[] args)
        {
            InstanceContext ic = new InstanceContext(new Callback());
            service = new AlarmServiceReference.AlarmDisplayClient(ic);
            service.InitAlarmService();

            Console.ReadKey();
        }
    }
}
