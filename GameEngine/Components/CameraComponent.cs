using Microsoft.Xna.Framework;

namespace GameEngine.Components
{
    public class CameraComponent : Component
    {
        public Vector3 perspectiveOffset { get; set; }

        public Matrix viewMatrix { get; set; }
        public Matrix projectionMatrix { get; set; }

        public bool isActive { get; set; } 

        public BoundingFrustum bFrustum { get; set; }

        public Vector3 cameraUp { get; set; }
        public Vector3 target { get; set; }


        public CameraComponent(Vector3 position, Vector3 target, Vector3 up, float aspectRatio, Vector3 perspectiveOffset, bool isActive = false)
        {
            this.target = target;
            cameraUp = up;

            this.perspectiveOffset = perspectiveOffset;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 5000.0f);
            viewMatrix = Matrix.CreateLookAt(position, target, up);

            bFrustum = new BoundingFrustum(viewMatrix * projectionMatrix);

            this.isActive = isActive;
        }


    }
}





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace GameEngine.Components
//{
//    public class CameraComponent : Component
//    {
//        private static Vector3 perspectiveOffset = new Vector3(0, 200, -200);

//        public Matrix viewMatrix { get; set; }
//        public Matrix projectionMatrix { get; set; }

//        public Vector3 cameraPosition { get; set; }
//        public Vector3 cameraDirection { get; set; }
//        public Vector3 cameraUp { get; set; }

//        float speed = 3f;

//        public CameraComponent(Game game, Vector3 position, Vector3 target, Vector3 up)
//        {
//            cameraPosition = position;
//            cameraDirection = target - position;
//            cameraDirection.Normalize();
//            cameraUp = up;
//            CreateLookAt();
//            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, game.GraphicsDevice.Viewport.AspectRatio, 1.0f, 500.0f);
//        }

//        public override void Initialize()
//        {
//            base.Initialize();
//        }

//        public override void Update(GameTime gametime)
//        {
//            if (Keyboard.GetState().IsKeyDown(Keys.W))
//                cameraPosition += cameraDirection * speed;
//            if (Keyboard.GetState().IsKeyDown(Keys.S))
//                cameraPosition -= cameraDirection * speed;
//            if (Keyboard.GetState().IsKeyDown(Keys.D))
//                cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
//            if (Keyboard.GetState().IsKeyDown(Keys.A))
//                cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
//            CreateLookAt();

//            base.Update(gametime);
//        }

//        public void CreateLookAt()
//        {
//            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
//        }
//    }
//}
