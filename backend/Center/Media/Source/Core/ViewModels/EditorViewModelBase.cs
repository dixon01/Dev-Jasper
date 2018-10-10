// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class for all editor view models.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.Extensions;

    /// <summary>
    /// Base class for all editor view models. It contains properties and methods used by all editors.
    /// </summary>
    public abstract class EditorViewModelBase : ViewModelBase, IEditorViewModel
    {
        private readonly IComparer<GraphicalElementDataViewModelBase> zcomparer = new ZIndexComparer();

        private ExtendedObservableCollection<LayoutElementDataViewModelBase> selectedElements;

        private AudioOutputElementDataViewModel currentAudioOutputElement;

        private ExtendedObservableCollection<GraphicalElementDataViewModelBase> elements;

        private ILayoutRenderer layoutRenderer;

        private bool canCreateAudioElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorViewModelBase"/> class.
        /// </summary>
        protected EditorViewModelBase()
        {
            this.elements = new ExtendedObservableCollection<GraphicalElementDataViewModelBase>();
            this.selectedElements = new ExtendedObservableCollection<LayoutElementDataViewModelBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorViewModelBase"/> class.
        /// </summary>
        /// <param name="parent">the parent view model</param>
        /// <param name="commandRegistry">the Command Registry</param>
        protected EditorViewModelBase(IMediaShell parent, ICommandRegistry commandRegistry)
            : this()
        {
            this.Parent = (MediaShell)parent;
            this.CommandRegistry = commandRegistry;
        }

         /// <summary>
        /// Event to take a screenshot.
        /// </summary>
        public event EventHandler<ScreenshotEventArgs> ScreenshotTaken;

        /// <summary>
        /// Gets or sets the parent ViewModel
        /// </summary>
        public MediaShell Parent { get; set; }

        /// <summary>
        /// Gets or sets the layout elements.
        /// </summary>
        public ExtendedObservableCollection<GraphicalElementDataViewModelBase> Elements
        {
            get
            {
                return this.elements;
            }

            set
            {
                this.SetProperty(ref this.elements, value, () => this.Elements);
            }
        }

        /// <summary>
        /// Gets the snap configuration.
        /// </summary>
        public virtual SnapConfiguration SnapConfiguration
        {
            get
            {
                return SnapConfiguration.Disabled;
            }
        }

        /// <summary>
        /// Gets the paste configuration.
        /// </summary>
        public virtual PasteConfiguration PasteConfiguration
        {
            get
            {
                return PasteConfiguration.Default;
            }
        }

        /// <summary>
        /// Gets or sets the current selected layout elements
        /// </summary>
        public ExtendedObservableCollection<LayoutElementDataViewModelBase> SelectedElements
        {
            get
            {
                return this.selectedElements;
            }

            set
            {
                this.SetProperty(ref this.selectedElements, value, () => this.SelectedElements);
            }
        }

        /// <summary>
        /// Gets or sets the current audio output element.
        /// </summary>
        public AudioOutputElementDataViewModel CurrentAudioOutputElement
        {
            get
            {
                return this.currentAudioOutputElement;
            }

            set
            {
                this.SetProperty(ref this.currentAudioOutputElement, value, () => this.CurrentAudioOutputElement);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether can create audio elements.
        /// </summary>
        public bool CanCreateAudioElements
        {
            get
            {
                return this.canCreateAudioElements;
            }

            set
            {
                this.SetProperty(ref this.canCreateAudioElements, value, () => this.CanCreateAudioElements);
            }
        }

        /// <summary>
        /// Gets the create layout element command.
        /// </summary>
        public abstract ICommand CreateLayoutElementCommand { get; }

        /// <summary>
        /// Gets or sets the current layout renderer
        /// </summary>
        public ILayoutRenderer LayoutRenderer
        {
            get
            {
                return this.layoutRenderer;
            }

            set
            {
                this.SetProperty(ref this.layoutRenderer, value, () => this.LayoutRenderer);
            }
        }

        /// <summary>
        /// Gets the SelectLayoutElements Command.
        /// </summary>
        public ICommand SelectLayoutElementsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.SelectElements);
            }
        }

        /// <summary>
        /// Gets the DeleteElementsCommand Command.
        /// </summary>
        public ICommand DeleteElementsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.DeleteElements);
            }
        }

        /// <summary>
        /// Gets the SelectLayoutElement Command.
        /// </summary>
        public ICommand SelectLayoutElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.SelectElement);
            }
        }

        /// <summary>
        /// Gets the MoveSelectedElements Command.
        /// </summary>
        public ICommand MoveSelectedElementsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.MoveSelectedElements);
            }
        }

        /// <summary>
        /// Gets the UpdateElement command.
        /// </summary>
        public ICommand UpdateElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.UpdateElement);
            }
        }

        /// <summary>
        /// Gets the ResizeElement command.
        /// </summary>
        public ICommand ResizeElementCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.ResizeElement);
            }
        }

        /// <summary>
        /// Gets the ShowElementEditPopup command.
        /// </summary>
        public abstract ICommand ShowElementEditPopupCommand { get; }

        /// <summary>
        /// Gets or sets the command registry.
        /// </summary>
        protected ICommandRegistry CommandRegistry { get; set; }

        /// <summary>
        /// Takes a screenshot of the current selected Layout
        /// </summary>
        /// <param name="encoder">
        /// The <see cref="BitmapEncoder"/>.
        /// </param>
        public void TakeScreenshot(BitmapEncoder encoder)
        {
            var args = new ScreenshotEventArgs { Encoder = encoder };
            this.OnScreenshotTaken(args);
        }

        /// <summary>
        /// The ensure z order for editor elements.
        /// </summary>
        public void SortByZOrder()
        {
            this.Elements.Sort(this.zcomparer);
        }

        private void OnScreenshotTaken(ScreenshotEventArgs args)
        {
            var handler = this.ScreenshotTaken;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private class ZIndexComparer : IComparer<GraphicalElementDataViewModelBase>
        {
            public int Compare(GraphicalElementDataViewModelBase x, GraphicalElementDataViewModelBase y)
            {
                var xdrawable = x as DrawableElementDataViewModelBase;
                var ydrawable = y as DrawableElementDataViewModelBase;

                // both null or same: no order change
                if (xdrawable == ydrawable)
                {
                    return 0;
                }

                if (xdrawable != null && ydrawable == null)
                {
                    return -1;
                }

                if (xdrawable == null && ydrawable != null)
                {
                    return 1;
                }

                if (xdrawable.ZIndex.Value > ydrawable.ZIndex.Value)
                {
                    return -1;
                }

                if (xdrawable.ZIndex.Value < ydrawable.ZIndex.Value)
                {
                    return 1;
                }

                return 0;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }
}
