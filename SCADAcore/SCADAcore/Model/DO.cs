using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SCADAcore.Model
{
    [DataContract]
    // (Digital Output)
    public class DO
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

        public DO() { }
        public DO(string id, string description, string address, double initvalue)
        {
            TagName = id;
            Description = description;
            Address = address;
            InitialValue = initvalue;
        }

        public override string ToString()
        {
            return $"Digital Output - {TagName}\n Description: {Description}\n Address: {Address}\n Initial value: {InitialValue}\n";
        }
    }
}