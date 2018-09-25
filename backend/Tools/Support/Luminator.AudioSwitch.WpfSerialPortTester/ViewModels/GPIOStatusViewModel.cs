namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    public class GpioStatusViewModel : BaseViewModel
    {
        #region Fields

        private bool adaStopRequest;

        private bool door1Active;

        private bool door2Active;

        private bool exteriorSpeakersActive;

        private bool interiorSpeakersActive;

        private int odometer;

        private bool pushToTalk;

        private bool reverse;

        private bool stopRequest;

        private bool interiorSpeakerMuted;

        private bool exteriorSpeakerMuted;

        private bool radioSpeakerMuted;

        private bool interorSpeakerNonMuting;

        private bool exteriorSpeakerNonMuting;

        private bool radioSpeakerNonMuting;

        public bool InteriorSpeakerMuted
        {
            get
            {
                return this.interiorSpeakerMuted;
            }
            set
            {
                this.interiorSpeakerMuted = value;
                this.RaisePropertyChanged(() => this.InteriorSpeakerMuted);
            }
        }

        public bool ExteriorSpeakerMuted
        {
            get
            {
                return this.exteriorSpeakerMuted;
            }
            set
            {
                this.exteriorSpeakerMuted = value;
                this.RaisePropertyChanged(() => this.ExteriorSpeakerMuted);
            }
        }

        public bool RadioSpeakerMuted
        {
            get
            {
                return this.radioSpeakerMuted;
            }
            set
            {
                this.radioSpeakerMuted = value;
                this.RaisePropertyChanged(() => this.RadioSpeakerMuted);
            }
        }

        public bool InterorSpeakerNonMuting
        {
            get
            {
                return this.interorSpeakerNonMuting;
            }
            set
            {
                this.interorSpeakerNonMuting = value;
                this.RaisePropertyChanged(() => this.InterorSpeakerNonMuting);
            }
        }

        public bool ExteriorSpeakerNonMuting
        {
            get
            {
                return this.exteriorSpeakerNonMuting;
            }
            set
            {
                this.exteriorSpeakerNonMuting = value;
                this.RaisePropertyChanged(() => this.ExteriorSpeakerNonMuting);
            }
        }

        public bool RadioSpeakerNonMuting
        {
            get
            {
                return this.radioSpeakerNonMuting;
            }
            set
            {
                this.radioSpeakerNonMuting = value;
                this.RaisePropertyChanged(() => this.RadioSpeakerNonMuting);
            }
        }

        #endregion

        #region Constructors and Destructors

        public GpioStatusViewModel()
        {
        }

        #endregion

        #region Public Properties

        public bool AdaStopRequest
        {
            get
            {
                return this.adaStopRequest;
            }
            set
            {
                this.adaStopRequest = value;
                this.RaisePropertyChanged(() => this.AdaStopRequest);
            }
        }

        public bool Door1Active
        {
            get
            {
                return this.door1Active;
            }
            set
            {
                this.door1Active = value;

                // Call OnPropertyChanged whenever the property is updated
                this.RaisePropertyChanged(() => this.Door1Active);
            }
        }

        public bool Door2Active
        {
            get
            {
                return this.door2Active;
            }
            set
            {
                this.door2Active = value;

                // Call OnPropertyChanged whenever the property is updated
                this.RaisePropertyChanged(() => this.Door2Active);
            }
        }

        public bool ExteriorSpeakersActive
        {
            get
            {
                return this.exteriorSpeakersActive;
            }
            set
            {
                this.exteriorSpeakersActive = value;

                // Call OnPropertyChanged whenever the property is updated
                this.RaisePropertyChanged(() => this.ExteriorSpeakersActive);
            }
        }

        public bool InteriorSpeakersActive
        {
            get
            {
                return this.interiorSpeakersActive;
            }
            set
            {
                this.interiorSpeakersActive = value;

                // Call OnPropertyChanged whenever the property is updated
                this.RaisePropertyChanged(() => this.InteriorSpeakersActive);
            }
        }

        public int Odometer
        {
            get
            {
                return this.odometer;
            }
            set
            {
                this.odometer = value;
                this.RaisePropertyChanged(() => this.Odometer);
            }
        }

        public bool PushToTalk
        {
            get
            {
                return this.pushToTalk;
            }
            set
            {
                this.pushToTalk = value;
                this.RaisePropertyChanged(() => this.PushToTalk);
            }
        }

        public bool Reverse
        {
            get
            {
                return this.reverse;
            }
            set
            {
                this.reverse = value;
                this.RaisePropertyChanged(() => this.Reverse);
            }
        }

        public bool StopRequest
        {
            get
            {
                return this.stopRequest;
            }
            set
            {
                this.stopRequest = value;
                this.RaisePropertyChanged(() => this.StopRequest);
            }
        }

        #endregion
    }
}