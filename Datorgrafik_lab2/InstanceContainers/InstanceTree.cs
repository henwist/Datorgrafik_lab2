using Datorgrafik_lab2.CreateModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Datorgrafik_lab2.InstanceContainers
{
    public class InstanceTree : IEnumerable<InstanceTree>
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
                return this.parent.GetParentTransforms() * this.nodeTransform;

            else
                return this.nodeTransform * Matrix.Identity;
        }


        public IEnumerator<InstanceTree> GetEnumerator()
        {
            return childNodes.Values.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
           return this.GetEnumerator();
        }
    }
}
