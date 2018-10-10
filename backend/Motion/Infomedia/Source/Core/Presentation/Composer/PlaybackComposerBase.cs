// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlaybackComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PlaybackComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for all composers that convert <see cref="PlaybackElementBase"/>s into
    /// <see cref="PlaybackItemBase"/> objects.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of element used by this composer.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of item created by this composer.
    /// </typeparam>
    public abstract partial class PlaybackComposerBase<TElement, TItem>
    {
        /// <summary>
        /// Raises the <see cref="AudioComposerBase{TElement}.EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseEnabledChanged(System.EventArgs e)
        {
            base.RaiseEnabledChanged(e);
            this.Item.Enabled = this.IsEnabled();
        }

        partial void PostInitializeItem()
        {
            this.Item.Enabled = this.IsEnabled();
            if (this.OutputParent == null)
            {
                return;
            }

            this.Item.Volume = this.OutputParent.Volume;
            this.Item.Priority = this.OutputParent.Priority;
        }
    }
}