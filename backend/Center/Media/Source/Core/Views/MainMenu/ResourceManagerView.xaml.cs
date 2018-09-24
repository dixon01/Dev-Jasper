// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceManagerView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ResourceManagerView.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.MainMenu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions.DragDropExtension;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Center.Media.Core.Views.Controls;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Utility.Files;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Interaction logic for ResourceManagerView.xaml
    /// </summary>
    public partial class ResourceManagerView
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan previewPopupDelay;

        private Point? dragStart;

        private IList<ResourceInfoDataViewModel> dragResource;

        private UIElement currentPopupElement;

        private Image currentThumbnail;

        private DispatcherTimer previewPopupTimer;

        private ResourceType currentResourceType = ResourceType.Image;

        private bool forceCollectionRefresh;

        private PoolConfigDataViewModel selectedPool;

        /// <summary>
        /// Waits a period of time.
        /// <param name="time">
        /// The time.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// </summary>
        public static void WaitFor(TimeSpan time, DispatcherPriority priority)
        {
            DispatcherTimer timer = new DispatcherTimer(priority);
            timer.Tick += new EventHandler(OnDispatched);
            timer.Interval = time;
            DispatcherFrame dispatcherFrame = new DispatcherFrame(false);
            timer.Tag = dispatcherFrame;
            timer.Start();
            Dispatcher.PushFrame(dispatcherFrame);
        }

        /// <summary>
        /// Clean up after the dispatch timer expires.
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The arguments.
        /// </param>
        /// </summary>
        public static void OnDispatched(object sender, EventArgs args)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Tick -= new EventHandler(OnDispatched);
            timer.Stop();
            DispatcherFrame frame = (DispatcherFrame)timer.Tag;
            frame.Continue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManagerView"/> class.
        /// </summary>
        public ResourceManagerView()
        {
            this.InitializeComponent();

            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            state.PropertyChanged += this.OnProjectChanged;
            if (state.CurrentProject != null)
            {
                this.SetMediaSource(state.CurrentProject.Resources, ResourceType.Image);
            }

            this.previewPopupDelay = Settings.Default.PreviewPopupDelay;
            this.forceCollectionRefresh = true;
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnLoaded;
        }

        /// <summary>
        /// Gets the validate Pool Name function
        /// </summary>
        public Func<string, object, string> IsValidPoolName => this.ValidatePoolName;

        /// <summary>
        /// Gets the is valid csv mapping name validator.
        /// </summary>
        public Func<string, object, string> IsValidCsvMappingName => this.ValidateCsvMappingName;

        /// <summary>
        /// Updates the detail list of the currently selected pool
        /// </summary>
        public void UpdatePoolMediaList()
        {
            var currentSelectedPool = (PoolConfigDataViewModel)this.PoolList.SelectedItem;
            this.SetPoolSource(currentSelectedPool);
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ResourceManagementPrompt context)
            {
                context.SelectedPool = null;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (state.CurrentProject != null)
            {
                var resources = state.CurrentProject.Resources;
                if (this.DataContext is ResourceManagementPrompt context)
                {
                    switch (context.SelectedType)
                    {
                        case ResourceType.Video:
                            this.VideosToggleButton.IsChecked = true;
                            break;
                        case ResourceType.Symbol:
                            this.SymbolsToggleButton.IsChecked = true;
                            break;
                        case ResourceType.Audio:
                            this.AudioToggleButton.IsChecked = true;
                            break;
                        case ResourceType.Font:
                            this.FontsToggleButton.IsChecked = true;
                            break;
                        case ResourceType.Csv:
                            this.CsvToggleButton.IsChecked = true;
                            break;
                        default:
                            this.PicturesToggleButton.IsChecked = true;
                            break;
                    }

                    this.SetMediaSource(resources, context.SelectedType);
                    return;
                }
            }

            this.PicturesToggleButton.IsChecked = false;
            this.PicturesToggleButton.IsChecked = true;
        }

        private void OnProjectChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentProject")
            {
                this.forceCollectionRefresh = true;
            }
        }

        private void SetMediaSource(
            INotifyPropertyChanged resources, ResourceType resourceType)
        {
            if (!(this.MainGrid.TryFindResource("Media") is CollectionViewSource media))
            {
                return;
            }

            this.currentResourceType = resourceType;
            this.forceCollectionRefresh = false;
            media.Source = resources;
            media.View.Refresh();
        }

        private void SetPoolSource(PoolConfigDataViewModel pool)
        {
            if (!(this.MainGrid.TryFindResource("Pool") is CollectionViewSource media))
            {
                return;
            }

            media.Source = pool.ResourceReferences;
            media.View.Refresh();

            if (this.selectedPool != null)
            {
                this.selectedPool.PropertyChanged -= this.OnSelectedPoolChanged;
            }

            pool.PropertyChanged += this.OnSelectedPoolChanged;

            this.selectedPool = pool;
        }

        private void SetSymbolSource(INotifyPropertyChanged resources, ResourceType resourceType)
        {
            if (!(this.MainGrid.TryFindResource("Symbols") is CollectionViewSource media))
            {
                return;
            }

            this.currentResourceType = resourceType;
            this.forceCollectionRefresh = false;
            media.Source = resources;
            media.View.Refresh();
        }

        private void SetCsvMappingSource(INotifyPropertyChanged resources)
        {
            if (!(this.MainGrid.TryFindResource("CsvMappings") is CollectionViewSource mappings))
            {
                return;
            }

            this.currentResourceType = ResourceType.Csv;
            this.forceCollectionRefresh = false;
            mappings.Source = resources;
            mappings.View.Refresh();
        }

        private void OnSelectedPoolChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ResourceReferences" && this.PoolList.SelectedItem != null)
            {
                this.UpdatePoolMediaList();
            }
        }

        private void OnAddCsvButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.AddCsvCommand.CanExecute(null))
            {
                return;
            }

            prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings.CollectionChanged +=
                this.OnCsvMappingAdded;
            prompt.AddCsvCommand.Execute(null);
        }

        private void OnCsvMappingAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count >= 0)
            {
                var lastItem = this.CsvMappingsList.Items.GetItemAt(this.CsvMappingsList.Items.Count - 1);
                this.CsvMappingsList.SetSelectedItem(lastItem);
                this.CsvMappingsList.ScrollIntoView(this.CsvMappingsList.SelectedItem);
            }

            var prompt = (ResourceManagementPrompt)this.DataContext;
            prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings.CollectionChanged -= this.OnCsvMappingAdded;
        }

        private void OnImportCsvButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings.CollectionChanged +=
               this.OnCsvMappingAdded;
            prompt.ImportCsvCommand.Execute(null);
        }

        private void OnEditCsvClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            if (this.CsvMappingsList.SelectedItems.Count != 1
                || !prompt.EditCsvCommand.CanExecute(this.CsvMappingsList.SelectedItems[0]))
            {
                return;
            }

            prompt.EditCsvCommand.Execute(this.CsvMappingsList.SelectedItems[0]);
        }

        private TextBlock CreateCsvPreview()
        {
            var item = this.GetCurrentCsvMappingsListItemContext(this.CsvMappingsList.Items.Count);

            if (string.IsNullOrEmpty(item.RawContent.Value))
            {
                return null;
            }

            var textBox = new TextBlock
            {
                Text = item.RawContent.Value,
                Background = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 300,
                MaxHeight = 200
            };
            return textBox;
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.ImportCsvCommand.CanExecute(null))
            {
                return;
            }

            string filter;
            DialogDirectoryType directoryType;
            string title;

            this.SetOpenFileDialogProperties(out filter, out directoryType, out title);

            Action<OpenFileDialogInteraction> onOpen = interaction =>
            {
                if (!interaction.Confirmed)
                {
                    return;
                }

                var validResources = new List<IFileInfo>();
                ResourceType currentType = ResourceType.Image;

                foreach (var fileName in interaction.FileNames)
                {
                    var file = FileSystemManager.Local.GetFile(fileName);
                    if (this.PoolList.SelectedItem != null)
                    {
                        if (!this.DetectResourceType(fileName, out currentType))
                        {
                            var message = string.Format(
                                MediaStrings.ResourceManager_couldNotDetectResourceType,
                                fileName);
                            MessageBox.Show(
                                message,
                                MediaStrings.ResourceManager_couldNotDetectResourceTypeTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                            continue;
                        }
                    }
                    else
                    {
                        currentType = this.currentResourceType;
                    }

                    validResources.Add(file);
                }

                if (validResources.Any())
                {
                    var parameters = new AddResourceParameters { Resources = validResources, Type = currentType };
                    try
                    {
                        prompt.AddNewPictureCommand.Execute(parameters);
                    }
                    catch (UpdateException exception)
                    {
                        Logger.ErrorException("Error while adding a resource.", exception);
                    }
                }
            };

            var openDialogInteraction = new OpenFileDialogInteraction
            {
                Filter = filter,
                Title = title,
                DirectoryType = directoryType,
                MultiSelect = true
            };
            InteractionManager<OpenFileDialogInteraction>.Current.Raise(openDialogInteraction, onOpen);
        }

        private bool DetectResourceType(string fileName, out ResourceType currentType)
        {
            var extension = Path.GetExtension(fileName);
            extension = extension?.ToLowerInvariant();

            if (Settings.Default.ImageExtensions_Tft.Contains(extension))
            {
                currentType = ResourceType.Image;
                return true;
            }

            if (Settings.Default.VideoExtensions.Contains(extension))
            {
                currentType = ResourceType.Video;
                return true;
            }

            currentType = ResourceType.Image;
            return false;
        }

        private void SetOpenFileDialogProperties(
          out string filter, out DialogDirectoryType directoryType, out string title)
        {
            if (this.PicturesToggleButton.IsChecked == true)
            {
                filter = MediaStrings.OpenFileDialog_ImageFilter;
                directoryType = DialogDirectoryTypes.Image;
                title = MediaStrings.ResourceManagerView_AddPicture;
            }
            else if (this.VideosToggleButton.IsChecked == true)
            {
                filter = MediaStrings.OpenFileDialog_VideoFilter;
                directoryType = DialogDirectoryTypes.Video;
                title = MediaStrings.ResourceManagerView_AddVideo;
            }
            else if (this.SymbolsToggleButton.IsChecked == true)
            {
                filter = MediaStrings.OpenFileDialog_ImageFilter;
                directoryType = DialogDirectoryTypes.Symbol;
                title = MediaStrings.ResourceManagerView_AddSymbol;
            }
            else if (this.AudioToggleButton.IsChecked == true)
            {
                filter = MediaStrings.OpenFileDialog_AudioFilter;
                directoryType = DialogDirectoryTypes.Audio;
                title = MediaStrings.ResourceManagerView_AddAudio;
            }
            else if (this.FontsToggleButton.IsChecked == true)
            {
                filter = MediaStrings.OpenFileDialog_FontsFilter;
                directoryType = DialogDirectoryTypes.Font;
                title = MediaStrings.ResourceManagerView_AddFont;
            }
            else if (this.PoolList.SelectedItem != null)
            {
                filter = MediaStrings.OpenFileDialog_ImageFilterTft + "|" + MediaStrings.OpenFileDialog_VideoFilter;
                directoryType = DialogDirectoryTypes.Image;
                title = string.Empty;
            }
            else
            {
                Logger.Warn("ResourceManager: unknown tab and no pool.");
                filter = string.Empty;
                directoryType = DialogDirectoryTypes.Image;
                title = string.Empty;
            }
        }

        private void OnMouseEnterPreview(object sender, MouseEventArgs e)
        {
            if (this.currentPopupElement != null)
            {
                this.ClosePopupElement();
            }

            this.previewPopupTimer?.Stop();
            this.currentThumbnail = (Image)sender;

            var parameters = new PreviewPopupParameters
            {
                ImageSource = ((Image)sender).Source,
                MousePosition = e.GetPosition(this.PopupContainer),
                Type = (ResourceType)((Image)sender).Tag
            };
            if (this.previewPopupTimer == null)
            {
                this.previewPopupTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = this.previewPopupDelay
                };
                this.previewPopupTimer.Tick += this.PreviewPopupTimerElapsed;
            }

            this.currentThumbnail.MouseLeave += this.ClosePopupElement;
            this.previewPopupTimer.Tag = parameters;
            this.previewPopupTimer.Start();
        }

        private ResourceInfoDataViewModel GetCurrentMediaItemContext(int itemsCount)
        {
            for (var i = 0; i < itemsCount; i++)
            {
                var item = this.GetMediaListViewItem(i);

                if (item == null)
                {
                    continue;
                }

                var position = Mouse.GetPosition(item);
                if (this.IsMouseOverTarget(item, position))
                {
                    return (ResourceInfoDataViewModel)item.DataContext;
                }
            }

            return null;
        }

        private ResourceInfoDataViewModel GetCurrentPoolMediaListItemContext(int itemsCount)
        {
            for (var i = 0; i < itemsCount; i++)
            {
                var item = this.GetPoolMediaListViewItem(i);

                if (item == null)
                {
                    continue;
                }

                var position = Mouse.GetPosition(item);
                if (this.IsMouseOverTarget(item, position))
                {
                    return ((ResourceReferenceDataViewModel)item.DataContext).ResourceInfo;
                }
            }

            return null;
        }

        private CsvMappingDataViewModel GetCurrentCsvMappingsListItemContext(int itemsCount)
        {
            for (var i = 0; i < itemsCount; i++)
            {
                var item = this.GetCsvMappingsListViewItem(i);

                if (item == null)
                {
                    continue;
                }

                var position = Mouse.GetPosition(item);
                if (this.IsMouseOverTarget(item, position))
                {
                    return (CsvMappingDataViewModel)item.DataContext;
                }
            }

            return null;
        }

        private ListViewItem GetMediaListViewItem(int index)
        {
            if (this.MediaList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return this.MediaList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private ListViewItem GetPoolMediaListViewItem(int index)
        {
            if (this.PoolMediaList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return this.PoolMediaList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private ListViewItem GetCsvMappingsListViewItem(int index)
        {
            if (this.CsvMappingsList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return this.CsvMappingsList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private bool IsMouseOverTarget(UIElement target, Point position)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(position);
        }

        private void ClosePopupElement(object o = null, MouseEventArgs e = null)
        {
            this.previewPopupTimer?.Stop();

            if (this.currentPopupElement != null)
            {
                if (this.currentPopupElement.IsMouseOver
                    || (this.currentThumbnail != null && this.currentThumbnail.IsMouseOver))
                {
                    return;
                }

                this.currentPopupElement.MouseLeave -= this.ClosePopupElement;
                this.PopupContainer.Children.Remove(this.currentPopupElement);
                this.currentPopupElement = null;
                if (this.currentThumbnail != null)
                {
                    this.currentThumbnail.MouseLeave -= this.ClosePopupElement;
                    this.currentThumbnail.MouseEnter += this.OnMouseEnterPreview;
                }
            }
        }

        private void PreviewPopupTimerElapsed(object sender, EventArgs e)
        {
            this.previewPopupTimer.Stop();

            var parameters = (PreviewPopupParameters)this.previewPopupTimer.Tag;

            var border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = Brushes.White,
                Effect = new DropShadowEffect
                {
                    Color = Colors.DarkGray,
                    Direction = 320,
                    ShadowDepth = 1,
                    Opacity = 1
                }
            };

            switch (parameters.Type)
            {
                case ResourceType.Image:
                case ResourceType.Symbol:
                    var image = new Image { Source = parameters.ImageSource, Width = 200 };
                    border.Child = image;
                    this.currentPopupElement = border;
                    break;
                case ResourceType.Video:
                    ResourceInfoDataViewModel resourceInfo;
                    if (this.PoolList.SelectedItem != null)
                    {
                        resourceInfo = this.GetCurrentPoolMediaListItemContext(this.PoolMediaList.Items.Count);
                    }
                    else
                    {
                        resourceInfo = this.GetCurrentMediaItemContext(this.MediaList.Items.Count);
                    }

                    var skimmingElement = new SkimmingElement { Width = 200, Height = 200, DataContext = resourceInfo };
                    border.Child = skimmingElement;
                    this.currentPopupElement = border;
                    break;
                case ResourceType.Audio:
                    var audioResourceInfo = this.GetCurrentMediaItemContext(this.MediaList.Items.Count);
                    var audioSkimmingElement = new AudioSkimmingElement
                    {
                        Width = 200,
                        Height = 60,
                        DataContext = audioResourceInfo
                    };
                    border.Child = audioSkimmingElement;
                    this.currentPopupElement = border;
                    break;
                case ResourceType.Font:
                    var fontPreview = this.CreateFontPreview();
                    border.Child = fontPreview;
                    this.currentPopupElement = border;
                    break;

                case ResourceType.Csv:
                    var csvPreview = this.CreateCsvPreview();
                    border.Child = csvPreview;
                    this.currentPopupElement = border;
                    break;

                default:
                    throw new Exception("Media element type not supported.");
            }

            var position = parameters.MousePosition;
            Canvas.SetLeft(this.currentPopupElement, position.X);
            Canvas.SetTop(this.currentPopupElement, position.Y);

            this.currentPopupElement.MouseLeave += this.ClosePopupElement;
            this.currentThumbnail.MouseEnter -= this.OnMouseEnterPreview;

            this.PopupContainer.Children.Add(this.currentPopupElement);
        }

        private FontPreview CreateFontPreview()
        {
            var fontDataViewModel = this.GetCurrentMediaItemContext(this.MediaList.Items.Count);
            var fontPreview = new FontPreview { Text = MediaStrings.FontPreview_Text };
            fontPreview.BitmapFont = null;
            if (!(this.DataContext is ResourceManagementPrompt context))
            {
                return fontPreview;
            }

            var resource = context.Shell.MediaApplicationState.ProjectManager.GetResource(fontDataViewModel.Hash);
            var extension = Path.GetExtension(fontDataViewModel.Filename);
            if (extension != null && (extension.Equals(".fon", StringComparison.InvariantCultureIgnoreCase)
                || extension.Equals(".fnt", StringComparison.InvariantCultureIgnoreCase)
                || extension.Equals(".cux", StringComparison.InvariantCultureIgnoreCase)))
            {
                using (var stream = resource.OpenRead())
                {
                    try
                    {
                        var font = new FontFile(stream, false);
                        fontPreview.BitmapFont = font;
                    }
                    catch (InvalidDataException ex)
                    {
                        var errorMessage = string.Format("Error while loading font {0}", fontDataViewModel.Facename);
                        Logger.ErrorException(errorMessage, ex);
                        fontPreview.BitmapFont = null;
                        var textMessage = string.Format(
                            MediaStrings.ResourceManager_CouldNotOpenFont,
                            fontDataViewModel.Facename);
                        var message = string.Format("{0}: {1}", textMessage, ex.Message);
                        fontPreview.Text = message;
                    }
                }
            }
            else
            {
                var tempPath = Path.GetTempPath();
                var filename = Path.GetFileName(fontDataViewModel.Filename);
                if (filename == null)
                {
                    return fontPreview;
                }

                var tempFontFile = Path.Combine(tempPath, filename);
                if (!File.Exists(tempFontFile))
                {
                    using (var stream = resource.OpenRead())
                    {
                        using (var file = File.Create(tempFontFile))
                        {
                            stream.CopyTo(file);
                        }
                    }
                }

                var uri = new Uri(tempPath);
                var fontFamilyName = string.Format("{0}/#{1}", uri, fontDataViewModel.Facename);
                var fontFamily = new FontFamily(fontFamilyName);
                fontPreview.WindowsFont = fontFamily;
            }

            return fontPreview;
        }

        private void MediaList_OnDragLeave(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void PoolList_OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void PoolList_OnDragOver(object sender, DragEventArgs e)
        {
            if (this.dragStart != null)
            {
                e.Effects = DragDropEffects.All;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private void MediaList_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (this.MediaList.IsEnabled)
            {
                var position = e.GetPosition((IInputElement)sender);

                if (this.dragStart.HasValue && e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Math.Abs(position.X - this.dragStart.Value.X) > SystemParameters.MinimumHorizontalDragDistance
                        || Math.Abs(position.Y - this.dragStart.Value.Y) > SystemParameters.MinimumVerticalDragDistance)
                    {
                        if (this.dragResource != null && e.LeftButton == MouseButtonState.Pressed)
                        {
                            DragDrop.DoDragDrop(this.MediaList, this.dragResource, DragDropEffects.Copy);
                        }
                    }
                }
            }
        }

        private void MediaList_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.MediaList.IsEnabled)
            {
                if (this.dragResource == null || this.dragResource.Count == 0)
                {
                    this.dragStart = null;
                    return;
                }

                this.dragStart = e.GetPosition((IInputElement)sender);
            }
        }

        private void MediaList_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.dragStart = null;
        }

        private void PoolList_OnDrop(object sender, DragEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.AddResourceReferenceCommand.CanExecute(null))
            {
                return;
            }

            if (this.MediaList.IsEnabled)
            {
                var resourceList =
                    (List<ResourceInfoDataViewModel>)e.Data.GetData(typeof(List<ResourceInfoDataViewModel>));

                var pool = this.GetPoolFromPoolListAtPosition(e);

                if (pool != null && resourceList != null)
                {
                    foreach (var resource in resourceList)
                    {
                        var parameters = new AddResourceReferenceParameters { Media = resource, Pool = pool, };
                        prompt.AddResourceReferenceCommand.Execute(parameters);
                    }
                }
            }
        }

        private void MediaList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var resourceList = this.MediaList.SelectedItems.Cast<ResourceInfoDataViewModel>().ToList();
            if (resourceList.Any(r => r.Type == ResourceType.Audio
                || r.Type == ResourceType.Font
                || r.Type == ResourceType.Csv
                || r.IsLedImage))
            {
                this.dragResource = null;
                return;
            }

            this.dragResource = resourceList;
        }

        private PoolConfigDataViewModel GetPoolFromPoolListAtPosition(DragEventArgs e)
        {
            var position = e.GetPosition(this.PoolList);

            var result = VisualTreeHelper.HitTest(this.PoolList, position);
            var obj = result.VisualHit;

            while (VisualTreeHelper.GetParent(obj) != null && !(obj is ListBoxItem))
            {
                obj = VisualTreeHelper.GetParent(obj);
            }

            var pool = this.PoolList.ItemContainerGenerator.ItemFromContainer(obj) as PoolConfigDataViewModel;
            return pool;
        }

        private string ValidatePoolName(string newPoolName, object sourceObject)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(newPoolName))
            {
                this.SuppressInteractionMouseEvents(prompt);
                return MediaStrings.ResourceManagerView_PoolNameMissing;
            }

            if (newPoolName == ((PoolConfigDataViewModel)sourceObject).Name.Value)
            {
                this.SuppressInteractionMouseEvents(prompt, false);
                return string.Empty;
            }

            var errorMessage = prompt.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Pools.Any(
                p => p.Name.Value == newPoolName) ? MediaStrings.ResourceManagerView_PoolNameDuplicate : string.Empty;

            this.SuppressInteractionMouseEvents(prompt, !string.IsNullOrEmpty(errorMessage));
            return errorMessage;
        }

        private string ValidateCsvMappingName(string newMappingName, object sourceObject)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(newMappingName))
            {
                this.SuppressInteractionMouseEvents(prompt);
                return MediaStrings.ResourceManagerView_CsvMappingNameMissing;
            }

            if (newMappingName == ((CsvMappingDataViewModel)sourceObject).Filename.Value)
            {
                this.SuppressInteractionMouseEvents(prompt, false);
                return string.Empty;
            }

            var errorMessage =
                prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings.Any(
                    p => p.Filename.Value == newMappingName)
                || newMappingName.Equals("codeconversion", StringComparison.InvariantCultureIgnoreCase)
                    ? MediaStrings.ResourceManagerView_CsvMappingNameDuplicate
                    : string.Empty;
            this.SuppressInteractionMouseEvents(prompt, !string.IsNullOrEmpty(errorMessage));
            return errorMessage;
        }

        private void SuppressInteractionMouseEvents(ResourceManagementPrompt prompt, bool suppress = true)
        {
            var mainMenuPrompt =
                ServiceLocator.Current.GetInstance<IMediaApplicationController>().ShellController.MainMenuPrompt;
            prompt.SuppressMouseEvents = suppress;
            mainMenuPrompt.SuppressMouseEvents = suppress;
        }

        private void OnPoolNameChanged(string oldName, string newName, object sourceObject)
        {
            var pool = (PoolConfigDataViewModel)sourceObject;
            var oldOne = (PoolConfigDataViewModel)pool.Clone();
            pool.Name.Value = newName;
            pool.BaseDirectory.Value = Path.Combine(Settings.Default.PoolExportPath, pool.Name.Value);

            var prompt = (ResourceManagementPrompt)this.DataContext;
            var parameters = new UpdateEntityParameters(
                new List<DataViewModelBase> { oldOne },
                new List<DataViewModelBase> { (PoolConfigDataViewModel)pool.Clone() },
                prompt.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Pools);
            prompt.UpdateResourceListElementCommand.Execute(parameters);

            if (this.PoolList.SelectedItem != null)
            {
                this.UpdatePoolMediaList();
            }
        }

        private void OnCsvMappingNameChanged(string oldName, string newName, object sourceObject)
        {
            var mapping = (CsvMappingDataViewModel)sourceObject;
            var oldOne = (CsvMappingDataViewModel)mapping.Clone();
            mapping.Filename.Value = newName;

            var prompt = (ResourceManagementPrompt)this.DataContext;
            var parameters = new UpdateEntityParameters(
                new List<DataViewModelBase> { oldOne },
                new List<DataViewModelBase> { (CsvMappingDataViewModel)mapping.Clone() },
                prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings);
            prompt.UpdateResourceListElementCommand.Execute(parameters);

            if (this.CsvMappingsList.SelectedItem != null)
            {
                this.SetCsvMappingSource(prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings);
            }
        }

        private void OnDeleteResourceClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.ImportCsvCommand.CanExecute(null))
            {
                return;
            }

            string hash;
            if (sender is MenuItem menuItem)
            {
                hash = menuItem.Tag.ToString();
            }
            else
            {
                if (sender is Button button && button.Tag != null)
                {
                    hash = button.Tag.ToString();
                }
                else
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(hash))
            {
                return;
            }

            IEnumerable<ResourceInfoDataViewModel> selectedItems = null;
            if (this.MediaList.IsVisible)
            {
                selectedItems = this.MediaList.SelectedItems.Cast<ResourceInfoDataViewModel>().ToList();
            }
            else if (this.SymbolMediaList.IsVisible)
            {
                selectedItems = this.SymbolMediaList.SelectedItems.Cast<ResourceInfoDataViewModel>().ToList();
            }

            if (selectedItems != null)
            {
                foreach (var selectedItem in selectedItems)
                {
                    if (!selectedItem.IsUsed)
                    {
                        prompt.DeleteMediaCommand.Execute(selectedItem.Hash);
                    }
                    else
                    {
                        var filename = Path.GetFileName(selectedItem.Filename);
                        var message = string.Format(MediaStrings.Project_DeleteResourceIsUsed, filename);
                        MessageBox.Show(
                            message,
                            MediaStrings.Project_DeleteResourceTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void OnDeleteCsvButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            var selectedItems = this.CsvMappingsList.SelectedItems.Cast<CsvMappingDataViewModel>().ToList();

            foreach (var selectedItem in selectedItems)
            {
                if (!prompt.DeleteCsvCommand.CanExecute(selectedItem))
                {
                    continue;
                }

                // usage check for csv, not tracked so far
                prompt.DeleteCsvCommand.Execute(selectedItem);
            }
        }

        private void OnRemoveReferenceButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.PoolMediaList.SelectedItems.Count > 0 && this.PoolList.SelectedItem != null)
            {
                var pool = (PoolConfigDataViewModel)this.PoolList.SelectedItem;
                var references = this.PoolMediaList.SelectedItems.Cast<ResourceReferenceDataViewModel>();
                var parameters = new RemoveMediaReferenceParameters
                {
                    Pool = pool,
                    References = references.ToList()
                };
                var prompt = (ResourceManagementPrompt)this.DataContext;
                prompt.RemoveMediaReferenceCommand.Execute(parameters);

                this.UpdatePoolMediaList();
            }
        }

        private void OnCreateNewPoolClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.AddNewPoolCommand.CanExecute(null))
            {
                return;
            }

            prompt.AddNewPoolCommand.Execute(null);

            prompt.SelectedPool = prompt.Shell.MediaApplicationState.CurrentProject.InfomediaConfig.Pools.Last();
            this.PoolList.ScrollIntoView(prompt.SelectedPool);

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)this.FocusFirstListBoxItem);
        }

        private void FocusFirstListBoxItem()
        {
            var listBoxItem = (ListBoxItem)this.PoolList.ItemContainerGenerator.ContainerFromIndex(0);
            if (listBoxItem == null)
            {
                this.PoolList.ScrollIntoView(this.PoolList.Items[0]);
                WaitFor(TimeSpan.Zero, DispatcherPriority.SystemIdle);
                listBoxItem = (ListBoxItem)this.PoolList.ItemContainerGenerator.ContainerFromIndex(0);
            }

            listBoxItem.GetVisualDescendent<TextBox>().Focus();
        }

        private void OnRemovePoolClick(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ResourceManagementPrompt prompt) || !prompt.DeletePoolCommand.CanExecute(null))
            {
                return;
            }

            var pool = (PoolConfigDataViewModel)((FrameworkElement)sender).DataContext;

            if (prompt.SelectedPool == pool)
            {
                this.PicturesToggleButton.IsChecked = true;
            }

            prompt.DeletePoolCommand.Execute(pool);
        }

        private void FilterMediaCollection(object sender, FilterEventArgs e)
        {
            if (!(e.Item is ResourceInfoDataViewModel resourceInfoDataViewModel))
            {
                return;
            }

            e.Accepted = resourceInfoDataViewModel.Type == this.currentResourceType;
        }

        private void OnPicturesChecked(object sender, RoutedEventArgs e)
        {
            this.VideosToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Visible;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Visible;
            this.MediaList.IsEnabled = true;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;

            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Image;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Image || this.forceCollectionRefresh))
            {
                this.SetMediaSource(prompt.Shell.MediaApplicationState.CurrentProject.Resources, ResourceType.Image);
            }
        }

        private void OnVideosChecked(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Visible;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Visible;
            this.MediaList.IsEnabled = true;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;
            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Video;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Video || this.forceCollectionRefresh))
            {
                this.SetMediaSource(prompt.Shell.MediaApplicationState.CurrentProject.Resources, ResourceType.Video);
            }
        }

        private void OnCsvChecked(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.VideosToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Visible;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Collapsed;
            this.SetCsvButtonsVisibility(Visibility.Visible);

            this.MediaList.Visibility = Visibility.Collapsed;
            this.MediaList.IsEnabled = false;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;

            this.CsvMappingsList.Visibility = Visibility.Visible;
            this.CsvMappingsList.IsEnabled = true;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Csv;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Csv || this.forceCollectionRefresh))
            {
                this.SetCsvMappingSource(prompt.Shell.MediaApplicationState.CurrentProject.CsvMappings);
            }
        }

        private void OnSymbolsChecked(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = false;
            this.VideosToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Collapsed;
            this.DeleteSymbolButton.Visibility = Visibility.Visible;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Collapsed;
            this.MediaList.IsEnabled = false;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Visible;
            this.SymbolMediaList.IsEnabled = true;
            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Symbol;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Symbol || this.forceCollectionRefresh))
            {
                this.SetSymbolSource(prompt.Shell.MediaApplicationState.CurrentProject.Resources, ResourceType.Symbol);
            }
        }

        private void OnAudioChecked(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = false;
            this.VideosToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Visible;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Visible;
            this.MediaList.IsEnabled = true;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;
            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Audio;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Audio || this.forceCollectionRefresh))
            {
                this.SetMediaSource(prompt.Shell.MediaApplicationState.CurrentProject.Resources, ResourceType.Audio);
            }
        }

        private void OnFontsChecked(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = false;
            this.VideosToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.PoolList.SelectedItem = null;
            this.RemoveReferenceButton.Visibility = Visibility.Collapsed;
            this.DeleteMediaButton.Visibility = Visibility.Visible;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Visible;
            this.MediaList.IsEnabled = true;
            this.PoolMediaList.Visibility = Visibility.Collapsed;
            this.PoolMediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;
            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            if (!(this.DataContext is ResourceManagementPrompt prompt))
            {
                return;
            }

            prompt.SelectedType = ResourceType.Font;
            if (prompt.Shell.MediaApplicationState.CurrentProject != null
                && (this.currentResourceType != ResourceType.Font || this.forceCollectionRefresh))
            {
                this.SetMediaSource(prompt.Shell.MediaApplicationState.CurrentProject.Resources, ResourceType.Font);
            }
        }

        private void OnPoolSelected(object sender, RoutedEventArgs e)
        {
            if (this.PoolList.SelectedItem == null)
            {
                return;
            }

            this.PicturesToggleButton.IsChecked = false;
            this.VideosToggleButton.IsChecked = false;
            this.SymbolsToggleButton.IsChecked = false;
            this.AudioToggleButton.IsChecked = false;
            this.FontsToggleButton.IsChecked = false;
            this.CsvToggleButton.IsChecked = false;

            this.RemoveReferenceButton.Visibility = Visibility.Visible;
            this.DeleteMediaButton.Visibility = Visibility.Collapsed;
            this.DeleteSymbolButton.Visibility = Visibility.Collapsed;
            this.AddButton.Visibility = Visibility.Visible;
            this.SetCsvButtonsVisibility(Visibility.Collapsed);

            this.MediaList.Visibility = Visibility.Collapsed;
            this.MediaList.IsEnabled = false;
            this.SymbolMediaList.Visibility = Visibility.Collapsed;
            this.SymbolMediaList.IsEnabled = false;
            this.PoolMediaList.Visibility = Visibility.Visible;
            this.PoolMediaList.IsEnabled = true;
            this.CsvMappingsList.Visibility = Visibility.Collapsed;
            this.CsvMappingsList.IsEnabled = false;

            this.UpdatePoolMediaList();
        }

        private void OnPoolListKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (!(this.DataContext is ResourceManagementPrompt context))
                {
                    return;
                }

                if (context.DeletePoolCommand.CanExecute(null))
                {
                    context.DeletePoolCommand.Execute(context.SelectedPool);
                }
            }
        }

        private void SetCsvButtonsVisibility(Visibility visibility)
        {
            this.AddCsvButton.Visibility = visibility;
            this.EditCsvButton.Visibility = visibility;
            this.ImportCsvButton.Visibility = visibility;
            this.DeleteCsvButton.Visibility = visibility;
        }

        private void OnSymbolMediaListKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (!(this.DataContext is ResourceManagementPrompt context))
                {
                    return;
                }

                var selectedItems = (IEnumerable)this.SymbolMediaList.SelectedItems;
                foreach (var selectedItem in selectedItems.Cast<ResourceInfoDataViewModel>().ToList())
                {
                    try
                    {
                        context.DeleteMediaCommand.Execute(selectedItem.Hash);
                    }
                    catch (UpdateException)
                    {
                        var filename = Path.GetFileName(selectedItem.Filename);
                        var message = string.Format(MediaStrings.Project_DeleteResourceIsUsed, filename);
                        MessageBox.Show(
                            message,
                            MediaStrings.Project_DeleteResourceTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void OnPoolMediaListKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.OnRemoveReferenceButtonClick(null, null);
            }
        }

        private void OnCsvMappingsListKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                this.OnDeleteCsvButtonClicked(null, null);
            }
        }

        private void OnMediaListKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (!(this.DataContext is ResourceManagementPrompt context))
                {
                    return;
                }

                var selectedItems = (IEnumerable)this.MediaList.SelectedItems;
                foreach (var selectedItem in selectedItems.Cast<ResourceInfoDataViewModel>().ToList())
                {
                    try
                    {
                        context.DeleteMediaCommand.Execute(selectedItem.Hash);
                    }
                    catch (UpdateException)
                    {
                        var filename = Path.GetFileName(selectedItem.Filename);
                        var message = string.Format(MediaStrings.Project_DeleteResourceIsUsed, filename);
                        MessageBox.Show(
                            message,
                            MediaStrings.Project_DeleteResourceTitle,
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                }
            }
        }

        private void OnContextMenuPicturesToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.PicturesToggleButton.IsChecked = true;
        }

        private void OnContextMenuVideosToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.VideosToggleButton.IsChecked = true;
        }

        private void OnContextMenuSymbolsToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.SymbolsToggleButton.IsChecked = true;
        }

        private void OnContextMenuAudioToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.AudioToggleButton.IsChecked = true;
        }

        private void OnContextMenuFontsToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.FontsToggleButton.IsChecked = true;
        }

        private void OnContextMenuCsvToggleButtonOpened(object sender, RoutedEventArgs e)
        {
            this.CsvToggleButton.IsChecked = true;
        }

        private void ToggleButtonPreventUncheck(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            if (toggleButton == null)
            {
                return;
            }

            if (toggleButton.IsChecked == false)
            {
                toggleButton.IsChecked = true;
            }
        }

        private class PreviewPopupParameters
        {
            public ImageSource ImageSource { get; set; }

            public Point MousePosition { get; set; }

            public ResourceType Type { get; set; }
        }
    }
}