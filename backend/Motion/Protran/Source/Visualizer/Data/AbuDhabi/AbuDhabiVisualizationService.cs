// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiVisualizationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.AbuDhabi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Common.Utility.Compatibility.Container;
    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;
    using Gorba.Motion.Protran.AbuDhabi.Ctu;
    using Gorba.Motion.Protran.AbuDhabi.Isi;
    using Gorba.Motion.Protran.AbuDhabi.Transformations;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Visualization service implementation for Abu Dhabi protocol.
    /// </summary>
    public class AbuDhabiVisualizationService : IAbuDhabiVisualizationService
    {
        private VisualizerRemoteComputer remoteComputer;

        private AbuDhabiVisualizationService()
        {
        }

        /// <summary>
        /// Event that is fired whenever an ISI message was sent out from Protran.
        /// </summary>
        public event EventHandler<IsiMessageEventArgs> IsiMessageSent;

        /// <summary>
        /// Event that is fired whenever an ISII message has been enqueued in Protran.
        /// </summary>
        public event EventHandler<IsiMessageEventArgs> IsiMessageEnqueued;

        /// <summary>
        /// Event that is fired whenever an ISI data item has been transformed.
        /// </summary>
        public event EventHandler<DataItemTransformationEventArgs> DataItemTransformed;

        /// <summary>
        /// Register a new instance of this class with the service container.
        /// This also registers all special factories used to
        /// intercept events inside the IBIS protocol.
        /// </summary>
        public static void Register()
        {
            Core.Protran.SetupCoreServices();
            var service = new AbuDhabiVisualizationService();
            var serviceContainer = ServiceLocator.Current.GetInstance<IServiceContainer>();
            serviceContainer.RegisterInstance<IAbuDhabiVisualizationService>(service);
            serviceContainer.RegisterInstance<RemoteComputer>(new VisualizerRemoteComputer(service));
            serviceContainer.RegisterInstance<CtuClient>(new VisualizerCtuClient());
            serviceContainer.RegisterInstance<TransformationManager>(new VisualizerTransformationManager(service));
        }

        /// <summary>
        /// Enqueues a new ISI message like it were received from the OBU.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void EnqueueIsiMessage(IsiMessageBase message)
        {
            var remote = this.remoteComputer;
            if (remote == null)
            {
                return;
            }

            var handler = this.IsiMessageEnqueued;
            if (handler != null)
            {
                handler(this, new IsiMessageEventArgs { IsiMessage = message });
            }

            remote.EnqueueIsiMessage(message);
        }

        private void RaiseIsiMessageSent(IsiMessageEventArgs e)
        {
            var handler = this.IsiMessageSent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void RaiseDataItemTransformed(DataItemTransformationEventArgs e)
        {
            var handler = this.DataItemTransformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Subclass of RemoteComputer that fakes a remote OBU for Visualizer.
        /// </summary>
        private class VisualizerRemoteComputer : RemoteComputer
        {
            private readonly AbuDhabiVisualizationService service;

            public VisualizerRemoteComputer(AbuDhabiVisualizationService service)
            {
                this.service = service;
            }

            public override void Connect()
            {
                if (this.IsConnected)
                {
                    return;
                }

                this.IsConnected = true;
                ExecuteLater(1000, this.OnConnected);
            }

            public override void Disconnect()
            {
                if (!this.IsConnected)
                {
                    return;
                }

                this.service.remoteComputer = null;
                this.IsConnected = false;
            }

            public override void SendIsiMessage(IsiMessageBase message)
            {
                this.service.RaiseIsiMessageSent(new IsiMessageEventArgs { IsiMessage = message });
            }

            /// <summary>
            /// Enqueues a message like it would have come from the remote computer.
            /// </summary>
            /// <param name="message">the message.</param>
            public void EnqueueIsiMessage(IsiMessageBase message)
            {
                this.RaiseIsiMessageReceived(new IsiMessageEventArgs { IsiMessage = message });
            }

            private static void ExecuteLater(int delay, ThreadStart task)
            {
                var timer = new Timer(s => task());
                timer.Change(delay, Timeout.Infinite);
            }

            private void OnConnected()
            {
                this.RaiseConnected(EventArgs.Empty);

                this.service.remoteComputer = this;

                // we need to trigger the client to start sending requests
                var get = new IsiGet { Items = new DataItemRequestList(DataItemName.IsiClientRunsFtpTransfers) };
                this.service.EnqueueIsiMessage(get);
            }
        }

        /// <summary>
        /// Subclass of TransformationManager that creates special
        /// transformation chains for the visualizer.
        /// </summary>
        private class VisualizerTransformationManager : TransformationManager
        {
            private readonly AbuDhabiVisualizationService service;

            public VisualizerTransformationManager(AbuDhabiVisualizationService service)
            {
                this.service = service;
            }

            protected override TransformationChain CreateChain(Chain chain, ICollection<Chain> allChains)
            {
                var transform = new VisualizerTransformationChain(chain.Id, chain.ResolveReferences(allChains));
                transform.Transformed += (s, e) => this.service.RaiseDataItemTransformed(e);
                return transform;
            }
        }
    }
}
