using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;


namespace ellipsis_drive_addin
{
    public class ToggleWindow : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public ToggleWindow()
        {

        }

        protected override void OnClick()
        {
            UID dockableWinUID = new UIDClass();
            dockableWinUID.Value = ThisAddIn.IDs.TreeDrive;
            IDockableWindow treeDrive = ArcMap.DockableWindowManager.GetDockableWindow(dockableWinUID);
            treeDrive.Show(true);
        }

        protected override void OnUpdate()
        {
        }
    }
}
