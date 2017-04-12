using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Components
{
    public class CameraComponent : Component
    {
        private static Vector3 perspectiveOffset = new Vector3(0, 200, -200);

        public Matrix viewMatrix { get; set; }
        public Matrix projectionMatrix { get; set; }

        public Vector3 cameraPosition { get; set; }
        public Vector3 cameraDirection { get; set; }
        public Vector3 cameraUp { get; set; }

        float speed = 3f;

        public CameraComponent(Game game, Vector3 position, Vector3 target, Vector3 up)
        {
            cameraPosition = position;
            cameraDirection = target - position;
            cameraDirection.Normalize();
            cameraUp = up;
            CreateLookAt();
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 500.0f);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gametime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                cameraPosition += cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                cameraPosition -= cameraDirection * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
            CreateLookAt();

            base.Update(gametime);
        }

        public void CreateLookAt()
        {
            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
        }
    }
}
