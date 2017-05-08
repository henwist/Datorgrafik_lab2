using Datorgrafik_lab2.CreateModels;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datorgrafik_lab2.InstanceContainers
{
    public class InstanceTree
    {

        private InstanceTree parent;
        private Dictionary<string, InstanceTree> childNodes;

        private string nodeName;
        private Matrix nodeTransform;

        public InstanceTree(string nodeName, Matrix nodeTransform)
        {
            childNodes = new Dictionary<string, InstanceTree>();

            this.parent = null;
            this.nodeName = nodeName;
            this.nodeTransform = nodeTransform;

        }

        public void AddChild(InstanceTree node)
        {

            if (!this.childNodes.ContainsKey(node.nodeName))
            {
                node.parent = this;
                this.childNodes.Add(node.nodeName, node);
            }

        }

        public Matrix GetParentTransforms()
        {
            if (this.parent != null)
                return this.nodeTransform * this.parent.GetParentTransforms();

            else
                return this.nodeTransform * Matrix.Identity;
        }
    }
}
