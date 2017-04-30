using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GameEngine.Managers;
using GameEngine.Components;
using Microsoft.Xna.Framework.Input;

namespace GameEngine.Systems
{
    public class TransformSystem : IUdatable
    {
        private static TransformSystem instance;

        public static TransformSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new TransformSystem();
                return instance;
            }
        }

        public void Update(GameTime gameTime)
        {
            //List<ulong> comps = ComponentManager.GetAllEntitiesWithComp<TransformComponent>();

            //foreach (ulong c in comps)
            //{
            //    CameraComponent camera = ComponentManager.GetComponent<CameraComponent>(c);
            //    TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(c);

            //    if (Keyboard.GetState().IsKeyDown(Keys.W))
            //    {
            //        Matrix movement = Matrix.CreateRotationY(transform.rotation);
            //        Vector3 v = new Vector3(0, 0, transform.speed);
            //        v = Vector3.Transform(v, movement);

            //        transform.position.Z += v.Z;
            //        transform.position.Z += v.X;
            //        //camera.cameraPosition += camera.cameraDirection * transform.speed;
            //    }
            //    if (Keyboard.GetState().IsKeyDown(Keys.S))
            //    {

            //        Matrix movement = Matrix.CreateRotationY(transform.rotation);
            //        Vector3 v = new Vector3(0, 0, -transform.speed);
            //        v = Vector3.Transform(v, movement);

            //        transform.position.Z += v.Z;
            //        transform.position.Z += v.X;
            //        //camera.cameraPosition -= camera.cameraDirection * transform.speed;
                    
            //    }
                    
            //    if (Keyboard.GetState().IsKeyDown(Keys.D))
            //        transform.rotation += transform.speed;
            //        //camera.cameraPosition += Vector3.Cross(camera.cameraUp, camera.cameraDirection) * transform.speed;
            //    if (Keyboard.GetState().IsKeyDown(Keys.A))
            //        transform.rotation += transform.speed;
            //        //camera.cameraPosition -= Vector3.Cross(camera.cameraUp, camera.cameraDirection) * transform.speed;

            //    //camera.CreateLookAt();
            //}
        }
    }
}
