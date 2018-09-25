
namespace Luminator.Motion.WpfIntegratedTester.Main.ViewModels
{
    using Luminator.UIFramework.Common.MVVM.ViewModelHelpers;

    public class VersionViewModel : BaseViewModel
    {
        #region Fields

        private string _displayName;

        private string _displayVersion;

        private string _description;

        #endregion

        #region Properties

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName == value) return;
                _displayName = value;
                this.RaisePropertyChanged(() => this.DisplayName);
            }
        }

        public string DisplayVersion
        {
            get { return _displayName; }
            set
            {
                if (_displayVersion == value) return;
                _displayVersion = value;
                this.RaisePropertyChanged(() => this.DisplayVersion);
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value) return;
                _description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        #endregion
    }
}
