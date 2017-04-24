using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.Objects;
using Microsoft.Xna.Framework.Content;
using System;

namespace GameEngine.Managers
{
    public class SceneManager
    {

        //private PhysicsSystem phys_sys;
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;

        private Matrix world;

        Matrix objRotation;
        instance_matrice[] objectWorldMatrices;
        //instance_pos[] objectWorldPos;
        static readonly int INSTANCECOUNT = 100;
        VertexBufferBinding[] bindings;
        VertexBuffer matriceIVB;
        VertexDeclaration matriceVD;
        //Tree tree;

        public SceneManager(GraphicsDevice gd, Matrix world)//, SpriteBatch spriteBatch, Rectangle window)
        {
            //phys_sys = new PhysicsSystem(window);
            this.gd = gd;
            this.world = world;

            heightmapObjects = new List<HeightmapObject>();
            createHeightmapObjects();

            heightmapSystem = new HeightmapSystem(gd, heightmapObjects);

            //float[,] hdata = HeightmapSystem.GetHeightData("..\\..\\..\\..\\Content\\Textures\\Play.png");
            //LoadComponents();
            //draw_sys = new DrawSystem(spriteBatch);

            //tree = new Tree(graphics.GraphicsDevice, 1f, MathHelper.PiOver4 + 0.4f, "F[LF]F[RF]F", 0, 1f, new string[] { "F" });

            GenerateIVD();

            t();
        }

        private void p()
        {
            //Trying rotation of object's world for all objects.
            //objRotation = Matrix.CreateRotationY(radObj) * Matrix.CreateRotationZ(0.0001f);
            //bindings[1].VertexBuffer.GetData<instance_matrice>(objectWorldMatrices);

            //int i = 0;
            //foreach (instance_matrice m in objectWorldMatrices)
            //{
            //    Vector3 translate = m.matrice.Translation;
            //    //translate.Normalize();
            //    objectWorldMatrices[i++].matrice = m.matrice * Matrix.CreateTranslation(-1 * translate) * objRotation * Matrix.CreateTranslation(translate);

            //}

            //bindings[1].VertexBuffer.SetData<instance_matrice>(objectWorldMatrices);
            ////end trying rotation.


            ////setBasiceffectParameters();


            //graphics.GraphicsDevice.Indices = tree.indexBuffer;

            //graphics.GraphicsDevice.SetVertexBuffers(bindings);

            ////tree
            //graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.LineList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 2, INSTANCECOUNT);

            //boxes
            //graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 3, INSTANCECOUNT);

        }

        private void GenerateIVD()
        {

            matriceVD = new VertexDeclaration(
                                             new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                                             new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                                             new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                                             new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
                                             );

            //posVD = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0));
        }


        private void t()
        {

            initMatrices();

            matriceIVB = new VertexBuffer(gd, matriceVD, INSTANCECOUNT, BufferUsage.None);
            matriceIVB.SetData<instance_matrice>(objectWorldMatrices);

            //posIVB = new VertexBuffer(graphics.GraphicsDevice, posVD, COUNTOBJECTPOSITIONS, BufferUsage.None);
            //posIVB.SetData<instance_pos>(objectWorldPos);

            bindings = new VertexBufferBinding[2];
            //bindings[0] = new VertexBufferBinding(tree.vertexBuffer);
            //bindings[1] = new VertexBufferBinding(matriceIVB, 0, INSTANCECOUNT);
            //bindings[2] = new VertexBufferBinding(posIVB, 0, OBJECTPOS_CHANGE_EVERY_X_INSTANCES_FREQUENCY);


        }


        struct instance_matrice
        {
            public Matrix matrice;
            //public Vector4 position;

        }


        private void initMatrices()
        {
            Random rnd = new Random();

            objectWorldMatrices = new instance_matrice[INSTANCECOUNT];

            float x = 0, y = 0, z = 0;

            float[] localRotation = { .7f, 1.57f };

            int index = 0;
            foreach (instance_matrice m in objectWorldMatrices)
            {
                objectWorldMatrices[index].matrice = Matrix.Identity
                                             * Matrix.CreateRotationY(localRotation[index]) /** Matrix.CreateTranslation(pos = new Vector3((x++) * scale, (x % 2) * scale, (z++) * scale))*/
                                             ;//* Matrix.CreateScale(rnd.Next(100, 140) / 100f);
                //objectWorldMatrices[index++].position = new Vector4((float)Math.Pow(index, 1.3));
            }


        }

        public void Update(GameTime gameTime)
        {
            //phys_sys.Update(gameTime);
        }

        public void Draw(Effect effect, GameTime gameTime)
        {
            heightmapSystem.Draw(effect);
            //draw_sys.Update(gameTime);
        }

        private void LoadComponents()
        {
            //Terrain component

        }

        private void createHeightmapObjects()
        {
            HeightmapObject hmobj = new HeightmapObject();
            hmobj.scaleFactor = 0.01f*Vector3.One;
            hmobj.position = Vector3.Zero;
            hmobj.terrainFileName = "..\\..\\..\\..\\Content\\Textures\\Play.png";
            hmobj.textureFileNames = new string[] {
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            //"..\\..\\..\\..\\Content\\Textures\\fire.png",
            };
            hmobj.objectWorld = Matrix.Identity;
            hmobj.world = Matrix.Identity;
            hmobj.breakUpInNumParts =2; //16 //match with count of textureNames above
            hmobj.spacingBetweenParts = new Vector3(0f,20f,0f);
            heightmapObjects.Add(hmobj);

            //HeightmapObject hmobj2 = new HeightmapObject();
            //hmobj2.scaleFactor = Vector3.One;
            //hmobj2.position = Vector3.Zero;
            //hmobj2.terrainMapName = "..\\..\\..\\..\\Content\\Textures\\play.png";
            //hmobj2.textureName = "..\\..\\..\\..\\Content\\Textures\\fire.png";
            //hmobj2.objectWorld = Matrix.Identity;
            //hmobj2.world = Matrix.Identity;
            //heightmapObjects.Add(hmobj2);

        }
    }
}