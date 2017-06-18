using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datorgrafik_lab2.CreateModels
{
    public class House
    {
        public VertexPositionNormalTexture[] Vertices;
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;
        public int[] Indices;
        public int PrimitiveCount;
        public PrimitiveType PrimitiveType;

        private readonly Vector3 VERTEX_BASE0 = new Vector3(0, 0, 0);
        private readonly Vector3 VERTEX_BASE1 = new Vector3(1, 0, 0);
        private readonly Vector3 VERTEX_BASE2 = new Vector3(1, 0, 1);
        private readonly Vector3 VERTEX_TOP = new Vector3(0.5f, 1, 0.5f);

        private readonly Vector2 TEXTURECOORDINATE = new Vector2(0, 0);

        private readonly Vector3 NORMAL_BASE = new Vector3(0,-1,0);
        private readonly Vector3 NORMAL_WALL1 = new Vector3(0.5f, 0.5f, -0.5f);
        private readonly Vector3 NORMAL_WALL2 = new Vector3(1, 0.5f, 0);
        private readonly Vector3 NORMAL_WALL3 = new Vector3(0.5f, 0.5f, 0.5f);

        public House()
        {
            buildVertices();

            buildIndices();

            setBuffers();
               
        }

        private void buildVertices()
        {
            List<VertexPositionNormalTexture> verts = new List<VertexPositionNormalTexture>();

            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE0, NORMAL_BASE, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE1, NORMAL_BASE, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE2, NORMAL_BASE, TEXTURECOORDINATE));

            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE0, NORMAL_WALL1, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_TOP, NORMAL_WALL1, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE1, NORMAL_WALL1, TEXTURECOORDINATE));

            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE1, NORMAL_WALL2, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_TOP, NORMAL_WALL2, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE2, NORMAL_WALL2, TEXTURECOORDINATE));

            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE2, NORMAL_WALL3, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_TOP, NORMAL_WALL3, TEXTURECOORDINATE));
            verts.Add(new VertexPositionNormalTexture(VERTEX_BASE0, NORMAL_WALL3, TEXTURECOORDINATE));

            Vertices = verts.ToArray();
        }

        private void buildIndices()
        {
            List<int> indices = new List<int>();

            //indices.Add(0);
            //indices.Add(1);
            //indices.Add(2);

            //indices.Add(0);
            //indices.Add(3);
            //indices.Add(1);

            //indices.Add(1);
            //indices.Add(3);
            //indices.Add(2);

            //indices.Add(2);
            //indices.Add(3);
            //indices.Add(0);

            indices.AddRange(Enumerable.Range(0, 12));

            Indices = indices.ToArray();
        
            PrimitiveCount = Indices.Count() / 3;
            PrimitiveType = PrimitiveType.TriangleList;
        }

        private void setBuffers()
        {
            VertexBuffer = new VertexBuffer(Game1.device, typeof(VertexPositionNormalTexture), Vertices.Count(), BufferUsage.None);
            VertexBuffer.SetData(Vertices);

            IndexBuffer = new IndexBuffer(Game1.device, IndexElementSize.ThirtyTwoBits, Indices.Count(), BufferUsage.None);
            IndexBuffer.SetData(Indices);
        }
    }
}
