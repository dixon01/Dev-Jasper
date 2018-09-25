// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelFactory.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChannelFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Channels
{
    /// <summary>
    /// Factory class that creates all channels for a given configuration.
    /// </summary>
    public partial class ChannelFactory
    {
        private IbisChannel CreateJsonIbisChannel(IIbisConfigContext configContext)
        {
            if (configContext.Config.Sources.Json == null)
            {
                return null;
            }

            return new JsonIbisChannel(configContext);
        }
    }
}
