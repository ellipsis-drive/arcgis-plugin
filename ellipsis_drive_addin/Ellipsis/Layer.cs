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

        public Layers(string _URL, string _map_id, string _login_token, string _protocol, string _timestamp_id, string _layer_id, string _date_from = null, string _timestamp = null, string _map_name = null, string _vis_name = null)
        {
            this.URL = _URL;
            this.map_id = _map_id;
            this.login_token = _login_token;
            this.timestamp_id = _timestamp_id;
            this.layer_id = _layer_id;
            this.protocol = _protocol.ToLower();
            map_name = _map_name;
            vis_name = _vis_name;
            this.ids = string.Format("{0}_{1}", this.timestamp_id, layer_id);
            this.url = string.Format("{0}/{1}/{2}/{3}", URL, protocol, map_id, login_token);
            if (_date_from != null && _timestamp != null)
            {
                string[] tokens = _date_from.Split(' ');
                this.stamp_name = tokens[0].Split('/')[2] + '-' + tokens[0].Split('/')[0] + '-' + tokens[0].Split('/')[1] + '_' + _timestamp;
            }
            if (protocol == "wms" || protocol == "wcs")
                this.url = string.Format("{0}/{1}/{2}/{3}?version=1.0.0", URL, protocol, map_id, login_token);
            //activeView.Activate();
            connect = new Connect();
        }

        public bool checkDuplicate(IWMSServiceDescription wmsServiceDescription)
        {
            var mapLayers = ArcMap.Document.ActiveView.FocusMap.Layers;
            for (var layer = mapLayers.Next(); layer != null; layer = mapLayers.Next())
            {
                if (layer.Name == wmsServiceDescription.WMSTitle)
                {
                    // Root folder exists, check if Map name exists
                    IGroupLayer rootGroup = layer as IGroupLayer;
                    ICompositeLayer rootComp = layer as ICompositeLayer;
                    for (int j = 0; j < rootComp.Count; ++j)
                    {
                        if (rootComp.Layer[j].Name == map_name)
                        {
                            // Map exists, check if timestamp exists
                            ICompositeLayer mapComp = rootComp.Layer[j] as ICompositeLayer;
                            if (mapComp == null)
                                continue;
                            for (int x = 0; x < mapComp.Count; ++x)
                            {
                                if (mapComp.Layer[x].Name == stamp_name)
                                {
                                    // Timestamp exists, check if visualization exists
                                    ICompositeLayer stampComp = mapComp.Layer[x] as ICompositeLayer;
                                    if (stampComp == null)
                                        continue;
                                    for (int y = 0; y < stampComp.Count; ++y)
                                    {
                                        bool subset = false;
                                        if (stampComp.Layer[y].Name != vis_name)
                                        {

                                            // In the future, it would be nice to merge the trees together
                                            // For now, just create a separate tree by returning
                                            // TODO: if current visualization is subset of new visualization, delete old visualization
                                            subset = true;           
                                        }
                                        if (subset == false)
                                            return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public void AddWMTS()
        {
            IPropertySet propSet = new PropertySetClass();
            IGroupLayer groupLayer = new GroupLayer();
            propSet.SetProperty("URL", url);
            IWMTSConnectionFactory wmtsConnFactory = new WMTSConnectionFactory();
            IWMTSConnection wmtsConnection = wmtsConnFactory.Open(propSet, 0, null);
            IWMTSServiceDescription wmtsServiceDescription = wmtsConnection as IWMTSServiceDescription;
            for (int i = 0; i < wmtsServiceDescription.LayerDescriptionCount; i++)
            {
                IWMTSLayerDescription layerDescription = wmtsServiceDescription.LayerDescription[i];
                IWMTSLayer wmtsLayer = new WMTSLayer();

                IPropertySet propSet_1 = new PropertySetClass();
                propSet_1.SetProperty("URL", url);
                propSet_1.SetProperty("LayerName", layerDescription.Identifier);

                WMTSConnectionName connectionName = new WMTSConnectionName();
                connectionName.ConnectionProperties = propSet_1;
                wmtsLayer.Connect((IName)connectionName);
                groupLayer.Add(wmtsLayer as ILayer);
            }
            groupLayer.Name = wmtsServiceDescription.WMTSTitle;
            IMap pMap = ArcMap.Document.ActiveView.FocusMap;
            pMap.AddLayer(groupLayer as ILayer);
        }

        public void AddWMSSingle()
        {
            IPropertySet propSet = new PropertySetClass();
            IWMSGroupLayer wmsMapLayer = new WMSMapLayer() as WMSGroupLayer;
            IWMSConnectionName connName = new WMSConnectionName();
            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", ids);
            propSet.SetProperty("SRS", "EPSG:3857");
            connName.ConnectionProperties = propSet;
            IDataLayer dataLayer = wmsMapLayer as IDataLayer;
            dataLayer.Connect((IName)connName);
            IWMSServiceDescription wmsServiceDescription = wmsMapLayer.WMSServiceDescription;
            IMap pMap = ArcMap.Document.ActiveView.FocusMap;
            if (checkDuplicate(wmsServiceDescription) == false)
                // First extend tree structure for single nodes, so we can check duplicates...
                pMap.AddLayer(wmsMapLayer as ILayer);
        }

        public void AddWMSGroup()
        {
            IPropertySet propSet = new PropertySetClass();
            IWMSGroupLayer wmsMapLayer = new WMSMapLayer() as WMSGroupLayer;
            IWMSConnectionName connName = new WMSConnectionName();
            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", ids);
            propSet.SetProperty("SRS", "EPSG:3857");
            connName.ConnectionProperties = propSet;
            IDataLayer dataLayer = wmsMapLayer as IDataLayer;
            dataLayer.Connect((IName)connName);
            IWMSServiceDescription wmsServiceDescription = wmsMapLayer.WMSServiceDescription;
            IMap pMap = ArcMap.Document.ActiveView.FocusMap;
            // Filter unwanted timestamps
            // Root folder does not exist, just add the tree and set nodes to visible
            // Adding the tree also adds timestamps we don't want, so remove layers with those timestamps
            IWMSGroupLayer stampLayer = new WMSGroupLayer();
            IWMSGroupLayer mapLayer = new WMSGroupLayer();
            for (int i = 0; i < wmsMapLayer.Count; ++i)
            {
                mapLayer = wmsMapLayer.Layer[i] as IWMSGroupLayer;
                for (int j = 0; j < mapLayer.Count; ++j)
                {
                    if (mapLayer.Layer[j].Name == stamp_name)
                    {
                        stampLayer = mapLayer.Layer[j] as IWMSGroupLayer;
                        mapLayer.Clear();
                        mapLayer.Add(stampLayer as ILayer);
                        break;
                    }
                }
                wmsMapLayer.Clear();
                wmsMapLayer.Add(mapLayer as ILayer);
            }
            // Now we have that wmsMapLayer is a tree structure.
            // Check if this map already exists.
            // We use the path as a uid for the map: first check if root name exists, then if map name exists, then if timestamp exists, then if visualization exists
            // The map is represented as a tree-like structure in arcmap. If you add a visualization from a map that already
            // exists in the table of contents, a new tree is created, even though it would be prefered to add the new visualization
            // to the tree that already exists. It is not possible to add nodes to existing trees in levels higher than 1.
            // Solution: delete old tree, and add new tree with new visualization?
            
            if (checkDuplicate(wmsServiceDescription) == false)
            {
                pMap.AddLayer(wmsMapLayer as ILayer);

                for (int i = 0; i < pMap.LayerCount; ++i)
                {
                    // Set root layer to visible
                    pMap.Layer[i].Visible = true;

                    ICompositeLayer rootLayer = pMap.Layer[i] as ICompositeLayer;
                    // Would be neater as a recursive DFS but the WMS layer order is always the same so this will work
                    for (int j = 0; j < rootLayer.Count; ++j)
                    {
                        // Set map layer to visible
                        rootLayer.Layer[j].Visible = true;

                        ICompositeLayer mapComp = rootLayer.Layer[j] as ICompositeLayer;
                        if (mapComp == null)
                            continue;
                        for (int x = 0; x < mapComp.Count; ++x)
                        {
                            mapComp.Layer[x].Visible = true;

                            ICompositeLayer timestampLayer = mapComp.Layer[x] as ICompositeLayer;
                            if (timestampLayer == null)
                                continue;
                            for (int y = 0; y < timestampLayer.Count; ++y)
                            {
                                // Set visualization layers to visible
                                timestampLayer.Layer[y].Visible = true;
                            }
                        }
                    }
                }
                ArcMap.Document.UpdateContents();
                ArcMap.Document.ActivatedView.Refresh();
            }
        }

        public void AddWCS()
        {
            IPropertySet propSet = new PropertySetClass();
            propSet.SetProperty("URL", this.url);
            propSet.SetProperty("LayerName", this.layer_id);
            IWCSConnectionFactory wcsConnFactory = new WCSConnectionFactory();
            IWCSConnection wcsConnection = wcsConnFactory.Open(propSet, 0, null);
            IWCSServiceDescription wcsServceDescription = wcsConnection as IWCSServiceDescription;
            IWCSLayer wcsLayer = new WCSLayer();
            WCSConnectionName connectionName = new WCSConnectionName();
            connectionName.ConnectionProperties = propSet;
            IDataLayer dataLayer = (IDataLayer)wcsLayer;
            dataLayer.Connect((IName)connectionName);
            AddData((ILayer)wcsLayer);
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
            // Get current map
            IMap pMap = ArcMap.Document.ActiveView.FocusMap;//mxDocument.FocusMap;

            // Check if there already exists a layer in the map with this ID
            /*
            for (int i = 0; i < pMap.LayerCount; ++i)
            {
                ILayerExtensions mapExtension = pMap.Layer[i] as ILayerExtensions;
                IServerLayerExtension mapServerExtension = mapExtension.get_Extension(0) as IServerLayerExtension;
                IPropertySet mapProperty = mapServerExtension.ServerProperties;
                if (mapProperty != null)
                    if (mapProperty.GetProperty("ServiceLayerID") as string == ids && mapProperty.GetProperty("ServiceLayerProtocol") as string == protocol)
                        return;
            }
            */
            // Initialize layer ID, so we can keep track if it's already added:
            ILayerExtensions pExtension = pLayer as ILayerExtensions;
            IServerLayerExtension pServerExtension = new ServerLayerExtension();
            pExtension.AddExtension(pServerExtension);
            //pExtension.get_Extension(0) as IServerLayerExtension;
            IPropertySet pSet = new PropertySet();
            pSet.SetProperty("ServiceLayerID", ids);
            pSet.SetProperty("ServiceLayerProtocol", protocol);
            pServerExtension.ServerProperties = pSet;
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
        private string stamp_name;
        private string map_name;
        private string vis_name;
    }
}