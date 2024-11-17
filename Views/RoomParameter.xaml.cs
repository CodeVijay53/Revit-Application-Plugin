using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using FinalApplication.Command;
using System;
using System.Collections.Generic;
using System.IO;
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
using OfficeOpenXml;

namespace FinalApplication.Views
{
    public partial class RoomParameter : Window
    {
        private readonly List<RoomInfor> _rooms;
        private readonly UIDocument _uiDoc;

        public RoomParameter(List<RoomInfor> rooms, UIDocument uiDoc)
        {
            InitializeComponent();
            _rooms = rooms;
            _uiDoc = uiDoc;

            // Bind data to the DataGrid
            RoomsDataGrid.ItemsSource = _rooms;
        }

        private void OnExportClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Save Room Data"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("Rooms");
                        worksheet.Cells[1, 1].Value = "Room Name";
                        worksheet.Cells[1, 2].Value = "Area (sqm)";
                        worksheet.Cells[1, 3].Value = "Level";

                        for (int i = 0; i < _rooms.Count; i++)
                        {
                            worksheet.Cells[i + 2, 1].Value = _rooms[i].Name;
                            worksheet.Cells[i + 2, 2].Value = _rooms[i].Area;
                            worksheet.Cells[i + 2, 3].Value = _rooms[i].LevelName;
                        }

                        package.SaveAs(new FileInfo(saveDialog.FileName));
                        MessageBox.Show("Exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnImportClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Import Room Data"
                };

                if (openDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage(new FileInfo(openDialog.FileName)))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            throw new Exception("No worksheet found!");

                        var rows = worksheet.Dimension.Rows;
                        for (int i = 2; i <= rows; i++)
                        {
                            string roomName = worksheet.Cells[i, 1].Text;
                            double.TryParse(worksheet.Cells[i, 2].Text, out double newArea);

                            var roomInfo = _rooms.FirstOrDefault(r => r.Name == roomName);
                            if (roomInfo != null)
                            {
                                using (var tx = new Transaction(_uiDoc.Document, "Update Room Data"))
                                {
                                    tx.Start();
                                    var room = _uiDoc.Document.GetElement(roomInfo.Id) as Room;
                                    room?.get_Parameter(BuiltInParameter.ROOM_AREA).Set(newArea / 0.092903); // Convert sqm to sqft
                                    tx.Commit();
                                }
                            }
                        }
                        MessageBox.Show("Imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
