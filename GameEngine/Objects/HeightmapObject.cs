using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Objects
{
    public class HeightmapObject
    {
        public Vector3 scaleFactor { get; set; }
        public Vector3 position { get; set; }
        public string terrainFileName { get; set; }
        public string[] textureFileNames { get; set; }
        //public Matrix objectWorld { get; set; }
        //public Matrix world { get; set; }
        public int breakUpInNumParts { get; set; }
        public Vector3 spacingBetweenParts { get; set; }
    }
}
