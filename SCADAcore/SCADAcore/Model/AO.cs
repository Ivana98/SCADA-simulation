using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SCADAcore.Model
{
    [DataContract]
    // (Analog Output)
    public class AO
    {
        [Key]
        [DataMember]
        public string TagName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public double InitialValue { get; set; }
        [DataMember]
        public double LowLimit { get; set; }
        [DataMember]
        public double HighLimit { get; set; }

        public AO() { }
        public AO(string id, string description, string address, double initvalue, double lowLimit, double highLimit)
        {
            TagName = id;
            Description = description;
            Address = address;
            InitialValue = initvalue;
            LowLimit = lowLimit;
            HighLimit = highLimit;
        }

        public override string ToString()
        {
            return $"Analog Output - {TagName}\n Description: {Description}\n Address: {Address}\n Initial value: {InitialValue}\n Low limit: {LowLimit}\n High Limit: {HighLimit}\n";
        }
    }
}