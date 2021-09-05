using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Trending" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Trending.svc or Trending.svc.cs at the Solution Explorer and start debugging.
    public class Trending : ITrending
    {
        delegate void trendingDelegate(string message);
        static event trendingDelegate OnScan;
        ITrendingCallback proxy;

        public void InitTrendingService()
        {
            proxy = OperationContext.Current.GetCallbackChannel<ITrendingCallback>();
            OnScan += proxy.WriteToConsole;
            TagProcessing.trendingService = this;
        }

        public void Write(string message)
        {
            try
            {
                if (OnScan != null)
                {
                    OnScan.Invoke(message);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
