// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DepartureFile.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IntegrationTests.Arriva.DepartureFiles
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Object tasked to represent a generic Arriva's file
    /// having the departures inside.
    /// </summary>
    public class DepartureFile
    {
        private const string ExpirationFormat = "yyyy-MM-dd HH:mm";
        private DateTime expirationTime;
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="DepartureFile"/> class.
        /// </summary>
        public DepartureFile()
        {
            this.Content = string.Empty;
        }

        /// <summary>
        /// Gets or sets PcName.
        /// </summary>
        public string PcName
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.UpdateComputerName();
            }
        }

        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
        public DateTime ExpirationTime
        {
            get
            {
                string expiration = this.ExtractExpirationTime();
                var dateTime = DateTime.ParseExact(expiration, ExpirationFormat, null, DateTimeStyles.None);
                return dateTime;
            }

            set
            {
                this.expirationTime = value;
                this.UpdateExpirationTime();
            }
        }

        /// <summary>
        /// Gets or sets the content of this file.
        /// </summary>
        public string Content { get; set; }

        private void UpdateComputerName()
        {
            this.Content = this.Content.Replace("device-id=\"PC1237\"", "device-id=\"" + this.name + "\"");
        }

        private void UpdateExpirationTime()
        {
            var toBeReplaced = this.ExtractExpirationTime();
            if (string.IsNullOrEmpty(toBeReplaced))
            {
                // nothing to be replaced
                return;
            }

            string newExpiration = this.expirationTime.ToString(ExpirationFormat);
            this.Content = this.Content.Replace(toBeReplaced, newExpiration);
        }

        private string ExtractExpirationTime()
        {
            const string ExpirationToken = "expiration=";
            int indexStartExpToken = this.Content.IndexOf(ExpirationToken, System.StringComparison.Ordinal);
            if (indexStartExpToken != -1)
            {
                indexStartExpToken += ExpirationToken.Length;
            }

            int indexEndExpToken = this.Content.IndexOf(":", indexStartExpToken, System.StringComparison.Ordinal);
            if (indexEndExpToken != -1)
            {
                // I add the two digits about the minutes.
                indexEndExpToken += 2;
            }

            string toBeReplaced = this.Content.Substring(indexStartExpToken + 1, indexEndExpToken - indexStartExpToken);
            return toBeReplaced;
        }
    }
}
