using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FinalApplication.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinalApplication.Views
{
    public partial class ChangeViewDetailLevel : Window
    {
        public ViewDetailLevel SelectedDetailLevel { get; private set; }

        public ChangeViewDetailLevel()
        {
            InitializeComponent();

            // Attach button click events
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;

            // Default selection
            DetailLevelComboBox.SelectedIndex = 0;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (DetailLevelComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedDetailLevel = (ViewDetailLevel)int.Parse(selectedItem.Tag.ToString());
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a detail level.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
