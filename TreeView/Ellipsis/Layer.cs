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
            if (protocol == "wms")
                this.ids = string.Format("{0}_{1}", this.timestamp_id, this.layer_id);
            this.url = string.Format("{0}/{1}/{2}/{3}", URL, protocol, map_id, login_token);
            //activeView.Activate();
        }

        public string GetAllFootprints(Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = new StringBuilder();

            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString.Append("File: " + frame.GetFileName());
                traceString.Append(", Method:" + frame.GetMethod().Name);
                traceString.Append(", LineNumber: " + frame.GetFileLineNumber());
                traceString.Append("  -->  ");
            }

            return traceString.ToString();
        }

        public void AddWMS()
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", this.ids);
            IWMSConnectionFactory wmsConnFactory = new WMSConnectionFactory();
            IWMSConnection wmsConnection = wmsConnFactory.Open(propSet, 0, null);
            IWMSServiceDescription wmsServceDescription = wmsConnection as IWMSServiceDescription;
            IWMSMapLayer wmsMapLayer = new WMSMapLayer();
            WMSConnectionName connectionName = new WMSConnectionName();
            connectionName.ConnectionProperties = propSet;
            IDataLayer dataLayer = (IDataLayer)wmsMapLayer;
            Debug.WriteLine((IName)connectionName);
            //dataLayer.Connect((IName)connectionName);
            //wmsLayer.Connect((IName)connectionName);
            //AddWMSData(wmsLayer);
            /*
            IWMSMapLayer wmsMapLayer = new WMSMapLayer();
            IWMSGroupLayer wmsGroupLayer = new WMSGroupLayer();
            wmsGroupLayer = (IWMSGroupLayer)wmsMapLayer;

            IWMSConnectionName connectionName = new WMSConnectionName();

            IPropertySet propertySet = new PropertySet();
            propertySet.SetProperty("URL", this.url);
            connectionName.ConnectionProperties = propertySet;

            IDataLayer dataLayer = (IDataLayer)wmsGroupLayer;
            try
            {
                dataLayer.Connect((IName)connectionName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(GetAllFootprints(ex));
                System.Environment.Exit(0);
            }
            IWMSServiceDescription serviceDescription = wmsGroupLayer.WMSServiceDescription;
            IWMSLayerDescription groupDescription = serviceDescription.LayerDescription[0];

            for (int i = 0; i < groupDescription.LayerDescriptionCount; i++)
            {
                IWMSLayerDescription layerDescription = groupDescription.LayerDescription[i];
                IWMSLayer wmsLayer = wmsGroupLayer.CreateWMSLayer(layerDescription);
                AddWMSData(wmsLayer);
            }
            */
        }

        private void AddWMSData(IWMSLayer wmslayer)
        {
            AppROT appRot = new AppROT();
            appRot.get_Item(0);
            IApplication myApp = appRot.get_Item(0);
            IMxDocument mxDocument = myApp.Document as IMxDocument;
            IMap pMap = mxDocument.FocusMap;
            ILayer pLayer = new FeatureLayer();



            pLayer = (ILayer)wmslayer;

            pMap.AddLayer(pLayer);
        }

        public void AddWFS()
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("URL", this.url);
            IFeatureLayer wfsLayer = new FeatureLayer();
            IImageServerName serverName = new ImageServerName();
            serverName.Properties = propSet;
            IDataLayer dataLayer = (IDataLayer)wfsLayer;
            dataLayer.Connect((IName)serverName);
            Debug.WriteLine(this.url);
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