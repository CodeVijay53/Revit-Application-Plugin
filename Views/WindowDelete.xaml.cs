using Autodesk.Revit.DB;
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
    /// <summary>
    /// Interaction logic for ColumnInfo.xaml
    /// </summary>
    public partial class WindowDelete : Window
    {
        private readonly List<WindowInfo> _windows;
        private readonly Document _document;

        public WindowDelete(List<WindowInfo> windows, Document document)
        {
            InitializeComponent();
            _windows = windows;
            _document = document;

            // Bind the data to the DataGrid
            WindowsDataGrid.ItemsSource = _windows;
        }

        private void OnDeleteWindowClick(object sender, RoutedEventArgs e)
        {
            // Get the selected window
            var selectedWindow = WindowsDataGrid.SelectedItem as WindowInfo;
            if (selectedWindow == null)
            {
                MessageBox.Show("Please select a window to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Delete the selected window
            using (Transaction tx = new Transaction(_document, "Delete Window"))
            {
                try
                {
                    tx.Start();
                    _document.Delete(selectedWindow.Id);
                    tx.Commit();

                    // Remove the window from the list and refresh the grid
                    _windows.Remove(selectedWindow);
                    WindowsDataGrid.ItemsSource = null;
                    WindowsDataGrid.ItemsSource = _windows;

                    MessageBox.Show($"Window '{selectedWindow.Name}' deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    tx.RollBack();
                    MessageBox.Show($"Error deleting window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
