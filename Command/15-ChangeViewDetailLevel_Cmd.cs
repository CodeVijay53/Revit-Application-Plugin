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
    public class ChangeViewDetailLevel_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Show WPF UI to select the detail level
            ChangeViewDetailLevel detailLevelSelector = new ChangeViewDetailLevel();
            if (detailLevelSelector.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the operation
            }

            ViewDetailLevel selectedDetailLevel = detailLevelSelector.SelectedDetailLevel;

            try
            {
                using (Transaction tx = new Transaction(doc, "Change Detail Level"))
                {
                    tx.Start();

                    // Get all applicable views
                    List<View> views = GetProjectViews(doc);

                    foreach (View view in views)
                    {
                        // Change the detail level using the parameter method
                        Parameter detailLevelParam = view.get_Parameter(BuiltInParameter.VIEW_DETAIL_LEVEL);
                        if (detailLevelParam != null && !detailLevelParam.IsReadOnly)
                        {
                            detailLevelParam.Set((int)selectedDetailLevel);
                        }
                    }

                    TaskDialog.Show("Success", $"Detail level updated to '{selectedDetailLevel}' for {views.Count} views.");
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

        private List<View> GetProjectViews(Document doc)
        {
            // Collect all valid views (exclude templates and system views)
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(View))
                            .Cast<View>()
                            .Where(v => !v.IsTemplate && v.CanBePrinted)
                            .ToList();
        }
    
}
}
