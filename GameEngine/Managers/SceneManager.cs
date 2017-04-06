using GameEngine.Systems;
using GameEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Managers
{
    public class SceneManager
    {

        //private PhysicsSystem phys_sys;
        private GraphicsDevice gd;
        //private DrawSystem draw_sys;

        private int terrainHeight;
        private int terrainWidth;
        private string terrainMapName = "bild.png";

        public SceneManager(GraphicsDevice gd)//, SpriteBatch spriteBatch, Rectangle window)
        {
            //phys_sys = new PhysicsSystem(window);
            this.gd = gd;
            terrainHeight = 100;
            terrainWidth = 100;

            LoadComponents();
            //draw_sys = new DrawSystem(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            //phys_sys.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            //draw_sys.Update(gameTime);
        }

        private void LoadComponents()
        {
            //Terrain component
            ComponentManager.StoreComponent(ComponentManager.GetNewId(),
                                            new HeightmapComponent(gd, terrainWidth, terrainHeight, terrainMapName));
        }

    }
}