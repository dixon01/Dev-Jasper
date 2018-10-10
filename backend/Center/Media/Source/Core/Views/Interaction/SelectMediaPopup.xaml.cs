// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectMediaPopup.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The SelectMediaPopup.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;
    using Gorba.Center.Common.Wpf.Views.Components.PropertyGrid;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Extensions.DragDropExtension;
    using Gorba.Center.Media.Core.Interaction;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Interaction logic for SelectMediaPopup.xaml
    /// </summary>
    public partial class SelectMediaPopup
    {
        /// <summary>
        /// the selected table property
        /// </summary>
        public static readonly DependencyProperty SelectedMediaProperty = DependencyProperty.Register(
            "SelectedMedia",
            typeof(string),
            typeof(SelectMediaPopup),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The is led canvas property.
        /// </summary>
        public static readonly DependencyProperty IsLedCanvasProperty = DependencyProperty.Register(
            "IsLedCanvas",
            typeof(bool),
            typeof(SelectMediaPopup),
            new PropertyMetadata(default(bool)));

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TimeSpan previewPopupDelay;

        private UIElement currentPopupElement;

        private Image currentThumbnail;

        private List<ResourceType> currentResourceTypes;

        private DispatcherTimer previewPopupTimer;

        private bool forceCollectionRefresh;

        private string searchText;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectMediaPopup"/> class.
        /// </summary>
        public SelectMediaPopup()
        {
            this.InitializeComponent();
            this.previewPopupDelay = Settings.Default.PreviewPopupDelay;
            this.previewPopupTimer = new DispatcherTimer(DispatcherPriority.Normal)
                                         {
                                             Interval = this.previewPopupDelay
                                         };
            this.previewPopupTimer.Tick += this.PreviewPopupTimerElapsed;
            this.forceCollectionRefresh = true;
            this.Loaded += this.PopupLoaded;
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            state.PropertyChanged += this.OnProjectChanged;

            this.RegisterRecentlyUsedChanged();

            DependencyPropertyDescriptor
                .FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl))
                .AddValueChanged(this.MediaList, (s, e) => this.OnMediaListViewItemSourceChanged());
        }

        /// <summary>
        /// Gets or sets the selected media.
        /// </summary>
        public string SelectedMedia
        {
            get
            {
                return (string)this.GetValue(SelectedMediaProperty);
            }

            set
            {
                this.SetValue(SelectedMediaProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is led canvas.
        /// </summary>
        public bool IsLedCanvas
        {
            get
            {
                return (bool)this.GetValue(IsLedCanvasProperty);
            }

            set
            {
                this.SetValue(IsLedCanvasProperty, value);
            }
        }

        /// <summary>
        /// the handler for close
        /// </summary>
        /// <param name="e">the event arguments</param>
        protected override void OnClose(EventArgs e)
        {
            base.OnClose(e);

            if (!(this.DataContext is SelectMediaPrompt selectMediaPrompt))
            {
                return;
            }

            selectMediaPrompt.PropertyChanged -= this.OnMediaChanged;
            if (this.SelectedMedia != null)
            {
                this.UpdateRecentMedia(selectMediaPrompt.SelectedResource);
            }

            if (this.previewPopupTimer != null)
            {
                this.previewPopupTimer.Stop();
            }
        }

        private void OnProjectChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentProject")
            {
                this.forceCollectionRefresh = true;
                this.RegisterRecentlyUsedChanged();
            }
        }

        private void RegisterRecentlyUsedChanged()
        {
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            if (state.CurrentProject != null)
            {
                ExtendedObservableCollection<ResourceInfoDataViewModel> recentlyUsedResources;
                state.RecentMediaResources.TryGetValue(state.CurrentProject.ProjectId, out recentlyUsedResources);
                if (recentlyUsedResources != null)
                {
                    recentlyUsedResources.CollectionChanged += this.RecentMediaChanged;
                }
            }
        }

        private void RecentMediaChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.forceCollectionRefresh = true;
        }

        private void PopupLoaded(object sender, RoutedEventArgs e)
        {
            this.SelectedMedia = string.Empty;

            this.MediaSearchBox.Text = string.Empty;

            if (!(this.DataContext is SelectMediaPrompt prompt))
            {
                return;
            }

            prompt.PropertyChanged += this.OnMediaChanged;

            var allowedResourceTypes = new List<ResourceType>();
            if (prompt.MediaElement is ImageElementDataViewModel)
            {
                allowedResourceTypes.Add(ResourceType.Image);
                allowedResourceTypes.Add(ResourceType.Symbol);
            }
            else if (prompt.MediaElement is VideoElementDataViewModel)
            {
                allowedResourceTypes.Add(ResourceType.Video);
            }
            else if (prompt.MediaElement is TextualReplacementDataViewModel)
            {
                allowedResourceTypes.Add(ResourceType.Symbol);
            }
            else if (prompt.MediaElement is AudioFileElementDataViewModel)
            {
                allowedResourceTypes.Add(ResourceType.Audio);
            }
            else
            {
                throw new Exception("Unhandled class.");
            }

            var resouceTypesChanged = this.HasResouceTypesChanged(allowedResourceTypes);
            this.currentResourceTypes = allowedResourceTypes;

            if (prompt.SelectedResource != null && !string.IsNullOrWhiteSpace(prompt.SelectedResource.Hash))
            {
                this.SelectedMedia = prompt.SelectedResource.Hash;
            }

            // sh: not using this: resouceTypeChanged || this.forceCollectionRefresh
            // since it breaks update on add new media
            this.SetMediaSources(prompt.Shell.MediaApplicationState.CurrentProject.Resources, "Media");

            if (!prompt.Shell.MediaApplicationState.RecentMediaResources.ContainsKey(
                    prompt.Shell.MediaApplicationState.CurrentProject.ProjectId))
            {
                return;
            }

            var resources = prompt.Shell.MediaApplicationState
                .RecentMediaResources[prompt.Shell.MediaApplicationState.CurrentProject.ProjectId];
            if (resouceTypesChanged || this.forceCollectionRefresh)
            {
                this.SetMediaSources(resources, "RecentMedia");
            }

            this.forceCollectionRefresh = false;
        }

        private bool HasResouceTypesChanged(List<ResourceType> allowedResourceTypes)
        {
            if (this.currentResourceTypes == null)
            {
                return true;
            }

            var resouceTypeChanged = this.currentResourceTypes.Intersect(allowedResourceTypes).Count()
                                     != allowedResourceTypes.Count;
            return resouceTypeChanged;
        }

        private void SetMediaSources(IEnumerable<ResourceInfoDataViewModel> resources, string resourceKey)
        {
            var media = this.MainGrid.TryFindResource(resourceKey) as CollectionViewSource;
            if (media == null)
            {
                return;
            }

            media.Source = resources.ToList();
            if (media.View == null)
            {
                return;
            }

            media.View.Refresh();
        }

        private void FilterMediaCollection(object sender, FilterEventArgs e)
        {
            var resourceInfoDataViewModel = e.Item as ResourceInfoDataViewModel;
            if (resourceInfoDataViewModel == null)
            {
                return;
            }

            if (this.IsLedCanvas)
            {
                e.Accepted = this.currentResourceTypes.Contains(resourceInfoDataViewModel.Type);
            }
            else
            {
                e.Accepted = !resourceInfoDataViewModel.IsLedImage
                             && this.currentResourceTypes.Contains(resourceInfoDataViewModel.Type);
            }
        }

        private void OnMouseEnterPreview(object sender, MouseEventArgs e)
        {
            if (this.previewPopupTimer != null)
            {
                this.previewPopupTimer.Stop();
            }

            if (this.currentPopupElement != null)
            {
                this.ClosePopupElement();
            }

            this.currentThumbnail = (Image)sender;
            var sourceName = (string)this.currentThumbnail.Tag;
            this.currentThumbnail.MouseLeave += this.ClosePopupElement;
            var parameters = new PreviewPopupParameters
                                 {
                                     MousePosition = e.GetPosition(this.PopupContainer),
                                     ImageSource = ((Image)e.Source).Source,
                                     ListViewName = sourceName
                                 };

            if (this.previewPopupTimer == null)
            {
                this.previewPopupTimer = new DispatcherTimer(DispatcherPriority.Normal)
                                             {
                                                 Interval = this.previewPopupDelay
                                             };
                this.previewPopupTimer.Tick += this.PreviewPopupTimerElapsed;
            }

            this.previewPopupTimer.Tag = parameters;
            this.previewPopupTimer.Start();
        }

        private void OnMediaChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(this.DataContext is SelectMediaPrompt prompt))
            {
                return;
            }

            if (prompt.SelectedResource != null)
            {
                var oldElement = (DataViewModelBase)((ICloneable)prompt.MediaElement).Clone();

                var done = false;

                var imageDataViewModel = prompt.MediaElement as ImageElementDataViewModel;
                if (imageDataViewModel != null)
                {
                    imageDataViewModel.Image = prompt.SelectedResource;

                    this.UpdateLayoutElement(oldElement);
                    done = true;
                }

                var videoDataViewModel = prompt.MediaElement as VideoElementDataViewModel;
                if (!done && videoDataViewModel != null)
                {
                    videoDataViewModel.Hash = prompt.SelectedResource.Hash;
                    videoDataViewModel.VideoUri.Value = prompt.SelectedResource.ThumbnailHash
                                                        != Settings.Default.UriVideoThumbnailHash
                                                            ? Path.GetFileName(prompt.SelectedResource.Filename)
                                                            : string.Empty;

                    videoDataViewModel.PreviewImageHash = prompt.SelectedResource.ThumbnailHash;

                    this.UpdateLayoutElement(oldElement);
                    done = true;
                }

                var textualReplacementViewModel = prompt.MediaElement as TextualReplacementDataViewModel;
                if (!done && textualReplacementViewModel != null)
                {
                    textualReplacementViewModel.Image = prompt.SelectedResource;
                    this.UpdateReplacementElement(
                        oldElement, new List<DataViewModelBase> { textualReplacementViewModel });
                    done = true;
                }

                var audioDataViewModel = prompt.MediaElement as AudioFileElementDataViewModel;
                if (!done && audioDataViewModel != null)
                {
                    audioDataViewModel.AudioFile = prompt.SelectedResource;

                    this.UpdateLayoutElement(oldElement);
                }

                this.SelectedMedia = prompt.SelectedResource.Hash;
            }
        }

        private void UpdateDimensions(SelectMediaPrompt prompt, DrawableElementDataViewModelBase dataViewModel)
        {
            if (prompt.SelectedResource.ThumbnailHash != Settings.Default.UriVideoThumbnailHash)
            {
                var dimensions = this.GetDimensions(prompt.SelectedResource);
                dataViewModel.Width.Value = dimensions.Width;
                dataViewModel.Height.Value = dimensions.Height;
            }
        }

        private MediaElementDimensions GetDimensions(ResourceInfoDataViewModel dataViewModel)
        {
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            string path;
            if (dataViewModel.Type == ResourceType.Image)
            {
                if (dataViewModel.Filename.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase)
                    || dataViewModel.Filename.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase)
                    || dataViewModel.Filename.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
                {
                    var dimension = dataViewModel.Dimension.Split('x');
                    return new MediaElementDimensions
                           {
                               Width = int.Parse(dimension[0]),
                               Height = int.Parse(dimension[1])
                           };
                }

                path = resourceManager.GetResourcePath(dataViewModel.Hash);
            }
            else
            {
                path = resourceManager.GetThumbnailPath(dataViewModel.ThumbnailHash);
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            var result = new MediaElementDimensions { Width = image.PixelWidth, Height = image.PixelHeight };
            return result;
        }

        private void UpdateLayoutElement(DataViewModelBase oldElement)
        {
            if (!(this.DataContext is SelectMediaPrompt selectMediaPrompt))
            {
                return;
            }

            var mediaShell = selectMediaPrompt.Shell;

            var newElement = (DataViewModelBase)((ICloneable)selectMediaPrompt.MediaElement).Clone();

            var editor = mediaShell.Editor as ViewModels.EditorViewModelBase;
            if (editor == null)
            {
                return;
            }

            UpdateEntityParameters parameters;
            if (newElement is AudioElementDataViewModelBase)
            {
                parameters = new UpdateEntityParameters(
                    new List<DataViewModelBase> { oldElement },
                    new List<DataViewModelBase> { newElement },
                    editor.CurrentAudioOutputElement.Elements);
            }
            else
            {
                parameters = new UpdateEntityParameters(
                    new List<DataViewModelBase> { oldElement },
                    new List<DataViewModelBase> { newElement },
                    editor.Elements);
            }

            editor.UpdateElementCommand.Execute(parameters);
        }

        private void UpdateReplacementElement(DataViewModelBase oldElement, IEnumerable<DataViewModelBase> container)
        {
            if (!(this.DataContext is SelectMediaPrompt selectMediaPrompt))
            {
                return;
            }

            var mediaShell = selectMediaPrompt.Shell;

            var newElement = (DataViewModelBase)((ICloneable)selectMediaPrompt.MediaElement).Clone();

            var editor = mediaShell.Editor as ViewModels.EditorViewModelBase;

            var parameters = new UpdateEntityParameters(
                new List<DataViewModelBase> { oldElement },
                new List<DataViewModelBase> { newElement },
                container);

            if (editor != null)
            {
                editor.UpdateElementCommand.Execute(parameters);
            }
        }

        private void OnAddButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SelectMediaPrompt prompt))
            {
                return;
            }

            if (!prompt.AddResourceCommand.CanExecute(null))
            {
                return;
            }

            var dialogParameters = this.GetDialogParameters(prompt);

            Action<OpenFileDialogInteraction> onOpen = async interaction =>
                {
                    if (!interaction.Confirmed)
                    {
                        return;
                    }

                    var file = FileSystemManager.Local.GetFile(interaction.FileName);
                    string addingResourceHash;
                    using (var stream = file.OpenRead())
                    {
                        addingResourceHash = ResourceHash.Create(stream);
                    }

                    var parameters = new AddResourceParameters
                                         {
                                             Resources = new[] { file },
                                             Type = dialogParameters.ResourceType,
                                         };
                    try
                    {
                        prompt.AddResourceCommand.Execute(parameters);
                        await parameters.Completed.Task;
                        var newResource =
                           prompt.Shell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                               r => r.Hash == addingResourceHash);
                        prompt.SelectedResource = newResource;
                        this.Close();
                    }
                    catch (UpdateException exception)
                    {
                        Logger.ErrorException("Error while adding a resource.", exception);
                        this.Close();
                    }
                };

            var openDialogInteraction = new OpenFileDialogInteraction
            {
                Filter = dialogParameters.Filter,
                Title = MediaStrings.SelectMediaPopup_Add,
                DirectoryType = dialogParameters.DirectoryType
            };
            InteractionManager<OpenFileDialogInteraction>.Current.Raise(openDialogInteraction, onOpen);
        }

        private OpenDialogParameters GetDialogParameters(SelectMediaPrompt prompt)
        {
            string filter = null;
            var directoryType = DialogDirectoryTypes.Image;
            var resourceType = ResourceType.Image;
            if (prompt.MediaElement is ImageElementDataViewModel)
            {
                if (this.IsLedCanvas)
                {
                    filter = MediaStrings.OpenFileDialog_ImageFilter;
                }
                else
                {
                    filter = MediaStrings.OpenFileDialog_ImageFilterTft;
                }
            }
            else if (prompt.MediaElement is VideoElementDataViewModel)
            {
                filter = MediaStrings.OpenFileDialog_VideoFilter;
                directoryType = DialogDirectoryTypes.Video;
                resourceType = ResourceType.Video;
            }
            else if (prompt.MediaElement is TextualReplacementDataViewModel)
            {
                filter = MediaStrings.OpenFileDialog_ImageFilter;
                directoryType = DialogDirectoryTypes.Symbol;
                resourceType = ResourceType.Symbol;
            }
            else if (prompt.MediaElement is AudioFileElementDataViewModel)
            {
                filter = MediaStrings.OpenFileDialog_AudioFilter;
                directoryType = DialogDirectoryTypes.Audio;
                resourceType = ResourceType.Audio;
            }

            return new OpenDialogParameters(filter, directoryType, resourceType);
        }

        private void OnFitToOrignalSizeButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is SelectMediaPrompt prompt))
            {
                return;
            }

            if (prompt.SelectedResource != null)
            {
                var oldElement = (DataViewModelBase)((ICloneable)prompt.MediaElement).Clone();

                var done = false;

                var imageDataViewModel = prompt.MediaElement as ImageElementDataViewModel;
                if (imageDataViewModel != null && imageDataViewModel.Image != null)
                {
                    this.UpdateDimensions(prompt, imageDataViewModel);
                    this.UpdateLayoutElement(oldElement);
                    done = true;
                }

                var videoDataViewModel = prompt.MediaElement as VideoElementDataViewModel;
                if (!done && videoDataViewModel != null
                    && videoDataViewModel.VideoUri != null
                    && videoDataViewModel.VideoUri.Value != string.Empty)
                {
                    this.UpdateDimensions(prompt, videoDataViewModel);
                    this.UpdateLayoutElement(oldElement);
                }
            }
        }

        private void UpdateRecentMedia(ResourceInfoDataViewModel newElement)
        {
            if (!(this.DataContext is SelectMediaPrompt prompt))
            {
                return;
            }

            if (prompt.RecentMediaResources != null && newElement != null)
            {
                Func<ResourceInfoDataViewModel, bool> predicate = v => v.Hash == newElement.Hash;

                if (prompt.RecentMediaResources.Any(predicate))
                {
                    prompt.RecentMediaResources.Remove(prompt.RecentMediaResources.First(predicate));
                }
                else if (prompt.RecentMediaResources.Count(
                    resource => this.currentResourceTypes.Contains(resource.Type))
                    >= Settings.Default.MaxRecentResources)
                {
                    var lastResource = prompt.RecentMediaResources.LastOrDefault(
                        resource => this.currentResourceTypes.Contains(resource.Type));
                    if (lastResource != null)
                    {
                        prompt.RecentMediaResources.Remove(lastResource);
                    }
                }

                prompt.RecentMediaResources.Insert(0, newElement);
            }

            if (prompt.RecentMediaResources == null)
            {
                return;
            }

            var originalResource = prompt.Shell.MediaApplicationState.CurrentProject.Resources;
            var recentResources = prompt.RecentMediaResources.ToArray();
            foreach (var resource in recentResources)
            {
                if (originalResource.All(or => or.Hash != resource.Hash))
                {
                    prompt.RecentMediaResources.Remove(resource);
                }
            }

            this.SetMediaSources(prompt.RecentMediaResources, "RecentMedia");
        }

        private void OnRecentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.RecentValuesList.SelectedItem != null)
            {
                var item = (ResourceInfoDataViewModel)this.RecentValuesList.SelectedItem;

                if (!(this.DataContext is SelectMediaPrompt prompt))
                {
                    return;
                }

                prompt.SelectedResource = item;

                this.RecentValuesList.SelectedItem = null;
                this.SelectedMedia = item.Hash;
            }
        }

        private void ClosePopupElement(object o = null, MouseEventArgs e = null)
        {
            if (this.previewPopupTimer != null)
            {
                this.previewPopupTimer.Stop();
            }

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
            var prompt = (SelectMediaPrompt)this.DataContext;
            var border = new Border
                             {
                                 BorderBrush = Brushes.Black,
                                 BorderThickness = new Thickness(1),
                                 Background = Brushes.White,
                                 Effect =
                                     new DropShadowEffect
                                         {
                                             Color = Colors.DarkGray,
                                             Direction = 320,
                                             ShadowDepth = 1,
                                             Opacity = 1
                                         }
                             };

            switch (prompt.MediaElement.GetType().Name)
            {
                case "ImageElementDataViewModel":
                    border.Child = new Image { Source = parameters.ImageSource, Width = 200 };
                    this.currentPopupElement = border;
                    break;
                case "VideoElementDataViewModel":
                    var resourceInfo = this.GetCurrentItemContext(parameters.ListViewName);
                    if (resourceInfo.Type == ResourceType.Video)
                    {
                        border.Child = new SkimmingElement { Width = 200, Height = 200, DataContext = resourceInfo };
                        this.currentPopupElement = border;
                    }

                    break;
                case "AudioFileElementDataViewModel":
                    var audioResourceInfo = this.GetCurrentItemContext(parameters.ListViewName);
                    if (audioResourceInfo.Type == ResourceType.Audio)
                    {
                        border.Child = new AudioSkimmingElement
                        {
                            Width = 200,
                            Height = 60,
                            DataContext = audioResourceInfo
                        };
                        this.currentPopupElement = border;
                    }

                    break;
                case "TextualReplacementDataViewModel":
                    return;
                default:
                    throw new NotImplementedException("Media element type is not supported.");
            }

            var position = parameters.MousePosition;
            Canvas.SetLeft(this.currentPopupElement, position.X);
            Canvas.SetTop(this.currentPopupElement, position.Y);

            this.currentPopupElement.MouseLeave += this.ClosePopupElement;
            this.currentThumbnail.MouseEnter -= this.OnMouseEnterPreview;
            this.PopupContainer.Children.Add(this.currentPopupElement);
        }

        private ResourceInfoDataViewModel GetCurrentItemContext(string listViewName)
        {
            if (listViewName == "MediaList")
            {
                for (var i = 0; i < this.MediaList.Items.Count; i++)
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
            }
            else
            {
                for (var i = 0; i < this.RecentValuesList.Items.Count; i++)
                {
                    var item = this.GetRecentListViewItem(i);
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

        private ListViewItem GetRecentListViewItem(int index)
        {
            if (this.RecentValuesList.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return null;
            }

            return this.RecentValuesList.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private bool IsMouseOverTarget(Visual target, Point position)
        {
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            return bounds.Contains(position);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void SearchBoxOnSearched(object sender, SearchBox.PropertyGridSearchEventArgs e)
        {
            this.searchText = e.Text;

            if (!string.IsNullOrWhiteSpace(this.searchText))
            {
                this.Search();
            }
            else
            {
                this.ClearSearch();
            }
        }

        private void OnMediaListViewItemSourceChanged()
        {
            if (this.MediaList.ItemsSource == null)
            {
                return;
            }

            var view = (CollectionView)CollectionViewSource.GetDefaultView(this.MediaList.ItemsSource);
            view.Filter = this.NameFilter;
        }

        private void SearchBoxClearSearch(object sender, EventArgs e)
        {
            this.ClearSearch();
        }

        private void ClearSearch()
        {
            this.searchText = string.Empty;
            this.RefreshMediaList();
        }

        private void Search()
        {
            this.RefreshMediaList();
        }

        private void RefreshMediaList()
        {
            if (this.MediaList.ItemsSource == null)
            {
                return;
            }

            CollectionViewSource.GetDefaultView(this.MediaList.ItemsSource).Refresh();

            if (!(this.DataContext is SelectMediaPrompt selectMediaPrompt))
            {
                return;
            }

            this.MediaList.SetSelectedItem(selectMediaPrompt.SelectedResource);
        }

        private bool NameFilter(object item)
        {
            var resource = item as ResourceInfoDataViewModel;
            if (resource == null)
            {
                throw new Exception(string.Format("Object type '{0}' can not filtered", item.GetType()));
            }

            bool result;
            if (this.IsLedCanvas)
            {
                result = this.currentResourceTypes.Contains(resource.Type);
            }
            else
            {
                result = !resource.IsLedImage
                             && this.currentResourceTypes.Contains(resource.Type);
            }

            if (!result)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.searchText))
            {
                return true;
            }

            var fileName = Path.GetFileNameWithoutExtension(resource.Filename);
            if (fileName == null)
            {
                return false;
            }

            result = fileName.IndexOf(this.searchText, StringComparison.OrdinalIgnoreCase) >= 0;

            return result;
        }

        private class PreviewPopupParameters
        {
            public ImageSource ImageSource { get; set; }

            public Point MousePosition { get; set; }

            public string ListViewName { get; set; }
        }

        private class MediaElementDimensions
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }

        private class OpenDialogParameters
        {
            public OpenDialogParameters(string filter, DialogDirectoryType directoryType, ResourceType resourceType)
            {
                this.Filter = filter;
                this.DirectoryType = directoryType;
                this.ResourceType = resourceType;
            }

            public string Filter { get; private set; }

            public DialogDirectoryType DirectoryType { get; private set; }

            public ResourceType ResourceType { get; private set; }
        }
    }
}
