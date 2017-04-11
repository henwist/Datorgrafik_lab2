using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameEngine.Components
{
    public class TransformComponent : Component
    {
        public Vector3 position;
        public float rotation;
        public float scale;
        public float speed = 3f;

        public TransformComponent(Vector3 pos, float rot, float scale)
        {
            position = pos;
            rotation = rot;
            this.scale = scale;
        }
    }
}
