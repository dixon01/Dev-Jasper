// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioOutputFrame.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for AudioOutputFrame.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.LayoutElements
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Media.Core.DataViewModels.Layout;

    /// <summary>
    /// Interaction logic for AudioOutputFrame.xaml
    /// </summary>
    public partial class AudioOutputFrame
    {
        private bool selectionChanging;

        private bool frameClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioOutputFrame"/> class.
        /// </summary>
        public AudioOutputFrame()
        {
            InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Gets the delete entity command.
        /// </summary>
        public ICommand DeleteEntityCommand
        {
            get
            {
                return new RelayCommand(this.DeleteEntity);
            }
        }

        /// <summary>
        /// Gets the select next element.
        /// </summary>
        public ICommand SelectNextElement
        {
            get
            {
                return new RelayCommand(this.SelectNext);
            }
        }

        /// <summary>
        /// Gets the select previous element.
        /// </summary>
        public ICommand SelectPreviousElement
        {
            get
            {
                return new RelayCommand(this.SelectPrevious);
            }
        }

        /// <summary>
        /// Gets the select first element.
        /// </summary>
        public ICommand SelectFirstElement
        {
            get
            {
                return new RelayCommand(this.SelectFirst);
            }
        }

        /// <summary>
        /// Gets the select last element.
        /// </summary>
        public ICommand SelectLastElement
        {
            get
            {
                return new RelayCommand(this.SelectLast);
            }
        }

        private void SelectNext()
        {
            // multiple selected take first
            var index = this.AudioElementList.SelectedIndex;
            if (index == -1)
            {
                return;
            }

            index--;
            if (index < 0)
            {
                index = this.AudioElementList.Items.Count - 1;
            }

            var nextItem = this.AudioElementList.Items.GetItemAt(index);
            this.AudioElementList.SelectedItems.Clear();
            this.AudioElementList.SelectedItems.Add(nextItem);
        }

        private void SelectPrevious()
        {
            // multiple selected take first
            var index = this.AudioElementList.SelectedIndex;
            if (index == -1)
            {
               return;
            }

            index++;
            if (index >= this.AudioElementList.Items.Count)
            {
                index = 0;
            }

            var nextItem = this.AudioElementList.Items.GetItemAt(index);
            this.AudioElementList.SelectedItems.Clear();
            this.AudioElementList.SelectedItems.Add(nextItem);
        }

        private void SelectFirst()
        {
            if (this.AudioElementList.Items.Count > 0)
            {
                var nextItem = this.AudioElementList.Items.GetItemAt(0);
                this.AudioElementList.SelectedItems.Clear();
                this.AudioElementList.SelectedItems.Add(nextItem);
            }
        }

        private void SelectLast()
        {
            if (this.AudioElementList.Items.Count > 0)
            {
                var nextItem = this.AudioElementList.Items.GetItemAt(this.AudioElementList.Items.Count - 1);
                this.AudioElementList.SelectedItems.Clear();
                this.AudioElementList.SelectedItems.Add(nextItem);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SetFrameSelected();
        }

        private void SetFrameSelected()
        {
            this.frameClicked = true;
            this.AudioElementList.SelectedItem = null;
            var context = (AudioOutputElementDataViewModel)this.DataContext;
            if (context != null)
            {
                context.MediaShell.Editor.SelectedElements.Clear();
                context.MediaShell.Editor.SelectedElements.Add(context);
            }

            this.frameClicked = false;
        }

        private void DeleteEntity(object obj)
        {
            var context = (AudioOutputElementDataViewModel)this.DataContext;
            var editor = context.MediaShell.Editor as ViewModels.AudioEditorViewModel;
            if (editor != null && obj != null)
            {
                var element = obj as LayoutElementDataViewModelBase;
                if (element != null)
                {
                    var elements = new List<LayoutElementDataViewModelBase> { element };
                    editor.DeleteElementsCommand.Execute(elements);
                }
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.frameClicked)
            {
                this.frameClicked = false;
                return;
            }

            this.selectionChanging = true;
            var context = (AudioOutputElementDataViewModel)this.DataContext;
            if (context == null)
            {
                this.selectionChanging = false;
                return;
            }

            context.MediaShell.Editor.SelectedElements.Remove(context);

            if (e.AddedItems.Count > 0)
            {
                foreach (LayoutElementDataViewModelBase addedItem in e.AddedItems)
                {
                    if (!context.MediaShell.Editor.SelectedElements.Contains(addedItem))
                    {
                        context.MediaShell.Editor.SelectedElements.Add(addedItem);
                    }
                }
            }

            if (e.RemovedItems.Count > 0)
            {
                foreach (LayoutElementDataViewModelBase removedItem in e.RemovedItems)
                {
                    context.MediaShell.Editor.SelectedElements.Remove(removedItem);
                }
            }
        }

        private void AudioOutputFrame_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.selectionChanging)
            {
                this.selectionChanging = false;
                return;
            }

            this.SetFrameSelected();
        }
    }
}
