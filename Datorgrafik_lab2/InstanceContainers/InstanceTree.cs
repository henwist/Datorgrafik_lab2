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
        public Matrix nodeTransform { get; set; }

        public Texture2D texture { get; private set; }

        public InstanceTree(string nodeName, Matrix nodeTransform, Texture2D texture)
        {
            childNodes = new Dictionary<string, InstanceTree>();

            this.parent = null;
            this.nodeName = nodeName;
            this.nodeTransform = nodeTransform;
            this.texture = texture;

        }


        public InstanceTree GetInstanceTree(string nodeName)
        {
            InstanceTree part = null;

            if (this.nodeName.Equals(nodeName))
                return this;

            foreach (InstanceTree instance in childNodes.Values)
            {
                part = GetInstanceTree(nodeName, instance);

                if (part != null)
                    return part;
            }

            return part;
        }


        private InstanceTree GetInstanceTree(string name, InstanceTree instance)
        {
            if (this.nodeName.Equals(name))
                return this;

            return instance.GetInstanceTree(name);
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
                return this.parent.nodeTransform * this.parent.GetParentTransforms();

            else
                return this.nodeTransform;
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
