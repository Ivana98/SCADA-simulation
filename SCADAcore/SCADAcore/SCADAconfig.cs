using SCADAcore.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace SCADAcore
{
    // works with SCADAconfig.xml file
    public static class SCADAconfig
    {
        private static readonly object locker = new object();
        public static Context db = new Context();
        //public static string configPath = HttpContext.Current.Server.MapPath("../SCADAConfig.xml");
        public static string configPath = "";


        public static Context LoadData()
        {
            //load from xml
            lock (locker)
            {
                configPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\", "SCADAConfig.xml"));
                //configPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\", "SCADAConfig-empty.xml"));
                XElement xmlData = XElement.Load(configPath);

                db.Users.RemoveRange(db.Users);
                db.AIset.RemoveRange(db.AIset);
                db.AOset.RemoveRange(db.AOset);
                db.DIset.RemoveRange(db.DIset);
                db.DOset.RemoveRange(db.DOset);
                db.Alarms.RemoveRange(db.Alarms);

                db.SaveChanges();

                var userXml = xmlData.Descendants("User");
                List<User> userList = new List<User>();
                foreach (XElement node in userXml)
                {
                    try
                    {
                        string username = (string)node;
                        string password = (string)node.Attribute("Password");
                        string role = (string)node.Attribute("Role");

                        userList.Add(new User(username, password, (UserRole)Enum.Parse(typeof(UserRole), role)));
                    }
                    catch
                    {
                        continue;
                    }
                }

                var AIXml = xmlData.Descendants("AI");
                List<AI> AIlist = new List<AI>();
                foreach (XElement node in AIXml)
                {
                    try
                    {
                        string tagName = (string)node;
                        string description = (string)node.Attribute("Description");
                        string driver = (string)node.Attribute("Driver"); 
                        string address = (string)node.Attribute("Address");
                        int scantime = (int)node.Attribute("ScanTime");
                        bool onoffscan = (bool)node.Attribute("OnOffScan");
                        double lowlimit = (double)node.Attribute("LowLimit");
                        double highlimit = (double)node.Attribute("HighLimit");
                        string units = (string)node.Attribute("Units");

                        AIlist.Add(new AI(tagName, description, (Driver)Enum.Parse(typeof(Driver), driver), address, scantime, onoffscan, lowlimit, highlimit, units));
                    }
                    catch
                    {
                        continue;
                    }
                }

                var DIXml = xmlData.Descendants("DI");
                List<DI> DIlist = new List<DI>();
                foreach (XElement node in DIXml)
                {
                    try
                    {
                        string tagName = (string)node;
                        string description = (string)node.Attribute("Description");
                        string driver = (string)node.Attribute("Driver"); 
                        string address = (string)node.Attribute("Address");
                        int scantime = (int)node.Attribute("ScanTime");
                        bool onoffscan = (bool)node.Attribute("OnOffScan");

                        DIlist.Add(new DI(tagName, description, (Driver)Enum.Parse(typeof(Driver), driver), address, scantime, onoffscan));
                    }
                    catch
                    {
                        continue;
                    }
                }

                var AOXml = xmlData.Descendants("AO");
                List<AO> AOlist = new List<AO>();
                foreach (XElement node in AOXml)
                {
                    try
                    {
                        string tagName = (string)node;
                        string description = (string)node.Attribute("Description");
                        string address = (string)node.Attribute("Address");
                        double initvalue = (double)node.Attribute("InitValue");
                        double lowlimit = (double)node.Attribute("LowLimit");
                        double highlimit = (double)node.Attribute("HighLimit");

                        AOlist.Add(new AO(tagName, description, address, initvalue, lowlimit, highlimit));
                    }
                    catch
                    {
                        continue;
                    }
                }

                var DOXml = xmlData.Descendants("DO");
                List<DO> DOlist = new List<DO>();
                foreach (XElement node in DOXml)
                {
                    try
                    {
                        string tagName = (string)node;
                        string description = (string)node.Attribute("Description");
                        string address = (string)node.Attribute("Address");
                        double initvalue = (double)node.Attribute("InitValue");

                        DOlist.Add(new DO(tagName, description, address, initvalue));
                    }
                    catch
                    {
                        continue;
                    }
                }

                var alarmsXml = xmlData.Descendants("Alarm");
                List<Alarm> alarmList = new List<Alarm>();               
                foreach (XElement node in alarmsXml)
                {
                    try
                    {
                        int id = (int)node;
                        string type = (string)node.Attribute("Type");
                        double value = (double)node.Attribute("CriticalValue");
                        string unit = (string)node.Attribute("Unit");
                        string priority = (string)node.Attribute("Priority");

                        string timestr = (string)node.Attribute("ActivationTime");
                        DateTime? time = null;
                        if (!timestr.Equals(""))
                        {
                            time = DateTime.ParseExact(timestr, "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture);
                        }

                        Alarm alarm = new Alarm(id, (AlarmType)Enum.Parse(typeof(AlarmType), type), value, unit, time, (AlarmPriority)Enum.Parse(typeof(AlarmPriority), priority));
                        alarmList.Add(alarm);
                    }
                    catch
                    {
                        continue;
                    }

                }
        
                db.Users.AddRange(userList);
                db.AIset.AddRange(AIlist);
                db.AOset.AddRange(AOlist);
                db.DIset.AddRange(DIlist);
                db.DOset.AddRange(DOlist);
                db.Alarms.AddRange(alarmList);
                db.SaveChanges();

                Console.WriteLine("Finished loading SCADAconfig.xml");
                return db;
            }
        }

        public static Context SaveData()
        {
            XElement users = new XElement("Users");
            foreach (User a in db.Users)
            {
                XElement el = new XElement("User", a)
                {
                    Value = a.Username
                };
                el.SetAttributeValue("Password", a.Password);
                el.SetAttributeValue("Role", a.Role.ToString());

                users.Add(el);
            }

            XElement AIset = new XElement("AIset");
            foreach (AI tag in db.AIset)
            {
                XElement el = new XElement("AI", tag)
                {
                    Value = tag.TagName
                };
                el.SetAttributeValue("Description", tag.Description);
                el.SetAttributeValue("Driver", tag.Driver.ToString());
                el.SetAttributeValue("Address", tag.Address);
                el.SetAttributeValue("ScanTime", tag.ScanTime);
                el.SetAttributeValue("OnOffScan", tag.OnOffScan);
                el.SetAttributeValue("LowLimit", tag.LowLimit);
                el.SetAttributeValue("HighLimit", tag.HighLimit);
                el.SetAttributeValue("Units", tag.Units);

                AIset.Add(el);
            }

            XElement DIset = new XElement("DIset");
            foreach (DI tag in db.DIset)
            {
                XElement el = new XElement("DI", tag)
                {
                    Value = tag.TagName
                };
                el.SetAttributeValue("Description", tag.Description);
                el.SetAttributeValue("Driver", tag.Driver.ToString());
                el.SetAttributeValue("Address", tag.Address);
                el.SetAttributeValue("ScanTime", tag.ScanTime);
                el.SetAttributeValue("OnOffScan", tag.OnOffScan);

                DIset.Add(el);
            }

            XElement AOset = new XElement("AOset");
            foreach (AO tag in db.AOset)
            {
                XElement el = new XElement("AO", tag)
                {
                    Value = tag.TagName
                };
                el.SetAttributeValue("Description", tag.Description);
                el.SetAttributeValue("Address", tag.Address);
                el.SetAttributeValue("InitValue", tag.InitialValue);
                el.SetAttributeValue("LowLimit", tag.LowLimit);
                el.SetAttributeValue("HighLimit", tag.HighLimit);

                AOset.Add(el);
            }

            XElement DOset = new XElement("DOset");
            foreach (DO tag in db.DOset)
            {
                XElement el = new XElement("DO", tag)
                {
                    Value = tag.TagName
                };
                el.SetAttributeValue("Description", tag.Description);
                el.SetAttributeValue("Address", tag.Address);
                el.SetAttributeValue("InitValue", tag.InitialValue);

                DOset.Add(el);
            }

            XElement alarms = new XElement("Alarms");           
            foreach (Alarm a in db.Alarms)
            {
                XElement el = new XElement("Alarm", a)
                {
                    Value = a.Id.ToString()
                };
                el.SetAttributeValue("Type", a.Type.ToString());
                el.SetAttributeValue("CriticalValue", a.CriticalValue.ToString());
                el.SetAttributeValue("ActivationTime", a.ActivationTime.ToString());
                el.SetAttributeValue("Priority", a.Priority.ToString());
                el.SetAttributeValue("Unit", a.Unit);

                alarms.Add(el);
            }

            XElement root = new XElement("Database");
            root.Add(users);
            root.Add(AOset);
            root.Add(DIset);
            root.Add(AIset);
            root.Add(DOset);
            root.Add(alarms);
            root.Save(configPath);

            Console.WriteLine("Finished saving SCADAconfig.xml");
            return db;
        }

    }
}