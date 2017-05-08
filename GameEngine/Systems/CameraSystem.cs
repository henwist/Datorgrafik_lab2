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
    public class CameraSystem
    {
        private static CameraSystem instance;

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


        public void Update(Effect effect, GameTime gameTime)
        {
            List<ulong> compIds = ComponentManager.GetAllEntitiesWithComp<CameraComponent>();

            foreach (ulong c in compIds)
            {
                TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(c);
                CameraComponent curCam = ComponentManager.GetComponent<CameraComponent>(c);

                if (curCam.isActive)
                {
                    Matrix rotation = Matrix.CreateRotationY(transform.rotation);
                    curCam.viewMatrix = Matrix.CreateLookAt(transform.position, curCam.target, Vector3.Up);

                    effect.Parameters["View"].SetValue(curCam.viewMatrix * rotation);
                    effect.Parameters["Projection"].SetValue(curCam.projectionMatrix);

                    curCam.bFrustum = new BoundingFrustum(curCam.viewMatrix * curCam.projectionMatrix);
                }

            }
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
//using GameEngine.Components;
//using GameEngine.Managers;

//namespace GameEngine.Systems
//{
//    public class CameraSystem : IUdatable
//    {
//        private static CameraSystem instance;
//        public GraphicsDevice device { get; protected set; }
//        public CameraComponent camera { get; protected set; }

//        public static CameraSystem Instance
//        {
//            get
//            {
//                if (instance == null)
//                    instance = new CameraSystem();
//                return instance;
//            }
//        }

//        private CameraSystem()
//        {

//        }

//        public void setUpCamera(Game game, Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUp)
//        {
//            camera = new CameraComponent(game, cameraPosition, cameraTarget, cameraUp);
//        }

//        public void Update(GameTime gameTime)
//        {
//            //List<Component> comps = ComponentManager.GetComponents<CameraComponent>();
//            List<ulong> comps = ComponentManager.GetAllEntitiesWithComp<CameraComponent>();

//            foreach (ulong c in comps)
//            {
//                TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(c);
//                CameraComponent curCam = ComponentManager.GetComponent<CameraComponent>(c);

//                curCam.cameraPosition = transform.position;
//                Matrix rotation = Matrix.CreateRotationY(transform.rotation);
//                Vector3 transformedRef = Vector3.Transform(curCam.cameraDirection, rotation);
//                curCam.viewMatrix = Matrix.CreateLookAt(curCam.cameraPosition, curCam.cameraPosition + transformedRef, Vector3.Up);

//                curCam.Update(gameTime);
//            }
//        }
//    }
//}
