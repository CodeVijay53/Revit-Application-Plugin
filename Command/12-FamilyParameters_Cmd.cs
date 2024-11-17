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
    public class FamilyParameters_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Show the WPF dialog to select a family file
            FamilyParameters selectorWindow = new FamilyParameters();
            if (selectorWindow.ShowDialog() != true)
            {
                return Result.Cancelled; // User canceled the dialog
            }

            string familyPath = selectorWindow.SelectedFilePath;

            if (string.IsNullOrEmpty(familyPath))
            {
                TaskDialog.Show("Error", "No family file was selected.");
                return Result.Failed;
            }

            // Open the selected family file
            Application app = uiApp.Application;
            Document familyDoc = app.OpenDocumentFile(familyPath);

            try
            {
                using (Transaction tx = new Transaction(familyDoc, "Add Family Parameter"))
                {
                    tx.Start();

                    FamilyManager familyManager = familyDoc.FamilyManager;

                    // Define parameter details
                    string paramName = "NewParameter";
                    ForgeTypeId groupTypeId = GroupTypeId.Data; // Specifies the group to categorize the parameter
                    ForgeTypeId specTypeId = SpecTypeId.String.Text; // Specifies the parameter data type (Text in this case)
                    bool isInstance = false; // Indicates whether it's an instance parameter or a type parameter

                    // Add the parameter if it does not already exist
                    if (!familyManager.Parameters.Cast<FamilyParameter>().Any(p => p.Definition.Name == paramName))
                    {
                        FamilyParameter newParam = familyManager.AddParameter(paramName, groupTypeId, specTypeId, isInstance);

                        TaskDialog.Show("Success", $"Parameter '{paramName}' added successfully.");
                    }
                    else
                    {
                        TaskDialog.Show("Info", $"Parameter '{paramName}' already exists.");
                    }

                    tx.Commit();
                }

                // Save the family and reload it into the project
                string tempPath = Path.Combine(
                    Path.GetTempPath(),
                    Path.GetFileName(familyPath));

                familyDoc.SaveAs(tempPath);
                familyDoc.Close();

                using (Transaction tx = new Transaction(doc, "Load Family"))
                {
                    tx.Start();
                    Family family;
                    if (doc.LoadFamily(tempPath, out family))
                    {
                        TaskDialog.Show("Success", "Family reloaded into the project.");
                    }
                    else
                    {
                        TaskDialog.Show("Error", "Failed to load family back into the project.");
                    }
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return Result.Failed;
            }
            finally
            {
                if (familyDoc != null)
                {
                    familyDoc.Close(false);
                }
            }

            return Result.Succeeded;
        }
    }

}
