// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockComposer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AnalogClockComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for an <see cref="AnalogClockElement"/>.
    /// It creates an <see cref="AnalogClockItem"/>.
    /// </summary>
    public partial class AnalogClockComposer
    {
        partial void Initialize()
        {
            this.Item.Hour = this.CreateHand(this.Element.Hour);
            this.Item.Minute = this.CreateHand(this.Element.Minute);
            this.Item.Seconds = this.CreateHand(this.Element.Seconds);
        }

        private AnalogClockHandItem CreateHand(AnalogClockHandElement hand)
        {
            if (hand == null || !hand.Visible || string.IsNullOrEmpty(hand.Filename))
            {
                return null;
            }

            var path = this.Context.Config.GetAbsolutePathRelatedToConfig(hand.Filename);
            if (!File.Exists(path))
            {
                return null;
            }

            return new AnalogClockHandItem
                {
                    Mode = hand.Mode,
                    CenterX = hand.CenterX,
                    CenterY = hand.CenterY,

                    Filename = path,
                    ZIndex = hand.ZIndex,

                    X = hand.X,
                    Y = hand.Y,
                    Width = hand.Width,
                    Height = hand.Height,
                    Visible = hand.Visible
                };
        }
    }
}