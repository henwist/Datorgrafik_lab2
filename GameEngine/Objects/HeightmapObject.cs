using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Objects
{
    public class HeightmapObject
    {
        public int terrainWidth { get; set; }
        public int terrainHeight { get; set; }
        public float scaleFactor { get; set; }
        public string terrainMapName { get; set; }
    }
}
