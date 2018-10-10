// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenIdentifier.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenIdentifier type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Identifier of a screen.
    /// </summary>
    [DataContract]
    [DebuggerDisplay("{DeviceName}")]
    public class ScreenIdentifier : IEquatable<ScreenIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenIdentifier"/> class.
        /// </summary>
        /// <param name="deviceName">
        /// The name.
        /// </param>
        /// <exception cref="ArgumentException">The <paramref name="deviceName"/> is null or empty.</exception>
        public ScreenIdentifier(string deviceName)
        {
            if (string.IsNullOrEmpty(deviceName))
            {
                throw new ArgumentException("deviceName");
            }

            this.DeviceName = deviceName;
        }

        /// <summary>
        /// Gets the identifier of the primary screen.
        /// </summary>
        public static ScreenIdentifier Primary
        {
            get { return new ScreenIdentifier(System.Windows.Forms.Screen.PrimaryScreen.DeviceName); }
        }

        /// <summary>
        /// Gets the name of the device corresponding to the screen.
        /// </summary>
        [DataMember]
        public string DeviceName { get; private set; }

        /// <summary>
        /// Gets all available screens.
        /// </summary>
        /// <returns>
        /// The <see cref="ScreenIdentifier"/>s of the available screens.
        /// </returns>
        public static IEnumerable<ScreenIdentifier> GetAvailableScreens()
        {
            return System.Windows.Forms.Screen.AllScreens.Select(screen => new ScreenIdentifier(screen.DeviceName));
        }

        /// <summary>
        /// Gets the identifier of the screen currently displaying the given window.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <returns>
        /// The <see cref="ScreenIdentifier"/> of the screen displaying the given window.
        /// </returns>
        public static ScreenIdentifier GetFrom(Window window)
        {
            var screen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(window).Handle);
            return new ScreenIdentifier(screen.DeviceName);
        }

        /// <summary>
        /// Verifies if this screen is available.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this screen is available; <c>false</c> otherwise.
        /// </returns>
        public bool IsScreenAvailable()
        {
            return GetAvailableScreens().Any(s => s.Equals(this));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ScreenIdentifier other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.DeviceName, other.DeviceName);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <returns>
        /// true if the specified object  is equal to the current object; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ScreenIdentifier)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return this.DeviceName.GetHashCode();
        }
    }
}