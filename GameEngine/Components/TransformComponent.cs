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
        public Vector3 Position { get; set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Roll { get; private set; }
        public float Scale { get; private set; }
        public bool IsMovable { get; set; }

        public Matrix ObjectWorld { get; set; }

        public TransformComponent(Vector3 pos, float yaw, float pitch, float roll, float scale, bool isMovable = false)
        {
            Position = pos;
            Yaw = yaw;
            Pitch = pitch;
            Roll = roll;
            Scale = scale;
            IsMovable = isMovable;

            Quaternion qrot = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);
            qrot.Normalize();

            ObjectWorld = Matrix.CreateScale(scale)
                        * Matrix.CreateFromQuaternion(qrot)
                        * Matrix.CreateTranslation(pos);
        }
    }
}
