namespace Luminator.AudioSwitch.WpfSerialPortTester.ViewModels
{
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    public class SetVolumeViewModel : BaseViewModel
    {
        public SetVolumeViewModel(int interiorVolume = 50, int exteriorVolume = 50)
        {
            this.InteriorVolume = interiorVolume;
            this.ExteriorVolume = exteriorVolume;
        }

        public int InteriorVolume { get; set; }

        public int ExteriorVolume { get; set; }
    }
}
