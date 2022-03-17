using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeView.Model
{
    class HierarchicalItem
    {
        public string Name;
        public int Depth;

        public HierarchicalItem(string name, int depth)
        {
            this.Name = name;
            this.Depth = depth;
        }
    }
}
