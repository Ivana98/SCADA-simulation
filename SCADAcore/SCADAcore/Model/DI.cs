using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SCADAcore.Model
{
    // (Digital Input)
    [DataContract]
    public class DI
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
        public List<Alarm> Alarms { get; set; }
        [DataMember]
        public bool OnOffScan { get; set; }

        public DI() { }
        public DI(string id, string description, Driver driver, string address, int scantime, bool onoffscan)
        {
            TagName = id;
            Description = description;
            Driver = driver;
            Address = address;
            ScanTime = scantime;
            OnOffScan = onoffscan;
            Alarms = new List<Alarm>();
        }

        public override string ToString()
        {
            return $"Digital Input - {TagName}\n Description: {Description}\n Driver: {Driver.ToString()}\n Address: {Address}\n Scan time: {ScanTime}\n Scan activity: {(OnOffScan ? "On" : "Off")}";
        }
    }
}