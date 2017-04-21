using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Datorgrafik_lab2.InstanceContainers
{
    public class InstanceStack
    {
        private const int MAXSIZE = 50;
        private int index;
        private Matrix[] instances;

        public InstanceStack()
        {
            instances = new Matrix[MAXSIZE];
            index = 0;
        }

        public void push(Matrix matrix)
        {
            instances[index++] = matrix;
        }

        public Matrix pop()
        {
            return instances[--index];
        }
    }
}
