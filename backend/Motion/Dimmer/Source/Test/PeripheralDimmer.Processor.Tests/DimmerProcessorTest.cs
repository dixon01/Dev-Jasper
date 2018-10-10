// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerProcessorTest.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Processor.Tests
{
    using Luminator.PeripheralDimmer.Processor.Interfaces;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>The dimmer processor test.</summary>
    [TestClass]
    public class DimmerProcessorTest
    {
        #region Fields

        private readonly DimmerProcessor dimmerProcessor;

        private readonly byte maximumPercentage = 90;

        private readonly byte minimumPercentage = 10;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DimmerProcessorTest"/> class.</summary>
        public DimmerProcessorTest()
        {
            dimmerProcessor = new DimmerProcessor();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The dimmer processor_ calculate dimmer output_ range 1 lower.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range1Lower()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0;
            byte inputLightSensorRangeScale = 0x00;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x00);
            Assert.AreEqual(resultBrightness, (byte)(minimumPercentage / 100.0f * 0xFF));
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 1 upper.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range1Upper()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0xcccb;
            byte inputLightSensorRangeScale = 0x00;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x00);
            Assert.AreEqual(resultBrightness, 169);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0xcccc;
            inputLightSensorRangeScale = 0x00;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 170);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0xffff;
            inputLightSensorRangeScale = 0x00;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 212);
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 2 lower.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range2Lower()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0x4ccd;
            byte inputLightSensorRangeScale = 0x01;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x4ccc;
            inputLightSensorRangeScale = 0x01;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x00);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x00;
            inputLightSensorRangeScale = 0x01;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x00);
            Assert.AreEqual(resultBrightness, 25);
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 2 upper.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range2Upper()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0xcccb;
            byte inputLightSensorRangeScale = 0x01;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0xcccc;
            inputLightSensorRangeScale = 0x01;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x02);
            Assert.AreEqual(resultBrightness, 229);
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 3 lower.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range3Lower()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0x4ccd;
            byte inputLightSensorRangeScale = 0x02;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x02);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x4ccc;
            inputLightSensorRangeScale = 0x02;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x00;
            inputLightSensorRangeScale = 0x02;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x01);
            Assert.AreEqual(resultBrightness, 25);
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 3 upper.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range3Upper()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0xcccb;
            byte inputLightSensorRangeScale = 0x02;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x02);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0xcccc;
            inputLightSensorRangeScale = 0x02;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x03);
            Assert.AreEqual(resultBrightness, 229);
        }

        /// <summary>The dimmer processor_ calculate dimmer output_ range 4 lower.</summary>
        [TestMethod]
        public void DimmerProcessor_CalculateDimmerOutput_Range4Lower()
        {
            Assert.IsNotNull(dimmerProcessor);

            IDimmerProcessorOutput output = null;

            // Default params are as follows:
            // Range2Lower = 0x4ccc;
            // Range3Lower = 0x4ccc;
            // Range4Lower = 0x4ccc;

            // Range1Upper = 0xcccc;
            // Range2Upper = 0xcccc;
            // Range3Upper = 0xcccc;
            IDimmerProcessorTuningParams defaultParams = new DimmerProcessorTuningParams();
            dimmerProcessor.TuningParams = defaultParams;

            byte inputMinimumPercentage = minimumPercentage;
            byte inputMaximumPercentage = maximumPercentage;
            ushort inputAmbientLightLevel = 0x4ccd;
            byte inputLightSensorRangeScale = 0x03;
            byte inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            byte resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x03);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x4ccc;
            inputLightSensorRangeScale = 0x03;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x02);
            Assert.AreEqual(resultBrightness, 229);

            // ------------------------------------------------------------------------------
            inputMinimumPercentage = minimumPercentage;
            inputMaximumPercentage = maximumPercentage;
            inputAmbientLightLevel = 0x00;
            inputLightSensorRangeScale = 0x03;
            inputBrightnessLevel = 0xFF;

            // <param name="minimumPercentage"></param>
            // <param name="maximumPercentage"></param>
            // <param name="ambientLightLevel"></param>
            // <param name="lightSensorRangeScale"></param>
            // <param name="brightnessLevel"></param>
            output = dimmerProcessor.CalculateDimmerOutput(
                inputMinimumPercentage, 
                inputMaximumPercentage, 
                inputAmbientLightLevel, 
                inputLightSensorRangeScale, 
                inputBrightnessLevel);

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.BrightnessSetting);
            Assert.IsTrue(output.BrightnessSetting.Length > 0);

            resultBrightness = output.BrightnessSetting[output.BrightnessSetting.Length - 1];

            Assert.AreEqual(output.RangeScale, 0x02);
            Assert.AreEqual(resultBrightness, 25);
        }

        #endregion
    }
}