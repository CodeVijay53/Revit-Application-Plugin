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
    public class ColumnInfo_Cmd : IExternalCommand
    {
        private const string SharedParameterFilePath = @"C:\Users\vijay.S\Desktop\sharedParameter.txt";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            ColumnInfo window = new ColumnInfo();
            bool? dialogResult = window.ShowDialog();

            if (dialogResult != true || string.IsNullOrEmpty(window.ParameterName) || string.IsNullOrEmpty(window.ParameterValue))
            {
                message = "Operation cancelled or invalid input.";
                return Result.Cancelled;
            }

            string parameterName = window.ParameterName;
            string parameterValue = window.ParameterValue;

            using (Transaction transaction = new Transaction(doc, "Add Parameter to Columns"))
            {
                transaction.Start();

                try
                {
                    SetUpSharedParameterFile(doc.Application);
                    Definition parameterDefinition = GetOrCreateSharedParameter(doc, parameterName);

                    if (parameterDefinition == null)
                    {
                        message = "Failed to create shared parameter.";
                        return Result.Failed;
                    }

                    BindParameterToCategory(doc, parameterDefinition, BuiltInCategory.OST_Columns);

                    var columns = new FilteredElementCollector(doc)
                                  .OfClass(typeof(FamilyInstance))
                                  .OfCategory(BuiltInCategory.OST_Columns)
                                  .Cast<FamilyInstance>();

                    foreach (FamilyInstance column in columns)
                    {
                        Parameter param = column.LookupParameter(parameterName);

                        if (param != null && param.StorageType == StorageType.String)
                        {
                            param.Set(parameterValue);
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.RollBack();
                    message = ex.Message;
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }

        private void SetUpSharedParameterFile(Autodesk.Revit.ApplicationServices.Application app)
        {
            string directoryPath = Path.GetDirectoryName(SharedParameterFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(SharedParameterFilePath))
            {
                using (StreamWriter sw = File.CreateText(SharedParameterFilePath))
                {
                    sw.WriteLine("# This is a Revit shared parameter file.");
                    sw.WriteLine("*GROUP\tID\tGROUPNAME");
                    sw.WriteLine("GROUP\t1\tCustomParameters");
                    sw.WriteLine("*PARAM\tGUID\tNAME\tDATATYPE");
                }
            }

            app.SharedParametersFilename = SharedParameterFilePath;
        }

        private Definition GetOrCreateSharedParameter(Document doc, string paramName)
        {
            DefinitionFile sharedParameterFile = doc.Application.OpenSharedParameterFile();

            if (sharedParameterFile == null)
            {
                throw new InvalidOperationException("Could not open shared parameter file.");
            }

            DefinitionGroup sharedParameterGroup = sharedParameterFile.Groups.get_Item("CustomParameters");

            if (sharedParameterGroup == null)
            {
                sharedParameterGroup = sharedParameterFile.Groups.Create("CustomParameters");
            }

            Definition definition = sharedParameterGroup.Definitions.get_Item(paramName);

            if (definition == null)
            {
                // Generate a new GUID for the shared parameter
                Guid paramGuid = Guid.NewGuid();

                // Use ForgeTypeId for a text parameter (string type)
                ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(paramName, new ForgeTypeId("autodesk.spec.aec:strings.string"))
                {
                    GUID = paramGuid
                };

                definition = sharedParameterGroup.Definitions.Create(options);

                // Append to shared parameter file
                using (StreamWriter sw = File.AppendText(SharedParameterFilePath))
                {
                    sw.WriteLine($"PARAM\t{paramGuid}\t{paramName}\tTEXT");
                }
            }

            return definition;
        }

        private void BindParameterToCategory(Document doc, Definition definition, BuiltInCategory category)
        {
            CategorySet categorySet = doc.Application.Create.NewCategorySet();
            Category columnCategory = doc.Settings.Categories.get_Item(category);
            categorySet.Insert(columnCategory);

            Binding binding = doc.Application.Create.NewInstanceBinding(categorySet);

            BindingMap bindingMap = doc.ParameterBindings;

            if (!bindingMap.Contains(definition))
            {
                bindingMap.Insert(definition, binding, BuiltInParameterGroup.PG_DATA);
            }
        }
    }
}
