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
    public class RoomParameter_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Collect all room information
            List<RoomInfor> rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<Room>()
                .Select(r => new RoomInfor
                {
                    Id = r.Id,
                    Name = r.get_Parameter(BuiltInParameter.ROOM_NAME).AsString(),
                    Area = r.get_Parameter(BuiltInParameter.ROOM_AREA).AsDouble() * 0.092903, // Convert from sqft to sqm
                    LevelName = (doc.GetElement(r.LevelId) as Level)?.Name ?? "Unassigned"
                })
                .ToList();

            // Launch the WPF UI
            RoomParameter window = new RoomParameter(rooms, uiDoc);
            window.ShowDialog();

            return Result.Succeeded;
        }
    }

    public class RoomInfor
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public double Area { get; set; }
        public string LevelName { get; set; }
    }

}
