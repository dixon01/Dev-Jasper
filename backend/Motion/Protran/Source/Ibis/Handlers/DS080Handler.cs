// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS080Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS080Handler type.
// </summary>
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
    /// Handler for the DS080 telegram that also takes into account the DS010b if configured.
    /// </summary>
    public class DS080Handler : TelegramHandler<DS080>, IManageableObject
    {
        private int lastStopIndex = -1;

        private DS080Config config;

        private GenericUsageHandler usage;

        private string doorState;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS080Handler"/> class.
        /// </summary>
        public DS080Handler()
            : base(10)
        {
        }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="telegramConfig">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public override void Configure(TelegramConfig telegramConfig, IIbisConfigContext configContext)
        {
            this.config = (DS080Config)telegramConfig;
            this.usage = new GenericUsageHandler(this.config.UsedFor, configContext.Dictionary);
            base.Configure(telegramConfig, configContext);
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
            return telegram is DS080 || (this.config.ResetWithDS010B && telegram is DS010B);
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

            var ds080Tlg = telegram as DS080;
            if (ds080Tlg != null)
            {
                this.HandleInput(ds080Tlg);
            }
        }

        IEnumerable<IManagementProvider> IManageable.GetChildren(IManagementProvider parent)
        {
            yield break;
        }

        IEnumerable<ManagementProperty> IManageableObject.GetProperties()
        {
            yield return new ManagementProperty<string>("Door state", this.doorState, true);
        }

        /// <summary>
        /// Handles the IBIS telegram regarding the opened door
        /// in conjunction with the DS010b and generates Ximple.
        /// </summary>
        /// <param name="telegram">The telegram to be handled.</param>
        protected override void HandleInput(DS080 telegram)
        {
            this.NotifyDoorState(this.config.OpenValue);
        }

        /// <summary>
        /// Handles the IBIS DS010B telegram.
        /// </summary>
        /// <param name="telegram">The telegram to be handled.</param>
        private void HandleTelegram(DS010B telegram)
        {
            if (this.lastStopIndex == telegram.StopIndex || !this.config.ResetWithDS010B)
            {
                return;
            }

            // now I update my internal variable
            this.lastStopIndex = telegram.StopIndex;

            // and now it's the time to notify this information.
            this.NotifyDoorState(this.config.CloseValue);
        }

        /// <summary>
        /// Sends a XIMPLE structure having the information
        /// about the door status.
        /// </summary>
        /// <param name="value">
        /// The value to be set.
        /// </param>
        private void NotifyDoorState(string value)
        {
            this.doorState = value;

            // now I can send the XIMPLE with this information.
            var ximple = new Ximple();
            this.usage.AddCell(ximple, value);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
