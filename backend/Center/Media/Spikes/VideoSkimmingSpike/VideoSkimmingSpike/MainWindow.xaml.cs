using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoSkimmingSpike
{
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBaseOnClick(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.ShowDialog(this);
            var filename = openDialog.FileName;
            this.SkimmingElement.OpenMedia(filename);
            this.VideoFileName.Text = filename;
        }
    }
}
