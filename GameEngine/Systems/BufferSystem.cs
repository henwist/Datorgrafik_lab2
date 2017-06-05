﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Managers;
using GameEngine.Components;

namespace GameEngine.Systems
{
    public class BufferSystem
    {
        private BasicEffect effect;
        private GraphicsDevice gd;

        public BufferSystem(GraphicsDevice gd)
        {
            this.gd = gd;

            effect = new BasicEffect(gd);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            gd.RasterizerState = rs;

            initBasicEffect();
        }

        private void initBasicEffect()
        {
            effect.World = Matrix.Identity;
            effect.PreferPerPixelLighting = true;

            effect.EnableDefaultLighting();
            effect.TextureEnabled = true;
            effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0f);

            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight0.Direction = new Vector3(0.5f, -1, 0);
            effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
            effect.DirectionalLight0.Enabled = true;

            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 100;
            effect.DiffuseColor = new Vector3(0.4f, 0.4f, 0.7f);
            effect.EmissiveColor = new Vector3(1.0f, 1.0f, 1.0f);
        }


        public void Draw(Effect effect, GameTime gametime)
        {
            TransformComponent transform;
            BufferComponent buffer;

            int i = 0;
            foreach(ulong id in ComponentManager.GetAllIds<BufferComponent>())
            {
                transform = ComponentManager.GetComponent<TransformComponent>(id);
                buffer = ComponentManager.GetComponent<BufferComponent>(id);

                CameraComponent camera = ComponentManager.GetComponents<CameraComponent>().Cast<CameraComponent>().Select(x => x).Where(y => y.isActive == true).ElementAt(0);

                //effect.World = transform.ObjectWorld;
                //effect.View = camera.viewMatrix;
                //effect.Projection = camera.projectionMatrix;
                //effect.Texture = buffer.Texture[1];

                effect.Parameters["World"].SetValue(transform.ObjectWorld);
                effect.Parameters["View"].SetValue(camera.viewMatrix);
                effect.Parameters["Projection"].SetValue(camera.projectionMatrix);
                effect.Parameters["Texture"].SetValue(buffer.Texture[i++]);

                gd.SetVertexBuffer(buffer.VertexBuffer);
                gd.Indices = buffer.IndexBuffer;
                
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    gd.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, buffer.PrimitiveCount);
                }
            }    
        }
    }
}
