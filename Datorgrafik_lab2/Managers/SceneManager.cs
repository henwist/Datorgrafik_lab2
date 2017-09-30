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
using GameEngine.Managers;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using GameEngine.Helpers;

namespace Datorgrafik_lab2.Managers
{
    public class SceneManager
    {
        public static ulong FigureId { get; private set; }
        public static ulong CameraId { get; private set; }

        private Game game;
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;
        private BufferSystem bufferSystem;

        private Microsoft.Xna.Framework.Matrix world;

        private static readonly int INSTANCECOUNT = 100;
        private Tree tree;
        private int TREE_SCALE_MIN = 5;
        private int TREE_SCALE_MAX = 10;
        private Texture2D[] textures;
        

        private float HEIGHTMAP_SCALE = 1f;

        private HeightmapSystem.HeightData heightMapData;

        private Vector3 cameraPos = new Vector3(40, 40, 20);
        private Vector3 cameraTarget = new Vector3(0, 0, 0);
        private Vector3 cameraUp = Vector3.Up;
        private Vector3 perspectiveOffset = new Vector3(0, 0, 0);

        private BasicEffect effect;

        private Figure figure;

        private House house;


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

            createFigure();

            createCameraStructures();

            createHeightmap();

            createHouse();

            createTreeStructures();

            bufferSystem = new BufferSystem(game.GraphicsDevice);
        }

        private void createHouse()
        {
            house = new House();
        }

        private void createFigure()
        {
            ulong entId = ComponentManager.GetNewId();
            FigureId = entId;

            Vector3 startPos = new Vector3(100, -10, -50);
            cameraTarget = startPos;

            ComponentManager.StoreComponent(entId, new TransformComponent(startPos, 0f, 0f, 0f, 1f, true));

            figure = new Figure(gd);



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
            TransformComponent transform = new TransformComponent(cameraPos, 0f, 0f, 0f, 1f, false);

            CameraComponent cameraCmp = new CameraComponent(cameraPos, cameraTarget, cameraUp, game.GraphicsDevice.DisplayMode.AspectRatio, perspectiveOffset, true);

            CameraId = ComponentManager.GetNewId();
            ComponentManager.StoreComponent(CameraId, transform);
            ComponentManager.StoreComponent(CameraId, cameraCmp);



            TransformComponent figureTransform = ComponentManager.GetComponent<TransformComponent>(FigureId);
            TransformComponent cameraTransform = ComponentManager.GetComponent<TransformComponent>(CameraId);

            cameraCmp.target = figureTransform.Position;

            cameraCmp.perspectiveOffset = cameraTransform.Position - figureTransform.Position;
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
                IndexBuffer = house.IndexBuffer,
                Indices = house.Indices,
                VertexBuffer = house.VertexBuffer,
                Vertices = house.Vertices,

                Texture = textures,

                //PrimitiveCount = tree.IndexBuffer.IndexCount / 2,
                //PrimitiveType = PrimitiveType.LineList,
                PrimitiveCount = house.PrimitiveCount,
                PrimitiveType = house.PrimitiveType,
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
            figure.Update();

            trackFigure();

            TransformSystem.Instance.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            
            heightmapSystem.Draw(gameTime);

            CameraSystem.Instance.Update(gameTime);

            bufferSystem.Draw(gameTime);

            figure.Draw(gameTime);

        }


        private void trackFigure()
        {
            CameraComponent camera = ComponentManager.GetComponent<CameraComponent>(CameraId);

            TransformComponent figureTransform = ComponentManager.GetComponent<TransformComponent>(FigureId);
            TransformComponent cameraTransform = ComponentManager.GetComponent<TransformComponent>(CameraId);

            camera.target = figureTransform.Position + figure.FIGURE_HEIGHT;

            cameraTransform.Position = figureTransform.Position + camera.perspectiveOffset + figure.FIGURE_HEIGHT;
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
            hmobj.breakUpInNumParts = 16; //16 //match with count of textureNames above
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