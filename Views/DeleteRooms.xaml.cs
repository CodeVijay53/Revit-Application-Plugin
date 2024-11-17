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
    public partial class DeleteRooms : Window
    {
        private readonly List<RoomInfo> _rooms;
        private readonly Document _document;

        public DeleteRooms(List<RoomInfo> rooms, Document document)
        {
            InitializeComponent();
            _rooms = rooms;
            _document = document;

            // Bind data to the DataGrid
            RoomsDataGrid.ItemsSource = _rooms;
        }

        private void OnDeleteUnplacedRoomsClick(object sender, RoutedEventArgs e)
        {
            // Get unplaced rooms
            var unplacedRooms = _rooms.Where(r => !r.IsPlaced).ToList();

            // Confirm deletion
            if (MessageBox.Show($"Are you sure you want to delete {unplacedRooms.Count} unplaced rooms?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            // Delete unplaced rooms
            using (Transaction tx = new Transaction(_document, "Delete Unplaced Rooms"))
            {
                try
                {
                    tx.Start();
                    foreach (var room in unplacedRooms)
                    {
                        _document.Delete(room.Id);
                    }
                    tx.Commit();

                    // Update the list and refresh the DataGrid
                    _rooms.RemoveAll(r => !r.IsPlaced);
                    RoomsDataGrid.ItemsSource = null;
                    RoomsDataGrid.ItemsSource = _rooms;

                    MessageBox.Show($"{unplacedRooms.Count} unplaced rooms have been deleted.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    tx.RollBack();
                    MessageBox.Show("Failed to delete unplaced rooms.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

}
