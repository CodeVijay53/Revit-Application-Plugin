using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Revit API Namespace
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.Windows.Media.Imaging;
using FinalApplication.Command;
#endregion

namespace FinalApplication.RvtAp
{
    public class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application) => Result.Succeeded;


        public Result OnStartup(UIControlledApplication application)
        {
            ///<summary>
            ///Tab
            ///Panel
            ///Button
            /// </summary>

            ///Constants
            string tabName = "HLSK Planer";

            ///Create the Panel Names
            string panelName1 = "Element Retrieval & Modification";
            string panelName2 = "Element Creation";
            string panelName3 = "Element Deletion";
            string panelName4 = "Pararameter Manipulation";
            string panelName5 = "View Creation";
            string panelName6 = "Sheet Management";
            string pluginLoc = Assembly.GetExecutingAssembly().Location;

            ////Add the Images
            ///Panel-1
            string wallIconPath = "/FinalApplication;component/Resources/wallButtonData_PNG.png";
            string doorIconPath = "/FinalApplication;component/Resources/doorButtonData_PNG.png";
            string columnIconPath = "/FinalApplication;component/Resources/columnButtonData_PNG.png";
            ///Panel-2
            string wallCIconPath = "/FinalApplication;component/Resources/wallCButtonData_PNG.png";
            string floorCIconPath = "/FinalApplication;component/Resources/floorButtonData_PNG.png";
            string roomCIconPath = "/FinalApplication;component/Resources/roomButtonData_PNG.png";
            ///Panel-3
            string winDelIconPath = "/FinalApplication;component/Resources/winDelButtonData_PNG.png";
            string viewClsIconPath = "/FinalApplication;component/Resources/viewClsButtonData_PNG.png";
            string roomDelIconPath = "/FinalApplication;component/Resources/roomDelButtonData_PNG.png";
            ///Panel-4
            string roomParamIconPath = "/FinalApplication;component/Resources/roomParamButtonData_PNG.png";
            string specParamIconPath = "/FinalApplication;component/Resources/specParamButtonData_PNG.png";
            string famParamIconPath = "/FinalApplication;component/Resources/famParamButtonData_PNG.png";
            ///Panel-5
            string threeDViewIconPath = "/FinalApplication;component/Resources/threeDViewButtonData_PNG.png";
            string floorPlanIconPath = "/FinalApplication;component/Resources/floorPlanButtonData_PNG.png";
            string detLevelIconPath = "/FinalApplication;component/Resources/detLevelButtonData_PNG.png";
            ///Panel-6
            string creMulSheetIconPath = "/FinalApplication;component/Resources/creMulSheetButtonData_PNG.png";
            string renMulSheetIconPath = "/FinalApplication;component/Resources/renMulSheetButtonData_PNG.png";

            ///Create Tab
            application.CreateRibbonTab(tabName);

            ///Create Panels
            RibbonPanel ribbonPanel1 = application.CreateRibbonPanel(tabName, panelName1);
            RibbonPanel ribbonPanel2 = application.CreateRibbonPanel(tabName, panelName2);
            RibbonPanel ribbonPanel3 = application.CreateRibbonPanel(tabName, panelName3);
            RibbonPanel ribbonPanel4 = application.CreateRibbonPanel(tabName, panelName4);
            RibbonPanel ribbonPanel5 = application.CreateRibbonPanel(tabName, panelName5);
            RibbonPanel ribbonPanel6 = application.CreateRibbonPanel(tabName, panelName6);

            #region Panel-1
            ///Button Data for Panel-1
            PushButtonData wallButtonData = new PushButtonData("btn_WallButton", "Change Walls", pluginLoc, typeof(WallHeight_Cmd).FullName);
            PushButtonData doorButtonData = new PushButtonData("btn_DoorButton", "Door Type", pluginLoc, typeof(DoorInfo_Cmd).FullName);
            PushButtonData columnButtonData = new PushButtonData("btn_ColumnButton", "Column Param", pluginLoc, typeof(ColumnInfo_Cmd).FullName);

            ///Create Buttons for Panel-1
            PushButton wallButton = ribbonPanel1.AddItem(wallButtonData) as PushButton;
            wallButton.LargeImage = new BitmapImage(new Uri(wallIconPath, UriKind.Relative));
            wallButton.ToolTip = "Change the wall height";
            ribbonPanel1.AddSeparator();
            PushButton doorButton = ribbonPanel1.AddItem(doorButtonData) as PushButton;
            doorButton.LargeImage = new BitmapImage(new Uri(doorIconPath, UriKind.Relative));
            doorButton.ToolTip = "Change the door type";
            ribbonPanel1.AddSeparator();
            PushButton columnButton = ribbonPanel1.AddItem(columnButtonData) as PushButton;
            columnButton.LargeImage = new BitmapImage(new Uri(columnIconPath, UriKind.Relative));
            columnButton.ToolTip = "Change the column parameter";
            #endregion
            #region Panel-2
            ///Button Data for Panel-2
            PushButtonData wallCButtonData = new PushButtonData("btn_WallCButton", "New Wall", pluginLoc, typeof(WallCreation_Cmd).FullName);
            PushButtonData roomButtonData = new PushButtonData("btn_RoomButton", "New Room", pluginLoc, typeof(RoomCreation_Cmd).FullName);
            PushButtonData floorButtonData = new PushButtonData("btn_FloorButton", "New Floor", pluginLoc, typeof(FloorCreation_Cmd).FullName);

            ///Create Buttons for Panel-2
            PushButton wallCButton = ribbonPanel2.AddItem(wallCButtonData) as PushButton;
            wallCButton.LargeImage = new BitmapImage(new Uri(wallCIconPath, UriKind.Relative));
            wallCButton.ToolTip = "Create a new wall";
            ribbonPanel2.AddSeparator();
            PushButton roomButton = ribbonPanel2.AddItem(roomButtonData) as PushButton;
            roomButton.LargeImage = new BitmapImage(new Uri(roomCIconPath, UriKind.Relative));
            roomButton.ToolTip = "Create a new room";
            ribbonPanel2.AddSeparator();
            PushButton floorButton = ribbonPanel2.AddItem(floorButtonData) as PushButton;
            floorButton.LargeImage = new BitmapImage(new Uri(floorCIconPath, UriKind.Relative));
            floorButton.ToolTip = "Create a new floor";
            #endregion
            #region Panel-3
            ///Button Data for Panel-3
            PushButtonData winDelButtonData = new PushButtonData("btn_WinDelButton", "Window Delete", pluginLoc, typeof(WindowDelete_Cmd).FullName);
            PushButtonData viewClsButtonData = new PushButtonData("btn_ViewClsButton", "Close Views", pluginLoc, typeof(ClosingViews_Cmd).FullName);
            PushButtonData roomDelButtonData = new PushButtonData("btn_RoomDelButton", "Unplaced Room Delete", pluginLoc, typeof(DeleteRooms_Cmd).FullName);

            ///Create Buttons for Panel-3
            PushButton winDelButton = ribbonPanel3.AddItem(winDelButtonData) as PushButton;
            winDelButton.LargeImage = new BitmapImage(new Uri(winDelIconPath, UriKind.Relative));
            winDelButton.ToolTip = "Delete all the windows";
            ribbonPanel3.AddSeparator();
            PushButton viewClsButton = ribbonPanel3.AddItem(viewClsButtonData) as PushButton;
            viewClsButton.LargeImage = new BitmapImage(new Uri(viewClsIconPath, UriKind.Relative));
            viewClsButton.ToolTip = "Close the views other than active view";
            ribbonPanel3.AddSeparator();
            PushButton roomDelButton = ribbonPanel3.AddItem(roomDelButtonData) as PushButton;
            roomDelButton.LargeImage = new BitmapImage(new Uri(roomDelIconPath, UriKind.Relative));
            roomDelButton.ToolTip = "Delete the unplaced room in the project";

            #endregion
            #region Panel-4
            ///Button Data for Panel-4
            PushButtonData roomParamButtonData = new PushButtonData("btn_RoomParamButton", "Room Parameter", pluginLoc, typeof(RoomParameter_Cmd).FullName);
            PushButtonData specParamButtonData = new PushButtonData("btn_SpecParamButton", "Spec Parameter", pluginLoc, typeof(ParameterChanger_Cmd).FullName);
            PushButtonData famParamButtonData = new PushButtonData("btn_FamParamlButton", "Family Parameter", pluginLoc, typeof(FamilyParameters_Cmd).FullName);

            ///Create Buttons for Panel-4
            PushButton roomParamButton = ribbonPanel4.AddItem(roomParamButtonData) as PushButton;
            roomParamButton.LargeImage = new BitmapImage(new Uri(roomParamIconPath, UriKind.Relative));
            roomParamButton.ToolTip = "Room parameter change";
            ribbonPanel4.AddSeparator();
            PushButton specParamButton = ribbonPanel4.AddItem(specParamButtonData) as PushButton;
            specParamButton.LargeImage = new BitmapImage(new Uri(specParamIconPath, UriKind.Relative));
            specParamButton.ToolTip = "Changing the specific parameter";
            ribbonPanel4.AddSeparator();
            PushButton famParamButton = ribbonPanel4.AddItem(famParamButtonData) as PushButton;
            famParamButton.LargeImage = new BitmapImage(new Uri(famParamIconPath, UriKind.Relative));
            famParamButton.ToolTip = "Edit exsiting family, add a new family parameter and load the family back to project";
            #endregion
            #region Panel-5
            ///Button Data for Panel-5
            PushButtonData threeDViewButtonData = new PushButtonData("btn_ThreeDViewButton", "3D View", pluginLoc, typeof(Create3DView_Cmd).FullName);
            PushButtonData floorPlanButtonData = new PushButtonData("btn_FloorPlanButton", "Du-Floor Plan", pluginLoc, typeof(DuplicateFloorPlan_Cmd).FullName);
            PushButtonData detLevelButtonData = new PushButtonData("btn_DetLevelButton", "Detail Level", pluginLoc, typeof(ChangeViewDetailLevel_Cmd).FullName);

            ///Create Buttons for Panel-5
            PushButton threeDViewButton = ribbonPanel5.AddItem(threeDViewButtonData) as PushButton;
            threeDViewButton.LargeImage = new BitmapImage(new Uri(threeDViewIconPath, UriKind.Relative));
            threeDViewButton.ToolTip = "3D-View creation";
            ribbonPanel5.AddSeparator();
            PushButton floorPlanButton = ribbonPanel5.AddItem(floorPlanButtonData) as PushButton;
            floorPlanButton.LargeImage = new BitmapImage(new Uri(floorPlanIconPath, UriKind.Relative));
            floorPlanButton.ToolTip = "Duplicates an existing floor plan view";
            ribbonPanel5.AddSeparator();
            PushButton detLevelButton = ribbonPanel5.AddItem(detLevelButtonData) as PushButton;
            detLevelButton.LargeImage = new BitmapImage(new Uri(detLevelIconPath, UriKind.Relative));
            detLevelButton.ToolTip = "Changes the detail level of all the views";
            #endregion
            #region Panel-6
            ///Button Data for Panel-6
            PushButtonData creMulSheetButtonData = new PushButtonData("btn_CreMulSheetButton", "Create Multi-Sheets", pluginLoc, typeof(CreateMultipleSheets_Cmd).FullName);
            PushButtonData renMulSheetButtonData = new PushButtonData("btn_RenMulSheetButton", "Rename Sheets", pluginLoc, typeof(RenameMultipleSheets_Cmd).FullName);
            
            ///Create Buttons for Panel-6
            PushButton creMulSheetButton = ribbonPanel6.AddItem(creMulSheetButtonData) as PushButton;
            creMulSheetButton.LargeImage = new BitmapImage(new Uri(creMulSheetIconPath, UriKind.Relative));
            creMulSheetButton.ToolTip = "Create multiple Sheets";
            ribbonPanel6.AddSeparator();
            PushButton renMulSheetButton = ribbonPanel6.AddItem(renMulSheetButtonData) as PushButton;
            renMulSheetButton.LargeImage = new BitmapImage(new Uri(renMulSheetIconPath, UriKind.Relative));
            renMulSheetButton.ToolTip = "Rename created multiple sheets";
            #endregion
            return Result.Succeeded;

        }
    }
}