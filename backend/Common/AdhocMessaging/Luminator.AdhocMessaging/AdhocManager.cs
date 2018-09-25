namespace Luminator.AdhocMessaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;

    using Luminator.AdhocMessaging.Helpers;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;

    using Newtonsoft.Json;

    using NLog;

    public class AdhocManager : IAdhocManager
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Properties

        public AdhocConfiguration AdhocConfiguration { get; set; }

        #endregion

        #region Constructors and Destructors

        public AdhocManager(AdhocConfiguration adhocConfiguration)
        {
            this.AdhocConfiguration = adhocConfiguration;
            //  var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            //  serviceContainer.RegisterInstance(this);
        }

        public AdhocManager(string serverUrl, string messageApiUrl = "")
        {
            if (this.AdhocConfiguration == null) this.AdhocConfiguration = new AdhocConfiguration(serverUrl, string.Empty, messageApiUrl, string.Empty);
        }

        #endregion

        #region Public Methods and Operators

        public List<Message> GetAllMessagesForUnit(string unitIdOrName, string onRoute, DateTime? unitDateTime = null)
        {
            var messagesForUnit = new List<Message>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.MessageApiPort == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.MessageApiBaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.MessageApiBaseUrl}:{this.AdhocConfiguration.MessageApiPort}/");

                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                        var datetimeFormated = unitDateTime?.ToString("yyyy-MM-dd h:mm tt");
                        var formattedUrl = !string.IsNullOrEmpty(onRoute)
                                               ? $"api/Message/ForUnit/{unitIdOrName}/Route/{onRoute}/{datetimeFormated}"
                                               : $"api/Message/ForUnit/{unitIdOrName}/Route/\"/{datetimeFormated}";

                        var response = client.GetAsync(formattedUrl).Result;
                        Logger.Info(formattedUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            messagesForUnit = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
                            Logger.Info($"Got back {messagesForUnit.Count} messages for {unitIdOrName}");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            messagesForUnit?.ForEach(i => Logger.Debug("{0}\n", i));
            return messagesForUnit;
        }

        public async Task<List<Message>> GetAllMessagesForUnitAsync(string unitIdOrName, string onRoute, DateTime? unitDateTime = null)
        {
            var messagesForUnit = new List<Message>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.MessageApiPort == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.MessageApiBaseUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.MessageApiBaseUrl}:{this.AdhocConfiguration.MessageApiPort}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                        unitDateTime = unitDateTime?.ToUniversalTime() ?? DateTime.Now.ToUniversalTime();
                        var datetimeFormated = unitDateTime.Value.ToString("yyyy-MM-dd h:mm tt");
                        var formattedUrl = !string.IsNullOrEmpty(onRoute)
                                               ? $"api/Message/ForUnit/{unitIdOrName}/Route/{onRoute}/{datetimeFormated}"
                                               : $"api/Message/ForUnit/{unitIdOrName}/Route/\"/{datetimeFormated}";

                        var response = await client.GetAsync(formattedUrl);
                        Logger.Info(formattedUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            messagesForUnit = await response.Content.ReadAsAsync<List<Message>>(); // Message.FromJson(response.Content.ToString()).ToList(); 
                            Logger.Info($"Got back {messagesForUnit.Count} messages for {unitIdOrName}");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            messagesForUnit?.ForEach(i => Logger.Debug("{0}\n", i));
            return messagesForUnit;
        }

        public async Task<Unit> GetUnitAsync(string unitName)
        {
            var unit = new Unit();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        var response = await client.GetAsync($"api/Unit/all/{this.AdhocConfiguration.TenentId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var tftUnits = await response.Content.ReadAsAsync<List<Unit>>();
                            unit = tftUnits.FirstOrDefault(x => x.Name == unitName);
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return unit;
        }

        public async Task<List<Unit>> GetAllUnitsAsync()
        {
            var tftUnits = new List<Unit>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        var response = await client.GetAsync($"api/Unit/all/{this.AdhocConfiguration.TenentId}");
                        if (response.IsSuccessStatusCode)
                        {
                            tftUnits = await response.Content.ReadAsAsync<List<Unit>>();
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw new HttpException((int)HttpStatusCode.ExpectationFailed, $"Rest API call failed {e}");
            }

            // tftUnits.ForEach(i => Console.Write("{0}\n", i));
            return tftUnits;
        }

        public async Task<List<Vehicle>> GetAllVechiclesAsync()
        {
            var vehicles = new List<Vehicle>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.GetAsync($"api/Vehicle/all/{this.AdhocConfiguration.TenentId}");
                        if (response.IsSuccessStatusCode)
                        {
                            vehicles = await response.Content.ReadAsAsync<List<Vehicle>>();
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            // vehicles?.ForEach(i => Logger.Debug("{0}\n", i));
            return vehicles;
        }

        public HttpStatusCode RegisterUnit(Unit unit)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var jsonString = unit.ToJsonString();
                        Console.WriteLine(jsonString);
                        var response = client.PostAsJsonAsync("api/Unit", unit).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            Logger.Debug("Successfully registered a unit");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at {client.BaseAddress}");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }

                        return response.StatusCode;
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public HttpStatusCode RegisterUnit(string unitName, string tenantId = "", string description = "", string vehicleId = "", bool isActive = true)
        {
            var tft = new Unit { Description = description, Name = unitName, CreatedOn = DateTime.Now, TenantId = tenantId, VehicleId = vehicleId };

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = client.PostAsJsonAsync("api/Unit", tft).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            Logger.Debug("Successfully registered a unit");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }

                        return response.StatusCode;
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public HttpStatusCode DeleteVehicle(string vehicleName)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode DeleteUnit(string unitName)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        Logger.Debug($"URL to delete unit {client.BaseAddress}api/Unit/{unitName}");
                        var response = client.DeleteAsync($"api/Unit/{unitName}").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            Logger.Debug($"Successfully Deleted unit {unitName}");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }

                        return response.StatusCode;
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public async Task<HttpStatusCode> RegisterUnitAsync(Unit unit)
        {
            try
            {
                if (this.UnitExistsAsync(unit.Id).Result != HttpStatusCode.OK)
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                                 ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                                 : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                        if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                        {
                            client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var response = await client.PostAsJsonAsync("api/Unit", unit);
                            if (response.IsSuccessStatusCode)
                            {
                                Logger.Debug("Successfully registered a unit");
                            }
                            else
                            {
                                Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                                throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                            }

                            return response.StatusCode;
                        }
                        else
                        {
                            Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                            throw new UriFormatException();
                        }
                    }

                return HttpStatusCode.Found; // Unit Already Exists
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public async Task<HttpStatusCode> RegisterUnitAsync(string unitName, string tenantId = "", string description = "", string vehicleId = "", bool isActive = true)
        {
            var tft = new Unit { Description = description, Name = unitName, CreatedOn = DateTime.Now, TenantId = tenantId, VehicleId = vehicleId, IsActive = isActive };

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        Console.WriteLine($"Base Address: {client.BaseAddress}");
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        var response = await client.PostAsJsonAsync("api/Unit", tft);
                        if (response.IsSuccessStatusCode)
                        {
                            Logger.Debug("Successfully registered a unit");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress} ");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }

                        return response.StatusCode;
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public HttpStatusCode RegisterUnits(IList<Unit> units)
        {
            var httpStatusCode = HttpStatusCode.BadRequest;
            try
            {
                foreach (var unit in units) httpStatusCode = this.RegisterUnit(unit);
                return httpStatusCode;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return HttpStatusCode.ExpectationFailed;
            }
        }

        public bool RegisterVehicle(Vehicle bus)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);

                        var response = client.PostAsJsonAsync("api/Vehicle", bus).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            Logger.Debug("Success");
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress}");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return true;
        }

        public async Task<HttpStatusCode> RegisterVehicleAndUnitAsync(
            string vehicleId,
            string unitName,
            string description = "New Registration",
            string tenantId = AdhocConstants.DefaultTenentId,
            bool isActive = true)
        {
            if (string.IsNullOrEmpty(vehicleId)) throw new ArgumentException($"Invalid Arguments {nameof(vehicleId)} cannot be empty.", nameof(vehicleId));
            if (string.IsNullOrEmpty(unitName)) throw new ArgumentException($"Invalid Arguments {nameof(unitName)} cannot be empty.", nameof(unitName));
            var vehicle = new Vehicle { Name = vehicleId, Description = $"Vehicle {vehicleId} {description}", TenantId = tenantId, IsActive = isActive, Units = new List<Unit>() };
            var unit = new Unit { Name = unitName, Description = description, IsActive = isActive, TenantId = tenantId };
            vehicle.Units.Add(unit);
            return await this.RegisterVehicleAsync(vehicle);
        }

        public async Task<HttpStatusCode> RegisterVehicleAsync(Vehicle bus)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);
                        var jsonObjectForDebug = JsonConvert.SerializeObject(
                            bus,
                            Formatting.Indented,
                            new JsonSerializerSettings
                            { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        Logger.Info(jsonObjectForDebug);
                        var response = await client.PostAsJsonAsync("api/Vehicle", bus);
                        if (response.IsSuccessStatusCode && response.StatusCode == HttpStatusCode.OK)
                        {
                            Logger.Debug("Success registering vechicle async");
                            return response.StatusCode;
                        }

                        Logger.Error($" Rest API call failed at  {client.BaseAddress}");
                        throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return HttpStatusCode.BadRequest;
        }

        public async Task<HttpStatusCode> UnitExistsAsync(string unitId)
        {
            var status = HttpStatusCode.NotFound;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.GetAsync($"api/Unit/{unitId}");
                        //Logger.Error("Test Error");
                        //Logger.Debug("Test Debug");
                        //Logger.Info("Test Info");
                        //Logger.Trace("Test Trace");
                        status = response.StatusCode;
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return status;
        }

        public async Task<HttpStatusCode> VehicleExistsAsync(string vehicleId)
        {
            var status = HttpStatusCode.NotFound;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = this.AdhocConfiguration.Port == string.Empty
                                             ? new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}/")
                                             : new Uri($"{this.AdhocConfiguration.DestinationsApiUrl}:{this.AdhocConfiguration.Port}/");
                    if (NetworkHelper.IsValidUri(client.BaseAddress.AbsoluteUri))
                    {
                        client.Timeout = TimeSpan.FromMilliseconds(this.AdhocConfiguration.HttpClientTimeoutInMilliseconds);

                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var response = await client.GetAsync($"api/Vehicle/{vehicleId}");
                        if (response.IsSuccessStatusCode)
                        {
                            status = response.StatusCode; // HttpStatusCode.NoContent when not found, OK - when found
                        }
                        else
                        {
                            Logger.Error($" Rest API call failed at  {client.BaseAddress}");
                            throw new HttpException((int)response.StatusCode, $"Rest API call failed at  {client.BaseAddress} with {response.StatusCode}");
                        }
                    }
                    else
                    {
                        Logger.Error($" Invalid Base Uri  {client.BaseAddress.AbsoluteUri} ");
                        throw new UriFormatException();
                    }
                }
            }
            catch (TaskCanceledException ex)
            {
                Logger.Error(ex);
                throw new TimeoutException($"The rest call timed out {ex}");
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return status;
        }

        #endregion
    }
}