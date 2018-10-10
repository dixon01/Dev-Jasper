



using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Gorba.Common.Configuration.Infomedia.Common;
using Gorba.Common.Configuration.Infomedia.Layout;

using Gorba.Motion.Infomedia.Entities;
using Gorba.Motion.Infomedia.Entities.Screen;

using Gorba.Center.Media.Core.DataViewModels.Layout;

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    public static class ComposerFactory
    {
        public static IComposer CreateComposer(
            IComposerContext context, IComposer parent, GraphicalElementDataViewModelBase viewModel)
        {
            var viewModelFrame = viewModel as FrameElementDataViewModel;
            if (viewModelFrame != null)
            {
                return new FrameComposer(context, parent, viewModelFrame);
            }

			var viewModelRssTicker = viewModel as RssTickerElementDataViewModel;
			if (viewModelRssTicker != null)
			{
				return new RssTickerComposer(context, parent, viewModelRssTicker);
			}

            var viewModelAnalogClock = viewModel as AnalogClockElementDataViewModel;
            if (viewModelAnalogClock != null)
            {
                return new AnalogClockComposer(context, parent, viewModelAnalogClock);
            }

            var viewModelImage = viewModel as ImageElementDataViewModel;
            if (viewModelImage != null)
            {
                return new ImageComposer(context, parent, viewModelImage);
            }

            var viewModelImageList = viewModel as ImageListElementDataViewModel;
            if (viewModelImageList != null)
            {
                return new ImageListComposer(context, parent, viewModelImageList);
            }

            var viewModelText = viewModel as TextElementDataViewModel;
            if (viewModelText != null)
            {
                return new TextComposer(context, parent, viewModelText);
            }

            var viewModelVideo = viewModel as VideoElementDataViewModel;
            if (viewModelVideo != null)
            {
                return new VideoComposer(context, parent, viewModelVideo);
            }

            var viewModelRectangle = viewModel as RectangleElementDataViewModel;
            if (viewModelRectangle != null)
            {
                return new RectangleComposer(context, parent, viewModelRectangle);
            }
            throw new NotSupportedException(
                string.Format("Unsupported layout element {0}", viewModel.GetType().FullName));
        }
    }


    public abstract partial class GraphicalComposerBase<TViewModel> : ComposerBase<TViewModel>
        where TViewModel : GraphicalElementDataViewModelBase
    {
        public GraphicalComposerBase(IComposerContext context, IComposer parent, TViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public abstract partial class DrawableComposerBase<TViewModel, TItem> : GraphicalComposerBase<TViewModel>, IPresentableComposer
        where TViewModel : DrawableElementDataViewModelBase
        where TItem : DrawableItemBase, new()
    {
        public DrawableComposerBase(IComposerContext context, IComposer parent, TViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item = new TItem();
            this.Item.ZIndex = this.ViewModel.ZIndex.Value;
            this.Initialize();
        }
            
        public TItem Item { get; private set; }

        ScreenItemBase IPresentableComposer.Item
        {
            get
            {
                return this.Item;
            }
        }
        
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "ZIndex":
                    this.Item.ZIndex = this.ViewModel.ZIndex.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class AnalogClockComposer : DrawableComposerBase<AnalogClockElementDataViewModel, AnalogClockItem>
    {
        public AnalogClockComposer(IComposerContext context, IComposer parent, AnalogClockElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class ImageComposer : DrawableComposerBase<ImageElementDataViewModel, ImageItem>
    {
        public ImageComposer(IComposerContext context, IComposer parent, ImageElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Scaling = this.ViewModel.Scaling.Value;
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "Scaling":
                    this.Item.Scaling = this.ViewModel.Scaling.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class ImageListComposer : DrawableComposerBase<ImageListElementDataViewModel, ImageListItem>
    {
        public ImageListComposer(IComposerContext context, IComposer parent, ImageListElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Overflow = this.ViewModel.Overflow.Value;
            this.Item.Align = this.ViewModel.Align.Value;
            this.Item.Direction = this.ViewModel.Direction.Value;
            this.Item.HorizontalImageGap = this.ViewModel.HorizontalImageGap.Value;
            this.Item.VerticalImageGap = this.ViewModel.VerticalImageGap.Value;
            this.Item.ImageWidth = this.ViewModel.ImageWidth.Value;
            this.Item.ImageHeight = this.ViewModel.ImageHeight.Value;
            this.Item.FallbackImage = this.ViewModel.FallbackImage.Value;
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "Overflow":
                    this.Item.Overflow = this.ViewModel.Overflow.Value;
                    break;
                case "Align":
                    this.Item.Align = this.ViewModel.Align.Value;
                    break;
                case "Direction":
                    this.Item.Direction = this.ViewModel.Direction.Value;
                    break;
                case "HorizontalImageGap":
                    this.Item.HorizontalImageGap = this.ViewModel.HorizontalImageGap.Value;
                    break;
                case "VerticalImageGap":
                    this.Item.VerticalImageGap = this.ViewModel.VerticalImageGap.Value;
                    break;
                case "ImageWidth":
                    this.Item.ImageWidth = this.ViewModel.ImageWidth.Value;
                    break;
                case "ImageHeight":
                    this.Item.ImageHeight = this.ViewModel.ImageHeight.Value;
                    break;
                case "FallbackImage":
                    this.Item.FallbackImage = this.ViewModel.FallbackImage.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class TextComposer : DrawableComposerBase<TextElementDataViewModel, TextItem>
    {
        public TextComposer(IComposerContext context, IComposer parent, TextElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Rotation = this.ViewModel.Rotation.Value;
            this.Item.Align = this.ViewModel.Align.Value;
            this.Item.VAlign = this.ViewModel.VAlign.Value;
            this.Item.Overflow = this.ViewModel.Overflow.Value;
            this.Item.ScrollSpeed = this.ViewModel.ScrollSpeed.Value;
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "Rotation":
                    this.Item.Rotation = this.ViewModel.Rotation.Value;
                    break;
                case "Align":
                    this.Item.Align = this.ViewModel.Align.Value;
                    break;
                case "VAlign":
                    this.Item.VAlign = this.ViewModel.VAlign.Value;
                    break;
                case "Overflow":
                    this.Item.Overflow = this.ViewModel.Overflow.Value;
                    break;
                case "ScrollSpeed":
                    this.Item.ScrollSpeed = this.ViewModel.ScrollSpeed.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class VideoComposer : DrawableComposerBase<VideoElementDataViewModel, VideoItem>
    {
        public VideoComposer(IComposerContext context, IComposer parent, VideoElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Scaling = this.ViewModel.Scaling.Value;
            this.Item.Replay = this.ViewModel.Replay.Value;
            this.Item.FallbackImage = this.ViewModel.FallbackImage.Value;
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "Scaling":
                    this.Item.Scaling = this.ViewModel.Scaling.Value;
                    break;
                case "Replay":
                    this.Item.Replay = this.ViewModel.Replay.Value;
                    break;
                case "FallbackImage":
                    this.Item.FallbackImage = this.ViewModel.FallbackImage.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class RectangleComposer : DrawableComposerBase<RectangleElementDataViewModel, RectangleItem>
    {
        public RectangleComposer(IComposerContext context, IComposer parent, RectangleElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public abstract partial class AudioComposerBase<TViewModel> : ComposerBase<TViewModel>
        where TViewModel : AudioElementDataViewModelBase
    {
        public AudioComposerBase(IComposerContext context, IComposer parent, TViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class AudioOutputComposer : AudioComposerBase<AudioOutputElementDataViewModel>
    {
        public AudioOutputComposer(IComposerContext context, IComposer parent, AudioOutputElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public abstract partial class PlaybackComposerBase<TViewModel, TItem> : AudioComposerBase<TViewModel>, IPresentableComposer
        where TViewModel : PlaybackElementDataViewModelBase
        where TItem : PlaybackItemBase, new()
    {
        public PlaybackComposerBase(IComposerContext context, IComposer parent, TViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item = new TItem();
            this.Initialize();
        }
            
        public TItem Item { get; private set; }

        ScreenItemBase IPresentableComposer.Item
        {
            get
            {
                return this.Item;
            }
        }
        
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class AudioFileComposer : PlaybackComposerBase<AudioFileElementDataViewModel, AudioFileItem>
    {
        public AudioFileComposer(IComposerContext context, IComposer parent, AudioFileElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class AudioPauseComposer : PlaybackComposerBase<AudioPauseElementDataViewModel, AudioPauseItem>
    {
        public AudioPauseComposer(IComposerContext context, IComposer parent, AudioPauseElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Item.Duration = this.ViewModel.Duration.Value;
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                case "Duration":
                    this.Item.Duration = this.ViewModel.Duration.Value;
                    break;
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
    public partial class TextToSpeechComposer : PlaybackComposerBase<TextToSpeechElementDataViewModel, TextToSpeechItem>
    {
        public TextToSpeechComposer(IComposerContext context, IComposer parent, TextToSpeechElementDataViewModel viewModel)
            : base(context, parent, viewModel)
        {
            this.Initialize();
        }
    
        public override void Dispose()
        {
            this.Deinitialize();
            base.Dispose();
        }
        
        partial void Initialize();

        partial void PreHandlePropertyChange(string propertyName, ref bool handled);

        partial void Deinitialize();

        protected override void HandlePropertyChange(string propertyName)
        {
            var handled = false;
            this.PreHandlePropertyChange(propertyName, ref handled);
            if (handled)
            {
                return;
            }

            switch (propertyName)
            {
                default:
                    base.HandlePropertyChange(propertyName);
                    break;
            }
        }
    }
    
}

