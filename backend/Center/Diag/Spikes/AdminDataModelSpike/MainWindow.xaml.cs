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

namespace AdminDataModelSpike
{
    using AdminDataModelSpike.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AdminRootViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            this.viewModel = new AdminRootViewModel();

            this.DataContext = this.viewModel;
        }

        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.viewModel.SelectedNode = e.NewValue as TabableViewModelBase;
        }
    }
}
