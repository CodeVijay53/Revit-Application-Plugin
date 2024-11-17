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

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class WindowDelete_Cmd : IExternalCommand
    {/// <summary>
    /// 
    /// </summary>
    /// <param name="commandData"></param>
    /// <param name="message"></param>
    /// <param name="elements"></param>
    /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Collect all windows
            List<WindowInfo> windows = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .WhereElementIsNotElementType()
                .Select(e =>
                {
                    Level level = doc.GetElement(e.LevelId) as Level;
                    return new WindowInfo
                    {
                        Id = e.Id,
                        Name = e.Name,
                        LevelName = level?.Name ?? "Unassigned"
                    };
                })
                .OrderBy(w => w.LevelName) // Sort by level name
                .ToList();

            // Open WPF Window
            WindowDelete window = new WindowDelete(windows, doc);
            window.ShowDialog();

            return Result.Succeeded;
        }
    }
    public class WindowInfo
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public string LevelName { get; set; }
    }
}
