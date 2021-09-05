using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace DatabaseManager
{
    public class Callback : DatabaseServiceReference.IDatabaseManagerCallback
    {
        public void WriteToConsole(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(1000);
        }
    }

    class Program
    {
        static DatabaseServiceReference.DatabaseManagerClient service;
        static string role;

        static void Main(string[] args)
        {
            InstanceContext ic = new InstanceContext(new Callback());
            service = new DatabaseServiceReference.DatabaseManagerClient(ic);
            Console.WriteLine("Started service initialization. Please be patient...");
            service.InitService();

            Thread.Sleep(3000);
            MainMenu();
        }

        private static void LogIn()
        {
            Console.WriteLine();
            bool haveUsers = service.DbContainsUser();
            if (!haveUsers)
            {
                Console.WriteLine("Running application for the first time...");
                Thread.Sleep(2000);
                Console.WriteLine("Enter credentials to register as admin:");
            }
            while (true)
            {
                Console.WriteLine("Username: ");
                string username = Console.ReadLine();
                string password = GetPasswordFromConsole();

                role = service.LogIn(username, password);
                if (role.Equals("USER") || role.Equals("ADMIN"))
                {
                    break;
                }
                else
                {
                    // role is here error message
                    Console.WriteLine(role);
                }
            }
            Console.Clear();
            RegisteredMainMenu();
        }

        private static void LogOut()
        {
            Console.Clear();
            role = "";
            MainMenu();
        }

        private static void RegisterUser()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter new user username:");
                string username = Console.ReadLine();
                Console.WriteLine("Enter new user password:");
                string password = Console.ReadLine();
                string userRole;

                while (true)
                {
                    Console.WriteLine("Enter number of role for new user: (where 1 - USER; 2 - ADMIN)");
                    string userInput = Console.ReadLine();
                    bool success = Int32.TryParse(userInput, out int userNum);

                    if (!success || (userNum == 1 || userNum == 2))
                    {
                        Console.WriteLine("Conversion failed. Must be int type, value 1 or 2.");
                        continue;
                    }

                    userRole = userNum == 1 ? "USER" : "ADMIN";
                    break;
                }

                bool regSuccess = service.Register(username, password, userRole);
                if (regSuccess)
                {
                    Console.WriteLine($"User {username} is successfully registered as {userRole}.");
                    return;
                }
                Console.WriteLine($"Registration failed. Username: {username} already exists in database.");
                Console.WriteLine();

                Console.Write("Press <Any key> to try again registration or <Enter> to back to menu... ");
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                {
                    return;
                }
            }         
        }

        private static string GetPasswordFromConsole()
        {
            SecureString password = new SecureString();
            Console.Write("Password: ");
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                // Backspace should not work
                if (!char.IsControl(key.KeyChar))
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops receving keys once enter is pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();

            // we use SecureString here because System.String is immutable
            // so we convert back to string in the end
            return new System.Net.NetworkCredential(string.Empty, password).Password;
        }

        private static void MainMenu()
        {
            Console.WriteLine("Press <any key> to login or <Enter> to quit application.");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                Environment.Exit(0);
            }
            else
            {
                LogIn();
            }
        }

        private static void WaitForEnter(string message = "Press <Enter> to back to menu... ")
        {
            Console.Write(message);
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        private static int ShowMainMenu()
        {
            Console.WriteLine("\n -------------------------MAIN MENU----------------------");
            if (role.Equals("ADMIN"))
            {
                Console.WriteLine("0. Register user");
            }
            Console.WriteLine("1. Manage Analog Input (AI)");
            Console.WriteLine("2. Manage Analog Output (AO)");
            Console.WriteLine("3. Manage Digital Input (DI)");
            Console.WriteLine("4. Manage Digital Output (DO)");
            Console.WriteLine("5. Log out");
            Console.WriteLine("6. Clear console");
            Console.WriteLine("Enter ordinal number of desired option: ");

            try
            {
                int option = Convert.ToInt32(Console.ReadLine());
                if ((role.Equals("ADMIN") && option < 7 && option >= 0) ||
                    (role.Equals("USER") && option < 7 && option > 0))
                {
                    return option;
                }
                throw new Exception("Input not in alowed range.");
            }
            catch
            {
                Console.WriteLine("Input not in alowed range.");
            }
            return -1;
        }

        private static void RegisteredMainMenu()
        {
            while (true)
            {
                int option = ShowMainMenu();
                switch (option)
                {
                    case 0:
                        {
                            RegisterUser();
                            WaitForEnter("Press <Enter> to continue...");
                            Console.Clear();
                            break;
                        }
                    case 1:
                        {
                            AImenu();
                            break;
                        }
                    case 2:
                        {
                            AOmenu();
                            break;
                        }
                    case 3:
                        {
                            DImenu();
                            break;
                        }
                    case 4:
                        {
                            DOmenu();
                            break;
                        }
                    case 5:
                        {
                            LogOut();
                            break;
                        }
                    case 6:
                        {
                            Console.Clear();
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong option input.");
                            break;
                        }
                }
            }
        }

        private static int ShowAIMenu()
        {
            Console.WriteLine("\n -------------------------ANALOG INPUT MENU----------------------");
            Console.WriteLine("1. Add tag");
            Console.WriteLine("2. Edit tag");
            Console.WriteLine("3. Delete tag");
            Console.WriteLine("4. Get all tags");
            Console.WriteLine("5. Turn scan on/off");
            Console.WriteLine("6. Add alarm");
            Console.WriteLine("7. Delete alarm");
            Console.WriteLine("8. Back to Main Menu");
            Console.WriteLine("Enter ordinal number of desired option: ");

            return CheckOptionInput(0, 9);
        }

        private static int ShowDIMenu()
        {
            Console.WriteLine("\n -------------------------DIGITAL INPUT MENU----------------------");
            Console.WriteLine("1. Add tag");
            Console.WriteLine("2. Edit tag");
            Console.WriteLine("3. Delete tag");
            Console.WriteLine("4. Get all tags");
            Console.WriteLine("5. Turn scan on/off");
            Console.WriteLine("6. Back to Main Menu");
            Console.WriteLine("Enter ordinal number of desired option: ");

            return CheckOptionInput(0, 7);
        }

        private static int ShowAOMenu()
        {
            Console.WriteLine("\n -------------------------ANALOG OUTPUT MENU----------------------");
            Console.WriteLine("1. Add tag");
            Console.WriteLine("2. Edit tag");
            Console.WriteLine("3. Delete tag");
            Console.WriteLine("4. Get all tags");
            Console.WriteLine("5. Set value");
            Console.WriteLine("6. Back to Main Menu");
            Console.WriteLine("Enter ordinal number of desired option: ");

            return CheckOptionInput(0, 7);
        }

        private static int ShowDOMenu()
        {
            Console.WriteLine("\n -------------------------DIGITAL OUTPUT MENU----------------------");
            Console.WriteLine("1. Add tag");
            Console.WriteLine("2. Edit tag");
            Console.WriteLine("3. Delete tag");
            Console.WriteLine("4. Get all tags");
            Console.WriteLine("5. Set value");
            Console.WriteLine("6. Back to Main Menu");
            Console.WriteLine("Enter ordinal number of desired option: ");

            return CheckOptionInput(0, 7);
        }

        private static int CheckOptionInput(int lowLimit, int highLimit)
        {
            try
            {
                int option = Convert.ToInt32(Console.ReadLine());
                if (option < highLimit && option > lowLimit)
                {
                    return option;
                }
                throw new Exception("Input not in alowed range.");
            }
            catch
            {
                Console.WriteLine("Wrong input.");
            }
            return -1;
        }

        private static void EnterDouble(string message, out double input)
        {
            while (true)
            {
                Console.WriteLine(message);
                string userInput = Console.ReadLine();
                bool success = Double.TryParse(userInput, out input);

                if (!success)
                {
                    Console.WriteLine("Conversion failed. Must be double.");
                    continue;
                }
                return;
            }
        }

        private static void EnterInt(string message, out int input)
        {
            while (true)
            {
                Console.WriteLine(message);
                string userInput = Console.ReadLine();
                bool success = Int32.TryParse(userInput, out input);

                if (!success)
                {
                    Console.WriteLine("Conversion failed. Must be integer.");
                    continue;
                }
                return;
            }
        }

        private static void EnterBoolean(string message, out bool input)
        {
            while (true)
            {
                Console.WriteLine(message);
                string userInput = Console.ReadLine();
                bool success = Boolean.TryParse(userInput, out input);

                if (!success)
                {
                    Console.WriteLine("Conversion failed. Must be boolean (true or false).");
                    continue;
                }
                return;
            }
        }

        private static void DOmenu()
        {
            while (true)
            {
                int option = ShowDOMenu();
                switch (option)
                {
                    case 1:
                        {
                            AddDO();   
                            break;
                        }
                    case 2:
                        {
                            EditDO();
                            break;
                        }
                    case 3:
                        {
                            DeleteDO();
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("All digital outputs:");
                            string result = service.GetAllDO();
                            Console.WriteLine(result);
                            WaitForEnter();
                            break;
                        }
                    case 5:
                        {
                            SetValueDO();
                            break;
                        }
                    case 6:
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong option input. Must be int between 1 and 6.");
                            break;
                        }
                }
            }
        }

        private static void AddDO()
        {
            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();
            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            Console.WriteLine("Address: ");
            string address = Console.ReadLine();
            EnterDouble("Initial value (double): ", out double initVal);

            service.AddDO(id, description, address, initVal);
        }

        private static void EditDO()
        {
            string idList = service.GetIdListDO();
            Console.WriteLine("List of tag names (DO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            Console.WriteLine("Address: ");
            string address = Console.ReadLine();
            EnterDouble("Initial value (double): ", out double initVal);

            service.UpdateDO(id, description, address, initVal);
        }

        private static void DeleteDO()
        {
            string idList = service.GetIdListDO();
            Console.WriteLine("List of tag names (DO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to delete: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine($"Press <Enter> to confirm deleting tag with id: {id} or <Any key> to cancel...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                service.DeleteDO(id);
            }
        }

        private static void SetValueDO()
        {
            string idList = service.GetIdListDO();
            Console.WriteLine("List of tag names (DO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to change value: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            EnterDouble("Enter initial value: ", out double val);
            service.ChangeValueDO(id, val);
        }

        private static void AddAO()
        {
            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();
            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            Console.WriteLine("Address: ");
            string address = Console.ReadLine();
            EnterDouble("Initial value (double): ", out double initVal);
            EnterDouble("Low limit (double): ", out double lowLimit);
            EnterDouble("High limit (double): ", out double highLimit);

            service.AddAO(id, description, address, initVal, lowLimit, highLimit);
        }

        private static void EditAO()
        {
            string idList = service.GetIdListAO();
            Console.WriteLine("List of tag names (AO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            Console.WriteLine("Address: ");
            string address = Console.ReadLine();
            EnterDouble("Initial value (double): ", out double initVal);
            EnterDouble("Low limit (double): ", out double lowLimit);
            EnterDouble("High limit (double): ", out double highLimit);

            service.UpdateAO(id, description, address, initVal, lowLimit, highLimit);
        }

        private static void DeleteAO()
        {
            string idList = service.GetIdListAO();
            Console.WriteLine("List of tag names (AO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to delete: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine($"Press <Enter> to confirm deleting tag with id: {id} or <Any key> to cancel...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                service.DeleteAO(id);
            }
        }

        private static void SetValueAO()
        {
            string idList = service.GetIdListAO();
            Console.WriteLine("List of tag names (AO): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to change value: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAO(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            EnterDouble("Enter initial value: ", out double val);
            service.ChangeValueAO(id, val);
        }

        private static void AOmenu()
        {
            while (true)
            {
                int option = ShowAOMenu();
                switch (option)
                {
                    case 1:
                        {
                            AddAO();
                            break;
                        }
                    case 2:
                        {
                            EditAO();
                            break;
                        }
                    case 3:
                        {
                            DeleteAO();
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("All analog outputs:");
                            string result = service.GetAllAO();
                            Console.WriteLine(result);
                            WaitForEnter();
                            break;
                        }
                    case 5:
                        {
                            SetValueAO();
                            break;
                        }
                    case 6:
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong option input. Must be int between 1 and 6.");
                            break;
                        }
                }
            }
        }

        private static void AddDI()
        {
            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();
            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            EnterDriverAddress(out string driver, out string address);
            EnterInt("Scan time: ", out int scanTime);
            EnterBoolean("Scan activity (enter boolean): ", out bool scanActivity);

            service.AddDI(id, description, driver, address, scanTime, scanActivity);
        }

        private static void EditDI()
        {
            string idList = service.GetIdListDI();
            Console.WriteLine("List of tag names (DI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            EnterDriverAddress(out string driver, out string address);
            EnterInt("Scan time: ", out int scanTime);
            EnterBoolean("Scan activity (enter boolean): ", out bool scanActivity);

            service.UpdateDI(id, description, driver, address, scanTime, scanActivity);
        }

        private static void DeleteDI()
        {
            string idList = service.GetIdListDI();
            Console.WriteLine("List of tag names (DI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to delete: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine($"Press <Enter> to confirm deleting tag with id: {id} or <Any key> to cancel...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                service.DeleteDI(id);
            }
        }

        private static void ScanActivityDI()
        {
            string idList = service.GetIdListDI();
            Console.WriteLine("List of tag names (DI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit scan activity: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrDI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            EnterBoolean("Enter true to turn on or false to turn off activity: ", out bool scan);
            service.TurnScanOnOff("DI", id, scan);
        }

        private static void DImenu()
        {
            while (true)
            {
                int option = ShowDIMenu();
                switch (option)
                {
                    case 1:
                        {
                            AddDI();
                            break;
                        }
                    case 2:
                        {
                            EditDI();
                            break;
                        }
                    case 3:
                        {
                            DeleteDI();
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("All digital inputs:");
                            string result = service.GetAllDI();
                            Console.WriteLine(result);
                            WaitForEnter();
                            break;
                        }
                    case 5:
                        {
                            ScanActivityDI();
                            break;
                        }
                    case 6:
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong option input. Must be int between 1 and 6.");
                            break;
                        }
                }
            }
        }

        private static void EnterDriverAddress(out string driver, out string address)
        {
            while(true)
            {
                while (true)
                {
                    Console.WriteLine("Driver (enter RTD for Real Time Driver or SD for Simulation Driver): ");
                    driver = Console.ReadLine();
                    if (driver.Equals("RTD") || driver.Equals("SD"))
                    {
                        break;
                    }
                    Console.WriteLine("Wrong input. Must be RTD or SD.");
                }
                Console.WriteLine("Address (Allowed values for Simulation driver: S - sine, C - cosine, R - ramp): ");
                address = Console.ReadLine();
                if (driver.Equals("RTD") && !String.IsNullOrEmpty(address))                {
                    return;
                }
                else if (driver.Equals("SD") && (address.Equals("S") || address.Equals("C") || address.Equals("R")))
                {
                    return;
                }
                Console.WriteLine("Not valid address for chosen driver.");
            }
        }

        private static void AddAI()
        {
            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();
            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            EnterDriverAddress(out string driver, out string address);
            EnterInt("Scan time: ", out int scanTime);
            EnterBoolean("Scan activity (enter boolean): ", out bool scanActivity);
            EnterDouble("Enter low limit (double): ", out double lowLimit);
            EnterDouble("Enter high limit (double): ", out double highLimit);
            Console.WriteLine("Units: ");
            string units = Console.ReadLine();

            service.AddAI(id, description, driver, address, scanTime, scanActivity, lowLimit, highLimit, units);
        }

        private static void EditAI()
        {
            string idList = service.GetIdListAI();
            Console.WriteLine("List of tag names (AI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine("Description: ");
            string description = Console.ReadLine();
            Console.WriteLine("Address: ");
            EnterDriverAddress(out string driver, out string address);
            EnterInt("Scan time: ", out int scanTime);
            EnterBoolean("Scan activity (enter boolean): ", out bool scanActivity);
            EnterDouble("Enter low limit (double): ", out double lowLimit);
            EnterDouble("Enter high limit (double): ", out double highLimit);
            Console.WriteLine("Units: ");
            string units = Console.ReadLine();

            service.UpdateAI(id, description, driver, address, scanTime, scanActivity, lowLimit, highLimit, units);
        }

        private static void DeleteAI()
        {
            string idList = service.GetIdListAI();
            Console.WriteLine("List of tag names (AI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to delete: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tagstr);

            Console.WriteLine($"Press <Enter> to confirm deleting tag with id: {id} or <Any key> to cancel...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                service.DeleteAI(id);
            }
        }

        private static void ScanActivityAI()
        {
            string idList = service.GetIdListAI();
            Console.WriteLine("List of tag names (AI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id) to edit scan activity: ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            EnterBoolean("Enter true to turn on or false to turn off activity: ", out bool scan);
            service.TurnScanOnOff("AI", id, scan);
        }

        private static void AddAlarm()
        {
            string idList = service.GetIdListAI();
            Console.WriteLine("List of tag names (AI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            string alarmType, priority;
            while (true)
            {
                Console.WriteLine("Alarm type (enter LOW or HIGH): ");
                alarmType = Console.ReadLine();
                if (alarmType.Equals("LOW") || alarmType.Equals("HIGH"))
                {
                    break;
                }
                Console.WriteLine("Wrong input. Must be LOW or HIGH.");
            }

            while (true)
            {
                Console.WriteLine("Alarm priority (enter LOW for 1, MEDIUM for 2, HIGH for 3): ");
                priority = Console.ReadLine();
                if (priority.Equals("LOW") || priority.Equals("MEDIUM") || priority.Equals("HIGH"))
                {
                    break;
                }
                Console.WriteLine("Wrong input. Must be LOW, MEDIUM or HIGH.");
            }

            EnterDouble("Critical value: ", out double value);
            Console.WriteLine("Units: ");
            string units = Console.ReadLine();

            service.AddAlarm(alarmType, value, units, priority, id);
        }

        private static void DeleteAlarm()
        {
            string idList = service.GetIdListAI();
            Console.WriteLine("List of tag names (AI): ");
            Console.WriteLine(idList);

            Console.WriteLine("Tag name (id): ");
            string id = Console.ReadLine();

            string tagstr = service.GetStrAI(id);
            if (String.IsNullOrEmpty(tagstr))
            {
                Console.WriteLine($"Tag with id {id} not found.");
                Console.ReadLine();
                return;
            }

            string allAlarms = service.GetAllAlarms(id);
            Console.WriteLine(allAlarms);
            Console.WriteLine("Alarm id: ");
            string alarmId = Console.ReadLine();

            Console.WriteLine($"Press <Enter> to confirm deleting tag with id: {id} or <Any key> to cancel...");
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                service.RemoveAlarm(id, alarmId);
            }
        }

        private static void AImenu()
        {
            while (true)
            {
                int option = ShowAIMenu();
                switch (option)
                {
                    case 1:
                        {
                            AddAI();
                            break;
                        }
                    case 2:
                        {
                            EditAI();
                            break;
                        }
                    case 3:
                        {
                            DeleteAI();
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("All analog inputs:");
                            string result = service.GetAllAI();
                            Console.WriteLine(result);
                            WaitForEnter();
                            break;
                        }
                    case 5:
                        {
                            ScanActivityAI();
                            break;
                        }
                    case 6:
                        {
                            AddAlarm();
                            break;
                        }
                    case 7:
                        {
                            DeleteAlarm();
                            break;
                        }
                    case 8:
                        {
                            return;
                        }
                    default:
                        {
                            Console.WriteLine("Wrong option input. Must be int between 1 and 8.");
                            break;
                        }
                }
            }
        }

    }
}
