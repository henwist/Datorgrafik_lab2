using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Datorgrafik_lab2.CreateModels
{

 public class Tree
    {
        private struct pos_or
        {
            public Vector3 position { get; set; }
            public Vector3 direction { get; set; }
            public float scale { get; set; }
        }

        private Stack<VertexPositionNormalTexture> treeStack;
        private Stack<VertexPositionNormalTexture> storeStack;
        private Stack<pos_or> turtleStack;

        private int replicationDepth;

        static string dna_example = "F[LF]F[RF]F";

        private char[] originalDna;
        private char[] dna;
        private char[] currentString; 
        private float unitLength;
        private float rotAroundZAxis;
        private float scaleReplication;
        private string[] recursiveStrings;
        private int countVertices;

        public VertexPositionNormalTexture[] vertices { get; set; }
        public int[] indices { get; set; }
        public VertexBuffer vertexBuffer { get; private set; }
        public IndexBuffer indexBuffer { get; private set; }

        private Vector3 startPosition;
        private Vector3 segmentVector;
        private Vector3 normal;
        private Vector2 textureCoordinate;
        private Vector3 directionF;
        private Vector3 directionL;
        private Vector3 directionR;



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

        public VertexPositionNormalTexture[] cubeVertices { get; private set; }
        public int[] cubeIndices { get; private set; }

        public int MyProperty { get; set; }


        public Tree(GraphicsDevice gd, float unitLength, float rotAroundZAxis, 
                    string dna, int replicationDepth, 
                    float scaleReplication, string[] recursiveStrings)
        {
            treeStack = new Stack<VertexPositionNormalTexture>();
            storeStack = new Stack<VertexPositionNormalTexture>();
            turtleStack = new Stack<pos_or>();

            this.replicationDepth = replicationDepth;

            this.originalDna = dna.ToArray();

            this.dna = dna.ToArray();

            this.unitLength = unitLength;

            this.rotAroundZAxis = rotAroundZAxis;

            this.scaleReplication = scaleReplication;

            this.recursiveStrings = recursiveStrings;

            startPosition = new Vector3(0, 0, 0);
            segmentVector = new Vector3(0, 1, 0);
            normal = new Vector3(0.5f, 0.3f, 0.7f);
            textureCoordinate = new Vector2(0, 0);

            directionF = new Vector3(0, 1, 0);
            directionL = new Vector3(0, 1, 0);
            directionR = new Vector3(0, 1, 0);


            buildDna(ref this.dna, 0);

            System.Diagnostics.Debug.WriteLine(currentString);

            countRecursiveStrings();

            buildStructure();

            createIndices();

            //Cubes - uncomment the right draw functionin Game1 too.
            //createTallCube();
            //setVertexAndIndexBuffer(gd, cubeVertices, cubeIndices);

            //Used for tree - uncomment the right draw function in Game1 too.
            setVertexAndIndexBuffer(gd, vertices, indices);
        }


        private void setVertexAndIndexBuffer(GraphicsDevice gd, VertexPositionNormalTexture[] vertices, int[] indices)
        {
            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vertices.Count(), BufferUsage.None);
            indexBuffer = new IndexBuffer(gd, typeof(int), indices.Count(), BufferUsage.None);

            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
            indexBuffer.SetData<int>(indices);
        }


        private void buildStructure()
        {
            Random rnd = new Random();
            Vector3 direction = directionF;
            Vector3 currentVector = Vector3.Zero;
            pos_or turtleValues;

            int counter = 1;
            float scale = 1f;

            treeStack.Push(new VertexPositionNormalTexture(startPosition, normal, textureCoordinate));

            foreach (char x in currentString)
            {
                switch(x)
                {
                    case 'F':
                        treeStack.Push(new VertexPositionNormalTexture(F(this.unitLength, currentVector, direction, scale), 
                                                                   normal, textureCoordinate));
                        currentVector = treeStack.Peek().Position;

                        counter++;
                        break;

                    case 'L':
                        direction = Vector3.Transform(direction,  Matrix.CreateRotationZ(rotAroundZAxis));
                        //                                         
                        break;

                    case 'R':
                        direction = Vector3.Transform(direction, Matrix.CreateRotationZ(-rotAroundZAxis));
                        break;

                    case '[':
                        scale = this.scaleReplication * scale;
                        turtleStack.Push(new pos_or() { position = currentVector,
                                                        direction = direction,
                                                        scale = scale});
                        
                        break;

                    case ']':
                        turtleValues = turtleStack.Pop();
                        currentVector = turtleValues.position;
                        direction = turtleValues.direction;
                        scale = turtleValues.scale;
                        break;


                }
            }

            vertices = treeStack.Reverse().ToArray();
        }


        private void createIndices()
        {
            Stack<int> indices = new Stack<int>();
            Stack<int> leftIndices = new Stack<int>();
            Stack<int> topIndex = new Stack<int>();

            leftIndices.Push(0);
            int leftIndex = 0;
            int rightIndex = 1;

            foreach (char x in currentString)
            {
                switch (x)
                {
                    case 'F':
                        indices.Push(leftIndices.Peek());
                        indices.Push(rightIndex);
                        rightIndex++;
                        break;

                    case '[':
                        leftIndex = indices.Peek();
                        leftIndices.Push(leftIndex);
                        
                        break;

                    case ']':
                        leftIndex = leftIndices.Pop(); // (indices.Peek());
                        break;
                }
            }

            this.indices = indices.Reverse().ToArray();
        }


        private void countRecursiveStrings()
        {
            countVertices = currentString.Count(x => x.Equals(recursiveStrings[0].ToCharArray()[0]));
        }


        public Vector3 F(float unitLength, Vector3 currentVector, Vector3 direction, float scale)
        {
            Vector3 segment = (currentVector + direction);
            //segment.Normalize();

            return  scale * unitLength * segment;
        }


        private void buildDna(ref char[] buildString, int replicationDepth)
        {

            if (replicationDepth > this.replicationDepth)
                return;

            foreach (string str in recursiveStrings)
                currentString = new string(buildString).Replace(str, new string(dna)).ToArray();

            buildDna(ref currentString, ++replicationDepth);

            dna = currentString;
        }


        private void createTallCube()
        {

            List<VertexPositionNormalTexture> cubeVertices = new List<VertexPositionNormalTexture>();
            List<int> cubeIndices = new List<int>();

            cubeIndices.AddRange(Enumerable.Range(0, 36));

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
            cubeVertices.Add(new VertexPositionNormalTexture(SIX    , FACE3, new Vector2(0, 0)));
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

            this.cubeVertices = cubeVertices.ToArray();
            this.cubeIndices = cubeIndices.ToArray();

        }
    }
}
