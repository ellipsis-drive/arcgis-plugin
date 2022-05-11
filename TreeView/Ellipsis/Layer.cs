using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Diagnostics;
using System;
using System.Text;

namespace Ellipsis.Api
{
    class Layers
    {
        public Layers() { }

        public Layers(string _URL, string _map_id, string _login_token, string _protocol, string _timestamp_id, string _layer_id)
        {
            this.URL = _URL;
            this.map_id = _map_id;
            this.login_token = _login_token;
            this.timestamp_id = _timestamp_id;
            this.layer_id = _layer_id;
            this.protocol = _protocol.ToLower();
            if (protocol == "wmts")
                this.ids = string.Format("{0}_{1}", this.timestamp_id, this.layer_id);
            this.url = string.Format("{0}/{1}/{2}/{3}", URL, protocol, map_id, login_token);
            //activeView.Activate();
        }

        public void AddWMTS()
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", this.ids);
            IWMTSConnectionFactory wmtsConnFactory = new WMTSConnectionFactory();
            IWMTSConnection wmtsConnection = wmtsConnFactory.Open(propSet, 0, null);
            IWMTSServiceDescription wmtsServceDescription = wmtsConnection as IWMTSServiceDescription;
            IWMTSLayer wmtsLayer = new WMTSLayer();
            WMTSConnectionName connectionName = new WMTSConnectionName();
            connectionName.ConnectionProperties = propSet;
            wmtsLayer.Connect((IName)connectionName);
            AddWMTSData(wmtsLayer);
        }

        private void AddWMTSData(IWMTSLayer wmtslayer)
        {
            AppROT appRot = new AppROT();
            appRot.get_Item(0);
            IApplication myApp = appRot.get_Item(0);
            IMxDocument mxDocument = myApp.Document as IMxDocument;
            IMap pMap = mxDocument.FocusMap;
            ILayer pLayer = new FeatureLayer();



            pLayer = (ILayer)wmtslayer;

            pMap.AddLayer(pLayer);
        }

        private string URL;
        private string map_id;
        private string login_token;
        private string url;
        private string protocol;
        private string timestamp_id;
        private string layer_id;
        private string ids;
    }
}