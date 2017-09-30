using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Datorgrafik_lab2.InstanceContainers;
using System.IO;
using Microsoft.Xna.Framework.Input;
using GameEngine.Components;
using GameEngine.Managers;
using Datorgrafik_lab2.Managers;

namespace Datorgrafik_lab2.CreateModels
{
    public class Figure
    {
        public List<VertexPositionNormalTexture> Vertices { get; protected set; }
        public int[] Indices { get; protected set; }
        public VertexBuffer VertexBuffer;
        public IndexBuffer IndexBuffer;

        public Vector3 FIGURE_HEIGHT = new Vector3(0f, 22.5f, 0f);

        private string TORSO = "torso";
        private int INDICES_COUNT = 36;

        private InstanceTree root;
        private InstanceTree head;
        private InstanceTree upperLeftArm;
        private InstanceTree lowerLeftArm;
        private InstanceTree upperRightArm;
        private InstanceTree lowerRightArm;
        private InstanceTree upperRightLeg;
        private InstanceTree lowerRightLeg;
        private InstanceTree upperLeftLeg;
        private InstanceTree lowerLeftLeg;
        //this bodypart is added as to cover-up for a suspect bug. This bodypart will always be drawn in center with identity matrix.
        private InstanceTree bug_adder;

        private InstanceBodyParts bodyParts;

        private GraphicsDevice gd;

        //private Matrix currentWorld;

        private Dictionary<string, Texture2D> textures;

        private float rotation;

        private Dictionary<string, float> bodypartRotation;
        private Dictionary<string, int> rotationDirection;
        private Dictionary<string, float> maxRotAngle;
        private Dictionary<string, float> minRotAngle;

        private float twoPI;

        private float MV = 0.01f;

        public Figure(GraphicsDevice gd)
        {
            this.gd = gd;

            rotation = 0f;

            bodypartRotation = new Dictionary<string, float>();
            rotationDirection = new Dictionary<string, int>();
            maxRotAngle = new Dictionary<string, float>();
            minRotAngle = new Dictionary<string, float>();

            twoPI = MathHelper.TwoPi;

            textures = new Dictionary<string, Texture2D>();

            LoadTextures();

            BuildBodyParts();

            InitBuffers();

            InitRotationDirections();

            InitMaxAngles();

            InitMinAngles();

            BuildInstanceTree();
        }


        private void InitRotationDirections()
        {
            rotationDirection.Add("upperLeftArm", 1);
            rotationDirection.Add("upperRightArm", -1);

            rotationDirection.Add("lowerLeftArm", 1);
            rotationDirection.Add("lowerRightArm", -1);



            rotationDirection.Add("upperLeftLeg", -1);
            rotationDirection.Add("upperRightLeg", 1);

            rotationDirection.Add("lowerLeftLeg", 1);
            rotationDirection.Add("lowerRightLeg", -1);

            rotationDirection.Add("torso", -1);



            bodypartRotation.Add("upperRightArm", 0.01f);
            bodypartRotation.Add("lowerRightArm", 0.05f);

            bodypartRotation.Add("upperLeftArm", 0.01f);
            bodypartRotation.Add("lowerLeftArm", 0.05f);



            bodypartRotation.Add("upperLeftLeg", 0.01f);
            bodypartRotation.Add("lowerLeftLeg", 0.05f);

            bodypartRotation.Add("upperRightLeg", 0.01f);
            bodypartRotation.Add("lowerRightLeg", 0.05f);

            bodypartRotation.Add("torso", 0.05f);


        }

        private void InitMaxAngles()
        {
            maxRotAngle.Add("upperLeftArm", 1 / 8f * twoPI);
            maxRotAngle.Add("upperRightArm", 1 / 8f * twoPI);

            maxRotAngle.Add("lowerLeftArm", 1 / 4f * twoPI);
            maxRotAngle.Add("lowerRightArm", 1 / 4f * twoPI);

            maxRotAngle.Add("torso", twoPI);


            maxRotAngle.Add("upperLeftLeg", 1 / 8f * twoPI);
            maxRotAngle.Add("upperRightLeg", 1 / 8f * twoPI);

            maxRotAngle.Add("lowerLeftLeg", 1 / 6f * twoPI);
            maxRotAngle.Add("lowerRightLeg", 1 / 6f * twoPI);
        }

        private void InitMinAngles()
        {
            minRotAngle.Add("upperLeftArm", -1 / 8f * twoPI);
            minRotAngle.Add("upperRightArm", -1 / 8f * twoPI);

            minRotAngle.Add("lowerLeftArm", 0);
            minRotAngle.Add("lowerRightArm", 0);



            minRotAngle.Add("upperLeftLeg", -1 / 8f * twoPI);
            minRotAngle.Add("upperRightLeg", -1 / 8f * twoPI);

            minRotAngle.Add("lowerLeftLeg", 0);
            minRotAngle.Add("lowerRightLeg", 0);

            minRotAngle.Add("torso", -twoPI);
        }

        private void LoadTextures()
        {
            textures.Add("blue", Texture2D.FromStream(gd, new StreamReader("Content/Textures/blue.png").BaseStream));
            textures.Add("green", Texture2D.FromStream(gd, new StreamReader("Content/Textures/green.png").BaseStream));
            textures.Add("orange", Texture2D.FromStream(gd, new StreamReader("Content/Textures/orange.png").BaseStream));
            textures.Add("red", Texture2D.FromStream(gd, new StreamReader("Content/Textures/red.png").BaseStream));
            textures.Add("yellow", Texture2D.FromStream(gd, new StreamReader("Content/Textures/yellow.png").BaseStream));

            textures["blue"].Name = "blue";
            textures["green"].Name = "green";
            textures["orange"].Name = "orange";
            textures["red"].Name = "red";
            textures["yellow"].Name = "yellow";
        }


        public void Draw(GameTime gameTime)
        {
            //currentWorld = effect.Parameters["World"].GetValueMatrix();
            EffectComponent effectCmp = ComponentManager.GetComponents<EffectComponent>().Cast<EffectComponent>().Select(x => x).ElementAt(0);
            BasicEffect effect = effectCmp.effect;

            Matrix world = ComponentManager.GetComponents<WorldMatrixComponent>().Cast<WorldMatrixComponent>().Select(x => x).ElementAt(0).WorldMatrix;

            gd.SetVertexBuffer(VertexBuffer);
            gd.Indices = IndexBuffer;

            Draw(world, effect, root);

            //effect.Parameters["World"].SetValue(currentWorld);
        }


        private void Draw(Matrix world, BasicEffect effect, InstanceTree root)
        {


            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                Matrix finalTransforms = root.nodeTransform *  root.GetParentTransforms() * world;
                effect.World = finalTransforms;
                effect.Texture = root.texture;

                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, INDICES_COUNT / 3);
            }

            foreach (InstanceTree instance in root)
                Draw(world, effect, instance);
        }


        private void BuildBodyParts()
        {
            bodyParts = new InstanceBodyParts();
            bodyParts.AddBodyPart(TORSO, new Cube());
        }


        public void Update()
        {

            TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(SceneManager.FigureId);
            HeightmapComponent hmCmp = ComponentManager.GetComponents<HeightmapComponent>().Cast<HeightmapComponent>().Select(n => n).ElementAt(0);

            int x = (int)(transform.Position.X * 1 / hmCmp.scaleFactor.X);
            int z = -(int)(transform.Position.Z * 1 / hmCmp.scaleFactor.X);

            int lengthx = hmCmp.heightData.GetLength(0);
            int lengthz = hmCmp.heightData.GetLength(1);

            //keep x, z within indices in array
            if (x < 0)
                x *= -1;

            if (z < 0)
                z *= -1;

            if (x >= lengthx - 1)
                x -= 1;


            if (z >= lengthz - 1 )
                z -= 1;


            //move figure
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && z < lengthz - 1)
                transform.Position += new Vector3(0, 0, -MV);

            if (Keyboard.GetState().IsKeyDown(Keys.Down) && z < lengthz - 1)
                transform.Position += new Vector3(0, 0, MV);

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && x < lengthx - 1)
                transform.Position += new Vector3(MV, 0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) && x < lengthx - 1)
                transform.Position += new Vector3(-MV, 0, 0);



            transform.Position = new Vector3(transform.Position.X, 
                                             hmCmp.scaleFactor.X * hmCmp.heightData[x, z], transform.Position.Z);

            transform.Position -= FIGURE_HEIGHT;
            //Console.WriteLine(transform.Position);

            BuildInstanceTree();
        }



        private void InitBuffers()
        {
            Vertices = new List<VertexPositionNormalTexture>();
            Vertices.AddRange(bodyParts.GetBodyPart(TORSO).vertices.ToArray());

            VertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), bodyParts.GetBodyPart(TORSO).vertices.Count(), BufferUsage.None);
            VertexBuffer.SetData(Vertices.ToArray());

            Indices = Enumerable.Range(0, Vertices.Count).ToArray();
            IndexBuffer = new IndexBuffer(gd, typeof(int), INDICES_COUNT, BufferUsage.None);
            IndexBuffer.SetData(Indices);
        }



        private void InitTreeParts()
        {
            //position += new Vector3(5, 5, 5);

            //Matrix torsoWorld = Matrix.Identity * Matrix.CreateScale(1f) * Matrix.CreateTranslation(position);

            //if (root != null)
            //{

            //    torsoWorld = rotateAroundYAxis(rot += 0.1f, root);
            //    //torsoWorld = Matrix.CreateTranslation(-1 * currentTransform.Translation)
            //    //                        * Matrix.CreateFromQuaternion(rotateAroundYAxis(rot, root))
            //    //                        * Matrix.CreateTranslation(1 * currentTransform.Translation)
            //    //                        /** currentTransform*/;
            //}
            TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(SceneManager.FigureId);

            root = new InstanceTree("torso", /*Matrix.CreateTranslation(position) * */ Matrix.CreateScale(1f) * transform.ObjectWorld/*Matrix.CreateTranslation(transform.Position)*/, textures["orange"]); //parent tree node

            head = new InstanceTree("head", Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(0, Cube.LENGTH_Y / 2, 0), textures["green"]);

            upperLeftArm = new InstanceTree("upperLeftArm", Matrix.CreateScale(0.5f)
                                                                        * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X / 2, Cube.LENGTH_Y / 4f, 0)), textures["red"]);
            lowerLeftArm = new InstanceTree("lowerLeftArm", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X / 4, -Cube.LENGTH_Y / 2f, 0)), textures["orange"]);

            upperRightArm = new InstanceTree("upperRightArm", Matrix.CreateScale(0.5f)
                                                                        * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 2, Cube.LENGTH_Y / 4f, 0)), textures["red"]);
            lowerRightArm = new InstanceTree("lowerRightArm", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 4, -Cube.LENGTH_Y / 2f, 0)), textures["orange"]);


            upperRightLeg = new InstanceTree("upperRightLeg", Matrix.CreateScale(0.5f)
                                                            * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 2, -Cube.LENGTH_Y / 1.7f, 0)), textures["red"]);
            lowerRightLeg = new InstanceTree("lowerRightLeg", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 4, -Cube.LENGTH_Y / 2f, 0)), textures["orange"]);

            upperLeftLeg = new InstanceTree("upperLeftLeg", Matrix.CreateScale(0.5f)
                                                * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X / 2, -Cube.LENGTH_Y / 1.7f, 0)), textures["red"]);
            lowerLeftLeg = new InstanceTree("lowerLeftLeg", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X / 4, -Cube.LENGTH_Y / 2f, 0)), textures["orange"]);

            //this bodypart is added as to cover-up for a suspect bug. This bodypart will always be drawn in center with identity matrix.
            bug_adder = new InstanceTree("bug_adder", Matrix.CreateScale(0.25f)
                                                           * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X + 4f, Cube.LENGTH_Y / 2f, 0)), textures["blue"]);
        }


        private void InitPartsRotation()
        {
            RotateBodyPart(0.01f, upperRightArm, 1, "upperRightArm");
            RotateBodyPart(0.01f, lowerRightArm, -1, "lowerRightArm");

            RotateBodyPart(0.01f, upperLeftArm, 1, "upperLeftArm");
            RotateBodyPart(0.01f, lowerLeftArm, -1, "lowerLeftArm");

            //RotateBodyPart(rot, root, 1, "torso");
        }


        private void ConnectParts()
        {

            //Head
            root.AddChild(head);

            //Left Arm
            root.AddChild(upperLeftArm);
            upperLeftArm.AddChild(lowerLeftArm);

            //Right Arm
            root.AddChild(upperRightArm);
            upperRightArm.AddChild(lowerRightArm);

            //Right Leg
            root.AddChild(upperRightLeg);
            upperRightLeg.AddChild(lowerRightLeg);


            //Left Leg
            root.AddChild(upperLeftLeg);
            upperLeftLeg.AddChild(lowerLeftLeg);


            //Bug Adder
            root.AddChild(bug_adder);
        }


        private void BuildInstanceTree()
        {
            InitTreeParts();

            InitPartsRotation();

            ConnectParts();
        }


        private void RotateBodyPart(float posX, InstanceTree bodyPart, int translationSign, string bodyPartName)
        {
            Matrix currentTransform = Matrix.Identity;

            bodypartRotation[bodyPartName] += posX * rotationDirection[bodyPartName];
            rotation = bodypartRotation[bodyPartName];

            if (rotation >= maxRotAngle[bodyPartName])
                rotationDirection[bodyPartName] *= -1;

            if (rotation <= minRotAngle[bodyPartName])
                rotationDirection[bodyPartName] *= -1;


            Quaternion qrot = bodyPart.GetParentTransforms().Rotation * Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(bodypartRotation[bodyPartName]));
            qrot.Normalize();

            currentTransform =  bodyPart.GetParentTransforms();

            bodyPart.nodeTransform = Matrix.CreateTranslation(translationSign * -1 * currentTransform.Translation)
                                    * Matrix.CreateFromQuaternion(qrot)
                                    * Matrix.CreateTranslation(translationSign * 1 * currentTransform.Translation)
                                    * currentTransform;
        }
    }
}