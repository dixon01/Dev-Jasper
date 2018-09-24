// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="IAudioSwitchContext.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Interfaces
{
    using System.IO;

    /// <summary>The IPeripheralContext interface.</summary>
    public interface IPeripheralContext
    {
        #region Public Properties

        /// <summary>Gets or sets the audio switch config.</summary>
        PeripheralConfig Config { get; set; }

        /// <summary>Gets or sets the stream.</summary>
        Stream Stream { get; set; }

        #endregion
    }
}