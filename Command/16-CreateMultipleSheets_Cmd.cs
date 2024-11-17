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
    public class CreateMultipleSheets_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Retrieve all title blocks in the project
            List<FamilySymbol> titleBlocks = GetTitleBlocks(doc);
            if (titleBlocks.Count == 0)
            {
                TaskDialog.Show("Error", "No title blocks found in the project.");
                return Result.Failed;
            }

            // Show WPF UI to select a title block and enter the number of sheets
            CreateMultipleSheets sheetWindow = new CreateMultipleSheets(titleBlocks);
            if (sheetWindow.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the operation
            }

            FamilySymbol selectedTitleBlock = sheetWindow.SelectedTitleBlock;
            int numberOfSheets = sheetWindow.NumberOfSheets;

            if (numberOfSheets <= 0)
            {
                TaskDialog.Show("Error", "The number of sheets must be greater than zero.");
                return Result.Failed;
            }

            try
            {
                using (Transaction tx = new Transaction(doc, "Create Multiple Sheets"))
                {
                    tx.Start();

                    for (int i = 1; i <= numberOfSheets; i++)
                    {
                        // Create a new sheet using the selected title block
                        ViewSheet newSheet = ViewSheet.Create(doc, selectedTitleBlock.Id);

                        if (newSheet == null)
                        {
                            throw new InvalidOperationException("Failed to create a new sheet.");
                        }

                        // Rename the sheet with a unique number or suffix
                        newSheet.Name = $"{selectedTitleBlock.Name} - Sheet {i}";
                    }

                    TaskDialog.Show("Success", $"{numberOfSheets} sheets created successfully using the template '{selectedTitleBlock.Name}'.");

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

        private List<FamilySymbol> GetTitleBlocks(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.OfCategory(BuiltInCategory.OST_TitleBlocks)
                            .OfClass(typeof(FamilySymbol))
                            .Cast<FamilySymbol>()
                            .ToList();
        }
    }
}
