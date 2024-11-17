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
    public partial class ParameterChanger : Window
    {
        private readonly List<ParameterInfo> _parameters;
        private readonly Element _selectedElement;
        private readonly UIDocument _uiDoc;

        public ParameterChanger(List<ParameterInfo> parameters, Element selectedElement, UIDocument uiDoc)
        {
            InitializeComponent();
            _parameters = parameters;
            _selectedElement = selectedElement;
            _uiDoc = uiDoc;

            // Bind the parameters to the DataGrid
            ParameterDataGrid.ItemsSource = _parameters;
        }

        private void OnUpdateParametersClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Document doc = _uiDoc.Document;

                // Get the element type to apply changes to all instances
                ElementId typeId = _selectedElement.GetTypeId();
                if (typeId == ElementId.InvalidElementId)
                {
                    MessageBox.Show("Selected element has no type. Parameters cannot be updated across instances.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ElementType elementType = doc.GetElement(typeId) as ElementType;
                if (elementType == null)
                {
                    MessageBox.Show("Unable to retrieve element type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Start a transaction to update parameters
                using (Transaction tx = new Transaction(doc, "Update Parameters"))
                {
                    tx.Start();

                    foreach (var parameterInfo in _parameters)
                    {
                        Parameter parameter = elementType.LookupParameter(parameterInfo.ParameterName);
                        if (parameter != null && !parameter.IsReadOnly)
                        {
                            SetParameterValue(parameter, parameterInfo.ParameterValue, parameterInfo.StorageType);
                        }
                    }

                    tx.Commit();
                }

                MessageBox.Show("Parameters updated successfully for all instances.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating parameters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void SetParameterValue(Parameter parameter, string value, StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.String:
                    parameter.Set(value);
                    break;
                case StorageType.Double:
                    if (double.TryParse(value, out double doubleValue))
                        parameter.Set(doubleValue);
                    break;
                case StorageType.Integer:
                    if (int.TryParse(value, out int intValue))
                        parameter.Set(intValue);
                    break;
                case StorageType.ElementId:
                    if (int.TryParse(value, out int elementIdValue))
                        parameter.Set(new ElementId(elementIdValue));
                    break;
            }
        }
    }

}
