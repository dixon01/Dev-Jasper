// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="AudioConfigViewModel.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Natraj Bontha</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;
    using Luminator.AudioSwitch.WpfSerialPortTester.Views;
    using System.IO;

    public class AudioConfigViewModel : BaseViewModel
    {
        #region Fields

        private int audioStatusDelay;

        private int interiorDefaultVolume;

        private int exteriorDefaultVolume;

        private string exteriorGain;

        private int exteriorMaxVolume;

        private int exteriorMinVolume;

        private string interiorGain;

        private int interiorMaxVolume;

        private int interiorMinVolume;

        private int lineInEnable;

        private int noiseLevel;

        private string pinMeaning;

        private string pinSense;

        private string priorityTable;

        private int pushToTalkLockoutTime;

        private int pushToTalkTimeout;

        private int interiorMaxAllowedVolume;

        private int exteriorMaxAllowedVolume;
        private string fileName;

        #endregion

        #region Public Properties

        public int AudioStatusDelay
        {
            get
            {
                return this.audioStatusDelay;
            }
            set
            {
                this.audioStatusDelay = value;
                this.RaisePropertyChanged(() => this.AudioStatusDelay);
            }
        }

        public int InteriorDefaultVolume
        {
            get
            {
                return this.interiorDefaultVolume;
            }
            set
            {
                this.interiorDefaultVolume = value;
                this.RaisePropertyChanged(() => this.InteriorDefaultVolume);
            }
        }


        public int ExteriorDefaultVolume
        {
            get
            {
                return this.exteriorDefaultVolume;
            }
            set
            {
                this.exteriorDefaultVolume = value;
                this.RaisePropertyChanged(() => this.ExteriorDefaultVolume);
            }
        }

        public string ExteriorGain
        {
            get
            {
                return this.exteriorGain;
            }
            set
            {
                this.exteriorGain = value;
                this.RaisePropertyChanged(() => this.ExteriorGain);
            }
        }

        public int ExteriorMaxVolume
        {
            get
            {
                return this.exteriorMaxVolume;
            }
            set
            {
                this.exteriorMaxVolume = value;
                this.RaisePropertyChanged(() => this.ExteriorMaxVolume);
            }
        }

        public int InteriorMaxAllowedVolume
        {
            get
            {
                return this.interiorMaxAllowedVolume;
            }

            set
            {
                this.interiorMaxAllowedVolume = value;
                this.RaisePropertyChanged(() => this.InteriorMaxAllowedVolume);
               
            }
        }
        public int ExteriorMaxAllowedVolume
        {
            get
            {
                return this.exteriorMaxAllowedVolume;
            }

            set
            {
                this.exteriorMaxAllowedVolume = value;
                this.RaisePropertyChanged(() => this.ExteriorMaxAllowedVolume);
            }
                
                }

        public int ExteriorMinVolume
        {
            get
            {
                return this.exteriorMinVolume;
            }
            set
            {
                this.exteriorMinVolume = value;
                this.RaisePropertyChanged(() => this.ExteriorMinVolume);
            }
        }

        public string InteriorGain
        {
            get
            {
                return this.interiorGain;
            }
            set
            {
                this.interiorGain = value;
                this.RaisePropertyChanged(() => this.InteriorGain);
            }
        }

        public int InteriorMaxVolume
        {
            get
            {
                return this.interiorMaxVolume;
            }
            set
            {
                this.interiorMaxVolume = value;
                this.RaisePropertyChanged(() => this.InteriorMaxVolume);
            }
        }

        public int InteriorMinVolume
        {
            get
            {
                return this.interiorMinVolume;
            }
            set
            {
                this.interiorMinVolume = value;
                this.RaisePropertyChanged(() => this.InteriorMinVolume);
            }
        }

        public int LineInEnable
        {
            get
            {
                return this.lineInEnable;
            }
            set
            {
                this.lineInEnable = value;
                this.RaisePropertyChanged(() => this.LineInEnable);
            }
        }

        public int NoiseLevel
        {
            get
            {
                return this.noiseLevel;
            }
            set
            {
                this.noiseLevel = value;
                this.RaisePropertyChanged(() => this.NoiseLevel);
            }
        }

        public string PinMeaning
        {
            get
            {
                return this.pinMeaning;
            }
            set
            {
                this.pinMeaning = value;
                this.RaisePropertyChanged(() => this.PinMeaning);
            }
        }

        public string PinSense
        {
            get
            {
                return this.pinSense;
            }
            set
            {
                this.pinSense = value;
                this.RaisePropertyChanged(() => this.PinSense);
            }
        }

        public string PriorityTable
        {
            get
            {
                return this.priorityTable;
            }
            set
            {
                this.priorityTable = value;
                this.RaisePropertyChanged(() => this.PriorityTable);
            }
        }

        public int PushToTalkLockoutTime
        {
            get
            {
                return this.pushToTalkLockoutTime;
            }
            set
            {
                this.pushToTalkLockoutTime = value;
                this.RaisePropertyChanged(() => this.PushToTalkLockoutTime);
            }
        }

        public int PushToTalkTimeout
        {
            get
            {
                return this.pushToTalkTimeout;
            }
            set
            {
                this.pushToTalkTimeout = value;
                this.RaisePropertyChanged(() => this.PushToTalkTimeout);
            }
        }

        #endregion

        #region Public Methods and Operators

        public void SetFromPeripheralAudioConfig(PeripheralAudioConfig peripheralAudioConfig)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PeripheralAudioConfig));
            PeripheralAudioConfig audioConfigViewModel;

            using (var reader = File.OpenText(@"D:\Progs\Protran\PeripheralAudioConfig.xml"))
            {
                audioConfigViewModel = (PeripheralAudioConfig)serializer.Deserialize(reader);

            }
            
            foreach (byte value in audioConfigViewModel.PinMeaning)
            {
                this.PinMeaning += value.ToString("X2");
            }

            this.PinSense = this.GetValueStringFromByteArray(audioConfigViewModel.PinSense);
            foreach (byte value in audioConfigViewModel.InteriorGain)
            {
                this.InteriorGain += value.ToString("X2");
            }
            
            foreach (byte value in audioConfigViewModel.ExteriorGain)
            {
                this.ExteriorGain += value.ToString("X2");
            }

            this.PriorityTable = this.GetValueStringFromByteArray(audioConfigViewModel.PriorityTable);
            this.NoiseLevel = audioConfigViewModel.NoiseLevel;
            this.InteriorDefaultVolume = audioConfigViewModel.InteriorDefaultVolume;
            this.ExteriorDefaultVolume = audioConfigViewModel.ExteriorDefaultVolume;
            this.InteriorMinVolume = audioConfigViewModel.InteriorMinVolume;
            this.ExteriorMinVolume = audioConfigViewModel.ExteriorMinVolume;        
            this.InteriorMaxVolume = audioConfigViewModel.InteriorMaxVolume;
            this.ExteriorMaxVolume = audioConfigViewModel.ExteriorMaxVolume;
            this.InteriorMaxAllowedVolume = audioConfigViewModel.InteriorMaxAllowedVolume;
            this.ExteriorMaxAllowedVolume = audioConfigViewModel.ExteriorMaxAllowedVolume;
            this.PushToTalkTimeout = audioConfigViewModel.PushToTalkTimeout;
            this.PushToTalkLockoutTime = audioConfigViewModel.PushToTalkLockoutTime;
            this.LineInEnable = audioConfigViewModel.LineInEnable;
            this.AudioStatusDelay = audioConfigViewModel.AudioStatusDelay;
        }

        public void WriteToPeripheralAudioConfig(AudioConfigViewModel audioConfigViewModel)
        {
            AudioConfigViewModel model = new AudioConfigViewModel();
            model.NoiseLevel = this.NoiseLevel;


        }
        #endregion

        #region Methods

        private string GetDescriptionStringFromByteArray(byte[] arrayBytes)
        {
            var returnResult = string.Empty;
            foreach (var pin in arrayBytes)
            {
                var temp = Convert.ToInt32(pin).ToString();
                PeripheralGpioType result;

                Enum.TryParse(temp, true, out result);
                returnResult += result + ", ";
            }
            return returnResult;
        }

        private string GetValueStringFromByteArray(byte[] arrayBytes)
        {
            var returnResult = string.Empty;
            foreach (var pin in arrayBytes)
            {
                var temp = Convert.ToInt32(pin).ToString();
                PeripheralGpioType result;
                if (temp.Length == 1)
                {
                    temp = "0" + temp;
                }
                Enum.TryParse(temp, true, out result);
                returnResult += temp;
            }
            return returnResult;
        }


        #endregion
    }
}

//var audioconfig = new PeripheralAudioConfig
//{
//    PinMeaning = Encoding.ASCII.GetBytes(this.AudioConfigViewModel.PinMeaning),
//    PinSense = Encoding.ASCII.GetBytes(this.AudioConfigViewModel.PinSense),
//    InteriorGain = Encoding.ASCII.GetBytes(this.AudioConfigViewModel.InteriorGain),
//    ExteriorGain = Encoding.ASCII.GetBytes(this.AudioConfigViewModel.ExteriorGain),
//    PriorityTable = Encoding.ASCII.GetBytes(this.AudioConfigViewModel.PriorityTable),
//    NoiseLevel = (byte)this.AudioConfigViewModel.NoiseLevel,
//    DefaultVolume = (byte)this.AudioConfigViewModel.DefaultVolume,
//    InteriorMinVolume = (byte)this.AudioConfigViewModel.InteriorMinVolume,
//    ExteriorMinVolume = (byte)this.AudioConfigViewModel.ExteriorMinVolume,
//    InteriorMaxVolume = (byte)this.AudioConfigViewModel.InteriorMaxVolume,
//    ExteriorMaxVolume = (byte)this.AudioConfigViewModel.ExteriorMaxVolume,
//    PushToTalkTimeout = (byte)this.AudioConfigViewModel.PushToTalkTimeout,
//    PushToTalkLockoutTime = (byte)this.AudioConfigViewModel.PushToTalkLockoutTime,
//    LineInEnable = (byte)this.AudioConfigViewModel.LineInEnable,
//    AudioStatusDelay = (byte)this.AudioConfigViewModel.AudioStatusDelay
//};