// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Ibis.Core.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;
    using Gorba.Common.Protocols.Vdv300;
    using Gorba.Common.Protocols.Vdv300.Telegrams;

    /// <summary>
    /// The telegram handler base class.
    /// </summary>
    public abstract class TelegramProviderBase
    {
        /// <summary>
        /// An empty array of telegrams.
        /// </summary>
        protected static readonly Telegram[] EmptyTelegrams = new Telegram[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramProviderBase"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected TelegramProviderBase(TelegramConfigBase config, IIbisContext context)
        {
            this.Config = config;
            this.Context = context;
        }

        /// <summary>
        /// Event that is fired whenever an "event" telegram is created.
        /// </summary>
        public event EventHandler<TelegramEventArgs> TelegramCreated;

        /// <summary>
        /// Gets the config.
        /// </summary>
        public TelegramConfigBase Config { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this parser expects an answer from an IBIS slave.
        /// </summary>
        public bool ExpectsAnswer { get; protected set; }

        /// <summary>
        /// Gets the IBIS context.
        /// </summary>
        protected IIbisContext Context { get; private set; }

        /// <summary>
        /// Creates a provider for the given <paramref name="config"/>.
        /// </summary>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        /// <param name="context">
        /// The IBIS context.
        /// </param>
        /// <returns>
        /// The <see cref="TelegramProviderBase"/> implementation.
        /// </returns>
        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Big switch over all possible telegrams, not nicer if refactored into multiple methods")]
        public static TelegramProviderBase Create(TelegramConfigBase config, IIbisContext context)
        {
            var ds001 = config as DS001Config;
            if (ds001 != null)
            {
                return new DS001Provider(ds001, context);
            }

            var ds002 = config as DS002Config;
            if (ds002 != null)
            {
                return new DS002Provider(ds002, context);
            }

            var ds003 = config as DS003Config;
            if (ds003 != null)
            {
                return new DS003Provider(ds003, context);
            }

            var ds003C = config as DS003CConfig;
            if (ds003C != null)
            {
                return new DS003CProvider(ds003C, context);
            }

            var ds004 = config as DS004Config;
            if (ds004 != null)
            {
                return new DS004Provider(ds004, context);
            }

            var ds004A = config as DS004AConfig;
            if (ds004A != null)
            {
                return new DS004AProvider(ds004A, context);
            }

            var ds004B = config as DS004BConfig;
            if (ds004B != null)
            {
                return new DS004BProvider(ds004B, context);
            }

            var ds004C = config as DS004CConfig;
            if (ds004C != null)
            {
                return new DS004CProvider(ds004C, context);
            }

            var ds005 = config as DS005Config;
            if (ds005 != null)
            {
                return new DS005Provider(ds005, context);
            }

            var ds006 = config as DS006Config;
            if (ds006 != null)
            {
                return new DS006Provider(ds006, context);
            }

            var ds009 = config as DS009Config;
            if (ds009 != null)
            {
                return new DS009Provider(ds009, context);
            }

            var ds010J = config as DS010JConfig;
            if (ds010J != null)
            {
                return new DS010JProvider(ds010J, context);
            }

            var ds020 = config as DS020Config;
            if (ds020 != null)
            {
                return new DS020Provider(ds020, context);
            }

            var ds021C = config as DS021CConfig;
            if (ds021C != null)
            {
                return new DS021CProvider(ds021C, context);
            }

            var ds036 = config as DS036Config;
            if (ds036 != null)
            {
                return new DS036Provider(ds036, context);
            }

            var ds070 = config as DS070Config;
            if (ds070 != null)
            {
                return new DS070Provider(ds070, context);
            }

            var ds080 = config as DS080Config;
            if (ds080 != null)
            {
                return new DS080Provider(ds080, context);
            }

            var ds081 = config as DS081Config;
            if (ds081 != null)
            {
                return new DS081Provider(ds081, context);
            }

            var ds084 = config as DS084Config;
            if (ds084 != null)
            {
                return new DS084Provider(ds084, context);
            }

            throw new NotSupportedException("Telegram config not supported: " + config.GetType().Name);
        }

        /// <summary>
        /// Starts this provider.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stops this provider.
        /// </summary>
        public virtual void Stop()
        {
        }

        /// <summary>
        /// Creates the periodic telegrams if needed.
        /// This method is called frequently and should return quickly if no telegrams are required.
        /// </summary>
        /// <returns>
        /// The list of <see cref="Telegram"/>s, this can be empty if currently no periodic telegram is to be sent.
        /// </returns>
        public abstract IEnumerable<Telegram> CreatePeriodicTelegrams();

        /// <summary>
        /// Handles the answer to a telegram.
        /// </summary>
        /// <param name="answer">
        /// The answer or null if no answer was received.
        /// </param>
        /// <param name="telegram">
        /// The original telegram that was sent to get the answer.
        /// </param>
        /// <returns>
        /// True if the answer was recognized and expected and handled by this provider.
        /// </returns>
        public virtual bool HandleAnswer(Telegram answer, Telegram telegram)
        {
            return false;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.GetType().Name;
        }

        /// <summary>
        /// Raises the <see cref="TelegramCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseTelegramCreated(TelegramEventArgs e)
        {
            var handler = this.TelegramCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
