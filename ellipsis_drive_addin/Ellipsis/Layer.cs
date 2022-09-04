using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Diagnostics;
using System;
using System.Text;
using ellipsis_drive_addin;
using System.Xml;
using ESRI.ArcGIS.Catalog;

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
            this.ids = string.Format("{0}_{1}", this.timestamp_id, this.layer_id);
            this.url = string.Format("{0}/{1}/{2}/{3}", URL, protocol, map_id, login_token);
            if (protocol == "wms" || protocol == "wcs")
                this.url = string.Format("{0}/{1}/{2}/{3}?version=1.0.0", URL, protocol, map_id, login_token);
            //activeView.Activate();
            connect = new Connect();
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
            Debug.WriteLine("url");
            Debug.WriteLine(url);
            ILayer pLayer = new FeatureLayer();

            pLayer = (ILayer)wmtsLayer;

            AddData(pLayer);
        }

        public void AddWMS()
        {
            IPropertySet propSet = new PropertySetClass();
            
            IWMSGroupLayer wmsMapLayer = new WMSMapLayer() as IWMSGroupLayer;
            IWMSConnectionName connName = new WMSConnectionName();

            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", ids);
            propSet.SetProperty("SRS", "EPSG:3857");

            connName.ConnectionProperties = propSet;

            IDataLayer dataLayer = (IDataLayer)wmsMapLayer;
            dataLayer.Connect((IName)connName);
            AddData((ILayer)wmsMapLayer);
        }

        public void AddWCS()
        {
            String[] s = url.Trim().Split('?');
            url = s[0] + "?request=GetCapabilities&service=WCS";
            String response = connect.SubmitHTTPRequest("", url);
            XmlDocument xmlDocument = new XmlDocument();
            try { xmlDocument.LoadXml(response); }
            catch (XmlException xmlEx)
            { }

            XmlNodeList contentMetadata = xmlDocument.GetElementsByTagName("CoverageSummary");
            Debug.WriteLine(contentMetadata.Count);
            if (contentMetadata != null && contentMetadata.Count > 0)
            {
                XmlNodeList coverageList = contentMetadata.Item(0).ChildNodes;
                foreach (XmlNode coverage in coverageList)
                {
                    if (coverage.Name.ToLower().Equals("identifier"))
                    {
                        url = s[0] + "?request=GetCoverage&service=WCS&format=GeoTIFF&coverage=" + coverage.InnerText;
                        Debug.WriteLine("url");
                        Debug.WriteLine(url);
                        try
                        {
                            String filePath = connect.SubmitHTTPRequest("DOWNLOAD", url);
                            AddAGSService(filePath);
                            
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }

            }
        }

        private void AddAGSService(string fileName)
        {
            try 
            {
                AppROT appRot = new AppROT();
                IApplication myApp = appRot.get_Item(0);
                IMxDocument mxDoc = myApp.Document as IMxDocument;
                IMap map = (IMap)mxDoc.FocusMap;
                IActiveView activeView = (IActiveView)map;
                if (fileName.ToLower().Contains("http") && !fileName.ToLower().Contains("arcgis/rest"))
                {
                    if (fileName.EndsWith("MapServer"))
                        fileName = fileName.Remove(fileName.LastIndexOf("MapServer"));

                    String[] s = fileName.ToLower().Split(new String[] { "/services" }, StringSplitOptions.RemoveEmptyEntries);

                    IPropertySet propertySet = new PropertySetClass();
                    propertySet.SetProperty("URL", s[0] + "/services"); // fileName

                    IMapServer mapServer = null;

                    IAGSServerConnectionFactory pAGSServerConFactory = new AGSServerConnectionFactory();
                    IAGSServerConnection agsCon = pAGSServerConFactory.Open(propertySet, 0);
                    IAGSEnumServerObjectName pAGSSObjs = agsCon.ServerObjectNames;
                    IAGSServerObjectName pAGSSObj = pAGSSObjs.Next();

                    while (pAGSSObj != null)
                    {
                        if (pAGSSObj.Type == "MapServer" && pAGSSObj.Name.ToLower() == s[1].TrimStart('/').TrimEnd('/'))
                        {
                            break;
                        }
                        pAGSSObj = pAGSSObjs.Next();
                    }


                    IName pName = (IName)pAGSSObj;
                    IAGSServerObject pAGSO = (IAGSServerObject)pName.Open();
                    mapServer = (IMapServer)pAGSO;


                    IPropertySet prop = new PropertySetClass();
                    prop.SetProperty("URL", fileName);
                    prop.SetProperty("Name", pAGSSObj.Name);


                    IMapServerLayer layer = new MapServerLayer() as IMapServerLayer;
                    layer.ServerConnect(pAGSSObj, mapServer.get_MapName(0));


                    mxDoc.AddLayer((ILayer)layer);

                }
                else
                {

                    IGxFile pGxFile;

                    if (fileName.ToLower().EndsWith(".tif"))
                    {
                        IRasterLayer pGxLayer = (IRasterLayer)new RasterLayer();
                        pGxLayer.CreateFromFilePath(fileName);
                        if (pGxLayer.Valid)
                        {
                            map.AddLayer((ILayer)pGxLayer);
                        }
                    }
                    else
                    {
                        IGxLayer pGxLayer = new GxLayer();
                        pGxFile = (GxFile)pGxLayer;
                        pGxFile.Path = fileName;

                        if (pGxLayer.Layer != null)
                        {
                            map.AddLayer(pGxLayer.Layer);
                        }
                    }

                }
            }
            catch (Exception e) { }
        }

        private void AddData(ILayer pLayer)
        {
            Debug.WriteLine("Layer");
            Debug.WriteLine(pLayer);
            AppROT appRot = new AppROT();
            appRot.get_Item(0);
            IApplication myApp = appRot.get_Item(0);
            IMxDocument mxDocument = myApp.Document as IMxDocument;
            IMap pMap = mxDocument.FocusMap;

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
        private Connect connect;
    }
}