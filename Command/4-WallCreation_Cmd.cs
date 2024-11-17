using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using FinalApplication.Views;

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class WallCreation_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Show the WPF UI
            WallCreation window = new WallCreation();
            if (window.ShowDialog() == true)
            {
                // Get user inputs
                var levelName = window.SelectedLevel;
                var height = window.HeightValue;
                var thickness = window.ThicknessValue;

                using (Transaction tx = new Transaction(doc, "Create Wall"))
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

                    // Define Wall Location
                    XYZ startPoint = new XYZ(0, 0, 0);
                    XYZ endPoint = new XYZ(10, 0, 0); // Adjust as needed
                    Line wallLine = Line.CreateBound(startPoint, endPoint);

                    // Create Wall
                    Wall wall = Wall.Create(doc, wallLine, level.Id, false);
                    wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(height);
                    //wall.Width = thickness;

                    tx.Commit();
                }

                return Result.Succeeded;
            }

            return Result.Cancelled;
        }

    }
}
