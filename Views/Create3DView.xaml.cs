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
    public partial class Create3DView : Window
    {
        public Dictionary<string, bool> VisibilitySettings { get; private set; }

        public Create3DView()
        {
            InitializeComponent();

            // Populate category checkboxes
            List<string> categories = GetRevitCategories();
            foreach (string category in categories)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = category,
                    IsChecked = true // Default to visible
                };
                CategoryStackPanel.Children.Add(checkBox);
            }

            // Handle button events
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            VisibilitySettings = new Dictionary<string, bool>();
            foreach (CheckBox checkBox in CategoryStackPanel.Children.OfType<CheckBox>())
            {
                VisibilitySettings.Add(checkBox.Content.ToString(), checkBox.IsChecked == true);
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private List<string> GetRevitCategories()
        {
            // A simplified hardcoded list of common categories; this can be extended
            return new List<string>
            {
                "Walls",
                "Floors",
                "Roofs",
                "Doors",
                "Windows",
                "Furniture",
                "Mechanical Equipment",
                "Lighting Fixtures",
                "Plumbing Fixtures"
            };
        }
    }

}
