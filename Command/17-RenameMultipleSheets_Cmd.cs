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
    public class RenameMultipleSheets_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Retrieve all sheets in the project
            List<ViewSheet> sheets = GetSheets(doc);
            if (sheets.Count == 0)
            {
                TaskDialog.Show("Error", "No sheets found in the project.");
                return Result.Failed;
            }

            // Open WPF UI for renaming sheets
            RenameMultipleSheets renameWindow = new RenameMultipleSheets(sheets);
            if (renameWindow.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the operation
            }

            List<ViewSheet> selectedSheets = renameWindow.SelectedSheets;
            string renamePattern = renameWindow.RenamePattern;

            if (string.IsNullOrWhiteSpace(renamePattern))
            {
                TaskDialog.Show("Error", "The rename pattern cannot be empty.");
                return Result.Failed;
            }

            try
            {
                using (Transaction tx = new Transaction(doc, "Rename Sheets"))
                {
                    tx.Start();

                    // Rename selected sheets based on the pattern
                    foreach (ViewSheet sheet in selectedSheets)
                    {
                        string oldName = sheet.Name;
                        string newName = renamePattern.Replace("{SheetNumber}", sheet.SheetNumber);

                        sheet.Name = newName;
                    }

                    TaskDialog.Show("Success", $"{selectedSheets.Count} sheets renamed successfully.");
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

        private List<ViewSheet> GetSheets(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(ViewSheet))
                            .Cast<ViewSheet>()
                            .Where(sheet => !sheet.IsPlaceholder) // Ignore placeholder sheets
                            .ToList();
        }
    }
}
