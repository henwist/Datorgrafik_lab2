using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Components;

namespace GameEngine.Systems
{
    public class CameraSystem : IUdatable
    {
        CameraComponent camera;

        public CameraSystem(Game game, Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUp)
        {
            camera = new CameraComponent(game, cameraPosition, cameraTarget, cameraUp);
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
