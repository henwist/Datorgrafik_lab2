
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Drawing;
using System.IO;

namespace GameEngine.Components
{
    public class HeightmapComponent : Component
    {
        public string terrainFileName { get; set; }
        public int terrainWidth  { get;  set; }
        public int terrainHeight { get;  set; }

        public Vector3 scaleFactor { get; set; }
        public Vector3 position { get; set; }

        public int vertexCount   { get;  set; }
        public int indexCount    { get;  set; }

        public VertexPositionNormalTexture[] vertices { get; set; }

        //public VertexBuffer vertexBuffer { get; set; }
        //public IndexBuffer indexBuffer   { get; set; }

        public int[] indices { get; set; }
        public float[,] heightData { get; set; }

        public Bitmap bmpHeightdata { get; private set; }

        //public Bitmap bmpTexture { get; private set; }

        public Texture2D texture { get; set; }
        public string[] textureFileNames { get; set; }

        //public Matrix objectWorld { get; set; }
        //public Matrix world { get; set; }

        public int breakUpInNumParts { get; set; }

        public Vector3 spacingBetweenParts { get; set; }

        public HeightmapComponent(GraphicsDevice gd, Vector3 scaleFactor, string terrainFileName, string[] textureFileNames/*, Matrix world*/)
        {
            this.terrainFileName = terrainFileName;

            bmpHeightdata = new Bitmap(terrainFileName);
            terrainHeight = bmpHeightdata.Height;
            terrainWidth = bmpHeightdata.Width;

            //this.terrainHeight = terrainHeight;
            //this.terrainWidth = terrainWidth;

            this.scaleFactor = scaleFactor;

            vertexCount = terrainWidth * terrainHeight;
            indexCount = (terrainWidth - 1) * (terrainHeight - 1) * 6;

            vertices = new VertexPositionNormalTexture[vertexCount];
            indices = new int[indexCount];

            heightData = new float[terrainWidth, terrainHeight];

            //vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            //indexBuffer = new IndexBuffer(gd, typeof(int), indexCount, BufferUsage.None);

            //bmpTexture = new Bitmap(textureFileName);
            //texture = Texture2D.CreateTex2DFromBitmap(bmpTexture, gd) //; bmpTexture, gd);
            //texture = Texture2D.FromStream(gd, new FileStream(textureFileName, System.IO.FileMode.Open));// CreateTex2DFromBitmap(bmpTexture, gd);
            //texture = Texture2D.FromStream(gd, new StreamReader(textureFileNames).BaseStream);
            //texture.Name = textureFileNames;
            this.textureFileNames = textureFileNames;

            //objectWorld = Matrix.Identity;
            //this.world = world;

            breakUpInNumParts = 1;

        }

        public  HeightmapComponent()
        {

        }
    }
}
