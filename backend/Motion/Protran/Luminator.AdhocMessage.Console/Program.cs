namespace Luminator.AdhocMessage.Console
{
    using System;

    using Luminator.AdhocMessaging;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var rnd = new Random();
            var adhocMessageFactory = new AdhocMessageFactory();

            IAdhocManager adhocManager = null;

            Console.WriteLine("Enter RestAPI Address - for ex http://swdevapi/ or http://localhost:65432. Leave empty to use default");
            var server = Console.ReadLine();
            Console.WriteLine("Enter the port number to use. Leave empty to use default. Note that localhost will require a port");
            var port = Console.ReadLine();
            var ac = new AdhocConfiguration(server, port);
            adhocManager = server == string.Empty ? adhocMessageFactory.CreateAdhocManager(new AdhocConfiguration()) : adhocMessageFactory.CreateAdhocManager(ac);
            DisplayHelp(adhocManager.AdhocConfiguration);
            ConsoleKeyInfo info;
            do
            {
                info = Console.ReadKey();
                while (!Console.KeyAvailable)
                    if (info.Key == ConsoleKey.A)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Unit");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        var tft = new Unit
                                      {
                                          Description = $"Test {next}",
                                          Name = $"TFT-TEST {next}",
                                          CreatedOn = DateTime.Now,
                                          TenantId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53",
                                          ProductTypeId = 32,
                                          UpdateGroupId = 121,
                                          Version = 4
                                      };
                        Console.WriteLine(adhocManager.RegisterUnit(tft) ? " Sucessfully Registered Unit" : "Failed to Register Unit");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.S)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Unit");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        var tft = new Unit
                                      {
                                          Description = $"Test {next}",
                                          Name = $"TFT-TEST {next}",
                                          CreatedOn = DateTime.Now,
                                          TenantId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53",
                                          ProductTypeId = 32,
                                          UpdateGroupId = 121,
                                          Version = 4
                                      };
                        var res = adhocManager.RegisterUnitAsync(tft);
                        Console.WriteLine(res.Result ? " Sucessfully Registered Unit" : "Failed to Register Unit");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.B)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Units");
                        var res = adhocManager.GetAllUnits();
                        res?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.N)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Units Async");
                        var res = adhocManager.GetAllUnitsAsync();
                        Console.WriteLine(res.Result != null ? $" Sucessfully Got {res.Result.Count} Units Async" : "Failed to Get any Unit");
                        res.Result?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.C)
                    {
                        Console.Clear();
                        Console.WriteLine("Registering Vehicle");
                        var next = rnd.Next(rnd.Next(100, 1000), 1500);
                        var bus = new Vehicle
                                      {
                                          Description = $"Test {next}",
                                          Name = $"TFT-TEST {next}",
                                          TenantId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53"
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
                        var bus = new Vehicle
                                      {
                                          Description = $"Vechicle {next}",
                                          Name = $"Vechile-TEST {next}",
                                          TenantId = "7141d8e5-b8e1-425a-21e5-08d541a9fc53"
                                      };
                        var res = adhocManager.RegisterVehicleAsync(bus);
                        Console.WriteLine(res.Result ? " Sucessfully Registered Vechile" : "Failed to Register Vechile");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.G)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Vehicles Async");
                        var res = adhocManager.GetAllVechiclesAsync();
                        Console.WriteLine(res.Result != null ? $" Sucessfully Got {res.Result.Count} Vechicles Async" : "Failed to get any Vehicles");
                        res.Result?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.D)
                    {
                        Console.Clear();
                        Console.WriteLine("Listing Vehicles");
                        var res = adhocManager.GetAllVechicles();
                        res?.ForEach(i => Console.Write("{0}\n", i));
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.E)
                    {
                        Console.Clear();
                        Console.WriteLine("Checking if Unit Exists. Enter the unit Id or press enter to use [90d8e05f-26ef-4ae6-d56b-08d55fbb45c0]");
                        var id = Console.ReadLine();
                        id = string.IsNullOrEmpty(id) ? "90d8e05f-26ef-4ae6-d56b-08d55fbb45c0" : id;
                        adhocManager.UnitExists(id);
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.F)
                    {
                        Console.Clear();
                        adhocManager.UnitExists("1241214-26ef-4ae6-d56b-08d55fbb45c0");
                        info = default(ConsoleKeyInfo);
                        DisplayHelp(adhocManager.AdhocConfiguration);
                    }
                    else if (info.Key == ConsoleKey.Escape || info.Key == ConsoleKey.Q)
                    {
                        // info = default(ConsoleKeyInfo);
                        break;
                    }
            }
            while (info.Key != ConsoleKey.Escape);
        }

        private static void DisplayHelp(AdhocConfiguration ac)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("========================================= Commands Help ==================================");
            Console.WriteLine($"                 Using {ac.BaseUrl}:{ac.Port} Url for Rest Operations");
            Console.WriteLine("  A - Register Unit,    S - Register Unit Async,     B - List Units,     N - List units async");
            Console.WriteLine("  C - Register Vehicle, V - Register Vechicle Async, D - List Vechicles, G - List Vechicles Async");
            Console.WriteLine("  M - Get Messages for Unit (NA), K - Get Messages for Vechicle (NA)");
            Console.WriteLine("  E to check if Unit Exists ,     F -  Check for not existing Unit");
            Console.WriteLine("================================== Press ESC to Exit ============================");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}