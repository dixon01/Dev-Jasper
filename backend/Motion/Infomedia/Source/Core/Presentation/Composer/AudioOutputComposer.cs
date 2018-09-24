// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioOutputComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioOutputComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Composer for <see cref="AudioOutputElement"/> that doesn't create a
    /// <see cref="ScreenItemBase"/> but just serves as a parent for all
    /// <see cref="PlaybackComposerBase{TItem}"/> implementations.
    /// </summary>
    public partial class AudioOutputComposer
    {
        /// <summary>
        /// Gets the sound output volume to set when playing the child elements.
        /// </summary>
        public int Volume
        {
            get
            {
                return this.Element.Volume;
            }
        }

        /// <summary>
        /// Gets the priority in which to play the child elements.
        /// The lower the number, the lower the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return this.Element.Priority;
            }
        }
    }
}