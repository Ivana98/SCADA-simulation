using SCADAcore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Security.Cryptography;

namespace SCADAcore.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DatabaseManager" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DatabaseManager.svc or DatabaseManager.svc.cs at the Solution Explorer and start debugging.
    public class DatabaseManager : IDatabaseManager
    {
        public Context db;
        delegate void notifyDelegate(string message);
        static event notifyDelegate OnChangeNotification;
        IDatabaseManagerCallback Proxy;

        public void InitService()
        {
            Proxy = OperationContext.Current.GetCallbackChannel<IDatabaseManagerCallback>();
            OnChangeNotification += Proxy.WriteToConsole;
            db = SCADAconfig.LoadData();
        }

        //user operations
        public bool DbContainsUser()
        {
            if (db.Users.Count() == 0)
            {
                return false;
            }
            return true;
        }

        public string LogIn(string username, string password)
        {
            lock (db.Users)
            {
                //if there is no users in database, then register admin
                if (db.Users.Count() == 0)
                {
                    Register(username, password, "ADMIN");
                    return "ADMIN registration succesful.";
                }

                //otherwise do login
                User user = FindUserByUsername(username);
                if (user is null)
                {
                    return "Failed to log in. Username not exist.\n";
                }

                if (ValidateEncryptedData(password, user.Password))
                {
                    OnChangeNotification.Invoke("Log in succesful.");
                    return user.Role.ToString();
                }
                else
                {
                    return $"Failed to log in. Wrong password for username: {username}.\n";
                }
            }
        }

        public bool Register(string username, string password, string role = "USER")
        {
            UserRole parsedRole = (UserRole)Enum.Parse(typeof(UserRole), role);
            lock (db.Users)
            {
                if(!(FindUserByUsername(username) is null))
                {
                    return false;
                }

                string encryptedPassword = EncryptData(password);
                User user = new User(username, encryptedPassword, parsedRole);
                db.Users.Add(user);
                db.SaveChanges();
                SCADAconfig.SaveData();
                return true;
            }
        }

        private User FindUserByUsername(string username)
        {
            var user = db.Users
                    .Where(u => u.Username == username)
                    .FirstOrDefault();
            return user;
        }

        private static string EncryptData(string valueToEncrypt)
        {
            string GenerateSalt()
            {
                RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
                byte[] salt = new byte[32];
                crypto.GetBytes(salt);
                return Convert.ToBase64String(salt);
            }
            string EncryptValue(string strValue)
            {
                string saltValue = GenerateSalt();
                byte[] saltedPassword = Encoding.UTF8.GetBytes(saltValue + strValue);
                using (SHA256Managed sha = new SHA256Managed())
                {
                    byte[] hash = sha.ComputeHash(saltedPassword);
                    return $"{Convert.ToBase64String(hash)}:{saltValue}";
                }
            }
            return EncryptValue(valueToEncrypt);
        }

        private static bool ValidateEncryptedData(string valueToValidate, string valueFromDatabase)
        {
            string[] arrValues = valueFromDatabase.Split(':');
            string encryptedDbValue = arrValues[0];
            string salt = arrValues[1];
            byte[] saltedValue = Encoding.UTF8.GetBytes(salt + valueToValidate);
            using (var sha = new SHA256Managed())
            {
                byte[] hash = sha.ComputeHash(saltedValue);
                string enteredValueToValidate = Convert.ToBase64String(hash);
                return encryptedDbValue.Equals(enteredValueToValidate);
            }
        }

        //return null if no object is found
        public AO GetByIdAO(string id)
        {
            return db.AOset.Find(id);
        }
        public AI GetByIdAI(string id)
        {
            return db.AIset.Find(id);
        }
        public DO GetByIdDO(string id)
        {
            return db.DOset.Find(id);
        }
        public DI GetByIdDI(string id)
        {
            return db.DIset.Find(id);
        }

        public string GetStrAO(string id)
        {
            return Convert.ToString(GetByIdAO(id)); //converts null to empty string
        }
        public string GetStrAI(string id)
        {
            return Convert.ToString(GetByIdAI(id));
        }
        public string GetStrDO(string id)
        {
            return Convert.ToString(GetByIdDO(id));
        }
        public string GetStrDI(string id)
        {
            return Convert.ToString(GetByIdDI(id));
        }

        public void DeleteAO(string id)
        {
            lock (db.AOset)
            {
                AO tag = GetByIdAO(id);
                db.AOset.Remove(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Deleting {tag.TagName} - succesful.");
            }
        }
        public void DeleteDO(string id)
        {
            lock (db.DOset)
            {
                DO tag = GetByIdDO(id);
                db.DOset.Remove(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Deleting {tag.TagName} - succesful.");
            }
        }
        public void DeleteAI(string id)
        {
            lock (db.AIset)
            {
                AI tag = GetByIdAI(id);
                db.AIset.Remove(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Deleting {tag.TagName} - succesful.");
            }
        }
        public void DeleteDI(string id)
        {
            lock (db.DIset)
            {
                DI tag = GetByIdDI(id);
                db.DIset.Remove(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Deleting {tag.TagName} - succesful.");
            }
        }

        public void AddAO(string tagName, string description, string address, double initvalue, double lowlimit, double highlimit)
        {
            lock (db.AOset)
            {
                if (!(GetByIdAO(tagName) is null))
                {
                    OnChangeNotification.Invoke($"Failed to add AO with tag name: {tagName} - name already exists");
                    return;
                }
                AO tag = new AO(tagName, description, address, initvalue, lowlimit, highlimit);
                db.AOset.Add(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Adding {tag.TagName} - succesful.");
            }
        }
        public void AddAI(string tagName, string description, string driver, string address, int scantime, bool onoffscan, double lowlimit, double highlimit, string units)
        {
            lock (db.AIset)
            {
                if (!(GetByIdAI(tagName) is null))
                {
                    OnChangeNotification.Invoke($"Failed to add AI with tag name: {tagName} - name already exists");
                    return;
                }
                AI tag = new AI(tagName, description, (Driver)Enum.Parse(typeof(Driver), driver), address, scantime, onoffscan, lowlimit, highlimit, units);
                db.AIset.Add(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Adding {tag.TagName} - succesful.");
            }
        }
        public void AddDO(string tagName, string description, string address, double initvalue)
        {
            CutDigitalValue(ref initvalue);

            lock (db.DOset)
            {
                if (!(GetByIdDO(tagName) is null))
                {
                    OnChangeNotification.Invoke($"Failed to add DO with tag name: {tagName} - name already exists");
                    return;
                }
                DO tag = new DO(tagName, description, address, initvalue);
                db.DOset.Add(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Adding {tag.TagName} - succesful.");
            }
        }
        public void AddDI(string tagName, string description, string driver, string address, int scantime, bool onoffscan)
        {
            lock (db.DIset)
            {
                if (!(GetByIdDI(tagName) is null))
                {
                    OnChangeNotification.Invoke($"Failed to add DI with tag name: {tagName} - name already exists");
                    return;
                }
                DI tag = new DI(tagName, description, (Driver)Enum.Parse(typeof(Driver), driver), address, scantime, onoffscan);
                db.DIset.Add(tag);
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Adding {tag.TagName} - succesful.");
            }
        }

        public void UpdateAO(string tagName, string description, string address, double initvalue, double lowlimit, double highlimit)
        {
            lock (db.AOset)
            {
                AO tag = GetByIdAO(tagName);

                if (tag is null)
                {
                    OnChangeNotification.Invoke($"Failed to update AO with tag name: {tagName} - name do not exist");
                    return;
                }

                tag.Description = description;
                tag.Address = address;
                tag.InitialValue = initvalue;
                tag.LowLimit = lowlimit;
                tag.HighLimit = highlimit;

                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Updating {tag.TagName} - succesful.");
            }
        }
        public void UpdateAI(string tagName, string description, string driver, string address, int scantime, bool onoffscan, double lowlimit, double highlimit, string units)
        {
            lock (db.AIset)
            {
                AI tag = GetByIdAI(tagName);

                if (tag is null)
                {
                    OnChangeNotification.Invoke($"Failed to update AI with tag name: {tagName} - name do not exist");
                    return;
                }

                tag.Description = description;
                tag.Driver = (Driver)Enum.Parse(typeof(Driver), driver);
                tag.Address = address;
                tag.ScanTime = scantime;
                tag.OnOffScan = onoffscan;
                tag.LowLimit = lowlimit;
                tag.HighLimit = highlimit;
                tag.Units = units;

                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Updating {tag.TagName} - succesful.");
            }
        }
        public void UpdateDO(string tagName, string description, string address, double initvalue)
        {
            CutDigitalValue(ref initvalue);
            lock (db.DOset)
            {
                DO tag = GetByIdDO(tagName);

                if (tag is null)
                {
                    OnChangeNotification.Invoke($"Failed to update DO with tag name: {tagName} - name do not exist");
                    return;
                }

                tag.Description = description;
                tag.Address = address;
                tag.InitialValue = initvalue;

                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Updating {tag.TagName} - succesful.");
            }
        }
        public void UpdateDI(string tagName, string description, string driver, string address, int scantime, bool onoffscan)
        {
            lock (db.DIset)
            {
                DI tag = GetByIdDI(tagName);

                if (tag is null)
                {
                    OnChangeNotification.Invoke($"Failed to update DI with tag name: {tagName} - name do not exist");
                    return;
                }

                tag.Description = description;
                tag.Driver = (Driver)Enum.Parse(typeof(Driver), driver);
                tag.Address = address;
                tag.ScanTime = scantime;
                tag.OnOffScan = onoffscan;

                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Updating {tag.TagName} - succesful.");
            }
        }

        public string GetAllAI()
        {
            lock (db.AIset)
            {
                return string.Join(Environment.NewLine, (object[])db.AIset.ToArray());
            }
        }
        public string GetAllAO()
        {
            lock (db.AOset)
            {
                return string.Join(Environment.NewLine, (object[])db.AOset.ToArray());
            }
        }
        public string GetAllDI()
        {
            lock (db.DIset)
            {
                return string.Join(Environment.NewLine, (object[])db.DIset.ToArray());
            }
        }
        public string GetAllDO()
        {
            lock (db.DOset)
            {
                return string.Join(Environment.NewLine, (object[])db.DOset.ToArray());
            }
        }

        public string GetIdListAO()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.AOset
                                      orderby tag.TagName ascending
                                      select tag.TagName).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string GetIdListAI()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.AIset
                                      orderby tag.TagName ascending
                                      select tag.TagName).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string GetIdListDO()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.DOset
                                      orderby tag.TagName ascending
                                      select tag.TagName).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }
        public string GetIdListDI()
        {
            lock (db)
            {
                var filteredResult = (from tag in db.DIset
                                      orderby tag.TagName ascending
                                      select tag.TagName).ToArray();
                return string.Join(Environment.NewLine, (object[])filteredResult);
            }
        }

        // Additional functions
        private void CutDigitalValue(ref double value)
        {
            if (value < 0)
            {
                value = 0;
            }
            if (value > 1)
            {
                value = 1;
            }
        }

        public void TurnScanOnOff(string tagType, string tagName, bool activity)
        {
            lock (db)
            {
                if (tagType == "AI")
                {
                    AI tag = GetByIdAI(tagName);
                    tag.OnOffScan = activity;
                }
                else
                {
                    DI tag = GetByIdDI(tagName);
                    tag.OnOffScan = activity;
                }

                db.SaveChanges();
                SCADAconfig.SaveData();
                if (activity)
                {
                    OnChangeNotification($"Tag {tagName} - Scan turned on.");
                }
                else
                {
                    OnChangeNotification($"Tag {tagName} - Scan turned off.");
                }
            }
        }

        public void ChangeValueAO(string tagName, double value)
        {
            lock (db.AOset)
            {
                AO tag = GetByIdAO(tagName);
                foreach (AO a in db.AOset)
                {
                    if (a.Address == tag.Address)
                        a.InitialValue = value;
                }
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke("Value changed.");
            }
        }

        public void ChangeValueDO(string tagName, double value)
        {
            lock (db.DOset)
            {
                DO tag = GetByIdDO(tagName);
                CutDigitalValue(ref value);
                foreach (DO a in db.DOset)
                {
                    if (a.Address == tag.Address)
                        a.InitialValue = value;
                }
                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke("Value changed.");
            }
        }

        //Alarm CRUD
        public Alarm GetByIdAlarm(int id)
        {
            return db.Alarms.Find(id);
        }

        //tagName = AI TagName (id)
        public void AddAlarm(string type, double value, string unit, string priority, string tagName)
        {
            lock (db)
            {
                Alarm alarm = new Alarm((AlarmType)Enum.Parse(typeof(AlarmType), type), value, unit, (AlarmPriority)Enum.Parse(typeof(AlarmPriority), priority));
                db.Alarms.Add(alarm);
                AI tag = GetByIdAI(tagName);
                tag.Alarms.Add(alarm);

                db.SaveChanges();
                SCADAconfig.SaveData();
                OnChangeNotification.Invoke($"Alarm added succesfully to {tagName} alarm list.");
            }
        }

        public void RemoveAlarm(string tagName, string alarmId)
        {
            lock (db)
            {
                bool parsed = Int32.TryParse(alarmId, out int parsedAlarmId);
                Alarm a = null;

                if(parsed)
                {
                    a = GetByIdAlarm(parsedAlarmId);
                }
                if (a is null)
                {
                    OnChangeNotification.Invoke($"Alarm with id {alarmId} does not exist");
                    return;
                }

                AI tag = GetByIdAI(tagName);
                if (tagName is null)
                {
                    OnChangeNotification.Invoke($"Tag with name {tagName} does not exist");
                    return;
                }

                try
                {
                    tag.Alarms.Remove(a);
                    db.Alarms.Remove(a);

                    db.SaveChanges();
                    SCADAconfig.SaveData();
                    OnChangeNotification.Invoke($"Alarm with id {alarmId} removed succesfully.");
                }
                catch 
                {
                    OnChangeNotification.Invoke($"Alarm with id {alarmId} not belong to tag with name {tagName}.");
                }
            }

        }

        public string GetAllAlarms(string tagName)
        {
            lock (db)
            {
                AI tag = GetByIdAI(tagName);
                return string.Join(Environment.NewLine, (object[])tag.Alarms.ToArray());
            }
        }

    }
}
