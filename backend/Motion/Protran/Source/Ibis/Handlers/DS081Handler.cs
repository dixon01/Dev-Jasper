// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS081Handler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS081Handler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Handler for the DS081 telegram.
    /// </summary>
    public class DS081Handler : TelegramHandler<DS081>
    {
        private DS081Config config;

        private GenericUsageHandler usage;

        /// <summary>
        /// Initializes a new instance of the <see cref="DS081Handler"/> class.
        /// </summary>
        public DS081Handler()
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
            this.config = (DS081Config)telegramConfig;
            this.usage = new GenericUsageHandler(this.config.UsedFor, configContext.Dictionary);
            base.Configure(telegramConfig, configContext);
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// This method needs to be implemented by subclasses to
        /// create the Ximple object for the given telegram.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(DS081 telegram)
        {
            // now I can send the XIMPLE with this information.
            var ximple = new Ximple();
            this.usage.AddCell(ximple, this.config.Value);
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}