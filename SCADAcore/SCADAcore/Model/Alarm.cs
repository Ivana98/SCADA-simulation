using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace SCADAcore.Model
{
    public class Alarm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public AlarmType Type { get; set; }

        public double CriticalValue { get; set; }

        public string Unit { get; set; }

        public DateTime? ActivationTime { get; set; }

        public AlarmPriority Priority { get; set; }

        public Alarm() { }
        public Alarm(int id, AlarmType type, double value, string unit, DateTime? date, AlarmPriority priority) 
        {
            Id = id;
            Type = type;
            CriticalValue = value;
            Unit = unit;
            ActivationTime = date;
            Priority = priority;
        }

        public Alarm(AlarmType type, double value, string unit, AlarmPriority priority)
        {
            Type = type;
            CriticalValue = value;
            Unit = unit;
            ActivationTime = null;
            Priority = priority;
        }

        public override string ToString()
        {
            String time = ActivationTime?.ToString("MM/dd/yyyy h:mm tt");
            time = (time is null) ? "" : time;
            return $"Alarm {Id} - Type: {Type} with priority: {Priority}; Detected value: {CriticalValue} {Unit}; Activation time: {time}";
        }

    }
}