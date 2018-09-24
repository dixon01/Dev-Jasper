// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsistencyCheckDialog.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for ConsistencyCheckDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Interaction
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.DataViewModels.Consistency;
    using Gorba.Center.Media.Core.Interaction;

    /// <summary>
    /// Interaction logic for ConsistencyCheckDialog.xaml
    /// </summary>
    public partial class ConsistencyCheckDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsistencyCheckDialog"/> class.
        /// </summary>
        public ConsistencyCheckDialog()
        {
            this.InitializeComponent();
            this.Loaded += this.PopupLoaded;
        }

        private void PopupLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is ConsistencyCheckPrompt prompt))
            {
                return;
            }

            this.Refresh(prompt.ConsistencyMessages, prompt.CompatibilityMessages);
        }

        private void Refresh(
            IEnumerable<ConsistencyMessageDataViewModel> messages,
            IEnumerable<ConsistencyMessageDataViewModel> compatibilityMessages)
        {
            var collection = this.MainGrid.TryFindResource("MessagesCollection") as CollectionViewSource;
            var compatibility
                = this.MainGrid.TryFindResource("CompatibilityMessagesCollection") as CollectionViewSource;
            if (collection == null || compatibility == null)
            {
                return;
            }

            if (messages != null)
            {
                collection.Source = messages;
                collection.View.Refresh();
            }

            compatibility.Source = compatibilityMessages ?? new List<ConsistencyMessageDataViewModel>();

            compatibility.View.Refresh();
        }

        private void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
