// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioEditor.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AudioEditor.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Editors
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Interaction logic for AudioEditor.xaml
    /// </summary>
    public partial class AudioEditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioEditor"/> class.
        /// </summary>
        public AudioEditor()
        {
            this.InitializeComponent();
            Mouse.AddPreviewMouseUpHandler(this, this.StopEditorInteraction);

            this.Loaded += (sender, args) =>
            {
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    Keyboard.AddPreviewKeyDownHandler(window, this.OnKeyDown);
                    Keyboard.AddPreviewKeyUpHandler(window, this.OnKeyUp);
                }
            };
        }

        /// <summary>
        /// the method that handles the start of editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void StartEditorInteraction(object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// the method that handles the ongoing editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void EditorInteraction(object sender, MouseEventArgs e)
        {
            this.MousePosition = e.GetPosition(this.Root);
            if (this.InteractionStartPosition.HasValue)
            {
                if (this.CurrentElementUnderMouse != null
                         && e.LeftButton == MouseButtonState.Pressed)
                {
                    var modifiers = new ModifiersState();
                    this.MoveElementsHandling(MouseButton.Left, modifiers);
                }
                else
                {
                    this.InteractionRectangle = new Rect(this.InteractionStartPosition.Value, this.MousePosition);
                }
            }
        }

        /// <summary>
        /// the method that handles the stop of editor interactions
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the mouse parameters</param>
        public void StopEditorInteraction(object sender, MouseButtonEventArgs e)
        {
            var interationEndpoint = e.GetPosition(this.Root);
            var isRect = this.InteractionStartPosition.HasValue
                         && this.Distance(this.InteractionStartPosition.Value, interationEndpoint)
                         > this.MinDragDistance.Length;

            var modifiers = new ModifiersState();

            if (this.CurrentElementUnderMouse == null || modifiers.IsControlPressed
                         || !this.HasHighestZIndex(this.CurrentElementUnderMouse))
            {
                this.SelectElementsHandling(e.ChangedButton, isRect, modifiers);
            }

            this.InteractionStartPosition = null;
            this.InteractionRectangle = new Rect(0, 0, 0, 0);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.CanProcessKeyDown())
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Up:
                    if (this.IsAudioToolbarActive())
                    {
                        this.AudioOutputFrame.SelectNextElement.Execute(null);

                        e.Handled = true;
                    }

                    break;

                case Key.Down:
                    if (this.IsAudioToolbarActive())
                    {
                        this.AudioOutputFrame.SelectPreviousElement.Execute(null);

                        e.Handled = true;
                    }

                    break;

                case Key.Home:
                    if (this.IsAudioToolbarActive())
                    {
                        this.AudioOutputFrame.SelectFirstElement.Execute(null);

                        e.Handled = true;
                    }

                    break;

                case Key.End:
                    if (this.IsAudioToolbarActive())
                    {
                        this.AudioOutputFrame.SelectLastElement.Execute(null);

                        e.Handled = true;
                    }

                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            ModifiersState modifiers;

            if (!this.IsAudioToolbarActive() || !this.CanProcessKeyUp(e, out modifiers))
            {
                return;
            }

            var audioEditor = (EditorViewModelBase)this.DataContext;
            if (this.HandleSharedKeyUp(audioEditor, e, modifiers))
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                    if (this.IsAudioToolbarActive() && this.LayoutIsEditable())
                    {
                        this.AddAudioElement(LayoutElementType.AudioFile);
                    }

                    break;

                case Key.S:
                    if (!modifiers.IsControlPressed && this.IsAudioToolbarActive() && this.LayoutIsEditable())
                    {
                        this.AddAudioElement(LayoutElementType.AudioPause);
                    }

                    break;

                case Key.D:
                    if (this.IsAudioToolbarActive() && this.LayoutIsEditable())
                    {
                        this.AddAudioElement(LayoutElementType.DynamicTts);
                    }

                    break;

                case Key.T:
                    if (this.IsAudioToolbarActive() && this.LayoutIsEditable())
                    {
                        this.AddAudioElement(LayoutElementType.TextToSpeech);
                    }

                    break;

                case Key.Delete:
                    if (this.IsAudioToolbarActive())
                    {
                        if (!audioEditor.SelectedElements.Any(element => element is AudioOutputElementDataViewModel))
                        {
                            audioEditor.DeleteElementsCommand.Execute(audioEditor.SelectedElements);

                            var audioElementList = this.AudioOutputFrame.AudioElementList;
                            if (audioElementList.Items.Count > 0)
                            {
                                var nextItem = audioElementList.Items.GetItemAt(0);
                                audioElementList.SelectedItems.Clear();
                                audioElementList.SelectedItems.Add(nextItem);
                            }
                        }
                    }

                    break;
            }
        }

        private void AddAudioElement(LayoutElementType type)
        {
            var audioEditor = (ViewModels.AudioEditorViewModel)this.DataContext;
            if (audioEditor == null || audioEditor.CurrentAudioOutputElement == null)
            {
                return;
            }

            var insertIndex = audioEditor.CurrentAudioOutputElement.Elements.Count;
            var createParams = new CreateElementParameters { Type = type, InsertIndex = insertIndex };
            audioEditor.CreateLayoutElementCommand.Execute(createParams);
        }

        private bool LayoutIsEditable()
        {
            var audioEditor = (ViewModels.AudioEditorViewModel)this.DataContext;

            if (audioEditor == null || audioEditor.CurrentAudioOutputElement == null)
            {
                return false;
            }

            return true;
        }

        private bool IsAudioToolbarActive()
        {
            var audioEditor = (ViewModels.AudioEditorViewModel)this.DataContext;
            if (audioEditor == null || audioEditor.Parent.MediaApplicationState.CurrentPhysicalScreen == null)
            {
                return false;
            }

            return audioEditor.Parent.MediaApplicationState.CurrentPhysicalScreen.Type.Value
                   == PhysicalScreenType.Audio;
        }

        private double Distance(Point a, Point b)
        {
            var dir = b - a;
            return dir.Length;
        }

        private bool HasHighestZIndex(DrawableElementDataViewModelBase element)
        {
            if (element == null)
            {
                return false;
            }

            return element.ZIndex.Value >= this.HighestElementUnderMouse.ZIndex.Value;
        }

        private void SelectElementsHandling(MouseButton changedButton, bool isRect, ModifiersState modifiers)
        {
            if (this.InteractionStartPosition.HasValue && changedButton == MouseButton.Left)
            {
                if (isRect)
                {
                    var parameters = new CreateElementParameters
                    {
                        Bounds =
                            new Rect(
                            this.InteractionStartPosition.Value,
                            this.MousePosition),
                        Modifiers = modifiers,
                    };
                    ((EditorViewModelBase)this.DataContext).SelectLayoutElementsCommand.Execute(parameters);
                }
                else
                {
                    var parameter = new SelectElementParameters
                    {
                        Position = this.InteractionStartPosition.Value,
                        Modifiers = modifiers,
                    };
                    ((EditorViewModelBase)this.DataContext).SelectLayoutElementCommand.Execute(parameter);
                }
            }
        }

        private void MoveElementsHandling(MouseButton changedButton, ModifiersState modifiers)
        {
            if (this.InteractionStartPosition.HasValue && changedButton == MouseButton.Left)
            {
                // Debug.WriteLine("StartPosition: " + (this.MousePosition - this.InteractionStartPosition.Value));
                var parameters = new MoveElementsCommandParameters
                {
                    Delta = this.MousePosition - this.LastMousePosition,
                    Modifiers = modifiers,
                };
                ((EditorViewModelBase)this.DataContext).MoveSelectedElementsCommand.Execute(parameters);

                this.LastMousePosition = new Point(this.MousePosition.X, this.MousePosition.Y);
            }
        }
    }
}
