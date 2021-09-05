using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeDriver
{
    public static class RealTimeDriver
    {
        private static Dictionary<string, double> AddressDict = new Dictionary<string, double>();

        public static double ReturnValue(string address)
        {
            if (AddressDict.ContainsKey(address))
            {
                return AddressDict[address];
            }
            return -1;
        }

        public static void WriteValue(string address, double data)
        {
            if (AddressDict.ContainsKey(address))
            {
                AddressDict[address] = data;
            }
            else
            {
                AddressDict.Add(address, data);
            }
        }
    }
}
