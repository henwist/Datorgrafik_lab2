﻿using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.Objects;
using Microsoft.Xna.Framework.Content;

namespace GameEngine.Managers
{
    public class SceneManager
    {

        //private PhysicsSystem phys_sys;
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;

        private Matrix world;

        public SceneManager(GraphicsDevice gd, Matrix world)//, SpriteBatch spriteBatch, Rectangle window)
        {
            //phys_sys = new PhysicsSystem(window);
            this.gd = gd;
            this.world = world;

            heightmapObjects = new List<HeightmapObject>();
            createHeightmapObjects();

            heightmapSystem = new HeightmapSystem(gd, heightmapObjects);

            //LoadComponents();
            //draw_sys = new DrawSystem(spriteBatch);
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
            hmobj.terrainMapName = "..\\..\\..\\..\\Content\\Textures\\Play.png";
            hmobj.textureNames = new string[] {
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