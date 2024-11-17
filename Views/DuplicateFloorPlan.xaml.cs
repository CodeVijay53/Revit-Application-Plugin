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
    public partial class DuplicateFloorPlan : Window
    {
        public ViewPlan SelectedFloorPlan { get; private set; }
        public string Suffix { get; private set; }

        public DuplicateFloorPlan(List<ViewPlan> floorPlans)
        {
            InitializeComponent();

            // Populate the ComboBox with floor plans
            foreach (var floorPlan in floorPlans)
            {
                ComboBoxItem item = new ComboBoxItem
                {
                    Content = floorPlan.Name,
                    Tag = floorPlan // Store the ViewPlan object in the Tag property
                };
                FloorPlanComboBox.Items.Add(item);
            }

            if (FloorPlanComboBox.Items.Count > 0)
            {
                FloorPlanComboBox.SelectedIndex = 0; // Select the first floor plan by default
            }

            // Attach button click events
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (FloorPlanComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                SelectedFloorPlan = selectedItem.Tag as ViewPlan;
                Suffix = SuffixTextBox.Text.Trim();
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please select a floor plan.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
