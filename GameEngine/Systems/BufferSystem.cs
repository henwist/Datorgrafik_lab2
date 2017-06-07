using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Managers;
using GameEngine.Components;
using CollisionSample;
using GameEngine.Helpers;

namespace GameEngine.Systems
{
    public class BufferSystem
    {
        private GraphicsDevice gd;

        public BufferSystem(GraphicsDevice gd)
        {
            this.gd = gd;
        }


        public void Draw(GameTime gametime)
        {
            TransformComponent transform;
            BufferComponent buffer;
            BoundingVolumeComponent boundingVolume;

            CameraComponent camera = ComponentManager.GetComponents<CameraComponent>().Cast<CameraComponent>().Select(x => x).Where(y => y.isActive == true).ElementAt(0);

            EffectComponent effectCmp = ComponentManager.GetComponents<EffectComponent>().Cast<EffectComponent>().Select(x => x).ElementAt(0);
            BasicEffect effect = effectCmp.effect;

            Matrix world = ComponentManager.GetComponents<WorldMatrixComponent>().Cast<WorldMatrixComponent>().Select(x => x).ElementAt(0).WorldMatrix;

            int i = 0;
            foreach(ulong id in ComponentManager.GetAllIds<BufferComponent>())
            {
                transform = ComponentManager.GetComponent<TransformComponent>(id);
                buffer = ComponentManager.GetComponent<BufferComponent>(id);
                boundingVolume = ComponentManager.GetComponent<BoundingVolumeComponent>(id);

                effect.World = transform.ObjectWorld * world;

                effect.View = camera.viewMatrix;
                effect.Projection = camera.projectionMatrix;
                effect.Texture = buffer.Texture[i++];

                gd.SetVertexBuffer(buffer.VertexBuffer);
                gd.Indices = buffer.IndexBuffer;


                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply(); 

                    gd.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, buffer.PrimitiveCount);
                }

                BoundingVolume.DrawBoundingVolume(gd, boundingVolume, camera, transform.ObjectWorld * world);
             }    
        }

    }
}
