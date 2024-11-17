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
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class DuplicateFloorPlan_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Get all floor plan views in the project
            List<ViewPlan> floorPlans = GetFloorPlans(doc);

            if (floorPlans.Count == 0)
            {
                TaskDialog.Show("Error", "No floor plan views found in the project.");
                return Result.Failed;
            }

            // Show WPF UI to select a floor plan and provide a suffix
            DuplicateFloorPlan selectorWindow = new DuplicateFloorPlan(floorPlans);
            if (selectorWindow.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the operation
            }

            ViewPlan selectedView = selectorWindow.SelectedFloorPlan;
            string suffix = selectorWindow.Suffix;

            if (string.IsNullOrEmpty(suffix))
            {
                TaskDialog.Show("Error", "The suffix cannot be empty.");
                return Result.Failed;
            }

            try
            {
                using (Transaction tx = new Transaction(doc, "Duplicate Floor Plan"))
                {
                    tx.Start();

                    // Duplicate the selected floor plan
                    ElementId duplicatedViewId = selectedView.Duplicate(ViewDuplicateOption.Duplicate);
                    ViewPlan duplicatedView = doc.GetElement(duplicatedViewId) as ViewPlan;

                    // Rename the duplicated view with the suffix
                    duplicatedView.Name = selectedView.Name + " " + suffix;

                    TaskDialog.Show("Success", $"Floor plan '{selectedView.Name}' duplicated as '{duplicatedView.Name}'.");

                    tx.Commit();
                }
            }
            catch (System.Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private List<ViewPlan> GetFloorPlans(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(ViewPlan))
                            .Cast<ViewPlan>()
                            .Where(vp => vp.ViewType == ViewType.FloorPlan)
                            .ToList();
        }
    }
}
