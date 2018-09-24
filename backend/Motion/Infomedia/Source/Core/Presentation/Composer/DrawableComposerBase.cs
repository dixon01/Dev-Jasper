// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawableComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DrawableComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for all composers that are representing a
    /// <see cref="DrawableItemBase"/>. This class takes care about
    /// the generic aspects of a layout element.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of element used by this composer.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of item created by this composer.
    /// </typeparam>
    public abstract partial class DrawableComposerBase<TElement, TItem>
    {
        /// <summary>
        /// Is the element currently visible or not.
        /// </summary>
        private bool visible;
        
        /// <summary>
        /// Raises the <see cref="GraphicalComposerBase{TElement}.VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseVisibleChanged(EventArgs e)
        {
            base.RaiseVisibleChanged(e);
            this.SetItemVisibility();
        }

        /// <summary>
        /// Sets the <see cref="GraphicalItemBase.Visible"/> flag from the
        /// result of <see cref="GraphicalComposerBase{TElement}.IsVisible"/>.
        /// </summary>
        protected void SetItemVisibility()
        {
            this.Item.SetVisible(this.IsVisible(), this.VisibleHandler.Animation);
        }

        /// <summary>
        /// Sends an element update message for presentation play logging.
        /// </summary>
        /// <param name="element">The graphical element</param>
        /// <param name="fileName">The element's source filename</param>
        /// <param name="status">The update status</param>
        protected void SendElementUpdateMessage(DrawableElementBase element, string fileName, DrawableStatus status)
        {
            if (this.visible || this.IsVisible())
            {
                this.visible = true;
                var message = new DrawableComposerInitMessage
                                  {
                                      UnitName = MessageDispatcher.Instance.LocalAddress.Unit,
                                      ElementID = element.Id,
                                      ElementFileName = fileName,
                                      Status = status
                                  };

                MessageDispatcher.Instance.Broadcast(message);
            }
        }

        partial void PostInitializeItem()
        {
            this.Item.X = this.X;
            this.Item.Y = this.Y;
            this.Item.Width = this.Width;
            this.Item.Height = this.Height;
            this.SetItemVisibility();
        }
    }
}