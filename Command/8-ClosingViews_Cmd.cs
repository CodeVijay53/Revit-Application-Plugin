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
    public class ClosingViews_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Get the currently active view
            View activeView = uiDoc.ActiveView;

            // Collect all opened views (excluding the active view)
            List<ViewInfo> openedViews = uiDoc.GetOpenUIViews()
                .Select(uiView =>
                {
                    View view = doc.GetElement(uiView.ViewId) as View;
                    return new ViewInfo
                    {
                        Id = view.Id,
                        Name = view.Name,
                        ViewType = view.ViewType.ToString()
                    };
                })
                .Where(v => v.Id != activeView.Id) // Exclude the active view
                .ToList();

            // Open the WPF UI
            ClosingViews window = new ClosingViews(openedViews, uiDoc);
            window.ShowDialog();

            return Result.Succeeded;
        }
    }

    public class ViewInfo
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public string ViewType { get; set; }
    }
}
