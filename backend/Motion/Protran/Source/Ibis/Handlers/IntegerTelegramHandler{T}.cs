// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerTelegramHandler{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerTelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Motion.Protran.Core.Utils;

    /// <summary>
    /// Default telegram handler for <see cref="StringTelegram"/>s.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram to be handled by this handler.
    /// </typeparam>
    public class IntegerTelegramHandler<T> : TelegramHandler<T>
        where T : IntegerTelegram
    {
        private GenericUsageHandler usage;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerTelegramHandler{T}"/> class.
        /// The priority is set to 100.
        /// </summary>
        public IntegerTelegramHandler()
            : this(100)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerTelegramHandler{T}"/> class.
        /// </summary>
        /// <param name="priority">
        /// The priority, the lower the number the higher the priority.
        /// </param>
        protected IntegerTelegramHandler(int priority)
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
            this.usage.AddCell(ximple, telegram.Data.ToString(CultureInfo.InvariantCulture));
            this.RaiseXimpleCreated(new XimpleEventArgs(ximple));
        }
    }
}
