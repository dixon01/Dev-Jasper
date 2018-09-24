namespace Luminator.AdhocMessage.Console
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using Luminator.AdhocMessaging;
    using Luminator.AdhocMessaging.Helpers;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;

    using NLog;

    internal class Program
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        public static void ChangeNLogLevel(LogLevel logLevel)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules) rule.EnableLoggingForLevel(logLevel);

            LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
        }

        #endregion

        #region Methods

        private static void DisplayHelp(AdhocConfiguration ac)
        {
            //  Console.SetCursorPosition(10,10);
            Console.BufferHeight = short.MaxValue - 1;
            Console.SetWindowSize(140, 39);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" Conneced to Destinations: {ac.DestinationsApiUrl}{(string.IsNullOrEmpty(ac.Port) ? string.Empty : ":")}{(string.IsNullOrEmpty(ac.Port) ? string.Empty : ac.Port)}  &  Messages: {ac.MessageApiBaseUrl}{(string.IsNullOrEmpty(ac.Port) ? string.Empty : ":")}{(string.IsNullOrEmpty(ac.MessageApiPort) ? string.Empty : ac.MessageApiPort)} ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("------------Unit Operations -----|--------Vechicle Operations-----------|------------ Message Operations --------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  S  :  Register                 |   C    :  Register Vehicle           |  M   :  Get Messages for Unit (Date & Route)   ");
            Console.WriteLine("  D  :  Delete Unit              |   V    :  Register Vechicle Async    |  L   :  Get Messages for Unit (date only)      ");
            Console.WriteLine("  B  :  List Units               |   R    :  Register Unit With Vehicle |  U   :  Get All Messages grouped by Unit       ");
            Console.WriteLine("  N  :  List Units Async         |   G    :  List Vechicles / Async     | ");
            Console.WriteLine("  E  :  Check  Unit Exists       |   O    :  Check Vehicle Exists       | ");
            Console.WriteLine("  F  :  Check Unit Exists (Fail) |   P    :  Check Vehicle Exists (Fail)| ");
            //  Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"====== ESC to Exit | H - Help | F1 - Change Server ! Internet Access: {(NetworkHelper.CheckNet() ? "OK" : "NO")} | ComputerName: {Environment.MachineName} | Window Dimensions {Console.WindowHeight} x {Console.WindowWidth} ====");
            Console.WriteLine();
            Console.ResetColor();
        }

        private static AdhocConfiguration PickServer()
        {
            Console.WriteLine($"Pick a Server: 1 - SWDEVICNTR, 2 - NYCTICNTR, 3 - localhost:63093,  4 - Enter Server Info");
            AdhocConfiguration ac = null;
            var userChoice = Console.ReadLine();
            if (userChoice == "1")
            {
                ac = new AdhocConfiguration();
            }
            else if (userChoice == "2")
            {
                ac = new AdhocConfiguration("http://nyctdestinations.luminatorusa.com", string.Empty, "http://nyctadhoc.luminatorusa.com", string.Empty);
            }
            else if (userChoice == "3")
            {
                ac = new AdhocConfiguration("http://localhost", "63093", "http://localhost", string.Empty);
            }
            else if (userChoice == "4")
            {
                Console.WriteLine("Enter RestAPI Address for Destinations [http://swdevicntrapp.luminatorusa.com]");
                var server = Console.ReadLine();
                var port = string.Empty;
                if (string.IsNullOrEmpty(server))
                {
                    server = "http://swdevicntrapp.luminatorusa.com"; // "http://localhost:63093";
                }
                else if (server.Contains("localhost"))
                {
                    Console.WriteLine("Enter the port number to use ");
                    port = Console.ReadLine();
                }

                Console.WriteLine("Enter RestAPI Address for Adhoc Messages [http://swdevicntrweb.luminatorusa.com]");
                var serverMessagesApi = Console.ReadLine();
                var portMessagesApi = string.Empty;
                if (string.IsNullOrEmpty(serverMessagesApi))
                {
                    serverMessagesApi = "http://swdevicntrweb.luminatorusa.com";
                }
                else if (serverMessagesApi.Contains("localhost"))
                {
                    Console.WriteLine("Enter the port number to use ");
                    portMessagesApi = Console.ReadLine();
                }

                ac = new AdhocConfiguration(server, port, serverMessagesApi, portMessagesApi);
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Incorrect Choice");
                PickServer();
            }

            return ac;
        }

        private static void Main(string[] args)
        {
            Console.BufferHeight = short.MaxValue - 1;
            Console.SetWindowSize(140, 39);
            var rnd = new Random();
            var adhocMessageFactory = new AdhocMessageFactory();
            IAdhocManager adhocManager = null;
            var ac = PickServer();
            adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
            DisplayHelp(adhocManager.AdhocConfiguration);
            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey();
                while (!Console.KeyAvailable)
                    if (info.Key == ConsoleKey.A)
                    {
                        Console.Clear();
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        Console.WriteLine($"Enter TFT name you want to Register or Enter to Accept Name[ {Environment.MachineName}] ");
                        var toRegisterTft = Console.ReadLine();
                        var tft = new Unit
                        {
                            Description = string.IsNullOrEmpty(toRegisterTft) ? $"Desc For {Environment.MachineName}" : $"Description for {toRegisterTft}",
                            Name = string.IsNullOrEmpty(toRegisterTft) ? $"{Environment.MachineName}" : $"{toRegisterTft}",
                            CreatedOn = DateTime.Now,
                            TenantId = AdhocConstants.DefaultTenentId
                        };
                        Task.Factory.StartNew(() => { Console.WriteLine($"Registering Unit Status:  {adhocManager.RegisterUnit(tft)}"); })
                            .ContinueWith(prev => { DisplayHelp(adhocManager.AdhocConfiguration); });
                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.S)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Unit Async");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        Console.WriteLine($"Enter TFT name you want to Register or Enter to Accept Name [ {Environment.MachineName}] ");
                        var toRegisterTft = Console.ReadLine();
                        var tft = new Unit
                        {
                            Description = string.IsNullOrEmpty(toRegisterTft) ? $"Description for {Environment.MachineName}" : $"Description for {toRegisterTft}",
                            Name = string.IsNullOrEmpty(toRegisterTft) ? $"{Environment.MachineName}" : $"{toRegisterTft}",
                            CreatedOn = DateTime.Now,
                            TenantId = AdhocConstants.DefaultTenentId
                        };
                        var res = adhocManager.RegisterUnitAsync(tft);
                        Console.WriteLine($"Registering Unit Status:  {res.Result}");
                        Console.WriteLine("Try to Register Duplicate Unit - Should Fail");
                        Console.WriteLine($"Registering Unit Status:  {adhocManager.RegisterUnitAsync(tft).Result}");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.D)
                    {
                        Console.Clear();
                        Console.WriteLine($"Enter TFT name you want to Delete or Enter to Accept Name [ {Environment.MachineName}] ");
                        var toDeleteUnit = Console.ReadLine();
                        toDeleteUnit = string.IsNullOrEmpty(toDeleteUnit) ? $"{Environment.MachineName}" : $"{toDeleteUnit}";
                        Console.WriteLine($"Are you sure you want to delete  {toDeleteUnit} Y/N ? ");
                        var response = Console.ReadLine();
                        if (response == "y" || response == "Y")
                            adhocManager.GetUnitAsync(toDeleteUnit).ContinueWith(
                                prev =>
                                    {
                                        if (prev.Result != null)
                                        {
                                            var unit = prev.Result;
                                            Console.WriteLine($"Deleting Unit Status:  {adhocManager.DeleteUnit(unit.Id)}");
                                        }
                                        else
                                        {
                                            Console.WriteLine($" {toDeleteUnit} was NOT deleted.");
                                        }
                                    },
                                TaskContinuationOptions.OnlyOnRanToCompletion);
                        else Console.WriteLine($" {toDeleteUnit} was NOT deleted.");

                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.B)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Units");
                        try
                        {
                            var getAllUnitsTask = new Task<List<Unit>>(() => adhocManager.GetAllUnitsAsync().Result);
                            var displayLoadedData = getAllUnitsTask.ContinueWith(
                                prev =>
                                    {
                                        prev.Result?.ForEach(i => Console.WriteLine("{0}", i));
                                        Console.WriteLine(prev.Result != null ? $" Sucessfully Got {prev.Result.Count} Units Async" : "Failed to Get any Unit");
                                    });
                            var displayHelp = displayLoadedData.ContinueWith(p => { DisplayHelp(adhocManager.AdhocConfiguration); });
                            getAllUnitsTask.Start();
                            Task.WaitAll(displayLoadedData, displayHelp);
                            getAllUnitsTask.Dispose();
                            displayLoadedData.Dispose();
                            displayHelp.Dispose();
                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine(ex);
                        }
                        catch (WebException ex)
                        {
                            Console.WriteLine(ex);
                        }
                        catch (TimeoutException exception)
                        {
                            Console.WriteLine(exception);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.N)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Units Async");

                        var t = Task.Run(() => adhocManager.GetAllUnitsAsync()).ContinueWith(
                            prev =>
                                {
                                    Console.WriteLine(prev.Result != null && prev.Result.Count > 0 ? $" Sucessfully Got {prev.Result.Count} Units Async" : "Failed to Get any Unit(s)");
                                    prev.Result?.ForEach(i => Console.Write("{0}\n", i));
                                    DisplayHelp(adhocManager.AdhocConfiguration);
                                },
                            TaskContinuationOptions.OnlyOnRanToCompletion);

                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.C)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Vehicle");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        Console.WriteLine($"Enter Vehicle name you want to Register or Enter to Accept Name [ Vehicle-TEST {next}] ");
                        var toRegisterBus = Console.ReadLine();

                        var bus = new Vehicle
                        {
                            Description = string.IsNullOrEmpty(toRegisterBus) ? $"Vehicle Desc {next}" : $"Vehicle for {toRegisterBus}",
                            Name = string.IsNullOrEmpty(toRegisterBus) ? $"Vehicle Name {next}" : $"{toRegisterBus}",
                            TenantId = AdhocConstants.DefaultTenentId
                        };

                        adhocManager.RegisterVehicle(bus);
                        Console.WriteLine(adhocManager.RegisterVehicle(bus) ? " Sucessfully Registered Bus" : "Failed to Register Bus");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.V)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Vehicle Async");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        Console.WriteLine($"Enter Vehicle name you want to Register or Enter to Accept Name [ Vehicle-TEST {next}] ");
                        var toRegisterBus = Console.ReadLine();
                        var bus = new Vehicle
                        {
                            Description = string.IsNullOrEmpty(toRegisterBus) ? $"Vehicle Desc {next}" : $"Vehicle Desc {toRegisterBus}",
                            Name = string.IsNullOrEmpty(toRegisterBus) ? $"Vehicle Name {next}" : $"{toRegisterBus}",
                            TenantId = AdhocConstants.DefaultTenentId
                        };

                        var res = adhocManager.RegisterVehicleAsync(bus);
                        Console.WriteLine(res.Result == HttpStatusCode.OK ? " Sucessfully Registered Vechile" : "Failed to Register Vechile");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.R)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Vehicle with Unit Async");
                        Console.WriteLine($"Enter TFT name you want to Register or Enter to Accept Name [ {Environment.MachineName}] ");
                        var toRegisterTft = Console.ReadLine();
                        toRegisterTft = string.IsNullOrEmpty(toRegisterTft) ? $"{Environment.MachineName}" : $"{toRegisterTft}";
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        var t = Task.Run(() => adhocManager.RegisterVehicleAndUnitAsync($"Vehicle {next} {DateTime.Now.ToShortDateString()}", $"{toRegisterTft}")).ContinueWith(
                            prev =>
                                {
                                    Console.WriteLine(prev.Result == HttpStatusCode.OK ? " Sucessfully Registered Vechile with unit" : "Failed to Register Vechile with Unit");
                                    DisplayHelp(adhocManager.AdhocConfiguration);
                                },
                            TaskContinuationOptions.OnlyOnRanToCompletion);
                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.G)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Vehicles Async");
                        var t = Task.Run(() => adhocManager.GetAllVechiclesAsync()).ContinueWith(
                            prev =>
                                {
                                    Console.WriteLine(prev.Result != null ? $" Sucessfully Got {prev.Result.Count} Vechicles Async" : "Failed to get any Vehicles");
                                    prev.Result?.ForEach(i => Console.Write("{0}\n", i));
                                    DisplayHelp(adhocManager.AdhocConfiguration);
                                },
                            TaskContinuationOptions.OnlyOnRanToCompletion);

                        info = default(ConsoleKeyInfo);
                    }

                    else if (info.Key == ConsoleKey.E)
                    {
                        Console.Clear();
                        Console.WriteLine($"Checking if Unit Exists. Enter the unit Id or press enter to use [{Environment.MachineName}]");
                        var id = Console.ReadLine();
                        id = string.IsNullOrEmpty(id) ? $"{Environment.MachineName}" : id;
                        Console.WriteLine($"Units Exists : {adhocManager.UnitExistsAsync(id).Result}");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.O)
                    {
                        Console.Clear();
                        Console.WriteLine("Checking if Vehicle Exists. Enter the vehicle Id or press enter to use [Test Vehicle 640]");
                        var id = Console.ReadLine();
                        id = string.IsNullOrEmpty(id) ? "Test Vehicle 640" : id;
                        Console.WriteLine($"Vehicle Exists : {adhocManager.VehicleExistsAsync(id).Result}");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.F)
                    {
                        Console.Clear();
                        Console.WriteLine($"Units Exists : {adhocManager.UnitExistsAsync("123456-d2a6-4786-7ff1-08d55f86ff3a").Result} <== Should Fail - With NoContent!");

                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.M)
                    {
                        Console.Clear();
                        Console.WriteLine($"Enter the Unit Name. Press Enter to use [{Environment.MachineName}]");
                        var id = Console.ReadLine();
                        id = string.IsNullOrEmpty(id) ? $"{Environment.MachineName}" : id;
                        Console.WriteLine("Enter the Route Name. Press Enter for No route");
                        var route = Console.ReadLine();
                        route = string.IsNullOrEmpty(route) ? "" : route;
                        var res = adhocManager.GetAllMessagesForUnitAsync(id, route, DateTime.Now);
                        //  res.Result?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.L)
                    {
                        Console.Clear();
                        Console.WriteLine($"Enter the Unit Name. Press Enter to use [{Environment.MachineName}]");
                        var id = Console.ReadLine();
                        id = string.IsNullOrEmpty(id) ? $"{Environment.MachineName}" : id;
                        var res = adhocManager.GetAllMessagesForUnitAsync(id, "", DateTime.Now);
                        //  res?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.U)
                    {
                        Console.Clear();
                        var t = Task.Run(() => adhocManager.GetAllUnitsAsync()).ContinueWith(
                            prev =>
                                {
                                    if (prev.Result != null)
                                        foreach (var re in prev.Result)
                                        {
                                            Console.WriteLine($" Messages for Unit {re.Name} - {re.Id}");
                                            Console.WriteLine();
                                            var messages = adhocManager.GetAllMessagesForUnit(re.Name, string.Empty, DateTime.Now);
                                            messages?.ForEach(i => Console.Write("{0}\n", i));
                                            Console.WriteLine();
                                        }

                                    DisplayHelp(adhocManager.AdhocConfiguration);
                                },
                            TaskContinuationOptions.OnlyOnRanToCompletion);

                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.K)
                    {
                        Console.Clear();
                        adhocManager.UnitExistsAsync("1241214-26ef-4ae6-d56b-08d55fbb45c0");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.H)
                    {
                        Console.Clear();
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.D1)
                    {
                        Console.Clear();
                        Console.WriteLine($"Changed NLog Level to Error");
                        info = default(ConsoleKeyInfo);
                        ChangeNLogLevel(LogLevel.Error);
                    }
                    else if (info.Key == ConsoleKey.D2)
                    {
                        Console.Clear();
                        Console.WriteLine($"Changed NLog Level to Debug");
                        info = default(ConsoleKeyInfo);
                        ChangeNLogLevel(LogLevel.Debug);
                    }
                    else if (info.Key == ConsoleKey.D3)
                    {
                        Console.Clear();
                        Console.WriteLine($"Changed NLog Level to Info");
                        info = default(ConsoleKeyInfo);
                        ChangeNLogLevel(LogLevel.Info);
                    }
                    else if (info.Key == ConsoleKey.D4)
                    {
                        Console.Clear();
                        Console.WriteLine($"Changed NLog Level to Trace");
                        info = default(ConsoleKeyInfo);
                        ChangeNLogLevel(LogLevel.Trace);
                    }
                    else if (info.Key == ConsoleKey.F1)
                    {
                        Console.Clear();
                        ac = PickServer();
                        adhocManager = adhocMessageFactory.CreateAdhocManager(ac);
                        DisplayHelp(ac);
                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.F2)
                    {
                        Console.Clear();
                        foreach (DictionaryEntry e in Environment.GetEnvironmentVariables()) Console.WriteLine(e.Key + ":" + e.Value);
                        DisplayHelp(ac);
                        info = default(ConsoleKeyInfo);
                    }
                    else if (info.Key == ConsoleKey.Escape || info.Key == ConsoleKey.Q)
                    {
                        // info = default(ConsoleKeyInfo);
                        break;
                    }
            }
            while (info.Key != ConsoleKey.Escape);
        }

        #endregion
    }
}