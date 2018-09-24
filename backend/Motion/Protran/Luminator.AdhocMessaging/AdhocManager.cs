namespace Luminator.AdhocMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Luminator.AdhocMessaging.Helpers;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;

    using NLog;

    public class AdhocManager : IAdhocManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AdhocManager(AdhocConfiguration adhocConfiguration)
        {
            this.AdhocConfiguration = adhocConfiguration;
        }

        public AdhocManager(string serverUrl)
        {
            if (this.AdhocConfiguration == null) this.AdhocConfiguration = new AdhocConfiguration(serverUrl, string.Empty);
        }

        public AdhocConfiguration AdhocConfiguration { get; set; }

        public bool RegisterUnit(Unit unit)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");
                    var response = client.PostAsJsonAsync("api/Unit", unit).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Debug(" Sucessfully Registered Unit ");
                    }
                    else
                    {
                        Logger.Error($" Error Registering Unit {unit}");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
            return true;
        }

        public bool UnitExists(string unitId)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    var result = client.DownloadString($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/api/Unit/{unitId}"); //URI  
                    var unit = result.ConvertJsonToClass<Unit>();
                    if (result != string.Empty)
                    {
                        Console.WriteLine($"Unit Exists {unit}");
                        return true;
                    }
                    Console.WriteLine("Unit was not found... ");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        public bool VehicleExists(string vechicleId)
        {
            throw new NotImplementedException();
        }

        public bool RegisterVehicle(Vehicle bus)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");
                    var response = client.PostAsJsonAsync("api/Vehicle", bus).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Debug("Success");
                    }
                    else
                    {
                        Logger.Error($" Error Registering Vehicle {bus}");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
            return true;
        }

       

        public List<Unit> GetAllUnits()
        {
            var tftUnits = new List<Unit>();
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    var result = client.DownloadString($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/api/Unit/all/{this.AdhocConfiguration.TenentId}"); //URI  
                    tftUnits = Helpers.Helpers.DeserializeToList<Unit>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            tftUnits.ForEach(i => Logger.Debug("{0}\n", i));
            return tftUnits;
        }

        public List<Vehicle> GetAllVechicles()
        {
            var vechiclesList = new List<Vehicle>();
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers.Add("Content-Type:application/json"); //Content-Type  
                    client.Headers.Add("Accept:application/json");
                    var result = client.DownloadString($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/api/Vehicle/all/{this.AdhocConfiguration.TenentId}"); //URI  
                    vechiclesList = Helpers.Helpers.DeserializeToList<Vehicle>(result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            vechiclesList.ForEach(i => Logger.Debug("{0}\n", i));
            return vechiclesList;
        }

        public async Task<bool> RegisterVehicleAsync(Vehicle bus)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");
                    var response = await client.PostAsJsonAsync("api/Vehicle", bus);
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Debug("Success registering vechicle async");
                         return response.IsSuccessStatusCode; 
                    }
                    else
                    {
                        Logger.Error($" Error Registering Vehicle {bus}");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
            return false;
        }

        public async Task<List<Vehicle>> GetAllVechiclesAsync()
        {
            var vehicles = new List<Vehicle>();
            try
            {
                using (var client = new HttpClient()) //WebClient  
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //GET Method  
                    HttpResponseMessage response = await client.GetAsync($"api/Vehicle/all/{this.AdhocConfiguration.TenentId}");
                    if (response.IsSuccessStatusCode)
                    {
                        vehicles = await response.Content.ReadAsAsync<List<Vehicle>>();
                    }
                    else
                    {
                        Console.WriteLine("Internal server Error");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
            vehicles?.ForEach(i => Logger.Debug("{0}\n", i));
            return vehicles;
        }

        public List<Message> GetAllMessagesForUnit(string forUnitIdOrName)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetAllMessagesForVechicle(string vechicleIdOrName)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetAllMessagesForUnitOnRoute(string forUnitIdOrName, string onRoute)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetAllMessagesForVechicleOnRoute(string vechicleIdOrName, string onRoute)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterUnitAsync(Unit unit)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");
                    var response = await client.PostAsJsonAsync("api/Unit", unit);
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.Debug("Success registering unit async");
                        return response.IsSuccessStatusCode; //Content.ReadAsAsync<bool>();
                    }
                    else
                    {
                        Logger.Error($" Error Registering Unit async {unit}");
                        return false;
                    }
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
            return false;
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            var tftUnits = new List<Unit>();
            try
            {
                using (var client = new HttpClient()) //WebClient  
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.BaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.BaseUrl}:{this.AdhocConfiguration.Port}/");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //GET Method  
                    HttpResponseMessage response = await client.GetAsync($"api/Unit/all/{this.AdhocConfiguration.TenentId}");
                    if (response.IsSuccessStatusCode)
                    {
                        tftUnits = await response.Content.ReadAsAsync<List<Unit>>();
                    }
                    else
                    {
                        Console.WriteLine("Internal server Error");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // throw;
            }
           // tftUnits.ForEach(i => Console.Write("{0}\n", i));
            return tftUnits;
        }
    }
}