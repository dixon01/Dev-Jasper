// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationTypeDefinition.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The AnimationTypeDefinition.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// The Animation Type Definition
    /// </summary>
    public class AnimationTypeDefinition : ViewModelBase
    {
        private const string LocalizationPrefix = "AnimationEditor_Eval_";

        private PropertyChangeAnimationType animationType;

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationTypeDefinition"/> class.
        /// </summary>
        /// <param name="animationType">the animation Type</param>
        public AnimationTypeDefinition(PropertyChangeAnimationType animationType)
        {
            this.AnimationType = animationType;
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.SetProperty(ref this.name, value, () => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the animation type
        /// </summary>
        public PropertyChangeAnimationType AnimationType
        {
            get
            {
                return this.animationType;
            }

            set
            {
                this.SetProperty(ref this.animationType, value, () => this.AnimationType);

                this.Name = MediaStrings.ResourceManager.GetString(LocalizationPrefix + this.animationType);
            }
        }
    }
}