// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdhocMessageService.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The adhoc message service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Motion.Protran.AdHocMessagingProtocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net;
    using System.Web;

    using AutoMapper;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.SystemManagement.Host.Path;

    using Luminator.AdhocMessaging;
    using Luminator.AdhocMessaging.Interfaces;
    using Luminator.AdhocMessaging.Models;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Models;

    using NLog;

    /// <summary>The adhoc message service.</summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    public class AdHocMessageService : IAdHocMessageService
    {
        private static readonly Logger Logging = LogManager.GetCurrentClassLogger();

        private IAdHocMessageServiceConfig adHocdHocMessageServiceConfig;

        private IAdhocManager manager;

        static AdHocMessageService()
        {
            // AutoMap model Types
            Mapper.Initialize(cfg => cfg.CreateMap<Message, XimpleAdHocMessage>());
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessageService"/> class.</summary>
        public AdHocMessageService()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessageService" /> class.</summary>
        /// <param name="config">The adhoc manager config.</param>
        /// <param name="manager">The adhoc manager.</param>
        public AdHocMessageService(IAdHocMessageServiceConfig config, IAdhocManager manager)
        {
            this.Configure(config, manager);
        }

        /// <summary>Gets the config.</summary>
        public IAdHocMessageServiceConfig Config
        {
            get => this.adHocdHocMessageServiceConfig;
            set
            {
                if (value != null)
                {
                    this.adHocdHocMessageServiceConfig = value;
                    Logging.Info("+AdHocMessageServiceConfig Changed");
                }
            }
        }

        private int MaxAdHocCount => this.Config?.MaxAdHocMessages > 0 ? this.Config.MaxAdHocMessages : 10;

        /// <summary>Configure the runtime.</summary>
        /// <param name="config">The adhoc manager config.</param>
        /// <param name="adhocManager">The manager.</param>
        public void Configure(IAdHocMessageServiceConfig config, IAdhocManager adhocManager)
        {
            if (adhocManager == null && config != null)
            {
                adhocManager = new AdhocMessageFactory().CreateAdhocManager(config.AdHocApiUri, config.DestinationsApiUri, config.ApiTimeout);
            }

            this.manager = adhocManager;
            this.Config = config;
        }

        /// <summary>Get the TFT Units adHoc messages.</summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="IAdHocMessages" />The AdHoc message response</returns>
        public IAdHocMessages GetUnitAdHocMessages(IAdHocGetMessagesRequest request)
        {
            var adHocMessages = new AdHocMessages { Status = HttpStatusCode.OK };

            if (request != null && request.IsValid)
            {
                // Make REST call to get the data, if successful fire the event
                Logging.Info("Request AdHocMessages Request={0}", request);
                var primaryUnit = request.FirstUnit;
                if (string.IsNullOrEmpty(primaryUnit))
                {
                    primaryUnit = MessageDispatcher.Instance.LocalAddress.Unit;
                }

                var route = request.Route;
                var localDateTime = request.UnitLocalTimeStamp ?? DateTime.Now;
                var messages = new List<Message>();
                try
                {
                    var task = this.manager.GetAllMessagesForUnitAsync(primaryUnit, route, localDateTime);
                    messages = task?.Result;
                }
                catch (HttpException httpException)
                {
                    HttpStatusCode code;
                    adHocMessages.Status = Enum.TryParse(httpException.GetHttpCode().ToString(), out code) ? code : HttpStatusCode.BadRequest;
                    Logging.Error("GetUnitAdHocMessages HttpException HttpStatusCode={0}, {1}", code, httpException.Message);
                    Debug.WriteLine(httpException.Message + " Code=" + code);
                }
                catch (Exception ex)
                {
                    adHocMessages.Status = HttpStatusCode.InternalServerError;
                    Logging.Error("GetUnitAdHocMessages General Exception {0}", ex.Message);
                }

                // map the response of one or more adhoc messages back. check the status.
                if (messages != null && adHocMessages.Status == HttpStatusCode.OK)
                {
                    var selectedMessages = messages.Take(this.MaxAdHocCount).ToList();
                    foreach (var m in selectedMessages)
                    {
                        var ximpleAdHocMessage = Mapper.Map<XimpleAdHocMessage>(m);
                        adHocMessages.Messages.Add(ximpleAdHocMessage);
                    }

                    // now add the remainder up to our max with empty messages
                    var count = this.MaxAdHocCount - adHocMessages.Messages.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var message = new Message { Text = string.Empty, Title = string.Empty };
                        var ximpleAdHocMessage = Mapper.Map<XimpleAdHocMessage>(message);
                        adHocMessages.Messages.Add(ximpleAdHocMessage);
                    }
                }
                else
                {
                    Logging.Info("No AdHoc Messages for Unit {0}", request.FirstUnit);
                }
            }
            else
            {
                throw new ArgumentException("Invalid request argument", nameof(request));
            }

            return adHocMessages;
        }

        /// <summary>Register vehicle and unit for Adhoc use with the desntiations service.</summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="IAdHocRegistrationResponse"/>.</returns>
        /// <exception cref="ArgumentException"></exception>
        public IAdHocRegistrationResponse RegisterVehicleAndUnit(IAdHocRegisterRequest request)
        {
            var response = new AdHocRegistrationResponse { Status = HttpStatusCode.OK };

            if (request != null && request.IsValid)
            {
                var unitNames = request.Units.Select(m => m.Name).ToList();
                var primaryUnitName = unitNames.FirstOrDefault();
                var vehicleId = request.VehicleId;
                if (string.IsNullOrEmpty(primaryUnitName) || string.IsNullOrEmpty(vehicleId))
                {
                    throw new ArgumentException("Invalid request argument", nameof(request));
                }

                var logFile = PathManager.Instance.CreatePath(FileType.Data, "AdHocRegistration.txt");

                try
                {
                    Logging.Info("Register Vehicle and Units Enter {0}", request);

                    var task = this.manager.RegisterVehicleAndUnitAsync(vehicleId, primaryUnitName);
                    response.Status = task?.Result ?? HttpStatusCode.BadRequest;
                    response.ResponseTimeStamp = DateTime.Now;
                    response.Response = $"Registration Completed for Vehicle:{request?.VehicleId}";
                
                    if (response.IsRegistered)
                    {
                        Debug.WriteLine("AdHoc Registration Successful " + request);
                        System.IO.File.WriteAllText(
                            logFile,
                            $"AdHoc registration successful for request {request} on {DateTime.Now}");
                    }
                    else
                    {
                        Debug.WriteLine("AdHoc Registration Failed! " + request);
                        System.IO.File.WriteAllText(
                            logFile,
                            $"! AdHoc registration failed for request {request} on {DateTime.Now}");
                    }
                }
                catch (HttpException httpException)
                {
                    HttpStatusCode code;
                    response.Status = Enum.TryParse(httpException.GetHttpCode().ToString(), out code) ? code : HttpStatusCode.BadRequest;
                    Logging.Error("RegisterUnit HttpException HttpStatusCode={0}, {1}", code, httpException.Message);
                }
                catch (Exception ex)
                {
                    response.Status = HttpStatusCode.InternalServerError;
                    Logging.Error("RegisterUnit General Exception {0}", ex.Message);
                }
            }
            else
            {
                throw new ArgumentException("Invalid request argument", nameof(request));
            }

            return response;
        }
    }
}