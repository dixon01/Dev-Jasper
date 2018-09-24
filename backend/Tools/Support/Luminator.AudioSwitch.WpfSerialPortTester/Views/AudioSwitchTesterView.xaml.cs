namespace Luminator.AudioSwitch.WpfSerialPortTester.Views
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Ports;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.Win32;

    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.AudioSwitch.WpfSerialPortTester.ViewModels;
   
    /// <summary>
    /// Interaction logic for AudioSwitchTesterView.xaml
    /// </summary>
    public partial class AudioSwitchTesterView : UserControl
    {
        private object audioConfigViewModel;

        public AudioSwitchTesterView()
        {
            InitializeComponent();
        }

        
        public void buttonSaveFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            Button clickedbutton = sender as Button;
            

            MainWindowViewModel viewModel = ((Button)e.OriginalSource).DataContext as MainWindowViewModel;

            var audioConfigViewModel = viewModel.AudioConfigViewModel;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == false)
            {
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(AudioConfigViewModel));

            using (var writer = new StreamWriter(saveFileDialog.FileName))
            {
                serializer.Serialize(writer, audioConfigViewModel);
            }

        }

        private void buttonEditFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PinMeaningView pinMeaningView = new PinMeaningView();
                PinMeaningViewModel pinMeaningViewModel = (PinMeaningViewModel)pinMeaningView.DataContext;
                pinMeaningViewModel.PinSenseString = this.PinSense.Text;
                pinMeaningViewModel.InitializeData();

                pinMeaningView.ShowDialog();
                this.PinSense.Text = pinMeaningViewModel.PinSenseString;
            } catch (Exception)
            {
                // error
            }

        }
    }
}
