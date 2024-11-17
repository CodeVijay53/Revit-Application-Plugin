using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using FinalApplication.Views;
using System;
using System.Collections.Generic;
using System.Linq;


#region Autodesk 
#endregion
namespace FinalApplication.Command

{
    [Transaction(TransactionMode.Manual)]
    public class DoorInfo_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Get the Revit document
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Collect all available door family types
            List<FamilySymbol> doorTypes = new FilteredElementCollector(doc)
                                            .OfClass(typeof(FamilySymbol))
                                            .OfCategory(BuiltInCategory.OST_Doors)
                                            .Cast<FamilySymbol>()
                                            .ToList();

            if (doorTypes.Count == 0)
            {
                message = "No door types found in the project.";
                return Result.Failed;
            }

            // Show the WPF window to allow the user to select the desired door type
            DoorType window = new DoorType(doorTypes);
            bool? dialogResult = window.ShowDialog();

            if (dialogResult != true || window.selectedDoorType == null)
            {
                message = "Operation cancelled or no door type selected.";
                return Result.Cancelled;
            }

            FamilySymbol selectedDoorType = window.selectedDoorType;

            // Start a transaction to modify elements
            using (Transaction transaction = new Transaction(doc, "Set Door Type"))
            {
                transaction.Start();

                try
                {
                    // Activate the selected door type if it's not already active
                    if (!selectedDoorType.IsActive)
                    {
                        selectedDoorType.Activate();
                        doc.Regenerate();  // Regenerate the document to apply changes
                    }

                    // Retrieve all door instances in the project
                    var doors = new FilteredElementCollector(doc)
                                .OfCategory(BuiltInCategory.OST_Doors)
                                .OfClass(typeof(FamilyInstance))
                                .Cast<FamilyInstance>();

                    foreach (FamilyInstance door in doors)
                    {
                        // Change the family type of each door instance to the selected type
                        door.Symbol = selectedDoorType;
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    // Roll back the transaction if something goes wrong
                    transaction.RollBack();
                    message = ex.Message;
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }
    }
}
