// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteLogControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for RemoteLogControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views.Controls
{
    using System.Windows.Controls;
    using System.Windows.Media;

    using Gorba.Center.Diag.Core.ViewModels.Log;

    using Telerik.Windows.Controls.GridView;

    /// <summary>
    /// Interaction logic for RemoteLogControl.xaml
    /// </summary>
    public partial class RemoteLogControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLogControl"/> class.
        /// </summary>
        public RemoteLogControl()
        {
            this.InitializeComponent();
        }

        private void LogGridViewOnRowLoaded(object sender, RowLoadedEventArgs e)
        {
            var logEntry = e.DataElement as LogEntryViewModel;
            if (logEntry == null)
            {
                return;
            }

            var row = e.Row as GridViewRow;
            if (row != null)
            {
                row.IsExpandable = !string.IsNullOrEmpty(logEntry.ExceptionInfo);
            }

            switch (logEntry.Level)
            {
                case LogLevel.Trace:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogTraceColorBrush");
                    break;
                case LogLevel.Debug:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogDebugColorBrush");
                    break;
                case LogLevel.Info:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogInfoColorBrush");
                    break;
                case LogLevel.Warn:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogWarnColorBrush");
                    break;
                case LogLevel.Error:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogErrorColorBrush");
                    break;
                case LogLevel.Fatal:
                    e.Row.Foreground = (Brush)this.FindResource("RemoteLogFatalColorBrush");
                    break;
            }
        }
    }
}
