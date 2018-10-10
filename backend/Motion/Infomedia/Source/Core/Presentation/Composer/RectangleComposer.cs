// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for an <see cref="RectangleElement"/>.
    /// It creates an <see cref="RectangleItem"/>.
    /// </summary>
    public partial class RectangleComposer
    {
        partial void Update()
        {
            this.Item.SetColor(this.HandlerColor.StringValue, this.HandlerColor.Animation);
        }
    }
}
