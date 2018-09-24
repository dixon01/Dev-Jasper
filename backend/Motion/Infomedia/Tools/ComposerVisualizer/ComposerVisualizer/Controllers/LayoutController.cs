// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;

    using Gorba.Motion.Infomedia.Core.Presentation;
    using Gorba.Motion.Infomedia.Core.Presentation.Composer;
    using Gorba.Motion.Infomedia.Core.Presentation.Master;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Evaluator;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.DataViewModels.Items;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.ViewModels;
    using Gorba.Motion.Infomedia.Tools.ComposerVisualizer.Views;

    /// <summary>
    /// Class to control the layout view
    /// </summary>
    public class LayoutController : ControllerBase, IController
    {
        private readonly PresentationManager presentationManager;

        private bool running;

        private Dictionary<LayoutView, LayoutViewModel> layouts;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutController"/> class.
        /// </summary>
        /// <param name="presentationManager">
        /// The presentation Manager.
        /// </param>
        public LayoutController(PresentationManager presentationManager)
        {
            this.presentationManager = presentationManager;
            this.presentationManager.ScreensUpdated += this.PresentationManagerOnScreensUpdated;
        }

        /// <summary>
        /// Gets or sets the layouts tab ViewModel.
        /// </summary>
        public LayoutTabViewModel LayoutsTabViewModel { get; set; }

        /// <summary>
        /// Gets or sets the layouts tab.
        /// </summary>
        public AllLayoutsView LayoutsTab { get; set; }

        /// <summary>
        /// Starts the layout controller
        /// </summary>
        public void Run()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;

            this.layouts = new Dictionary<LayoutView, LayoutViewModel>();
            this.LayoutsTabViewModel = new LayoutTabViewModel();
            this.CreateLayoutView();
        }

        /// <summary>
        /// Stops the layout controller
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.presentationManager.ScreensUpdated -= this.PresentationManagerOnScreensUpdated;
        }

        /// <summary>
        /// Freezes or unfreezes the visualization of layout view
        /// </summary>
        /// <param name="isFrozen">
        /// Flag indicating when the visualization must be frozen or not.
        /// </param>
        public void FreezeVisualization(bool isFrozen)
        {
            if (isFrozen)
            {
                this.presentationManager.ScreensUpdated -= this.PresentationManagerOnScreensUpdated;
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, new Action(this.UpdateLayoutView));
                this.presentationManager.ScreensUpdated += this.PresentationManagerOnScreensUpdated;
            }
        }

        private void CreateLayoutView()
        {
            this.LayoutsTab = new AllLayoutsView();
            this.layouts.Clear();
            foreach (var masterPresentationEngine in this.presentationManager.MasterEngines)
            {
                var tabName = this.GetTabName(masterPresentationEngine);
                var layoutView = new LayoutView();
                var layoutViewModel = new LayoutViewModel(tabName);
                foreach (var composer in masterPresentationEngine.VirtualDisplayComposer.PresentationEngine.Composers)
                {
                    var layoutItem = this.ObtainLayoutItem(composer);
                    layoutViewModel.LayoutItems.Add(layoutItem);
                }

                layoutView.DataContext = layoutViewModel;
                layoutView.Name = masterPresentationEngine.ScreenConfig.Name;
                this.layouts[layoutView] = layoutViewModel;
                this.LayoutsTabViewModel.Layouts.Add(layoutViewModel);
            }

            this.LayoutsTab.DataContext = this.LayoutsTabViewModel;
        }

        private string GetTabName(MasterPresentationEngine masterPresentationEngine)
        {
            var currentCycle =
                masterPresentationEngine.VirtualDisplayComposer.PresentationEngine.PackageCycleManager.CurrentCycle;
            var tabName = string.Format(
                "{0}: {1} {2} {3}: {4}",
                @"Physical Screen Name",
                masterPresentationEngine.ScreenConfig.Name,
                Environment.NewLine,
                @"Layout Name",
                currentCycle.CurrentSection.Config.Layout);
            return tabName;
        }

        private ItemBaseDataViewModel ObtainLayoutItem(IComposer composer)
        {
            var text = composer as TextComposer;
            if (text != null)
            {
                return this.CreateTextLayoutItem(text);
            }

            var image = composer as ImageComposer;
            if (image != null)
            {
                return this.CreateImageLayoutItem(image);
            }

            var imageList = composer as ImageListComposer;
            if (imageList != null)
            {
                return this.CreateImageListLayoutItem(imageList);
            }

            var video = composer as VideoComposer;
            if (video != null)
            {
                return this.CreateVideoLayoutItem(video);
            }

            var analogClock = composer as AnalogClockComposer;
            if (analogClock != null)
            {
                return this.CreateAnalogClockLayoutItem(analogClock);
            }

            var audiofile = composer as AudioFileComposer;
            if (audiofile != null)
            {
                return this.CreateAudioFileLayoutItem(audiofile);
            }

            var audioPause = composer as AudioPauseComposer;
            if (audioPause != null)
            {
                return this.CreateAudioPauseLayoutItem(audioPause);
            }

            var rectangle = composer as RectangleComposer;
            if (rectangle != null)
            {
                return this.CreateRectangleLayoutItem(rectangle);
            }

            var textToSpeech = composer as TextToSpeechComposer;
            if (textToSpeech != null)
            {
                return this.CreateTextToSpeechLayoutItem(textToSpeech);
            }

            return new GraphicalItemBaseDataViewModel();
        }

        private ItemBaseDataViewModel CreateAudioPauseLayoutItem(AudioPauseComposer audioPause)
        {
            var audioPauseProperties = new AudioPauseItemDataViewModel();
            audioPauseProperties.Duration = audioPause.Item.Duration;
            audioPauseProperties.Volume = audioPause.Item.Volume;
            audioPauseProperties.Priority = audioPause.Item.Priority;
            audioPauseProperties.Enabled = this.RetrieveEvaluators(audioPause.HandlerEnabled.Evaluator);
            audioPauseProperties.Enabled.Value = audioPause.Item.Enabled;
            audioPauseProperties.Id = audioPause.Item.Id;
            audioPauseProperties.ImagePath = "/Images/audiodata_44x44.png";

            return audioPauseProperties;
        }

        private ItemBaseDataViewModel CreateTextToSpeechLayoutItem(TextToSpeechComposer textToSpeech)
        {
            var ttsProperties = new TextToSpeechItemDataViewModel();
            ttsProperties.Voice = this.RetrieveEvaluators(textToSpeech.HandlerVoice.Evaluator);
            ttsProperties.Voice.Value = textToSpeech.Item.Voice;
            ttsProperties.Value = this.RetrieveEvaluators(textToSpeech.HandlerValue.Evaluator);
            ttsProperties.Value.Value = textToSpeech.Item.Value;
            ttsProperties.Volume = textToSpeech.Item.Volume;
            ttsProperties.Priority = textToSpeech.Item.Priority;
            ttsProperties.Enabled = this.RetrieveEvaluators(textToSpeech.HandlerEnabled.Evaluator);
            ttsProperties.Enabled.Value = textToSpeech.Item.Enabled;
            ttsProperties.Id = textToSpeech.Item.Id;
            ttsProperties.ImagePath = "/Images/texttospeech_44x44.png";

            return ttsProperties;
        }

        private ItemBaseDataViewModel CreateRectangleLayoutItem(RectangleComposer rectangle)
        {
            var rectangleProperties = new RectangleItemDataViewModel();
            rectangleProperties.Color = rectangle.Item.Color;
            rectangleProperties.ZIndex = rectangle.Item.ZIndex;
            rectangleProperties.X = rectangle.Item.X;
            rectangleProperties.Y = rectangle.Item.Y;
            rectangleProperties.Width = rectangle.Item.Width;
            rectangleProperties.Height = rectangle.Item.Height;
            rectangleProperties.Visible = this.RetrieveEvaluators(rectangle.HandlerVisible.Evaluator);
            rectangleProperties.Visible.Value = rectangle.Item.Visible;

            rectangleProperties.Id = rectangle.Item.Id;
            rectangleProperties.ImagePath = "/Images/rectangle_44x44.png";

            return rectangleProperties;
        }

        private ItemBaseDataViewModel CreateAudioFileLayoutItem(AudioFileComposer audiofile)
        {
            var audioFileProperties = new AudioFileItemDataViewModel();
            audioFileProperties.Filename = this.RetrieveEvaluators(audiofile.HandlerFilename.Evaluator);
            audioFileProperties.Volume = audiofile.Item.Volume;
            audioFileProperties.Priority = audiofile.Item.Priority;
            audioFileProperties.Enabled = this.RetrieveEvaluators(audiofile.HandlerEnabled.Evaluator);
            audioFileProperties.Enabled.Value = audiofile.Item.Enabled;
            audioFileProperties.Id = audiofile.Item.Id;
            audioFileProperties.ImagePath = "/Images/audiodata_44x44.png";

            return audioFileProperties;
        }

        private GraphicalItemBaseDataViewModel CreateAnalogClockLayoutItem(
            AnalogClockComposer analogClock)
        {
            var analogClockProperties = new AnalogClockItemDataViewModel();
            analogClockProperties.Hour = new AnalogClockHandItemDataViewModel();
            analogClockProperties.Hour.CenterX = analogClock.Item.Hour.CenterX;
            analogClockProperties.Hour.CenterY = analogClock.Item.Hour.CenterY;
            analogClockProperties.Hour.Mode = analogClock.Item.Hour.Mode;
            var evaluatorBaseDataViewModel = new EvaluatorBaseDataViewModel();
            evaluatorBaseDataViewModel.Value = analogClock.Item.Hour.Filename;
            analogClockProperties.Hour.Filename = evaluatorBaseDataViewModel;
            analogClockProperties.Hour.Scaling = analogClock.Item.Hour.Scaling;
            analogClockProperties.Hour.Blink = analogClock.Item.Hour.Blink;
            analogClockProperties.Hour.ZIndex = analogClock.Item.Hour.ZIndex;
            analogClockProperties.Hour.X = analogClock.Item.Hour.X;
            analogClockProperties.Hour.Y = analogClock.Item.Hour.Y;
            analogClockProperties.Hour.Width = analogClock.Item.Hour.Width;
            analogClockProperties.Hour.Height = analogClock.Item.Hour.Height;
            analogClockProperties.Hour.Visible = this.RetrieveEvaluators(analogClock.HandlerVisible.Evaluator);
            analogClockProperties.Hour.Visible.Value = analogClock.Item.Hour.Visible;

            analogClockProperties.Minute = new AnalogClockHandItemDataViewModel();
            analogClockProperties.Minute.CenterX = analogClock.Item.Minute.CenterX;
            analogClockProperties.Minute.CenterY = analogClock.Item.Minute.CenterY;
            analogClockProperties.Minute.Mode = analogClock.Item.Minute.Mode;
            evaluatorBaseDataViewModel = new EvaluatorBaseDataViewModel();
            evaluatorBaseDataViewModel.Value = analogClock.Item.Minute.Filename;
            analogClockProperties.Minute.Filename = evaluatorBaseDataViewModel;
            analogClockProperties.Minute.Scaling = analogClock.Item.Minute.Scaling;
            analogClockProperties.Minute.Blink = analogClock.Item.Minute.Blink;
            analogClockProperties.Minute.ZIndex = analogClock.Item.Minute.ZIndex;
            analogClockProperties.Minute.X = analogClock.Item.Minute.X;
            analogClockProperties.Minute.Y = analogClock.Item.Minute.Y;
            analogClockProperties.Minute.Width = analogClock.Item.Minute.Width;
            analogClockProperties.Minute.Height = analogClock.Item.Minute.Height;
            analogClockProperties.Minute.Visible = this.RetrieveEvaluators(analogClock.HandlerVisible.Evaluator);
            analogClockProperties.Minute.Visible.Value = analogClock.Item.Minute.Visible;

            analogClockProperties.Seconds = new AnalogClockHandItemDataViewModel();
            analogClockProperties.Seconds.CenterX = analogClock.Item.Seconds.CenterX;
            analogClockProperties.Seconds.CenterY = analogClock.Item.Seconds.CenterY;
            analogClockProperties.Seconds.Mode = analogClock.Item.Seconds.Mode;
            evaluatorBaseDataViewModel = new EvaluatorBaseDataViewModel();
            evaluatorBaseDataViewModel.Value = analogClock.Item.Seconds.Filename;
            analogClockProperties.Seconds.Filename = evaluatorBaseDataViewModel;
            analogClockProperties.Seconds.Scaling = analogClock.Item.Seconds.Scaling;
            analogClockProperties.Seconds.Blink = analogClock.Item.Seconds.Blink;
            analogClockProperties.Seconds.ZIndex = analogClock.Item.Seconds.ZIndex;
            analogClockProperties.Seconds.X = analogClock.Item.Seconds.X;
            analogClockProperties.Seconds.Y = analogClock.Item.Seconds.Y;
            analogClockProperties.Seconds.Width = analogClock.Item.Seconds.Width;
            analogClockProperties.Seconds.Height = analogClock.Item.Seconds.Height;
            analogClockProperties.Seconds.Visible = this.RetrieveEvaluators(analogClock.HandlerVisible.Evaluator);
            analogClockProperties.Seconds.Visible.Value = analogClock.Item.Seconds.Visible;

            analogClockProperties.ZIndex = analogClock.Item.ZIndex;
            analogClockProperties.X = analogClock.Item.X;
            analogClockProperties.Y = analogClock.Item.Y;
            analogClockProperties.Width = analogClock.Item.Width;
            analogClockProperties.Height = analogClock.Item.Height;
            analogClockProperties.Visible = this.RetrieveEvaluators(analogClock.HandlerVisible.Evaluator);
            analogClockProperties.Visible.Value = analogClock.Item.Visible;

            analogClockProperties.Id = analogClock.Item.Id;
            analogClockProperties.ImagePath = "/Images/clock_44x44.png";
            return analogClockProperties;
        }

        private GraphicalItemBaseDataViewModel CreateVideoLayoutItem(VideoComposer video)
        {
            var videoProperties = new VideoItemDataViewModel();
            videoProperties.Replay = video.Item.Replay;
            videoProperties.Scaling = video.Item.Scaling;
            videoProperties.VideoUri = this.RetrieveEvaluators(video.HandlerVideoUri.Evaluator);
            videoProperties.ZIndex = video.Item.ZIndex;
            videoProperties.X = video.Item.X;
            videoProperties.Y = video.Item.Y;
            videoProperties.Width = video.Item.Width;
            videoProperties.Height = video.Item.Height;
            videoProperties.Visible = this.RetrieveEvaluators(video.HandlerVisible.Evaluator);
            videoProperties.Visible.Value = video.Item.Visible;
            videoProperties.Id = video.Item.Id;
            videoProperties.ImagePath = "/Images/video_44x44.png";
            return videoProperties;
        }

        private GraphicalItemBaseDataViewModel CreateImageListLayoutItem(
            ImageListComposer imageList)
        {
            var imageListProperties = new ImageListItemDataViewModel();
            imageListProperties.Align = imageList.Item.Align;
            imageListProperties.Direction = imageList.Item.Direction;
            imageListProperties.FallbackImage = imageList.Item.FallbackImage;
            imageListProperties.HorizontalImageGap = imageList.Item.HorizontalImageGap;
            imageListProperties.ImageHeight = imageList.Item.ImageHeight;
            imageListProperties.ImageWidth = imageList.Item.ImageWidth;
            imageListProperties.Images = this.RetrieveEvaluators(imageList.HandlerValues.Evaluator);
            imageListProperties.Overflow = imageList.Item.Overflow;
            imageListProperties.VerticalImageGap = imageList.Item.VerticalImageGap;
            imageListProperties.ZIndex = imageList.Item.ZIndex;
            imageListProperties.X = imageList.Item.X;
            imageListProperties.Y = imageList.Item.Y;
            imageListProperties.Width = imageList.Item.Width;
            imageListProperties.Height = imageList.Item.Height;
            imageListProperties.Visible = this.RetrieveEvaluators(imageList.HandlerVisible.Evaluator);
            imageListProperties.Visible.Value = imageList.Item.Visible;

            imageListProperties.Id = imageList.Item.Id;
            imageListProperties.ImagePath = "/Images/imagelist_44x44.png";
            return imageListProperties;
        }

        private GraphicalItemBaseDataViewModel CreateImageLayoutItem(ImageComposer image)
        {
            var imageProperties = new ImageItemDataViewModel();
            imageProperties.Blink = image.Item.Blink;
            imageProperties.Filename = this.RetrieveEvaluators(image.HandlerFilename.Evaluator);
            imageProperties.Scaling = image.Item.Scaling;
            imageProperties.ZIndex = image.Item.ZIndex;
            imageProperties.X = image.Item.X;
            imageProperties.Y = image.Item.Y;
            imageProperties.Width = image.Item.Width;
            imageProperties.Height = image.Item.Height;
            imageProperties.Visible = this.RetrieveEvaluators(image.HandlerVisible.Evaluator);
            imageProperties.Visible.Value = image.Item.Visible;

            imageProperties.Id = image.Item.Id;
            imageProperties.ImagePath = "/Images/image_44x44.png";
            return imageProperties;
        }

        private GraphicalItemBaseDataViewModel CreateTextLayoutItem(TextComposer text)
        {
            var textProperties = new TextItemDataViewModel();
            textProperties.Align = text.Item.Align;
            textProperties.Font = text.Item.Font;
            textProperties.Overflow = text.Item.Overflow;
            textProperties.Rotation = text.Item.Rotation;
            textProperties.ScrollSpeed = text.Item.ScrollSpeed;
            textProperties.Text = this.RetrieveEvaluators(text.HandlerValue.Evaluator);

            textProperties.VAlign = text.Item.VAlign;
            textProperties.ZIndex = text.Item.ZIndex;
            textProperties.X = text.Item.X;
            textProperties.Y = text.Item.Y;
            textProperties.VAlign = text.Item.VAlign;
            textProperties.Width = text.Item.Width;
            textProperties.Height = text.Item.Height;
            textProperties.Visible = this.RetrieveEvaluators(text.HandlerVisible.Evaluator);
            textProperties.Visible.Value = text.Item.Visible;

            textProperties.Id = text.Item.Id;
            textProperties.ImagePath = "/Images/statictext_44x44.png";
            return textProperties;
        }

        private void PresentationManagerOnScreensUpdated(object sender, ScreenChangesEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background, new Action(this.UpdateLayoutView));
        }

        private void UpdateLayoutView()
        {
            foreach (var masterPresentationEngine in this.presentationManager.MasterEngines)
            {
                var composers = masterPresentationEngine.VirtualDisplayComposer.PresentationEngine.Composers;
                var layout = this.layouts.FirstOrDefault(
                    i => i.Key.Name.Equals(masterPresentationEngine.ScreenConfig.Name));

                var layoutViewModel = this.LayoutsTabViewModel.Layouts.FirstOrDefault(
                    i => i.TabName.Equals(layout.Value.TabName));

                var tabName = this.GetTabName(masterPresentationEngine);
                if (layoutViewModel != null)
                {
                    if (!layoutViewModel.TabName.Equals(tabName))
                    {
                        layoutViewModel.TabName = tabName;

                        layout.Value.TabName = tabName;
                        layout.Value.LayoutItems.Clear();
                        foreach (var composer in composers)
                        {
                            var layoutItem = this.ObtainLayoutItem(composer);
                            layout.Value.LayoutItems.Add(layoutItem);
                        }
                    }
                }
            }
        }
    }
}
