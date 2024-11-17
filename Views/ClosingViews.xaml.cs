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
    public partial class ClosingViews : Window
    {
        private readonly List<ViewInfo> _views;
        private readonly UIDocument _uiDoc;

        public ClosingViews(List<ViewInfo> views, UIDocument uiDoc)
        {
            InitializeComponent();
            _views = views;
            _uiDoc = uiDoc;

            // Bind the data to the DataGrid
            ViewsDataGrid.ItemsSource = _views;
        }

        private void OnCloseViewsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Loop through the views and close them
                foreach (var viewInfo in _views)
                {
                    var uiView = _uiDoc.GetOpenUIViews()
                        .FirstOrDefault(v => v.ViewId == viewInfo.Id);
                    if (uiView != null)
                    {
                        uiView.Close();
                    }
                }

                MessageBox.Show("All views except the active view have been closed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error closing views: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
