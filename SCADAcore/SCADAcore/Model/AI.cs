using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SCADAcore.Model
{
    [DataContract]
    // (Analog Input)
    public class AI
    {
        [Key]
        [DataMember]
        public string TagName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Driver Driver { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public int ScanTime { get; set; }
        [DataMember]
        public virtual List<Alarm> Alarms { get; set; }
        [DataMember]
        public bool OnOffScan { get; set; }
        [DataMember]
        public double LowLimit { get; set; }
        [DataMember]
        public double HighLimit { get; set; }
        [DataMember]
        public string Units { get; set; }

        public AI() { }
        public AI(string id, string description, Driver driver, string address, int scantime, bool onoffscan, double lowLimit, double highLimit, string units)
        {
            TagName = id;
            Description = description;
            Driver = driver;
            Address = address;
            ScanTime = scantime;
            OnOffScan = onoffscan;
            LowLimit = lowLimit;
            HighLimit = highLimit;
            Units = units;
            Alarms = new List<Alarm>();
        }

        public override string ToString()
        {
            string alarms = string.Join(Environment.NewLine, (object[])Alarms.ToArray());
            return $"Analog Input - {TagName}\n Description: {Description}\n Driver: {Driver}\n Address: {Address}\n Scan time: {ScanTime}\n Scan activity: {(OnOffScan ? "On" : "Off")}\n Low limit: {LowLimit}\n High Limit: {HighLimit}\n Units: {Units}\n Number of alarms: {Alarms.Count}\n Alarms:\n{alarms}";
        }
    }
}