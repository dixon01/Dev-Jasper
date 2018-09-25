// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringArrayTelegramHandler{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StringArrayTelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Default telegram handler for <see cref="StringArrayTelegram"/>s.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram to be handled by this handler.
    /// </typeparam>
    public class StringArrayTelegramHandler<T> : TelegramHandler<T>
        where T : StringArrayTelegram
    {
        private GenericUsageHandler usage;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArrayTelegramHandler{T}"/> class.
        /// The priority is set to 100.
        /// </summary>
        public StringArrayTelegramHandler()
            : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringArrayTelegramHandler{T}"/> class.
        /// </summary>
        /// <param name="priority">
        /// The priority, the lower the number the higher the priority.
        /// </param>
        protected StringArrayTelegramHandler(int priority)
            : base(priority)
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
            base.Configure(config, configContext);

            this.usage = new GenericUsageHandler(config.UsedFor, configContext.Dictionary);
        }

        /// <summary>
        /// Handles the telegram and generates Ximple if needed.
        /// </summary>
        /// <param name="telegram">
        /// The telegram.
        /// </param>
        protected override void HandleInput(T telegram)
        {
            if (this.Config.UsedFor == null)
            {
                return;
            }

            var ximple = new Ximple();

            for (int i = 0; i < telegram.Data.Length; i++)
            {
                var value = telegram.Data[i];
                this.usage.AddCell(ximple, value, i);
            }

            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
