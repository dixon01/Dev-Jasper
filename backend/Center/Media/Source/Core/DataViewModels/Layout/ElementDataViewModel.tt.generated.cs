

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
	public partial class GraphicalElementDataViewModelBase
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "X")
			{
				this.ValidateX(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Y")
			{
				this.ValidateY(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Width")
			{
				this.ValidateWidth(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Height")
			{
				this.ValidateHeight(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Visible")
			{
				this.ValidateVisible(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateX(object dataValue, ref string errorMessage);

        partial void ValidateY(object dataValue, ref string errorMessage);

        partial void ValidateWidth(object dataValue, ref string errorMessage);

        partial void ValidateHeight(object dataValue, ref string errorMessage);

        partial void ValidateVisible(object dataValue, ref string errorMessage);
	}

	public partial class DrawableElementDataViewModelBase
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "ZIndex")
			{
				this.ValidateZIndex(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateZIndex(object dataValue, ref string errorMessage);
	}

	public partial class AnalogClockElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "HourMode")
			{
				this.ValidateHourMode(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "HourCenterX")
			{
				this.ValidateHourCenterX(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "HourCenterY")
			{
				this.ValidateHourCenterY(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "MinuteMode")
			{
				this.ValidateMinuteMode(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "MinuteCenterX")
			{
				this.ValidateMinuteCenterX(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "MinuteCenterY")
			{
				this.ValidateMinuteCenterY(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "SecondsMode")
			{
				this.ValidateSecondsMode(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "SecondsCenterX")
			{
				this.ValidateSecondsCenterX(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "SecondsCenterY")
			{
				this.ValidateSecondsCenterY(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateHourMode(object dataValue, ref string errorMessage);

        partial void ValidateHourCenterX(object dataValue, ref string errorMessage);

        partial void ValidateHourCenterY(object dataValue, ref string errorMessage);

        partial void ValidateMinuteMode(object dataValue, ref string errorMessage);

        partial void ValidateMinuteCenterX(object dataValue, ref string errorMessage);

        partial void ValidateMinuteCenterY(object dataValue, ref string errorMessage);

        partial void ValidateSecondsMode(object dataValue, ref string errorMessage);

        partial void ValidateSecondsCenterX(object dataValue, ref string errorMessage);

        partial void ValidateSecondsCenterY(object dataValue, ref string errorMessage);
	}

	public partial class AnalogClockHandElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Mode")
			{
				this.ValidateMode(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "CenterX")
			{
				this.ValidateCenterX(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "CenterY")
			{
				this.ValidateCenterY(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateMode(object dataValue, ref string errorMessage);

        partial void ValidateCenterX(object dataValue, ref string errorMessage);

        partial void ValidateCenterY(object dataValue, ref string errorMessage);
	}

	public partial class FrameElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "FrameId")
			{
				this.ValidateFrameId(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateFrameId(object dataValue, ref string errorMessage);
	}

	public partial class ImageElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Filename")
			{
				this.ValidateFilename(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Scaling")
			{
				this.ValidateScaling(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateFilename(object dataValue, ref string errorMessage);

        partial void ValidateScaling(object dataValue, ref string errorMessage);
	}

	public partial class ImageListElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Overflow")
			{
				this.ValidateOverflow(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Align")
			{
				this.ValidateAlign(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Direction")
			{
				this.ValidateDirection(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "HorizontalImageGap")
			{
				this.ValidateHorizontalImageGap(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "VerticalImageGap")
			{
				this.ValidateVerticalImageGap(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "ImageWidth")
			{
				this.ValidateImageWidth(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "ImageHeight")
			{
				this.ValidateImageHeight(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Delimiter")
			{
				this.ValidateDelimiter(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FilePatterns")
			{
				this.ValidateFilePatterns(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FallbackImage")
			{
				this.ValidateFallbackImage(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Values")
			{
				this.ValidateValues(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateOverflow(object dataValue, ref string errorMessage);

        partial void ValidateAlign(object dataValue, ref string errorMessage);

        partial void ValidateDirection(object dataValue, ref string errorMessage);

        partial void ValidateHorizontalImageGap(object dataValue, ref string errorMessage);

        partial void ValidateVerticalImageGap(object dataValue, ref string errorMessage);

        partial void ValidateImageWidth(object dataValue, ref string errorMessage);

        partial void ValidateImageHeight(object dataValue, ref string errorMessage);

        partial void ValidateDelimiter(object dataValue, ref string errorMessage);

        partial void ValidateFilePatterns(object dataValue, ref string errorMessage);

        partial void ValidateFallbackImage(object dataValue, ref string errorMessage);

        partial void ValidateValues(object dataValue, ref string errorMessage);
	}

	public partial class TextElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Rotation")
			{
				this.ValidateRotation(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Align")
			{
				this.ValidateAlign(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "VAlign")
			{
				this.ValidateVAlign(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Overflow")
			{
				this.ValidateOverflow(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "ScrollSpeed")
			{
				this.ValidateScrollSpeed(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontFace")
			{
				this.ValidateFontFace(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontHeight")
			{
				this.ValidateFontHeight(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontWeight")
			{
				this.ValidateFontWeight(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontItalic")
			{
				this.ValidateFontItalic(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontColor")
			{
				this.ValidateFontColor(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontOutlineColor")
			{
				this.ValidateFontOutlineColor(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FontCharSpacing")
			{
				this.ValidateFontCharSpacing(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Value")
			{
				this.ValidateValue(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateRotation(object dataValue, ref string errorMessage);

        partial void ValidateAlign(object dataValue, ref string errorMessage);

        partial void ValidateVAlign(object dataValue, ref string errorMessage);

        partial void ValidateOverflow(object dataValue, ref string errorMessage);

        partial void ValidateScrollSpeed(object dataValue, ref string errorMessage);

        partial void ValidateFontFace(object dataValue, ref string errorMessage);

        partial void ValidateFontHeight(object dataValue, ref string errorMessage);

        partial void ValidateFontWeight(object dataValue, ref string errorMessage);

        partial void ValidateFontItalic(object dataValue, ref string errorMessage);

        partial void ValidateFontColor(object dataValue, ref string errorMessage);

        partial void ValidateFontOutlineColor(object dataValue, ref string errorMessage);

        partial void ValidateFontCharSpacing(object dataValue, ref string errorMessage);

        partial void ValidateValue(object dataValue, ref string errorMessage);
	}

	public partial class VideoElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "VideoUri")
			{
				this.ValidateVideoUri(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Scaling")
			{
				this.ValidateScaling(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Replay")
			{
				this.ValidateReplay(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "FallbackImage")
			{
				this.ValidateFallbackImage(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateVideoUri(object dataValue, ref string errorMessage);

        partial void ValidateScaling(object dataValue, ref string errorMessage);

        partial void ValidateReplay(object dataValue, ref string errorMessage);

        partial void ValidateFallbackImage(object dataValue, ref string errorMessage);
	}

	public partial class RectangleElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Color")
			{
				this.ValidateColor(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateColor(object dataValue, ref string errorMessage);
	}

	public partial class AudioElementDataViewModelBase
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Enabled")
			{
				this.ValidateEnabled(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateEnabled(object dataValue, ref string errorMessage);
	}

	public partial class AudioOutputElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Volume")
			{
				this.ValidateVolume(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Priority")
			{
				this.ValidatePriority(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateVolume(object dataValue, ref string errorMessage);

        partial void ValidatePriority(object dataValue, ref string errorMessage);
	}

	public partial class AudioFileElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Filename")
			{
				this.ValidateFilename(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateFilename(object dataValue, ref string errorMessage);
	}

	public partial class AudioPauseElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Duration")
			{
				this.ValidateDuration(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateDuration(object dataValue, ref string errorMessage);
	}

	public partial class TextToSpeechElementDataViewModel
	{
        /// <summary>
        /// If overridden this method validates a property before it is definitely set.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="dataValue">
        /// The value.
        /// </param>
        /// <returns>
        /// The error string if the value is invalid.
        /// </returns>
        public override string IsValid(string propertyName, object dataValue)
        {
			string errorMessage = null;

			if (propertyName == "Voice")
			{
				this.ValidateVoice(dataValue, ref errorMessage);
				return errorMessage;
			}

			if (propertyName == "Value")
			{
				this.ValidateValue(dataValue, ref errorMessage);
				return errorMessage;
			}

            return base.IsValid(propertyName, dataValue);
		}

        partial void ValidateVoice(object dataValue, ref string errorMessage);

        partial void ValidateValue(object dataValue, ref string errorMessage);
	}
}