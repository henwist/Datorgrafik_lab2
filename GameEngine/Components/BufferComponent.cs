using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Components
{
    public class BufferComponent : Component
    {
        public VertexBuffer VertexBuffer { get; set; }

        public IndexBuffer IndexBuffer { get; set; }

        public  VertexPositionNormalTexture[] Vertices { get; set; }

        public int[] Indices { get; set; }

        public Texture2D[] Texture { get; set; }

        public int PrimitiveCount { get; set; }

        public PrimitiveType PrimitiveType { get; set; }
    }
}
