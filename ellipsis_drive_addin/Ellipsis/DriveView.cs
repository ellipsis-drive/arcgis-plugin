using Ellipsis.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ellipsis.Drive
{
    //These are basically describing how event handler functions (or CB's) have to look.
    public delegate void LayerEvent(JObject block, JObject layer);
    public delegate void TimestampEvent(JObject block, JObject timeStamp, JObject visualization, string protocol);

    class DriveView
    {
        private System.Windows.Forms.TreeView view;
        private Connect connect;
        private TextBox searchInput;

        //Event fires when user doubleclicks on a (vector) layer.
        public event LayerEvent onLayerClick;
        //Event fires when user doubleclicks on a (raster) timestamp.
        public event TimestampEvent onTimestampClick;
        //Event fires when user selects a (vector) layer.
        public event LayerEvent onLayerSelect;
        //Event fires when user selects a (raster) timestamp.
        public event TimestampEvent onTimestampSelect;

        //Load ellipsis drive folders in view. Images are used to show folder and corresponding block icons.
        //DriveView will handle searchInput changes, refreshButton clicks and openInBrowser clicks.
        public DriveView(System.Windows.Forms.TreeView view, Connect connect, ImageList images, TextBox searchInput, Button refreshButton, Button openInBrowser)
        {
            this.view = view;
            this.connect = connect;

            if (images != null && images.Images != null && images.Images.Count >= 3)
            {
                view.ImageList = images;
            }

            if (searchInput != null)
            {
                searchInput.TextChanged += handleSearch;
                this.searchInput = searchInput;
            }

            if (refreshButton != null)
            {
                refreshButton.Click += handleRefreshClick;
            }

            if (openInBrowser != null)
            {
                openInBrowser.Click += (sender, e) => handleOpenInBrowserClick(sender, e);
            }

            view.Indent = 12;
            view.BeforeExpand += handleNodeExpand;

            //Empty event handlers because we cannot pass empty event handlers into runInfoCallback.
            onLayerClick += (a, b) => { };
            onTimestampClick += (a, b, c, d) => { };
            onLayerSelect += (a, b) => { };
            onTimestampSelect += (a, b, c, d) => { };

            //Map node click event handlers to run info callbacks.
            view.NodeMouseDoubleClick += (sender, e) => runInfoCallbackForNode(e.Node, onLayerClick.Invoke, onTimestampClick.Invoke);
            view.AfterSelect += (sender, e) => runInfoCallbackForNode(e.Node, onLayerSelect.Invoke, onTimestampSelect.Invoke);

            //Set root nodes.
            resetNodes();
        }

        //Open current selected node in browser. Return process of current browser.
        private System.Diagnostics.Process handleOpenInBrowserClick(object sender, EventArgs e)
        {
            if (view.SelectedNode == null || view.SelectedNode.Name == getLoadingNode().Name) return null;

            string baseUrl = "https://app.ellipsis-drive.com";
            if (view.SelectedNode.Level == 0)
            {
                return System.Diagnostics.Process.Start($"{baseUrl}/drive/{view.SelectedNode.Name}");
            }

            if (view.SelectedNode.Tag != null)
            {
                JObject selectedInfo = view.SelectedNode.Tag as JObject;
                if (selectedInfo.Value<string>("type") == "folder")
                {
                    var path = selectedInfo.Value<JObject>("path");
                    string root = path.Value<string>("root");
                    string pathId = path.Value<JArray>("path")[0].Value<string>("id");

                    return System.Diagnostics.Process.Start($"{baseUrl}/drive/{root}?pathId={pathId}");

                }
                if (selectedInfo.Value<string>("type") == "map" || selectedInfo.Value<string>("type") == "shape")
                {
                    return System.Diagnostics.Process.Start($"{baseUrl}/view?mapId={selectedInfo.Value<string>("id")}");
                }

                System.Diagnostics.Process started = null;
                runInfoCallbackForNode(view.SelectedNode, (block, layer) =>
                {
                    started = System.Diagnostics.Process.Start($"{baseUrl}/view?mapId={block.Value<string>("id")}");
                }, (block, timestamp, visualization, protocol) =>
                {
                    started = System.Diagnostics.Process.Start($"{baseUrl}/view?mapId={block.Value<string>("id")}");
                });

                return started;
            }

            return null;
        }

        //Refreshes view.
        private void handleRefreshClick(object sender, EventArgs e)
        {
            if (searchInput.Text.Trim() == "")
            {
                resetNodes();
            }
            else if (view.Nodes.Count != 1 || view.Nodes[0].Name != getLoadingNode().Name)
            {
                _ = searchDrive(searchInput.Text);
            }
        }

        //Resets nodes to the three root nodes.
        public void resetNodes()
        {
            view.Nodes.Clear();
            view.Nodes.AddRange(new TreeNode[] {
            getNode("My Drive", "myMaps"),
            getNode("Shared", "shared"),
            getNode("Favorites", "favorites")
        });

            foreach (TreeNode rootFolder in view.Nodes)
            {
                rootFolder.Nodes.Add(getLoadingNode());
                rootFolder.ImageIndex = 0;
                rootFolder.SelectedImageIndex = 0;
            }
        }

        //Handle searchInput change.
        private void handleSearch(object sender, EventArgs e)
        {
            if (searchInput.Text.Trim() == "")
            {
                resetNodes();
                return;
            }
            _ = searchDrive(searchInput.Text);
        }

        //Looks at context of node to find all relevant information. Runs layerCb if it's
        //a shape and timestampCb if it's a map. See delegates for parameters of these cb's.
        private void runInfoCallbackForNode(TreeNode node, LayerEvent layerCb, TimestampEvent timestampCb)
        {
            JObject nodeTag = node.Tag as JObject;
            string baseUrl = "https://api.ellipsis-drive.com/v2/ogc";
            if (nodeTag == null) return;

            if (nodeTag.Value<string>("method") != null)
            {
                //node tag is visualization
                string protocol = node.Parent.Name;
                JObject timestamp = node.Parent.Parent.Tag as JObject;
                JObject block = node.Parent.Parent.Parent.Tag as JObject;
                JObject maplayer = block["mapLayers"][0] as JObject;
                timestampCb(block, timestamp, nodeTag, protocol);
            }

            else if (nodeTag.Value<string>("dateFrom") != null)
            {
                //node tag is timestamp
                //This is a protocol with no visualization or the WMTS protocol that doesn't render visualizations
                if (node.Nodes.Count == 0)
                {
                    string protocol = node.Text;
                    JObject timestamp = nodeTag;
                    JObject block = node.Parent.Parent.Tag as JObject;
                    JObject maplayer = block["mapLayers"][0] as JObject;
                    timestampCb(block, timestamp, null, protocol);
                    Layers layer = new Layers(baseUrl, block.Value<string>("id"), this.connect.GetLoginToken(), protocol, timestamp.Value<string>("id"), maplayer.Value<string>("id"));
                    layer.AddWMTS();
                }
            }

            else if (nodeTag.Value<JObject>("extent") != null)
            {
                JObject block = node.Parent.Tag as JObject;
                //node tag is geometryLayer
                layerCb(block, nodeTag);
                string message = "Vector layers are no longer supported in ArcMap. The ArcGIS Pro plugin does support vector layers.";
                string title = "Notice";
                MessageBox.Show(message, title, MessageBoxButtons.OK);
            }
        }

        //Handle expansion of node. Will start loading if needed.
        private void handleNodeExpand(object sender, TreeViewCancelEventArgs e)
        {
            //Every folder contains loading node by default, so upon opening, convert this to contents.
            if (e.Action != TreeViewAction.Expand) return;
            if (e.Node.Nodes == null || e.Node.Nodes.Count != 1 || e.Node.Nodes[0].Name != "loading") return;

            var loadingMarker = e.Node.Nodes[0];
            if (!loadingMarker.Name.Equals("loading")) return;

            _ = loadFolder(e.Node);
        }

        //Constructs a treenode.
        private TreeNode getNode(string name, string id = null, object tag = null)
        {
            return new TreeNode
            {
                Text = name,
                Name = id,
                Tag = tag,
                ImageIndex = 3,
                SelectedImageIndex = 3,
                StateImageIndex = 3
            };
        }

        //Constructs a loading treenode.
        private TreeNode getLoadingNode()
        {
            return getNode("Loading...", "loading");
        }

        //Gets folder node from info. Checks for all render conditions.
        private TreeNode getFolderNode(JObject info)
        {
            TreeNode folder = getNode(info.Value<string>("name"), info.Value<string>("id"));
            folder.Tag = info;
            folder.ImageIndex = 0;
            folder.SelectedImageIndex = 0;
            folder.StateImageIndex = 0;

            if (info.Value<bool>("deleted"))
            {
                folder.Text += " (folder deleted)";
                return folder;
            }
            if (info.Value<bool>("disabled"))
            {
                folder.Text += " (folder disabled)";
                return folder;
            }
            if (info.Value<JObject>("yourAccess").Value<int>("accessLevel") < 200)
            {
                folder.Text += " (access level too low)";
                return folder;
            }

            folder.Nodes.Add(getLoadingNode());
            return folder;
        }

        //Converts layers into a list of corresponding treenodes.
        private List<TreeNode> getLayerNodes(JArray layers)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            var children = layers.Children<JObject>();
            foreach (var child in children)
            {
                //TODO parse {availability: {blocked: false}} ??
                if (child.Value<bool>("deleted"))
                    continue;
                TreeNode parsedChild = getNode(child.Value<string>("name"), child.Value<string>("id"));
                parsedChild.Tag = child;
                nodes.Add(parsedChild);
            }
            return nodes;
        }

        //Converts visualizations into a list of corresponding treenodes.
        private List<TreeNode> getVisualizationNodes(JArray visualizations)
        {
            List<TreeNode> visualizationNodes = new List<TreeNode>();
            foreach (var child in visualizations.Children<JObject>())
            {
                visualizationNodes.Add(getNode(child.Value<string>("name"), child.Value<string>("id"), child));
            }

            return visualizationNodes;
        }

        //Get treenodes to represent the protocols.
        private TreeNode getProtocolNode(JObject timestamp)
        {
            TreeNode t = new TreeNode();
            t = getNode("WMTS", "WMTS", timestamp);
            return t;
        }

        //Convert timestamps into a list of corresponding treenodes.
        private List<TreeNode> getTimestampNodes(JArray timestamps)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            var children = timestamps.Children<JObject>();
            foreach (var child in children)
            {
                //TODO parse {availability: {blocked: false}} ??
                if (child.Value<string>("status") != "finished")
                    continue;
                TreeNode parsedChild = getNode(child.Value<string>("dateFrom") + " - " + child.Value<string>("dateTo"), child.Value<string>("id"));
                parsedChild.Tag = child;
                nodes.Add(parsedChild);
            }
            return nodes;
        }

        //Gets block node from info. Checks for all render conditions.
        private TreeNode getBlockNode(JObject info)
        {
            //Check for deleted, no time stamps, no layers, disabled, low access level
            TreeNode block = getNode(info.Value<string>("name"), info.Value<string>("id"));
            block.Tag = info;

            if (info.Value<bool>("deleted"))
            {
                block.Text += " (block deleted)";
                return block;
            }
            if (info.Value<bool>("disabled"))
            {
                block.Text += " (block disabled)";
                return block;
            }
            if (info.Value<JObject>("yourAccess").Value<int>("accessLevel") < 200)
            {
                block.Text += " (access level too low)";
                return block;
            }

            if (info.Value<string>("type") == "shape")
            {
                JArray layers = info.Value<JArray>("geometryLayers");
                block.ImageIndex = 2;
                block.SelectedImageIndex = 2;
                block.StateImageIndex = 2;
                if (layers.Count == 0)
                {
                    block.Text += " (shape has no layers)";
                    return block;
                }

                var layerNodes = getLayerNodes(layers);
                if (layerNodes.Count == 0)
                {
                    block.Text += " (shape has no layers)";
                    return block;
                }

                block.Nodes.AddRange(layerNodes.ToArray());
            }
            else if (info.Value<string>("type") == "map")
            {
                JArray timestamps = info.Value<JArray>("timestamps");
                block.ImageIndex = 1;
                block.SelectedImageIndex = 1;
                block.StateImageIndex = 1;
                if (timestamps.Count == 0)
                {
                    block.Text += " (map has no timestamps)";
                    return block;
                }

                var timestampNodes = getTimestampNodes(timestamps);

                if (timestampNodes.Count == 0)
                {
                    block.Text += " (map has no active timestamps)";
                    return block;
                }

                JArray visualizations = info.Value<JArray>("mapLayers");
                timestampNodes.ForEach(t =>
                {
                    TreeNode protocol = getProtocolNode((JObject)t.Tag);
                    t.Nodes.Add(protocol);

                });
                block.Nodes.Add(timestampNodes.ToArray()[0]);
            }

            return block;
        }

        //Fetch all folders inside parent folder.
        private List<TreeNode> fetchFolders(TreeNode parent)
        {
            List<TreeNode> buffer = new List<TreeNode>();

            string nextFolderPageStart = null;
            do
            {
                JObject nestedFolders = connect.GetPath(parent.Name, true, nextFolderPageStart, parent.Level == 0);
                foreach (JObject nestedFolder in nestedFolders["result"]) // <-- Note that here we used JObject instead of usual JProperty
                {
                    //nestedFolders.Value<List<JObject>>("result").ForEach(x => buffer.Add(getFolderNode(x)));
                    buffer.Add(getFolderNode(nestedFolder));
                }
                nextFolderPageStart = nestedFolders.Value<string>("nextPageStart");
            } while (nextFolderPageStart != null);

            return buffer;
        }

        //Fetch all blocks inside parent folder
        private List<TreeNode> fetchBlocks(TreeNode parent)
        {
            List<TreeNode> buffer = new List<TreeNode>();
            string nextBlockPageStart = null;
            do
            {
                JObject nestedBlocks = connect.GetPath(parent.Name, false, nextBlockPageStart, parent.Level == 0);
                foreach (JObject nestedFolder in nestedBlocks["result"]) // <-- Note that here we used JObject instead of usual JProperty
                {
                    buffer.Add(getBlockNode(nestedFolder));
                }
                nextBlockPageStart = nestedBlocks.Value<string>("nextPageStart");
            } while (nextBlockPageStart != null);

            return buffer;
        }

        //Fetch all blocks and folders inside folder asynchrounously
        private async Task loadFolder(TreeNode folder)
        {
            var results = await Task.WhenAll(
                Task.Run(() => fetchFolders(folder)),
                Task.Run(() => fetchBlocks(folder)));

            folder.Nodes[0].Remove();
            folder.Nodes.AddRange(results[0].ToArray());
            folder.Nodes.AddRange(results[1].ToArray());
        }

        //Search for folders with name. Stops when searchInput changed.
        private List<TreeNode> searchFolders(string name)
        {
            List<TreeNode> buffer = new List<TreeNode>();

            string nextFolderPageStart = null;
            do
            {
                if (name != searchInput.Text)
                {
                    return null;
                }

                Console.WriteLine("Searching for folder");

                JObject folders = connect.SearchByName(name, "folders", nextFolderPageStart);
                foreach (JObject folder in folders["result"])
                {
                    buffer.Add(getFolderNode(folder));
                }
                nextFolderPageStart = folders.Value<string>("nextPageStart");
            } while (nextFolderPageStart != null);

            return buffer;
        }

        //Search for blocks with name with specified type by isShape. Stops when searchInput changed.
        private List<TreeNode> searchBlocks(string name, bool isShape)
        {
            List<TreeNode> buffer = new List<TreeNode>();
            string nextBlockPageStart = null;
            do
            {
                if (name != searchInput.Text)
                {
                    return null;
                }

                Console.WriteLine($"Searching for {(isShape ? "shape" : "map")}");

                JObject blocks = connect.SearchByName(name, isShape ? "shapes" : "maps", nextBlockPageStart);
                foreach (JObject block in blocks["result"])
                {
                    buffer.Add(getBlockNode(block));
                }
                nextBlockPageStart = blocks.Value<string>("nextPageStart");
            } while (nextBlockPageStart != null);

            return buffer;
        }

        //Searches drive asynchronously until name is not equal to searchInput text.
        private async Task searchDrive(string name)
        {
            view.Nodes.Clear();
            view.Nodes.Add(getLoadingNode());

            var results = await Task.WhenAll(
                Task.Run(() => searchFolders(name)),
                Task.Run(() => searchBlocks(name, false)),
                Task.Run(() => searchBlocks(name, true)));

            if (name != searchInput.Text || results.Any((x) => x == null)) return;

            view.Nodes.Clear();
            for (int i = 0; i < results.Length; i++)
                view.Nodes.AddRange(results[i].ToArray());
        }
    }
}
