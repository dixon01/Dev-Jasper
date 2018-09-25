// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="SharedFilesConfig.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Protran.XimpleProtocol
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Xml.Serialization;

    /// <summary>The network ftp settings for third-party to access local Ftp server on the display.</summary>
    [Serializable]
    public class NetworkFtpSettings
    {
        #region Fields

        private Uri uri;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkFtpSettings"/> class. Initializes a new instance of the <see cref="SharedFilesConfig"/> class.</summary>
        /// <param name="userName">The user name.</param>
        /// <param name="password">The password.</param>
        /// <param name="uri">The uri.</param>
        /// <param name="enabled">The enabled.</param>
        public NetworkFtpSettings(string userName, string password, Uri uri, bool enabled = true)
            : this()
        {
            this.UserName = userName;
            this.Password = password;
            this.Uri = uri;
            this.Enabled = enabled;
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkFtpSettings"/> class. Initializes a new instance of the <see cref="SharedFilesConfig"/> class.</summary>
        public NetworkFtpSettings()
        {
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Enabled = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether enabled.</summary>
        public bool Enabled { get; set; }

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets the shared uri as string. </summary>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="uriString" /> is null. </exception>
        /// <exception cref="UriFormatException" accessor="set">
        ///     <paramref name="uriString" /> is empty.-or- The scheme specified in
        ///     <paramref name="uriString" /> is not correctly formed. See
        ///     <see cref="M:System.Uri.CheckSchemeName(System.String)" />.-or- <paramref name="uriString" /> contains too many
        ///     slashes.-or- The password specified in <paramref name="uriString" /> is not valid.-or- The host name specified in
        ///     <paramref name="uriString" /> is not valid.-or- The file name specified in <paramref name="uriString" /> is not
        ///     valid. -or- The user name specified in <paramref name="uriString" /> is not valid.-or- The host or authority name
        ///     specified in <paramref name="uriString" /> cannot be terminated by backslashes.-or- The port number specified in
        ///     <paramref name="uriString" /> is not valid or cannot be parsed.-or- The length of <paramref name="uriString" />
        ///     exceeds 65519 characters.-or- The length of the scheme specified in <paramref name="uriString" /> exceeds 1023
        ///     characters.-or- There is an invalid character sequence in <paramref name="uriString" />.-or- The MS-DOS path
        ///     specified in <paramref name="uriString" /> must start with c:\\.
        /// </exception>
        [XmlElement("Uri")]
        public string SharedUriString
        {
            get
            {
                if (this.Uri != null)
                {
                    return this.Uri.AbsoluteUri;
                }

                return string.Empty;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.Uri = new Uri(value);
                }
            }
        }

        /// <summary>Gets or sets the shared uri. Get the local IPV4 IP Address when the Uri Host equals 'localhost'</summary>
        /// <exception cref="ArgumentOutOfRangeException" accessor="set"><paramref name="port" /> is less than -1 or greater than 65,535. </exception>
        [XmlIgnore]
        public Uri Uri
        {
            get
            {
                return this.uri;
            }

            set
            {
                if (value != null && value.Host == "localhost")
                {
                    var uriBuilder = new UriBuilder(value.Scheme, GetIP4Address(), value.Port, value.PathAndQuery);
                    this.uri = uriBuilder.Uri;
                }
                else
                {
                    this.uri  = value;
                }
            }
        }

        /// <summary>Gets or sets the user name.</summary>
        public string UserName { get; set; }

        #endregion

        #region Methods

        public static string GetIP4Address()
        {
            var ip4Address = string.Empty;

            foreach (
                var ipAddress in Dns.GetHostAddresses(Dns.GetHostName()).Where(ipAddress => ipAddress.AddressFamily == AddressFamily.InterNetwork))
            {
                ip4Address = ipAddress.ToString();
                break;
            }

            return ip4Address;
        }

        #endregion
    }
}