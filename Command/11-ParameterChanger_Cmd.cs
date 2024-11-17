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

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class ParameterChanger_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Ask user to select an element
                Reference selectedRef = uiDoc.Selection.PickObject(ObjectType.Element, "Select an element");
                if (selectedRef == null) return Result.Cancelled;

                Element selectedElement = doc.GetElement(selectedRef);
                if (selectedElement == null) return Result.Cancelled;

                // Retrieve all parameters for the selected element
                List<ParameterInfo> parameters = selectedElement.Parameters
                    .Cast<Parameter>()
                    .Where(p => p.HasValue && !p.IsReadOnly)
                    .Select(p => new ParameterInfo
                    {
                        ParameterName = p.Definition.Name,
                        ParameterValue = GetParameterValue(p),
                        StorageType = p.StorageType,
                        Parameter = p
                    })
                    .ToList();

                // Show WPF UI
                ParameterChanger window = new ParameterChanger(parameters, selectedElement, uiDoc);
                window.ShowDialog();
                return Result.Succeeded;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }

        private static string GetParameterValue(Parameter parameter)
        {
            switch (parameter.StorageType)
            {
                case StorageType.String: return parameter.AsString();
                case StorageType.Double: return parameter.AsDouble().ToString();
                case StorageType.Integer: return parameter.AsInteger().ToString();
                case StorageType.ElementId: return parameter.AsElementId()?.IntegerValue.ToString() ?? string.Empty;
                default: return string.Empty;
            }
        }
    }

    public class ParameterInfo
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public StorageType StorageType { get; set; }
        public Parameter Parameter { get; set; }
    }

}
