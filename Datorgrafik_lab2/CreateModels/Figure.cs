using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Datorgrafik_lab2.InstanceContainers;
using System.IO;

namespace Datorgrafik_lab2.CreateModels
{
    public class Figure
    {
        public List<VertexPositionNormalTexture> vertices { get; protected set; }
        public int[] indices { get; protected set; }
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;

        private string TORSO = "torso";
        private int INDICES_COUNT = 36;

        private InstanceTree root;
        private InstanceBodyParts bodyParts;

        private GraphicsDevice gd;

        private Matrix currentWorld;

        private Dictionary<string, Texture2D> textures;

        public Figure(GraphicsDevice gd)
        {
            this.gd = gd;

            textures = new Dictionary<string, Texture2D>();

            LoadTextures();

            BuildBodyParts();

            InitBuffers();

            BuildInstanceTree();
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
            textures["red"].Name ="red";
            textures["yellow"].Name = "yellow";
        }


        public void Draw(Effect effect)
        {
            currentWorld = effect.Parameters["World"].GetValueMatrix();

            gd.SetVertexBuffer(vertexBuffer);
            gd.Indices = indexBuffer;

            Draw(effect, root);

            effect.Parameters["World"].SetValue(currentWorld);
        }


        private void Draw(Effect effect, InstanceTree root)
        {

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            { 
                pass.Apply();

                Matrix finalTransforms = root.GetParentTransforms() * currentWorld;
                effect.Parameters["World"].SetValue(finalTransforms );
                effect.Parameters["Texture"].SetValue(root.texture);

                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, INDICES_COUNT / 3);
            }

            foreach (InstanceTree instance in root)
                Draw(effect, instance);
        }


        private void BuildBodyParts()
        {
            bodyParts = new InstanceBodyParts();
            bodyParts.AddBodyPart(TORSO, new Cube());


        }


        private void InitBuffers()
        {
            vertices = new List<VertexPositionNormalTexture>();
            vertices.AddRange(bodyParts.GetBodyPart(TORSO).vertices.ToArray());

            vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), bodyParts.GetBodyPart(TORSO).vertices.Count(), BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());

            indices = Enumerable.Range(0, vertices.Count).ToArray();
            indexBuffer = new IndexBuffer(gd, typeof(int), INDICES_COUNT, BufferUsage.None);
            indexBuffer.SetData(indices);
        }


        private void BuildInstanceTree()
        {
            root = new InstanceTree("torso", Matrix.CreateScale(1f) * Matrix.CreateTranslation(new Vector3(0, 0, 0)), textures["orange"]); //parent tree node

            InstanceTree head = new InstanceTree("head", Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(0,Cube.LENGTH_Y/2,0), textures["green"]);

            InstanceTree upperLeftArm = new InstanceTree("upperLeftArm",  Matrix.CreateScale(0.5f) 
                                                                        * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X/2, Cube.LENGTH_Y/4f, 0)), textures["red"]);
            InstanceTree lowerLeftArm = new InstanceTree("lowerLeftArm", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(-Cube.LENGTH_X/4, -Cube.LENGTH_Y/2f, 0)), textures["orange"]);

            InstanceTree upperRightArm = new InstanceTree("upperRightArm", Matrix.CreateScale(0.5f)
                                                                        * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 2, Cube.LENGTH_Y / 4f, 0)), textures["red"]);
            InstanceTree lowerRightArm = new InstanceTree("lowerRightArm", Matrix.CreateScale(0.7f)
                                                                       * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X / 4, -Cube.LENGTH_Y / 2f, 0)), textures["orange"]);

            //this bodypart is added as to cover-up for a suspect bug. This bodypart will always be drawn in center with identity matrix.
            InstanceTree bug_adder = new InstanceTree("bug_adder", Matrix.CreateScale(0.25f)
                                                           * Matrix.CreateTranslation(new Vector3(Cube.LENGTH_X + 4f, Cube.LENGTH_Y / 2f, 0)), textures["blue"]);

            root.AddChild(head);

            root.AddChild(upperLeftArm);
            upperLeftArm.AddChild(lowerLeftArm);

            root.AddChild(upperRightArm);
            upperRightArm.AddChild(lowerRightArm);


            root.AddChild(bug_adder);


        }


        public void buildFigure()
        {
            //Torso();
            //Head();
            //UpperLeftArm();
            //LowerLeftArm();
            //UpperRightArm();
            //LowerRightArm();
            //UpperLeftLeg();
            //LowerLeftLeg();
            //UpperRightLeg();
            //LowerRightLeg();

            //foreach (Cube part in parts)
            //{
            //    vertices.AddRange(part.vertices);
            //}
        }


        public void RotateUpperRightArm(float posX)
        {
            InstanceTree bodyPart = root.GetInstanceTree("upperRightArm");

            if (bodyPart != null)
            {
                Quaternion qrot = bodyPart.GetParentTransforms().Rotation * Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(posX));
                qrot.Normalize();

                bodyPart.nodeTransform = Matrix.CreateFromQuaternion(qrot) * Matrix.CreateTranslation(bodyPart.GetParentTransforms().Translation);
            }

        }
    }
}



//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Datorgrafik_lab2.InstanceContainers;

//namespace Datorgrafik_lab2.CreateModels
//{
//    public class Figure
//    {
//        public List<VertexPositionNormalTexture> vertices { get; protected set; }
//        public int[] indices { get; protected set; }
//        List<Cube> parts;
//        Dictionary<String, Matrix> transforms;
//        InstanceStack stack;

//        private readonly int RIGHT_LEG_INDEX_START = 216;

//        public Figure()
//        {
//            vertices = new List<VertexPositionNormalTexture>();
//            parts = new List<Cube>();
//            transforms = new Dictionary<string, Matrix>();
//            stack = new InstanceStack();

//            buildFigure();
//            indices = Enumerable.Range(0, vertices.Count).ToArray();
//        }

//        public void buildFigure()
//        {
//            Torso();
//            Head();
//            UpperLeftArm();
//            LowerLeftArm();
//            UpperRightArm();
//            LowerRightArm();
//            UpperLeftLeg();
//            LowerLeftLeg();
//            UpperRightLeg();
//            LowerRightLeg();

//            foreach(Cube part in parts)
//            {
//                vertices.AddRange(part.vertices);
//            }
//        }

//        private void Torso()
//        {
//            Cube torso = new Cube();
//            torso.transform(Matrix.Identity);
//            parts.Add(torso);
//        }

//        private void Head()
//        {
//            Cube head = new Cube();

//            head.transform(Matrix.CreateTranslation(new Vector3(0f,2f/0.5f,0f)) 
//                * Matrix.CreateScale(0.5f));
//            parts.Add(head);
//        }

//        private void UpperLeftArm()
//        {
//            Cube upperLeftArm = new Cube();

//            Matrix instance = Matrix.CreateTranslation(new Vector3(1f / .25f, 1f / .25f, 0f))
//                * Matrix.CreateScale(.25f);
//            transforms.Add("UpperLeftArm", instance);

//            upperLeftArm.transform(instance);
//            parts.Add(upperLeftArm);
//        }

//        private void LowerLeftArm()
//        {
//            Cube lowerLeftArm = new Cube();
//            Matrix relPos;
//            transforms.TryGetValue("UpperLeftArm", out relPos);

//            Matrix instance = Matrix.CreateScale(.3f) * Matrix.CreateTranslation(new Vector3(1f, -2f, 0f)) *
//                relPos;


//            lowerLeftArm.transform(instance);
//            parts.Add(lowerLeftArm);
//        }

//        public void UpperRightArm()
//        {
//            Cube upperRightArm = new Cube();

//            Matrix instance = Matrix.CreateTranslation(new Vector3(-1f / .25f, 1f / .25f, 0f))
//                * Matrix.CreateScale(.25f);
//            transforms.Add("UpperRightArm", instance);

//            upperRightArm.transform(instance);
//            parts.Add(upperRightArm);
//        }

//        private void LowerRightArm()
//        {
//            Cube lowerRightArm = new Cube();
//            Matrix relPos;
//            transforms.TryGetValue("UpperRightArm", out relPos);

//            Matrix instance = Matrix.CreateScale(.3f) * Matrix.CreateTranslation(new Vector3(-1f, -2f, 0f)) *
//                relPos;


//            lowerRightArm.transform(instance);
//            parts.Add(lowerRightArm);
//        }

//        private void UpperLeftLeg()
//        {
//            Cube upperLeftLeg = new Cube();

//            Matrix instance = Matrix.CreateTranslation(new Vector3(.5f / .25f, -2f / .25f, 0f))
//                * Matrix.CreateScale(.25f);
//            transforms.Add("UpperLeftLeg", instance);

//            upperLeftLeg.transform(instance);
//            parts.Add(upperLeftLeg);
//        }

//        private void LowerLeftLeg()
//        {
//            Cube lowerLeftLeg = new Cube();
//            Matrix relPos;
//            transforms.TryGetValue("UpperLeftLeg", out relPos);

//            Matrix instance = Matrix.CreateScale(.3f) * Matrix.CreateTranslation(new Vector3(1f, -2f, 0f)) *
//                relPos;


//            lowerLeftLeg.transform(instance);
//            parts.Add(lowerLeftLeg);
//        }

//        private void UpperRightLeg()
//        {
//            Cube upperRightLeg = new Cube();

//            Matrix instance = Matrix.CreateTranslation(new Vector3(-.5f / .25f, -2f / .25f, 0f))
//                * Matrix.CreateScale(.25f);
//            transforms.Add("UpperRightLeg", instance);

//            upperRightLeg.transform(instance);
//            parts.Add(upperRightLeg);
//        }

//        private void LowerRightLeg()
//        {
//            Cube lowerRightLeg = new Cube();
//            Matrix relPos;
//            transforms.TryGetValue("UpperRightLeg", out relPos);

//            Matrix instance = Matrix.CreateScale(.3f) * Matrix.CreateTranslation(new Vector3(-1f, -2f, 0f)) *
//                relPos;


//            lowerRightLeg.transform(instance);
//            parts.Add(lowerRightLeg);
//        }

//        public void RotateUpperRightLeg(float posX)
//        {
//            Vector3 translation = transforms["UpperRightLeg"].Translation;
//            Matrix translateBackToPos = Matrix.CreateTranslation(translation);
//            Matrix translateToOrigo = Matrix.CreateTranslation(-1*translation);
//;
//            Quaternion qrot = (transforms["UpperRightLeg"].Rotation
//                              + Quaternion.CreateFromRotationMatrix(Matrix.CreateRotationX(posX)));
//            qrot.Normalize();

//            Matrix rotate = Matrix.CreateFromQuaternion(qrot);

//            VertexPositionNormalTexture vertex;

//            foreach (int index in Enumerable.Range(RIGHT_LEG_INDEX_START, 36))
//            {
//                vertex = vertices.ElementAt(index);
//                vertex.Position = Vector3.Transform(vertices[index].Position, translateToOrigo * rotate * translateBackToPos);
//                vertices[index] = vertex;
//            }

//        }
//    }
//}
