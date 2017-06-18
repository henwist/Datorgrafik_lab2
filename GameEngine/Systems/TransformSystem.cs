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
            TransformComponent transform;
            Quaternion qrot;
            Vector3 prevTrans = Vector3.One;
            foreach (ulong id in ComponentManager.GetAllIds<TransformComponent>())
            {
                transform = ComponentManager.GetComponent<TransformComponent>(id);

                if (transform.IsMovable == true)
                {
                    qrot = Quaternion.CreateFromYawPitchRoll(transform.Yaw, transform.Pitch, transform.Roll);
                    qrot.Normalize();

                    prevTrans = transform.ObjectWorld.Translation;

                    transform.ObjectWorld = Matrix.CreateScale(transform.Scale)
                                          //*(Matrix.CreateTranslation(prevTrans) * -1)
                                          * Matrix.CreateFromQuaternion(qrot)
                                          //* 1 * Matrix.CreateTranslation(prevTrans)
                                          * Matrix.CreateTranslation(transform.Position);

                    if (Double.IsNaN(transform.ObjectWorld.Translation.X))
                        break;
                }
            }
        }
    }
}
