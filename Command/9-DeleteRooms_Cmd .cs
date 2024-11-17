using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using FinalApplication.Views;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteRooms_Cmd  : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Collect all rooms
            List<RoomInfo> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<SpatialElement>()
                .OfType<Room>()
                .Select(room => new RoomInfo
                {
                    Id = room.Id,
                    Name = room.Name,
                    Number = room.Number,
                    IsPlaced = room.Location != null
                })
                .ToList();

            // Open WPF Window
            DeleteRooms window = new DeleteRooms(rooms, doc);
            window.ShowDialog();

            return Result.Succeeded;
        }
    }

    public class RoomInfo
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public bool IsPlaced { get; set; }
    }

}
