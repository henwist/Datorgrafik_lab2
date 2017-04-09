using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using GameEngine.Components;
using GameEngine.Managers;

namespace GameEngine.Systems
{
    public class ModelSystem : IUdatable, ISysDrawable
    {
        List<Component> models = new List<Component>();
        CameraComponent camera;

        public ModelSystem(CameraComponent camera)
        {
            this.camera = camera;
        }

        public void LoadContent()
        {
            models = ComponentManager.GetComponents<ModelComponent>();
        }

        public void Draw(GameTime gametime)
        {
            foreach (ModelComponent m in models)
            {
                m.Draw(gametime, camera);
            }
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
