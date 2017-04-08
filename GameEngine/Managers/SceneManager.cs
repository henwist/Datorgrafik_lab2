using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using GameEngine.Objects;

namespace GameEngine.Managers
{
    public class SceneManager
    {

        //private PhysicsSystem phys_sys;
        private GraphicsDevice gd;

        private List<HeightmapObject> heightmapObjects;

        private HeightmapSystem heightmapSystem;

        public SceneManager(GraphicsDevice gd)//, SpriteBatch spriteBatch, Rectangle window)
        {
            //phys_sys = new PhysicsSystem(window);
            this.gd = gd;

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

        public void Draw(GameTime gameTime)
        {
            heightmapSystem.Draw();
            //draw_sys.Update(gameTime);
        }

        private void LoadComponents()
        {
            //Terrain component

        }

        private void createHeightmapObjects()
        {
            HeightmapObject hmobj = new HeightmapObject();
            hmobj.scaleFactor = 1.0f;
            //hmobj.terrainHeight = 300;
            //hmobj.terrainWidth = 400;
            hmobj.terrainMapName = "..\\..\\..\\..\\Content\\Textures\\US_Canyon.png";

            //System.Console.Out.WriteLine("current folder: " + System.Environment.CurrentDirectory);
            

            heightmapObjects.Add(hmobj);
        }

    }
}