using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportManager
{
    class Program
    {
        static ReportManagerServiceReference.ReportManagerClient service = new ReportManagerServiceReference.ReportManagerClient();
        static void Main(string[] args)
        {
            while (true)
            {
                int option = ShowMainMenu();
                switch (option)
                {
                    case 1:
                        {
                            EnterDate("Enter start date (dd-MM-yyyy):", out DateTime start);
                            EnterDate("Enter end date (dd-MM-yyyy):", out DateTime end);
                            string report = service.AllAlarmsByTime(start, end);
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 2:
                        {
                            EnterPriority(out int priority);
                            string report = service.AllAlarmsByPriority(priority);
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 3:
                        {
                            EnterDate("Enter start date (dd-MM-yyyy):", out DateTime start);
                            EnterDate("Enter end date (dd-MM-yyyy):", out DateTime end);
                            string report = service.AllTagsByTime(start, end);
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 4:
                        {
                            string report = service.AllAI();
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 5:
                        {
                            string report = service.AllDI();
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 6:
                        {
                            Console.WriteLine("Enter tag name:");
                            string userInput = Console.ReadLine();
                            string report = service.AllTagById(userInput);
                            Console.WriteLine(report);
                            WaitForEnter();
                            break;
                        }
                    case 7:
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

        private static int ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("\n -------------------------REPORTS MENU----------------------");
            Console.WriteLine("1. All alarms that occurred in a certain period of time (sorted by time, priority)");
            Console.WriteLine("2. All alarms of a certain priority (sorted by time)");
            Console.WriteLine("3. All values of tags that have reached the service in a certain period of time (sorted by time)");
            Console.WriteLine("4. All values of AI tags (sorted by time)");
            Console.WriteLine("5. All values of DI tags (sorted by time)");
            Console.WriteLine("6. All tag values ​​with a specific identifier - tag name (sorted by value)");
            Console.WriteLine("7. Clear console.");
            Console.WriteLine("Enter ordinal number of desired report: ");

            try
            {
                int option = Convert.ToInt32(Console.ReadLine());
                if (option < 8 && option > 0)
                {
                    return option;
                }
                throw new Exception("Input not in alowed range.");
            }
            catch 
            {
                Console.WriteLine("Wrong input.");
            }
            return 0;
        }

        private static void EnterDate(string inputMessage, out DateTime date)
        {
            while (true)
            {
                Console.WriteLine(inputMessage);
                string userInput = Console.ReadLine();

                DateTime.TryParseExact(userInput,
                           "dd-MM-yyyy",
                           CultureInfo.InvariantCulture,
                           DateTimeStyles.None,
                           out date);
                if (date.Equals(DateTime.MinValue)) //date is not parsed correctly
                {
                    Console.WriteLine("Wrong date format.");
                    continue;
                }
                return;
            }       
        }

        private static void EnterPriority(out int priority)
        {
            while (true)
            {
                Console.WriteLine("Enter alarm priority (number 1-3):");
                string userInput = Console.ReadLine();
                bool success = Int32.TryParse(userInput, out priority);

                if (!success)
                {
                    Console.WriteLine("Conversion failed. Must be int.");
                    continue;
                }
                return;
            }         
        }

        private static void WaitForEnter()
        {
            Console.Write("Press <Enter> to back to reports menu... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}
