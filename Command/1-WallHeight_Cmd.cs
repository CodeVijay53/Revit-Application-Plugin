using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;
using FinalApplication.Views;
using System.Runtime.InteropServices.WindowsRuntime;
#endregion

namespace FinalApplication.Command
{
    [Transaction(TransactionMode.Manual)]
    public class WallHeight_Cmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            ///Show the WPF
            WallHeight window = new WallHeight();
            bool?dialogResult= window.ShowDialog();

            if (dialogResult != true || window.Wallheight == null) {
                message = "Operation cancelled";
                return Result.Cancelled;
            
            }
            double newHeight = UnitUtils.ConvertFromInternalUnits(window.Wallheight.Value,UnitTypeId.Millimeters);

            /// To find the list of element Ids
            List<Element> wallEle = new FilteredElementCollector(doc)
                .OfClass(typeof(Wall))
                .WhereElementIsNotElementType()
                .ToList();

            Transaction editParam = new Transaction(doc, "Change the Wall Height");

            editParam.Start();
            try
            {

                ///foreach loop in the elements to get the Type Name
                foreach (Element ele in wallEle)
                {   ///Parameter height change
                    Parameter heightParam = ele.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM);
                    /// Parameter and Lookup Parameter
                    Parameter markParam = ele.LookupParameter("Mark");
                    ///Set the Mark parameter
                    if (heightParam != null && !heightParam.IsReadOnly)
                    {
                        heightParam.Set(newHeight);
                    }

                }
                ///Task Dialog
                TaskDialog.Show("Success", "All wall heights are updated");
            }
            catch (Exception ex)
            {
                editParam.RollBack();
                editParam.Dispose();
            }

            if (editParam.IsValidObject && editParam.HasStarted())
            {
                editParam.Commit();
                editParam.Dispose();

            }

            return Result.Succeeded;
        }
    }

}
