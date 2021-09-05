using SCADAcore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using SCADAcore.Service;
using System.IO;
using SimulationDriver;
using RealTimeDriver;

namespace SCADAcore
{
    
     //Tag Processing omogucava pravovremeno ocitavanje vrednosti tagova sa  odredjenih I/O adresa
     //i generise neophodne dogadjaje za njihov prikaz u Trending aplikaciji
     
    public static class TagProcessing
    {
        public static Dictionary<string, AI> AIdict { get; set; }
        public static Dictionary<string, DI> DIdict { get; set; }

        public static Dictionary<string, Thread> AIthreads { get; set; }
        public static Dictionary<string, Thread> DIthreads { get; set; }

        public static Context db;
        public static Trending trendingService;
        public static AlarmDisplay alarmService;
        private readonly static object locker = new object();
        private static string alarmLogPath;

        static TagProcessing()
        {
            alarmLogPath = HttpContext.Current.Server.MapPath("../alarmsLog.txt");
            db = SCADAconfig.LoadData();

            AIdict = new Dictionary<string, AI>();
            DIdict = new Dictionary<string, DI>();
            AIthreads = new Dictionary<string, Thread>();
            DIthreads = new Dictionary<string, Thread>();

            trendingService = new Trending();
            alarmService = new AlarmDisplay();

            foreach (AI a in db.AIset)
            {
                AIdict.Add(a.TagName, a);
                AIthreads.Add(a.TagName, new Thread(() => ReadAI(a.TagName)));
            }

            foreach (DI a in db.DIset)
            {
                DIdict.Add(a.TagName, a);
                DIthreads.Add(a.TagName, new Thread(() => ReadDI(a.TagName)));
            }

            foreach (Thread t in AIthreads.Values)
            {
                t.Start();
            }

            foreach (Thread t in DIthreads.Values)
            {
                t.Start();
            }

            Thread saveThread = new Thread(() =>
            {
                while (true)
                {
                    lock (db)
                    {
                        db.SaveChanges();
                    }
                    Thread.Sleep(10000);
                }
            });

            saveThread.Start();
        }

        public static void ReadAI(string id)
        {
            double oldVal = -10000;
            while (true)
            {
                AI tag;
                lock (db.AIset)
                {
                    try
                    {
                        tag = AIdict[id];
                    }
                    catch(KeyNotFoundException)
                    {
                        return;
                    }

                    double value;
                    if (tag.OnOffScan)
                    {
                        if (tag.Driver.Equals(Driver.SD))
                            value = SimulationDriver.SimulationDriver.ReturnValue(tag.Address);
                        else
                            value = RealTimeDriver.RealTimeDriver.ReturnValue(tag.Address);

                        CutValue(ref value, tag.LowLimit, tag.HighLimit, out AlarmType type);

                        lock (locker)
                        {
                            db.TagValues.Add(new TagValue {
                                ModificationTime = DateTime.Now,
                                Value = value,
                                TagName = tag.TagName,
                                TagType = TagType.AI
                            });
                            db.SaveChanges();
                        }

                        foreach (Alarm a in tag.Alarms)
                        {
                            if ((type.Equals(AlarmType.LOW) && a.Type.Equals(AlarmType.LOW)) ||
                                (type.Equals(AlarmType.HIGH) && a.Type.Equals(AlarmType.HIGH)))
                            {
                                lock (locker)
                                {
                                    WriteToAlarmsLog($"AI tag with ID: {tag.TagName} activated alarm of type: {a.Type.ToString()}; Alarm occurred: {DateTime.Now}");
                                    int priority = (int)a.Priority;
                                    for (int i = 0; i < priority; i++)
                                    {
                                        lock (locker)
                                        {
                                            if (!(alarmService is null))
                                                alarmService.Write($"WARNING! AI tag  ID: {tag.TagName} is {a.Type.ToString()}");
                                        }
                                    }
                                }
                            }
                        }

                        if (!(trendingService is null) && oldVal != value)
                            trendingService.Write($"AI tag\t ID: {tag.TagName}; VALUE: {value} ");
                        oldVal = value;
                    }
                }

                if (!(tag is null))
                {
                    Thread.Sleep(tag.ScanTime * 1000);
                }
            }
        }

        public static void ReadDI(string id)
        {
            double oldVal = -10000;
            while (true)
            {
                DI tag;
                lock (db.DIset)
                {
                    try
                    {
                        tag = DIdict[id];
                    }
                    catch(KeyNotFoundException)
                    {
                        return;
                    }

                    double value;
                    if (tag.OnOffScan)
                    {
                        if (tag.Driver.Equals(Driver.SD))
                            value = SimulationDriver.SimulationDriver.ReturnValue(tag.Address);
                        else
                            value = RealTimeDriver.RealTimeDriver.ReturnValue(tag.Address);

                        CutValue(ref value, 0, 1, out AlarmType type);

                        lock (locker)
                        {
                            db.TagValues.Add(new TagValue
                            {
                                ModificationTime = DateTime.Now,
                                Value = value,
                                TagName = tag.TagName,
                                TagType = TagType.DI
                            });
                            
                            db.SaveChanges();
                        }

                        if (!(trendingService is null) && oldVal != value)
                            trendingService.Write($"DI tag\t ID: {tag.TagName}; VALUE: {value} ");
                        oldVal = value;
                    }
                }

                if (!(tag is null))
                {
                    Thread.Sleep(tag.ScanTime * 1000);
                }
            }
        }

        private static void CutValue(ref double value, double lowLimit, double highLimit, out AlarmType type)
        {
            if (value > highLimit)
            {
                value = highLimit;
                type = AlarmType.HIGH;
                return;
            }
            else if (value < lowLimit)
            {
                value = lowLimit;
                type = AlarmType.LOW;
                return;
            }
            type = AlarmType.NOT_ACTIVATED;
        }

        private static void WriteToAlarmsLog(string message)
        {
            StreamWriter file = new StreamWriter(alarmLogPath, append: true);
            file.WriteLine(message);
            file.Close();
        }

    }

}