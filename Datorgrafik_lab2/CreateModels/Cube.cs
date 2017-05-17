using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Datorgrafik_lab2.CreateModels
{
    public class Cube
    {
        public static int LENGTH_X = 2;
        public static int LENGTH_Y = 4;
        public static int LENGTH_Z = 2;

        public VertexPositionNormalTexture[] vertices { get ;  set; }

        public static readonly Vector3 ONE = new Vector3(1, -2, 1);
        public static readonly Vector3 TWO = new Vector3(-1, -2, -1);
        public static readonly Vector3 THREE = new Vector3(1, -2, -1);
        public static readonly Vector3 FOUR = new Vector3(-1, 2, -1);

        public static readonly Vector3 FIVE = new Vector3(1, 2, 1);
        public static readonly Vector3 SIX = new Vector3(1, 2, -1);
        public static readonly Vector3 SEVEN = new Vector3(-1, -2, 1);
        public static readonly Vector3 EIGHT = new Vector3(-1, 2, 1);

        public static readonly Vector3 FACE1 = new Vector3(0, -1, 0);//-Y
        public static readonly Vector3 FACE2 = new Vector3(0, 1, 0); //+Y
        public static readonly Vector3 FACE3 = new Vector3(1, 0, 0); //+X
        public static readonly Vector3 FACE4 = new Vector3(0, 0, 1); //+Z
        public static readonly Vector3 FACE5 = new Vector3(-1, 0, 0);//-X
        public static readonly Vector3 FACE6 = new Vector3(0, 0, -1); //-Z

        public Cube()
        {
            List<VertexPositionNormalTexture> cubeVertices = new List<VertexPositionNormalTexture>();

            cubeVertices.Add(new VertexPositionNormalTexture(ONE, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(TWO, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(THREE, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FOUR, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FIVE, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SIX, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SIX, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(ONE, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(THREE, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FIVE, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SEVEN, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(ONE, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(EIGHT, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(TWO, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SEVEN, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(THREE, FACE6, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FOUR, FACE6, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SIX, FACE6, new Vector2(0, 0)));




            cubeVertices.Add(new VertexPositionNormalTexture(ONE, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SEVEN, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(TWO, FACE1, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FOUR, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(EIGHT, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FIVE, FACE2, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SIX, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FIVE, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(ONE, FACE3, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FIVE, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(EIGHT, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(SEVEN, FACE4, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(EIGHT, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FOUR, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(TWO, FACE5, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(THREE, FACE6, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(TWO, FACE6, new Vector2(0, 0)));
            cubeVertices.Add(new VertexPositionNormalTexture(FOUR, FACE6, new Vector2(0, 0)));

            vertices = cubeVertices.ToArray();
        }
        public void transform(Matrix transformation)
        {
            int i = 0;
            foreach(VertexPositionNormalTexture vert in vertices)
            {
                vertices[i++].Position = Vector3.Transform(vert.Position ,transformation);
            }
        }
    }
}
