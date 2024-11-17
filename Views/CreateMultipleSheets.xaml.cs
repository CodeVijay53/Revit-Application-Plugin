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
    public partial class CreateMultipleSheets : Window
    {
        public FamilySymbol SelectedTitleBlock { get; private set; }
    public int NumberOfSheets { get; private set; }

    public CreateMultipleSheets(List<FamilySymbol> titleBlocks)
    {
        InitializeComponent();

        // Populate the ComboBox with title blocks
        foreach (var titleBlock in titleBlocks)
        {
            ComboBoxItem item = new ComboBoxItem
            {
                Content = titleBlock.Name,
                Tag = titleBlock // Store the FamilySymbol in the Tag property
            };
            TitleBlockComboBox.Items.Add(item);
        }

        if (TitleBlockComboBox.Items.Count > 0)
        {
            TitleBlockComboBox.SelectedIndex = 0; // Select the first title block by default
        }

        // Attach button click events
        OkButton.Click += OkButton_Click;
        CancelButton.Click += CancelButton_Click;
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        if (TitleBlockComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            SelectedTitleBlock = selectedItem.Tag as FamilySymbol;

            if (int.TryParse(NumberOfSheetsTextBox.Text.Trim(), out int numSheets) && numSheets > 0)
            {
                NumberOfSheets = numSheets;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid number of sheets.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Please select a title block template.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

}
