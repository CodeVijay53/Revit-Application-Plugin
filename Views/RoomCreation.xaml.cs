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
    public partial class RoomCreation : Window
    {
        public string RoomName { get; private set; }
        public string RoomNumber { get; private set; }
        public double RoomWidth { get; private set; }
        public double RoomHeight { get; private set; }
        public string RoomType { get; private set; }
        public string SelectedLevel { get; private set; }

        public RoomCreation()
        {
            InitializeComponent();

            // Populate Level dropdown (dummy data for now; replace with dynamic Revit data)
            LevelComboBox.Items.Add("Level 1");
            LevelComboBox.Items.Add("Level 2");
        }

        private void OnCreateRoomClick(object sender, RoutedEventArgs e)
        {
            try
            {
                RoomName = RoomNameTextBox.Text;
                RoomNumber = RoomNumberTextBox.Text;
                RoomWidth = double.Parse(RoomWidthTextBox.Text);
                RoomHeight = double.Parse(RoomHeightTextBox.Text);
                RoomType = RoomTypeTextBox.Text;
                SelectedLevel = LevelComboBox.SelectedItem?.ToString();

                if (string.IsNullOrEmpty(RoomName) || string.IsNullOrEmpty(RoomNumber) || string.IsNullOrEmpty(SelectedLevel))
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
