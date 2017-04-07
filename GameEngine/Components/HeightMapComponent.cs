using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Components
{
    public class HeightmapComponent : Component
    {
        
        public int terrainWidth  { get; private set; }
        public int terrainHeight { get; private set; }

        public float scaleFactor { get; set; }

        public int vertexCount   { get; private set; }

        public VertexPositionNormalTexture[] vertices { get; set; }

        public VertexBuffer vertexBuffer { get; set; }
        public IndexBuffer indexBuffer   { get; set; }

        public int[] indices       { get; set; }
        public float[,] heightData { get; set; }

        public Bitmap bmp { get; private set; }

        public HeightmapComponent(GraphicsDevice gd, int terrainWidth, int terrainHeight, string pictureFileName)
        {
            this.terrainHeight = terrainHeight;
            this.terrainWidth = terrainWidth;

            vertexCount = terrainWidth * terrainHeight;

            vertices = new VertexPositionNormalTexture[vertexCount];
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];

            heightData = new float[terrainWidth, terrainHeight];

            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);

            bmp = new Bitmap(pictureFileName);

        }
    }
}
