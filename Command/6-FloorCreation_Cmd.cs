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
    public class FloorCreation_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Open the WPF FloorCreation
            FloorCreation window = new FloorCreation();
            if (window.ShowDialog() == true)
            {
                // Retrieve user inputs from the WPF form
                var floorWidth = window.FloorWidth;
                var floorHeight = window.FloorHeight;
                var selectedLevel = window.SelectedLevel;
                var selectedFloorType = window.SelectedFloorType;

                using (Transaction tx = new Transaction(doc, "Create Floor"))
                {
                    tx.Start();

                    // Get the selected Level
                    Level level = new FilteredElementCollector(doc)
                        .OfClass(typeof(Level))
                        .FirstOrDefault(e => e.Name == selectedLevel) as Level;

                    if (level == null)
                    {
                        message = $"Level '{selectedLevel}' not found.";
                        return Result.Failed;
                    }

                    // Get the selected Floor Type
                    FloorType floorType = new FilteredElementCollector(doc)
                        .OfClass(typeof(FloorType))
                        .FirstOrDefault(e => e.Name == selectedFloorType) as FloorType;

                    if (floorType == null)
                    {
                        message = $"Floor Type '{selectedFloorType}' not found.";
                        return Result.Failed;
                    }

                    // Create floor boundary points
                    XYZ p1 = new XYZ(0, 0, 0);
                    XYZ p2 = new XYZ(floorWidth, 0, 0);
                    XYZ p3 = new XYZ(floorWidth, floorHeight, 0);
                    XYZ p4 = new XYZ(0, floorHeight, 0);

                    // Create CurveLoop for floor boundary
                    CurveLoop floorBoundary = new CurveLoop();
                    floorBoundary.Append(Line.CreateBound(p1, p2));
                    floorBoundary.Append(Line.CreateBound(p2, p3));
                    floorBoundary.Append(Line.CreateBound(p3, p4));
                    floorBoundary.Append(Line.CreateBound(p4, p1));

                    // Create the floor
                    Floor floor = Floor.Create(doc, new List<CurveLoop> { floorBoundary }, floorType.Id, level.Id);

                    if (floor == null)
                    {
                        message = "Failed to create the floor.";
                        return Result.Failed;
                    }

                    tx.Commit();
                }

                return Result.Succeeded;
            }

            return Result.Cancelled;
        }
    }
}
