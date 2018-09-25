// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalEditorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicalEditorViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// The graphical editor view model base.
    /// </summary>
    public abstract class GraphicalEditorViewModelBase : EditorViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalEditorViewModelBase"/> class.
        /// </summary>
        protected GraphicalEditorViewModelBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicalEditorViewModelBase"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        protected GraphicalEditorViewModelBase(IMediaShell parent, ICommandRegistry commandRegistry)
            : base(parent, commandRegistry)
        {
        }

        /// <summary>
        /// Gets the EditDynamicText Interaction Request.
        /// </summary>
        public IInteractionRequest EditDynamicTextInteractionRequest
        {
            get
            {
                return InteractionManager<EditDynamicTextPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the SelectMedia Interaction Request.
        /// </summary>
        public IInteractionRequest SelectMediaInteractionRequest
        {
            get
            {
                return InteractionManager<SelectMediaPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }

        /// <summary>
        /// Gets the SelectMedia Interaction Request.
        /// </summary>
        public IInteractionRequest EditImageListInteractionRequest
        {
            get
            {
                var request = InteractionManager<ImageListDictionaryPrompt>.Current.GetOrCreateInteractionRequest();

                return request;
            }
        }
    }
}