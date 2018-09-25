// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.CF35.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    using System;

    /// <summary>
    /// Factory class that creates all channels for a given configuration.
    /// </summary>
    public partial class ChannelFactory
    {
        private IbisChannel CreateJsonIbisChannel(IIbisConfigContext configContext)
        {
            throw new NotSupportedException("JSON source is not supported in Compact Framework");
        }
    }
}
