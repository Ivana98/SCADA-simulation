using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SCADAcore.Model
{
    public class TagValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime ModificationTime { get; set; }
        public double Value { get; set; }
        public string TagName { get; set; }
        public TagType TagType { get; set; }

        public TagValue() { }

        public TagValue(int id, DateTime time, double value, string tagname, TagType tagtype)
        {
            Id = id;
            ModificationTime = time;
            Value = value;
            TagName = tagname;
            TagType = tagtype;
        }

        public override string ToString()
        {
            return $"{TagType} - {TagName} with value: {Value}; time: {ModificationTime}";
        }
    }
}