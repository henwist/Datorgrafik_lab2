using GameEngine.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Managers
{
    public class SceneManager
    {

        //private PhysicsSystem phys_sys;

        private DrawSystem draw_sys;

        public SceneManager(SpriteBatch spriteBatch, Rectangle window)
        {
            //phys_sys = new PhysicsSystem(window);

            draw_sys = new DrawSystem(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            //phys_sys.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            draw_sys.Update(gameTime);
        }

    }
}