// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ErrorHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Utility
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using NLog;

    /// <summary>
    /// Applies a behavior to handle and log errors.
    /// </summary>
    internal class ErrorHandler : Attribute, IErrorHandler, IServiceBehavior
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <inheritdoc />
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var operationName = string.Empty;
            var operationFullName = string.Empty;

            try
            {
                var operationContext = OperationContext.Current;
                var operation = GetOperation(operationContext);
                if (operation != null)
                {
                    operationName = operation.Name;
                    operationFullName = operationContext.Host.Description.ServiceType.FullName;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "ErrorHandler:ProvideFault():Couldn't get the operation info");
            }

            var faultException =
                new FaultException(
                    string.Format(
                        "Service error on Operation: {1}{0}Type: {2}{0}Message: {3}",
                        Environment.NewLine,
                        operationName,
                        operationFullName,
                        error.Message));

            var faultMessage = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, faultMessage, faultException.Action);
        }

        /// <inheritdoc />
        public bool HandleError(Exception error)
        {
            try
            {
                var operationContext = OperationContext.Current;
                var operation = GetOperation(operationContext);

                if (operation == null)
                {
                    Logger.Warn("ErrorHandler:HandleError(): Operation null");
                }
                else
                {
                    Logger.Warn(
                        "ErrorHandler:HandleError(): Operation {0} on type {1}",
                        operation.Name,
                        operationContext.Host.Description.ServiceType.FullName);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "ErrorHandler: HandleError():Couldn't get the operation info");
            }

            Logger.Error(error, "Error");

            // error is not handled, allows further processing
            return false;
        }

        /// <inheritdoc />
        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <inheritdoc />
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                channelDispatcher.ErrorHandlers.Add(this);
            }
        }

        /// <inheritdoc />
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        private static DispatchOperation GetOperation(OperationContext operationContext)
        {
            var operation =
                operationContext.EndpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(
                    o => o.Action == OperationContext.Current.IncomingMessageHeaders.Action);
            return operation;
        }
    }
}