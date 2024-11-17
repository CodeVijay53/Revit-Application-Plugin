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
    public partial class RenameMultipleSheets : Window
    {
        public List<ViewSheet> SelectedSheets { get; private set; }
        public string RenamePattern { get; private set; }

        public RenameMultipleSheets(List<ViewSheet> sheets)
        {
            InitializeComponent();

            // Populate the ListBox with available sheets
            foreach (var sheet in sheets)
            {
                ListBoxItem item = new ListBoxItem
                {
                    Content = $"{sheet.SheetNumber} - {sheet.Name}",
                    Tag = sheet // Store the ViewSheet in the Tag property
                };
                SheetsListBox.Items.Add(item);
            }

            // Attach button click events
            OkButton.Click += OkButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected sheets from the ListBox
            SelectedSheets = SheetsListBox.SelectedItems
                            .Cast<ListBoxItem>()
                            .Select(item => item.Tag as ViewSheet)
                            .ToList();

            if (SelectedSheets.Count == 0)
            {
                MessageBox.Show("Please select at least one sheet.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get rename pattern
            RenamePattern = RenamePatternTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(RenamePattern))
            {
                MessageBox.Show("Please provide a rename pattern.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
