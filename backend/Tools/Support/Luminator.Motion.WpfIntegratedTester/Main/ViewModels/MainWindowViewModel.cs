
using Luminator.Motion.WpfIntegratedTester.AudioTTS.ViewModels;

namespace Luminator.Motion.WpfIntegratedTester.Main.ViewModels
{
    using System;

    using NLog;

    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;
    using Luminator.Motion.WpfIntegratedTester.Dimmer.ViewModels;

    public class MainWindowViewModel : BaseViewModel, IDisposable
    {
        #region Static Fields

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            AudioTesterViewModel = new AudioSwitch.WpfSerialPortTester.ViewModels.MainWindowViewModel();
            AudioTextToSpeechTesterViewModel = new AudioTextToSpeechTesterViewModel();
            DimmerTesterViewModel = new DimmerTesterViewModel();
        }
        
        #endregion

        #region Public Properties (Alphabetically Sorted)

        public AudioSwitch.WpfSerialPortTester.ViewModels.MainWindowViewModel AudioTesterViewModel { get; set; }

        public AudioTextToSpeechTesterViewModel AudioTextToSpeechTesterViewModel { get; set; }

        public DimmerTesterViewModel DimmerTesterViewModel { get; set; }

        #endregion

        #region Public Methods and Operators

        #endregion

        #region Methods

        public void Dispose()
        {
        }

        #endregion
    }
}