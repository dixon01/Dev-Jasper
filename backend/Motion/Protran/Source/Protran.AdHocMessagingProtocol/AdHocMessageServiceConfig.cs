// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdHocMessageServiceConfig.cs" company="Luminator LTG">
//   Copyright © 2011-2018 LuminatorLTG. All rights reserved.
// </copyright>
// <summary>
//   The ad hoc message service config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.Motion.Protran.AdHocMessagingProtocol
{
    using System;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    /// <summary>The ad hoc message service config.</summary>
    [Serializable]
    public class AdHocMessageServiceConfig : IAdHocMessageServiceConfig
    {
        /// <summary>The default api timeout.</summary>
        public const int DefaultApiTimeout = 30000;

        /// <summary>The default destinations api url.</summary>
        public const string DefaultDestinationsApiUrl = "http://swdevicntrapp.luminatorusa.com/";

        /// <summary>The default max ad hoc messages.</summary>
        public const int DefaultMaxAdHocMessages = 6;

        /// <summary>The default max registration attempts.</summary>
        public const int DefaultMaxRegistrationAttempts = 3;

        /// <summary>The default message api url.</summary>
        public const string DefaultMessageApiUrl = "http://swdevicntrweb.luminatorusa.com/";

        /// <param name="hostUri">The host uri.</param>
        /// <param name="destinationsUri">The destinations uri.</param>
        /// <param name="timeout">The timeout.</param>
        public AdHocMessageServiceConfig(Uri hostUri, Uri destinationsUri, int timeout = DefaultApiTimeout)
            : this()
        {
            this.AdHocApiUri = hostUri;
            this.DestinationsApiUri = destinationsUri;
            this.ApiTimeout = timeout;
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessageServiceConfig"/> class.</summary>
        public AdHocMessageServiceConfig()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocMessageServiceConfig" /> class.</summary>
        /// <param name="config">The config.</param>
        public AdHocMessageServiceConfig(IAdHocMessagingProtocolConfig config)
            : this()
        {
            this.AdHocApiUri = config.AdHocApiUri.HostUri;
            this.DestinationsApiUri = config.DestinationsApiUri.HostUri;
            this.ApiTimeout = config.ApiTimeout;
            this.MaxAdHocMessages = config.MaxAdHocMessages;
            this.MaxRegistrationAttempts = config.MaxAdhocRegistrationAttempts;
        }

        /// <summary>Gets or sets the ad hoc api uri.</summary>
        public Uri AdHocApiUri { get; set; }

        /// <summary>Gets or sets the api timeout.</summary>
        public int ApiTimeout { get; set; }

        /// <summary>Gets or sets the destinations api uri.</summary>
        public Uri DestinationsApiUri { get; set; }

        /// <summary>Gets or sets the max ad hoc messages.</summary>
        public int MaxAdHocMessages { get; set; } = DefaultMaxAdHocMessages;

        /// <summary>Gets or sets the max registration attempts.</summary>
        public int MaxRegistrationAttempts { get; set; } = DefaultMaxRegistrationAttempts;

        /// <summary>Create ad hoc message service config.</summary>
        /// <param name="adhocApiUrl">The ad hoc url.</param>
        /// <param name="destinationsUrl">The destinations url.</param>
        /// <param name="apiTimeout">The api timeout.</param>
        /// <param name="maxAdHocMessages">The max Ad Hoc Messages to return as Ximple.</param>
        /// <param name="maxRegistrationAttempts">The max Registration Attempts.</param>
        /// <returns>The <see cref="AdHocMessageServiceConfig"/>.</returns>
        public static AdHocMessageServiceConfig CreateAdHocMessageServiceConfig(
            string adhocApiUrl = DefaultMessageApiUrl,
            string destinationsUrl = DefaultDestinationsApiUrl,
            int apiTimeout = DefaultApiTimeout,
            int maxAdHocMessages = DefaultMaxAdHocMessages,
            int maxRegistrationAttempts = DefaultMaxRegistrationAttempts)
        {
            var config = new AdHocMessageServiceConfig
                             {
                                 AdHocApiUri = new Uri(adhocApiUrl),
                                 DestinationsApiUri = new Uri(destinationsUrl),
                                 ApiTimeout = apiTimeout,
                                 MaxAdHocMessages = maxAdHocMessages,
                                 MaxRegistrationAttempts = maxRegistrationAttempts
                             };
            return config;
        }
    }
}