﻿using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.Objects;
using Microsoft.Xna.Framework.Content;
using System;
using Datorgrafik_lab2.CreateModels;
using static Datorgrafik_lab2.Enums.Enums;
using GameEngine.Managers;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using GameEngine.Helpers;

namespace Datorgrafik_lab2.Managers
{
    public class SceneManager
    {
        private Game game;
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;
        private BufferSystem bufferSystem;

        private Microsoft.Xna.Framework.Matrix world;

        private static readonly int INSTANCECOUNT = 10;
        private Tree tree;
        private int TREE_SCALE_MIN = 5;
        private int TREE_SCALE_MAX = 50;
        private Texture2D[] textures;
        

        private float HEIGHTMAP_SCALE = .1f;

        private HeightmapSystem.HeightData heightMapData;

        private Vector3 cameraPos = new Vector3(200, 200, 200);
        private Vector3 cameraTarget = new Vector3(0, 0, 0);
        private Vector3 cameraUp = Vector3.Up;
        public static ulong cameraID { get; private set; }

        private BasicEffect effect;



        public SceneManager(Game game, Microsoft.Xna.Framework.Matrix world)
        {
            this.game = game;
            this.gd = game.GraphicsDevice;
            this.world = world;

            effect = new BasicEffect(gd);
            initBasicEffect();
            initRasterizerState();


            createWorldMatrix();

            textures = new Texture2D[INSTANCECOUNT];

            createCameraStructures();

            createHeightmap();

            createTreeStructures();

            bufferSystem = new BufferSystem(game.GraphicsDevice);
        }

        private void initRasterizerState()
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            gd.RasterizerState = rs;
        }


        private void initBasicEffect()
        {
            effect.World = Matrix.Identity;
            effect.PreferPerPixelLighting = true;

            effect.EnableDefaultLighting();
            effect.TextureEnabled = true;
            effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0f);

            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight0.Direction = new Vector3(0.5f, -1, 0);
            effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
            effect.DirectionalLight0.Enabled = true;

            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 100;
            effect.DiffuseColor = new Vector3(0.4f, 0.4f, 0.7f);
            effect.EmissiveColor = new Vector3(1.0f, 1.0f, 1.0f);

            EffectComponent effectCmp = new EffectComponent()
            {
                effect = effect,
            };

            ComponentManager.StoreComponent(ComponentManager.GetNewId(), effectCmp);
        }



        private void createWorldMatrix()
        {
            ComponentManager.StoreComponent(ComponentManager.GetNewId(), new WorldMatrixComponent() {  WorldMatrix = Matrix.Identity});
        }


        private void createCameraStructures()
        {
            Vector3 position = new Vector3(100, 100, 100);
            TransformComponent transform = new TransformComponent(position, 0f, 0f, 0f, 1f);

            CameraComponent cameraCmp = new CameraComponent(position, cameraTarget, cameraUp, game.GraphicsDevice.DisplayMode.AspectRatio, new Vector3(0, -200, -200), true);

            cameraID = ComponentManager.GetNewId();
            ComponentManager.StoreComponent(cameraID, transform);
            ComponentManager.StoreComponent(cameraID, cameraCmp);
        }


        private void createHeightmap()
        {
            heightmapObjects = new List<HeightmapObject>();
            createHeightmapObjects();

            heightmapSystem = new HeightmapSystem(gd, heightmapObjects);

            heightMapData = HeightmapSystem.GetHeightData("..\\..\\..\\..\\Content\\Textures\\fire.png");
        }


        private void createTreeStructures()
        {
            tree = new Tree(gd, 1f, MathHelper.PiOver4 - 0.4f, "F[LF]F[RF]F", 0, 1f, new string[] { "F" });

            initTextures();

            BufferComponent buffer = new BufferComponent()
            {
                IndexBuffer = tree.indexBuffer,
                Indices = tree.indices,
                VertexBuffer = tree.vertexBuffer,
                Vertices = tree.vertices,

                Texture = textures,

                PrimitiveCount = tree.indexBuffer.IndexCount / 2,
                PrimitiveType = PrimitiveType.LineList,
            };

            TransformComponent[] transforms = new TransformComponent[INSTANCECOUNT];

            initTransforms(transforms);

            sendToCompManager(buffer, transforms);
        }



        private void initTextures()
        {
            Texture2D red = Game1.ContentManager.Load<Texture2D>(@"Textures/red");
            Texture2D blue = Game1.ContentManager.Load<Texture2D>(@"Textures/blue");

            for (int i = 0; i < textures.Length; i++)
                textures[i] = i % 2 == 0 ? red : blue;
        }


        private void sendToCompManager(BufferComponent buffer, TransformComponent[] transforms)
        {
            ulong id;

            for (int i=0; i<INSTANCECOUNT; i++)
            {
                id = ComponentManager.GetNewId();

                ComponentManager.StoreComponent(id, buffer);
                ComponentManager.StoreComponent(id, transforms[i]);
                ComponentManager.StoreComponent(id, BoundingVolume.GetBoundingBoxVolume(buffer.Vertices, transforms[i].ObjectWorld));

            }
        }



        private void initTransforms(TransformComponent[] transforms)
        {
            Random rnd = new Random();

            int x = 0, z = 0;
            float y = 0f;
            float treeScale = 1f;

            int index = 0;
            foreach (TransformComponent transform in transforms)
            {
                x = rnd.Next(0, heightMapData.terrainWidth);
                z = rnd.Next(0, heightMapData.terrainHeight);
                y = heightMapData.heightData[x, z];

                treeScale = rnd.Next(TREE_SCALE_MIN, TREE_SCALE_MAX) * HEIGHTMAP_SCALE;
                transforms[index++] = new TransformComponent(new Vector3(x, y, -z) * HEIGHTMAP_SCALE, rnd.Next(0, 171) / 100f, 0f, 0f, treeScale);
            }


        }


        public void Update(GameTime gameTime)
        {


        }

        public void Draw(GameTime gameTime)
        {
            
            heightmapSystem.Draw(gameTime);

            CameraSystem.Instance.Update(gameTime);

            bufferSystem.Draw(gameTime);

        }



        private void createHeightmapObjects()
        {
            HeightmapObject hmobj = new HeightmapObject();
            hmobj.scaleFactor = HEIGHTMAP_SCALE*Vector3.One;
            hmobj.position = new Vector3(0f, 0f, 0f);  /* Vector3.Zero;*/
            hmobj.terrainFileName = "..\\..\\..\\..\\Content\\Textures\\fire.png";
            hmobj.textureFileNames = new string[] {
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                                                                        "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",




                                                                                        "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                                                                        "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
                                            "..\\..\\..\\..\\Content\\Textures\\grass.png",
                                            "..\\..\\..\\..\\Content\\Textures\\fire.png",
            };
            //hmobj.objectWorld = Microsoft.Xna.Framework.Matrix.Identity;
            //hmobj.world = Microsoft.Xna.Framework.Matrix.Identity;
            hmobj.breakUpInNumParts =64; //16 //match with count of textureNames above
            hmobj.spacingBetweenParts = new Vector3(0f,0f,0f);
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