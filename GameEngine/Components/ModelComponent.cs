using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameEngine.Components
{
    public class ModelComponent : DrawableComponent
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;

        public ModelComponent(GraphicsDevice device, Model m) : base(device)
        {
            model = m;
        }

        public override void Update(GameTime gametime)
        {

        }

        public override void Draw(GameTime gametime, CameraComponent camera)
        {
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect be in mesh.Effects)
                {
                    be.EnableDefaultLighting();
                    be.Projection = camera.projectionMatrix;
                    be.View = camera.viewMatrix;
                    be.World = world * mesh.ParentBone.Transform;
                }
                mesh.Draw();
            }
        }

    }
}
