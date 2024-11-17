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
    /// <summary>
    /// Interaction logic for WallHeight.xaml
    /// </summary>
    public partial class FloorCreation : Window
    {
       public double FloorWidth { get; private set; }
        public double FloorHeight { get; private set; }
        public string SelectedLevel { get; private set; }
        public string SelectedFloorType { get; private set; }

        public FloorCreation()
        {
            InitializeComponent();

            // Populate levels and floor types (dummy data for now)
            LevelComboBox.Items.Add("Level 1");
            LevelComboBox.Items.Add("Level 2");
            FloorTypeComboBox.Items.Add("Generic - 12\"");

            // In production, replace with dynamic data collection from Revit:
            // PopulateLevels();
            // PopulateFloorTypes();
        }

        private void OnCreateFloorClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FloorWidth = double.Parse(WidthTextBox.Text);
                FloorHeight = double.Parse(HeightTextBox.Text);
                SelectedLevel = LevelComboBox.SelectedItem?.ToString();
                SelectedFloorType = FloorTypeComboBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(SelectedLevel) || string.IsNullOrEmpty(SelectedFloorType))
                {
                    throw new Exception("All fields are required.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
