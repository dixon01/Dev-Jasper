namespace Luminator.Multicast.Receive.Client.Wpf.ViewModels
{
    using System.Collections.ObjectModel;

    public class MessageReceivedViewModel
    {
        #region Public Properties

        public string Message { get; set; }

        #endregion

        #region Properties

        private ObservableCollection<string> Messages { get; set; }

        #endregion
    }
}