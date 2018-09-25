// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerProcessor.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace Luminator.PeripheralDimmer.Processor
{
    using System;

    using Luminator.PeripheralDimmer.Processor.Interfaces;

    /// <summary>
    ///     Dimmer processor for calculating the light sensor range scale and TFT brightness
    /// </summary>
    public class DimmerProcessor : IDimmerProcessor
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DimmerProcessor"/> class. 
        ///     Default constructor</summary>
        public DimmerProcessor()
        {
            TuningParams = new DimmerProcessorTuningParams();
            LastDimmerInput = new DimmerProcessorInput();
            LastDimmerOutput = new DimmerProcessorOutput();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimmerProcessor"/> class. 
        /// </summary>
        /// <param name="tuning">Tuning params</param>
        public DimmerProcessor(IDimmerProcessorTuningParams tuning)
        {
            TuningParams = tuning ?? new DimmerProcessorTuningParams();
            LastDimmerInput = new DimmerProcessorInput();
            LastDimmerOutput = new DimmerProcessorOutput();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the last dimmer processor input
        /// </summary>
        public IDimmerProcessorInput LastDimmerInput { get; private set; }

        /// <summary>
        ///     Gets the last dimmer processor output
        /// </summary>
        public IDimmerProcessorOutput LastDimmerOutput { get; private set; }

        /// <summary>
        ///     Gets or sets the tuning parameters
        /// </summary>
        public IDimmerProcessorTuningParams TuningParams { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>Dimmer output</summary>
        /// <param name="minimumPercent">MinimumPercent percentage 0...100</param>
        /// <param name="maximumPercent">MaximumPercent percentage 0...100</param>
        /// <param name="ambientLightLevel">0...65535</param>
        /// <param name="rangeScale">0..3</param>
        /// <param name="brightnessLevel">0...255</param>
        /// <returns>The <see cref="IDimmerProcessorOutput"/>.</returns>
        public IDimmerProcessorOutput CalculateDimmerOutput(
            byte minimumPercent, 
            byte maximumPercent, 
            ushort ambientLightLevel, 
            byte rangeScale, 
            byte brightnessLevel)
        {
            if (TuningParams.DimmerProcessorDebugMode)
            {
                byte brightness = 0;

                if (LastDimmerOutput == null)
                {
                    LastDimmerOutput = new DimmerProcessorOutput();
                    LastDimmerOutput.RangeScale = 0x00;
                    LastDimmerOutput.IntervalDelay = 25;
                    LastDimmerOutput.BrightnessSetting = new byte[8];

                    for (int i = 0; i < 8; i++)
                    {
                        LastDimmerOutput.BrightnessSetting[i] = brightness;
                        brightness += 5;
                    }

                    return LastDimmerOutput;
                }

                brightness = LastDimmerOutput.BrightnessSetting.LastOrDefault();

                for (int i = 0; i < 8; i++)
                {
                    if (brightness >= 255)
                    {
                        brightness = 0;
                        LastDimmerOutput.BrightnessSetting[i] = brightness;
                        LastDimmerOutput.RangeScale++;

                        if (LastDimmerOutput.RangeScale > 0x03)
                        {
                            LastDimmerOutput.RangeScale = 0;
                        }
                    }
                    else
                    {
                        LastDimmerOutput.BrightnessSetting[i] = brightness;
                        brightness += 5;
                    }
                }

                return LastDimmerOutput;
            }

            if (minimumPercent > 100)
            {
                throw new ArgumentOutOfRangeException("minimumPercent");
            }

            if (maximumPercent > 100)
            {
                throw new ArgumentOutOfRangeException("maximumPercent");
            }

            if (rangeScale > 0x03)
            {
                throw new ArgumentOutOfRangeException("rangeScale");
            }

            // Calculate the light levels in LUX and the new range scale
            float ambientLightLux = 0.0f;
            byte newRangeScale = rangeScale;

            if (rangeScale == 0x00)
            {
                ambientLightLux = (ambientLightLevel / 65535.0f) * 1000.0f + 0.015f;

                if (ambientLightLevel >= TuningParams.Range1Upper)
                {
                    newRangeScale = 0x01;
                }
            }
            else if (rangeScale == 0x01)
            {
                ambientLightLux = (ambientLightLevel / 65535.0f) * 4000.0f + 0.06f;

                if (ambientLightLevel <= TuningParams.Range2Lower)
                {
                    newRangeScale = 0x00;
                }
                else if (ambientLightLevel >= TuningParams.Range2Upper)
                {
                    newRangeScale = 0x02;
                }
            }
            else if (rangeScale == 0x02)
            {
                ambientLightLux = (ambientLightLevel / 65535.0f) * 16000.0f + 0.24f;

                if (ambientLightLevel <= TuningParams.Range3Lower)
                {
                    newRangeScale = 0x01;
                }
                else if (ambientLightLevel >= TuningParams.Range3Upper)
                {
                    newRangeScale = 0x03;
                }
            }
            else if (rangeScale == 0x03)
            {
                ambientLightLux = (ambientLightLevel / 65535.0f) * 64000.0f + 0.96f;

                if (ambientLightLevel <= TuningParams.Range4Lower)
                {
                    newRangeScale = 0x02;
                }
            }

            // Save last input for reference
            lock (LastDimmerInput)
            {
                LastDimmerInput.MinimumPercent = minimumPercent;
                LastDimmerInput.MaximumPercent = maximumPercent;
                LastDimmerInput.AmbientLightLevel = ambientLightLevel;
                LastDimmerInput.AmbientLightLux = ambientLightLux;
                LastDimmerInput.RangeScale = rangeScale;
                LastDimmerInput.BrightnessLevel = brightnessLevel;
            }

            // TODO: Calcuate the output, find out how the sensor value is scaled with LUX (is it linear on the same scale???)
            IDimmerProcessorOutput resultOutput = new DimmerProcessorOutput();
            resultOutput.RangeScale = newRangeScale;
            byte newBrightnessLevel = (byte)(Math.Min(ambientLightLux / 1200.0f, 1.0f) * 0xFF);

            if (TuningParams.InvertBrightness)
            {
                newBrightnessLevel = (byte)(0xFF - newBrightnessLevel);
            }

            newBrightnessLevel = Math.Min(newBrightnessLevel, (byte)(maximumPercent / 100.0f * 0xFF));
            newBrightnessLevel = Math.Max(newBrightnessLevel, (byte)(minimumPercent / 100.0f * 0xFF));

            // Same as last time
            if (newBrightnessLevel == brightnessLevel)
            {
                resultOutput.BrightnessSetting = new byte[] { };
                resultOutput.IntervalDelay = 0;
                LastDimmerOutput = resultOutput;
                return resultOutput;
            }

            // TODO: Refine this to be based on heuristics
            int steps = TuningParams.Steps;
            var stepValue = (newBrightnessLevel - brightnessLevel) / steps;

            resultOutput.BrightnessSetting = new byte[steps];

            for (int i = 0; i < steps; i++)
            {
                resultOutput.BrightnessSetting[i] = (byte)(brightnessLevel + stepValue * (i + 1));
            }

            resultOutput.BrightnessSetting[steps - 1] = newBrightnessLevel;
            resultOutput.IntervalDelay = TuningParams.IntervalDelay;

            LastDimmerOutput = resultOutput;
            return resultOutput;
        }

        #endregion
    }
}