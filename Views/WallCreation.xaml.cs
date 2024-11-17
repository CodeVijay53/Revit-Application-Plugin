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
    /// 
    /// </summary>
    public partial class WallCreation : Window
    {
        public string SelectedLevel { get; private set; }
        public double HeightValue { get; private set; }
        public double ThicknessValue { get; private set; }

        public WallCreation()
        {
            InitializeComponent();
            // Populate Level Dropdown (dummy for now)
            LevelComboBox.Items.Add("Level 1");
            LevelComboBox.Items.Add("Level 2");
        }

        private void OnCreateWallClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedLevel = LevelComboBox.SelectedItem?.ToString();
                HeightValue = double.Parse(HeightTextBox.Text);
                ThicknessValue = double.Parse(ThicknessTextBox.Text);

                if (string.IsNullOrEmpty(SelectedLevel))
                    throw new Exception("Level is required.");

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
