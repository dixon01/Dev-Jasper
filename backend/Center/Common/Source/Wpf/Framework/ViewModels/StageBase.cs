// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StageBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels
{
    using System;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Defines a base class for stages.
    /// A stage is a "partition" of a <see cref="StagedShellBase"/>.
    /// </summary>
    /// <typeparam name="TController">The type of the controller.</typeparam>
    public abstract class StageBase<TController> : ViewModelBase, IStage
        where TController : IStageController
    {
        private IStageAuthorizationContext stageAuthorizationContext;

        private int index;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        IStageController IStage.Controller { get; set; }

        IStageAuthorizationContext IStage.StageAuthorizationContext
        {
            get
            {
                return this.stageAuthorizationContext;
            }
        }

        /// <summary>
        /// Gets or sets the index in the list of stages.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index
        {
            get
            {
                return this.index;
            }

            set
            {
                this.SetProperty(ref this.index, value, () => this.Index);
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the navigation bar image.
        /// </summary>
        public BitmapImage NavBarImage { get; set; }

        /// <summary>
        /// Updates the specified stage authorization context.
        /// </summary>
        /// <param name="authorizationContext">The stage authorization context.</param>
        public virtual void UpdateAuthorizationContext(IStageAuthorizationContext authorizationContext)
        {
            if (authorizationContext == null)
            {
                throw new ArgumentNullException("authorizationContext");
            }

            this.stageAuthorizationContext = authorizationContext.Clone() as IStageAuthorizationContext;
            if (this.stageAuthorizationContext == null)
            {
                throw new ApplicationException("Null stage authorization context");
            }
        }
    }
}