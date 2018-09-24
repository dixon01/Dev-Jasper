// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramHandler{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// Telegram handler base class that provides some helpful
    /// methods for implementing telegram handlers.
    /// </summary>
    /// <typeparam name="T">
    /// The type of telegram that is handled by this class.
    /// </typeparam>
    public abstract class TelegramHandler<T> : InputHandler<T>, ITelegramHandler
        where T : Telegram
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramHandler{T}"/> class.
        /// </summary>
        /// <param name="priority">
        /// The priority, the lower the number the higher the priority.
        /// </param>
        protected TelegramHandler(int priority)
            : base(priority)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this handler is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.Config.Enabled;
            }
        }

        /// <summary>
        /// Gets the telegram config.
        /// </summary>
        protected TelegramConfig Config { get; private set; }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="config">
        ///     The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        public virtual void Configure(TelegramConfig config, IIbisConfigContext configContext)
        {
            this.Configure(configContext);
            this.Config = config;
        }
    }
}
