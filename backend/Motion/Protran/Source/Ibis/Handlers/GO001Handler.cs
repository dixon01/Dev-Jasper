// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO001Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Medi.Core.Management;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Telegram handler for IBIS telegram regarding the stop's approach.
    /// </summary>
    public class GO001Handler : TelegramHandler<GO001>, IManageableObject
    {
        private GenericUsageHandler usage;

        /// <summary>
        /// Variable that stores the index of the last stop.
        /// </summary>
        private int lastStopIndex = -1;

        /// <summary>
        /// Flags that tells whether the system is approaching to a stop or not.
        /// </summary>
        private bool isApproaching;

        /// <summary>
        /// Initializes a new instance of the <see cref="GO001Handler"/> class.
        /// </summary>
        public GO001Handler()
            : base(10)
        {
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig config, IIbisConfigContext configContext)
        {
            this.usage = new GenericUsageHandler(config.UsedFor, configContext.Dictionary);
            base.Configure(config, configContext);
        }

        /// <summary>
        /// Check whether this object can handle the given event.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        /// <returns>
        /// true if and only if this class can handle the given event.
        /// </returns>
        public override bool Accept(Telegram telegram)
        {
            return telegram is GO001 || telegram is DS010B;
        }

        /// <summary>
        /// Handles the event and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// the telegram.
        /// </param>
        public override void HandleInput(Telegram telegram)
        {
            if (this.Config == null)
            {
                // the telegram is not configured.
                // I cannot use it rightly.
                return;
            }

            var ds010BTlg = telegram as DS010B;
            if (ds010BTlg != null)
            {
                this.HandleTelegram(ds010BTlg);
                return;
            }

            var eventTlg = telegram as GO001;
            if (eventTlg != null)
            {
                this.HandleInput(eventTlg);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<bool>("Stop Approacing", this.isApproaching, true);
        }

        /// <summary>
        /// Handles the IBIS telegram regarding the stop approach
        /// in conjunction with the DS010b and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be handled.
        /// </param>
        protected override void HandleInput(GO001 telegram)
        {
            if (telegram.EventCode != 1)
            {
                return;
            }

            // we have received right now the GO001
            // so, it's the time to update the "IsApproaching" flag.
            this.isApproaching = true;

            // and now it's the time to notify this information.
            this.NotifyApproachingInfo();
        }

        /// <summary>
        /// Handles the IBIS DS010B telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram to be handled.
        /// </param>
        private void HandleTelegram(DS010B telegram)
        {
            if (this.lastStopIndex == telegram.StopIndex)
            {
                return;
            }

            // the stop index is changed.
            // this means that we are not anymore approaching to a stop
            // (rather, we are running to the next one).
            this.isApproaching = false;

            // now I update my internal variable
            this.lastStopIndex = telegram.StopIndex;

            // and now it's the time to notify this information.
            this.NotifyApproachingInfo();
        }

        /// <summary>
        /// Sends a XIMPLE structure having the information
        /// about the stop approaching status.
        /// </summary>
        private void NotifyApproachingInfo()
        {
            // now I can send the XIMPLE with this information.
            var ximple = new Ximple();
            this.usage.AddCell(ximple, this.isApproaching ? "1" : "0");
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
