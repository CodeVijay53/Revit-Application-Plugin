using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using FinalApplication.Views;
using Autodesk.Revit.DB.Architecture;

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class RoomCreation_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Open the WPF Window
            RoomCreation window = new RoomCreation();
            if (window.ShowDialog() == true)
            {
                // Get user input from the WPF form
                string roomName = window.RoomName;
                string roomNumber = window.RoomNumber;
                double width = window.RoomWidth;
                double height = window.RoomHeight;
                string roomType = window.RoomType;
                string levelName = window.SelectedLevel;

                using (Transaction tx = new Transaction(doc, "Create Room"))
                {
                    tx.Start();

                    // Get Level
                    Level level = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .FirstOrDefault(e => e.Name.Equals(levelName)) as Level;

                    if (level == null)
                    {
                        message = $"Level '{levelName}' not found.";
                        return Result.Failed;
                    }

                    // Define room placement point
                    XYZ roomLocation = new XYZ(0, 0, 0); // Modify as needed

                    // Create Room
                    Room room = doc.Create.NewRoom(level, new UV(roomLocation.X, roomLocation.Y));

                    // Set Room Parameters
                    room.Name = roomName;
                    room.Number = roomNumber;

                    // Set custom parameters for width, height, and type
                    Parameter widthParam = room.LookupParameter("Width");
                    if (widthParam != null && !widthParam.IsReadOnly)
                    {
                        widthParam.Set(width);
                    }

                    Parameter heightParam = room.LookupParameter("Height");
                    if (heightParam != null && !heightParam.IsReadOnly)
                    {
                        heightParam.Set(height);
                    }

                    Parameter roomTypeParam = room.LookupParameter("Room Type");
                    if (roomTypeParam != null && !roomTypeParam.IsReadOnly)
                    {
                        roomTypeParam.Set(roomType);
                    }

                    tx.Commit();
                }

                return Result.Succeeded;
            }

            return Result.Cancelled;
        }

    }
}
