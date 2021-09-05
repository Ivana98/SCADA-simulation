using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Trending
{
    public class Callback : TrendingServiceReference.ITrendingCallback
    {
        public void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }

    class Program
    {
        static TrendingServiceReference.TrendingClient service;

        static void Main(string[] args)
        {
            InstanceContext ic = new InstanceContext(new Callback());
            service = new TrendingServiceReference.TrendingClient(ic);
            service.InitTrendingService();

            Console.ReadKey();
        }
    }
}
