using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using System.Diagnostics;
using System;
using System.Text;

namespace Ellipsis.Api
{
    class Layers
    {
        public Layers() { }

        public Layers(string _URL, string _map_id, string _login_token, string _protocol)
        {
            this.URL = _URL;
            this.map_id = _map_id;
            this.login_token = _login_token;
            this.protocol = _protocol.ToLower();
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

        public void AddWMTS()
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("URL", this.url);

            IWMTSConnectionFactory wmtsConnFactory = new WMTSConnectionFactory();
            IWMTSConnection wmtsConnection = wmtsConnFactory.Open(propSet, 0, null);
            IWMTSServiceDescription wmtsServceDescription = wmtsConnection as IWMTSServiceDescription;

            for (int i = 0; i < wmtsServceDescription.LayerDescriptionCount; i++)
            {
                IWMTSLayerDescription layerDescription = wmtsServceDescription.get_LayerDescription(i);
                IWMTSLayer wmtsLayer = new WMTSLayer();

                IPropertySet propSet_1 = new PropertySetClass();
                propSet_1.SetProperty("URL", this.url);
                propSet_1.SetProperty("LayerName", layerDescription.Identifier);

                WMTSConnectionName connectionName = new WMTSConnectionName();
                connectionName.ConnectionProperties = propSet_1;
                wmtsLayer.Connect((IName)connectionName);
                AddWMTSData(wmtsLayer);
            }
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
    }
}

/*
 * Python example:
 * theurl = F"{URL}/wmts/{mapid}/{self.loginToken}"
            actualurl = f"tileMatrixSet=matrix_{self.currentZoom}&crs=EPSG:3857&layers={ids}&styles=&format=image/png&url={theurl}"
            log(actualurl)
            rlayer = QgsRasterLayer(actualurl, f"{self.currentTimestamp['dateTo']}_{itemdata['name']}", 'wms')
 */
