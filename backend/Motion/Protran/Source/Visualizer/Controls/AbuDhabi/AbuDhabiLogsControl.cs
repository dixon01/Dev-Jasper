// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbuDhabiLogsControl.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AbuDhabiLogsControl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Controls.AbuDhabi
{
    using Gorba.Motion.Protran.AbuDhabi;
    using Gorba.Motion.Protran.AbuDhabi.Isi;
    using Gorba.Motion.Protran.Visualizer.Data.AbuDhabi;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// Logs control for Abu Dhabi protocol.
    /// </summary>
    public partial class AbuDhabiLogsControl : LogsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbuDhabiLogsControl"/> class.
        /// </summary>
        public AbuDhabiLogsControl()
        {
            this.InitializeComponent();

            try
            {
                var service = ServiceLocator.Current.GetInstance<IAbuDhabiVisualizationService>();
                service.IsiMessageSent += this.ServiceOnIsiMessageSent;
                service.IsiMessageEnqueued += this.ServiceOnIsiMessageEnqueued;

                var protocol = ServiceLocator.Current.GetInstance<AbuDhabiProtocol>();
                protocol.XimpleCreated += (s, e) => this.AddXimpleEvent(e);
            }
            catch
            {
                // ignore, this only happens in design mode
            }
        }

        private void ServiceOnIsiMessageSent(object sender, IsiMessageEventArgs e)
        {
            this.AddEvent(EventType.TelegramOut, e.IsiMessage.ToString(), e);
        }

        private void ServiceOnIsiMessageEnqueued(object sender, IsiMessageEventArgs e)
        {
            this.AddEvent(EventType.TelegramIn, e.IsiMessage.ToString(), e);
        }
    }
}
