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
    public class ModelSystem : ISysDrawable
    {
        private static ModelSystem instance;

        
        CameraComponent camera;

        

        //private ModelSystem(CameraComponent camera)
        //{
        //    this.camera = camera;
        //}
        

        public static ModelSystem Instance
        {
            get
            {
                if (instance == null)
                    instance = new ModelSystem();
                return instance;
            }
        }

        public void LoadContent()
        {
            
        }

        public void Draw(GameTime gametime)
        {
            List<Component> models = ComponentManager.GetComponents<ModelComponent>(); 
            foreach (ModelComponent m in models)
            {
                m.Draw(gametime, camera);
            }
        }

    }
}
