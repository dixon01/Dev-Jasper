// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationDisplay.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   The notification display.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///     The notification display.
    /// </summary>
    public partial class NotificationDisplay : UserControl, INotifyPropertyChanged
    {
        
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NotificationDisplay" /> class.
        /// </summary>
        public NotificationDisplay()
        {
            this.InitializeComponent();

            this.DataContext = this;

            this.MockNotifications();
        }

        static NotificationDisplay()
        {
            NotificationDisplay.NotificationsItemsSourceProperty =
                                       DependencyProperty.Register("NotificationsItemsSource",
                                       typeof(IEnumerable), typeof(NotificationDisplay));
        }
    

        #endregion

        #region Public Events

        /// <summary>
        ///     The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        


        #endregion

        #region Methods

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property that has a new value.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     The mock notifications.
        /// </summary>
        private void MockNotifications()
        {
            var NotificationsItems = new ObservableCollection<string>();
            for (int i = 0; i < 20; i++)
            {
                string item = "Item " + i;
                NotificationsItems.Add(item);
            }
            
        }

        #endregion

        public static readonly DependencyProperty NotificationsItemsSourceProperty;
        public IEnumerable NotificationsItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(NotificationsItemsSourceProperty);
            }
            set
            {
                this.SetValue(NotificationsItemsSourceProperty, value);
            }
        }
    }
}