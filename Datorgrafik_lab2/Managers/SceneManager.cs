using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.Objects;
using Microsoft.Xna.Framework.Content;
using System;
using Datorgrafik_lab2.CreateModels;
using static Datorgrafik_lab2.Enums.Enums;

namespace Datorgrafik_lab2.Managers
{
    public class SceneManager
    {
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;

        private Microsoft.Xna.Framework.Matrix world;

        private Matrix[] objectWorldMatrices;
        private static readonly int INSTANCECOUNT = 100;
        private VertexBufferBinding[] bindings;
        private VertexBuffer matriceIVB;
        private VertexDeclaration matriceVD;
        private Tree tree;

        private float HEIGHTMAP_SCALE = 0.1f;

        private HeightmapSystem.HeightData heightMapData;

        //struct Matrix
        //{
        //    public Microsoft.Xna.Framework.Matrix matrice;
        //}

        public SceneManager(GraphicsDevice gd, Microsoft.Xna.Framework.Matrix world)
        {
            this.gd = gd;
            this.world = world;

            heightmapObjects = new List<HeightmapObject>();
            createHeightmapObjects();

            heightmapSystem = new HeightmapSystem(gd, heightmapObjects);

            heightMapData = HeightmapSystem.GetHeightData("..\\..\\..\\..\\Content\\Textures\\Play.png");

            tree = new Tree(gd, 1f, MathHelper.PiOver4 + 0.4f, "F[LF]F[RF]F", 0, 1f, new string[] { "F" });

            initStructures();
        }


        private void initStructures()
        {
            matriceVD = new VertexDeclaration(
                                 new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                                 new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                                 new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                                 new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
                                 );

            initMatrices();

            matriceIVB = new VertexBuffer(gd, matriceVD, INSTANCECOUNT, BufferUsage.None);
            matriceIVB.SetData<Matrix>(objectWorldMatrices);

            bindings = new VertexBufferBinding[2];
            bindings[0] = new VertexBufferBinding(tree.vertexBuffer);
            bindings[1] = new VertexBufferBinding(matriceIVB, 0, 1);
            //bindings[2] = new VertexBufferBinding(posIVB, 0, OBJECTPOS_CHANGE_EVERY_X_INSTANCES_FREQUENCY);


        }



        private void initMatrices()
        {
            Random rnd = new Random();

            objectWorldMatrices = new Matrix[INSTANCECOUNT];

            int x = 0, z = 0;
            float y = 0f;

            int index = 0;
            foreach (Matrix m in objectWorldMatrices)
            {
                x = rnd.Next(0, heightMapData.terrainWidth);
                z = rnd.Next(0, heightMapData.terrainHeight);
                y = heightMapData.heightData[x, z];


                objectWorldMatrices[index++] = Matrix.Identity
                                             * Matrix.CreateScale(rnd.Next(10, 14) * HEIGHTMAP_SCALE)
                                             * Matrix.CreateTranslation(
                                                new Vector3(x, y, -z) * HEIGHTMAP_SCALE);
                                             //* Matrix.CreateScale(rnd.Next(100, 140) * HEIGHTMAP_SCALE);
                                            // * Matrix.CreateRotationY(rnd.Next(0, 171) / 100f);
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

            ////Trying rotation of object's world for all objects.
            ////objRotation = Matrix.CreateRotationY(radObj) * Matrix.CreateRotationZ(0.0001f);
            //bindings[1].VertexBuffer.GetData<Matrix>(objectWorldMatrices);

            //int i = 0;
            //foreach (Matrix m in objectWorldMatrices)
            //{
            //    //Vector3 translate = m.matrice.Translation;
            //    ////translate.Normalize()<
            //    objectWorldMatrices[i++] = heightmapSystem.objWorld; /*m.matrice * Matrix.CreateTranslation(-1 * translate) * objRotation * Matrix.CreateTranslation(translate);*/

            //}

            //bindings[1].VertexBuffer.SetData<Matrix>(objectWorldMatrices);
            ////end trying rotation.

            foreach (EffectPass pass in effect.Techniques[(int)EnumTechnique.InstancedTechnique].Passes)
            {
                pass.Apply();

                gd.Indices = tree.indexBuffer;

                gd.SetVertexBuffers(bindings);

                ////tree
                gd.DrawInstancedPrimitives(PrimitiveType.LineList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 2, INSTANCECOUNT);

                //boxes
                //graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 3, INSTANCECOUNT);
            }

        }

        private void LoadComponents()
        {
            //Terrain component

        }

        private void createHeightmapObjects()
        {
            HeightmapObject hmobj = new HeightmapObject();
            hmobj.scaleFactor = HEIGHTMAP_SCALE*Vector3.One;
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
            hmobj.objectWorld = Microsoft.Xna.Framework.Matrix.Identity;
            hmobj.world = Microsoft.Xna.Framework.Matrix.Identity;
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