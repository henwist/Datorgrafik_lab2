using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Components;
using GameEngine.Managers;

namespace GameEngine.Systems
{
    public class CameraSystem : IUdatable
    {
        private static CameraSystem instance;
        public GraphicsDevice device { get; protected set; }
        public CameraComponent camera { get; protected set; }

        public static CameraSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new CameraSystem();
                return instance;
            }
        }

        private CameraSystem()
        {
            
        }

        public void setUpCamera(Game game, Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUp)
        {
            camera = new CameraComponent(game, cameraPosition, cameraTarget, cameraUp);
        }

        public void Update(GameTime gameTime)
        {
            List<Component> comps = ComponentManager.GetComponents<CameraComponent>();
            
            foreach(Component c in comps)
            {
                c.Update(gameTime);
            }
        }
    }
}
