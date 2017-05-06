using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Components
{
    public class BoundingVolumeComponent : Component
    {
        public BoundingBox bbox { get; set; }
        public BoundingSphere bsphere { get; set; }
    }
}
