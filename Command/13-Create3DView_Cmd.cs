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
    public class Create3DView_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Display WPF UI for visibility settings
            Create3DView settingsWindow = new Create3DView();
            if (settingsWindow.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the operation
            }

            Dictionary<string, bool> visibilitySettings = settingsWindow.VisibilitySettings;

            try
            {
                using (Transaction tx = new Transaction(doc, "Create 3D View"))
                {
                    tx.Start();

                    // Create a new 3D view
                    ViewFamilyType viewFamilyType = Get3DViewFamilyType(doc);
                    if (viewFamilyType == null)
                    {
                        TaskDialog.Show("Error", "No 3D view family type found in the project.");
                        return Result.Failed;
                    }

                    View3D view3D = View3D.CreateIsometric(doc, viewFamilyType.Id);

                    // Set visibility settings based on user input
                    SetVisibilitySettings(view3D, visibilitySettings);

                    TaskDialog.Show("Success", $"New 3D view '{view3D.Name}' created with specified visibility settings.");
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private ViewFamilyType Get3DViewFamilyType(Document doc)
        {
            // Retrieve the 3D view family type
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(ViewFamilyType))
                            .Cast<ViewFamilyType>()
                            .FirstOrDefault(vft => vft.ViewFamily == ViewFamily.ThreeDimensional);
        }

        private void SetVisibilitySettings(View3D view3D, Dictionary<string, bool> settings)
        {
            foreach (KeyValuePair<string, bool> setting in settings)
            {
                // Find the category in the project
                Category category = view3D.Document.Settings.Categories.get_Item(setting.Key);
                if (category != null)
                {
                    // Toggle visibility: SetCategoryHidden hides the category if passed `true`
                    view3D.SetCategoryHidden(category.Id, !setting.Value);
                }
            }
        }
    }
}
