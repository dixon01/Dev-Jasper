// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfotransitPresentationInfoScreenChangeMapping.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Motion.Infomedia.Entities.Messages;

    using Luminator.PresentationPlayLogging.Core.Models;

    /// <summary>The infotransit presentation info screen change message.</summary>
    [Obsolete("Future to map from a ScreenChange medi class message to ours")]
    public class InfotransitPresentationInfoScreenChangeRelayedMessage : ScreenChangeRelayedMessage<InfotransitPresentationInfo>
    {
        /// <summary>The from screen change.</summary>
        /// <param name="screenChange">The screen change.</param>
        /// <returns>The <see cref="InfotransitPresentationInfo"/>.</returns>
        public override InfotransitPresentationInfo FromScreenChange(ScreenChange screenChange)
        {
            // throw new NotImplementedException();
            var info = new InfotransitPresentationInfo();

            if (screenChange.Screen.Type != PhysicalScreenType.TFT)
            {
                return null;
            }

            // TODO
            // Handle TFT Screen change types here
            int elementId = screenChange.ScreenRoot.Root.ElementId;
            info.ResourceId = elementId.ToString();

            return info;
        }
    }
}