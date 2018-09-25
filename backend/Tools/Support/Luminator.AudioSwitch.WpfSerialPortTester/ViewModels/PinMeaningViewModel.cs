using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Luminator.AudioSwitch.WpfSerialPortTester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    public class PinMeaningViewModel : ViewModelBase
    {
        public PinMeaning PinMeaningValues { get; set; }
        public RelayCommand<Window> SaveButton { get; set; }

        public String PinSenseString { get; set; }
        

        // example pinsense string: 01010101000000010101010000000000
        public PinMeaningViewModel()
        {
            SaveButton = new RelayCommand<Window>(SaveButtonClick);
        }

        public void InitializeData()
        {
            PinMeaningValues = new PinMeaning();
            PinMeaningValues.Options = new List<string>() { "00", "01" };
            PinMeaningValues.Door1 = PinSenseString.Substring(0, 2);
            PinMeaningValues.Door2 = PinSenseString.Substring(2, 2);
            PinMeaningValues.StopRequest = PinSenseString.Substring(4, 2);
            PinMeaningValues.ADAStopRequest = PinSenseString.Substring(6, 2);
            PinMeaningValues.PushToTalk = PinSenseString.Substring(8, 2);
            PinMeaningValues.InteriorSelect = PinSenseString.Substring(10, 2);
            PinMeaningValues.ExteriorSelect = PinSenseString.Substring(12, 2);
            PinMeaningValues.Odometer = PinSenseString.Substring(14, 2);
            PinMeaningValues.Reverse = PinSenseString.Substring(16, 2);
            PinMeaningValues.InteriorSpeakerPriority = PinSenseString.Substring(18, 2);
            PinMeaningValues.ExteriorSpeakerPriority = PinSenseString.Substring(20, 2);
            PinMeaningValues.RadioSpeakerPriority = PinSenseString.Substring(22, 2);

            RaisePropertyChanged(nameof(PinMeaningValues));
            RaisePropertyChanged(nameof(PinMeaningValues.Options));
        }
        
        public void SaveButtonClick(Window w)
        {
            // Save Pin Sense value
            StringBuilder newString = new StringBuilder();
            newString.Append(PinMeaningValues.Door1);
            newString.Append(PinMeaningValues.Door2);
            newString.Append(PinMeaningValues.StopRequest);
            newString.Append(PinMeaningValues.ADAStopRequest);
            newString.Append(PinMeaningValues.PushToTalk);
            newString.Append(PinMeaningValues.InteriorSelect);
            newString.Append(PinMeaningValues.ExteriorSelect);
            newString.Append(PinMeaningValues.Odometer);
            newString.Append(PinMeaningValues.Reverse);
            newString.Append(PinMeaningValues.InteriorSpeakerPriority);
            newString.Append(PinMeaningValues.ExteriorSpeakerPriority);
            newString.Append(PinMeaningValues.RadioSpeakerPriority);
            PinSenseString = newString.ToString();

            if (w != null)
                w.Close();
        }
    }
}
