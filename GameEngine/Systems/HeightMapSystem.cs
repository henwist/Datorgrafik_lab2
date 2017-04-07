using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Systems
{

    public class HeightmapSystem :IUdatable
    {

        private List<HeightmapComponent> heightmapComponents;
        private List<HeightmapObject> hmobjects;
        private GraphicsDevice gd;

        public HeightmapSystem(GraphicsDevice gd, List<HeightmapObject> hmobjects)
        {
            this.gd = gd;
            this.hmobjects = hmobjects;

            CreateHeightmapComponents();

            

            LoadHeightData();
            SetUpVertices();
        }

        private void CreateHeightmapComponents()
        {
            foreach(HeightmapObject hmobj in hmobjects)
            {
                ComponentManager.StoreComponent(ComponentManager.GetNewId(),
                    new HeightmapComponent(gd, hmobj.terrainWidth, hmobj.terrainHeight, hmobj.terrainMapName));
            }

            heightmapComponents = ComponentManager.GetComponents<HeightmapComponent>();
        }

        private void SetUpVertices()
        {
            foreach(HeightmapComponent cmp in heightmapComponents)
            for (int x = 0; x < cmp.terrainWidth; x++)
            {
                for (int y = 0; y < cmp.terrainHeight; y++)
                {
                    cmp.vertices[x + y * cmp.terrainWidth].Position = new Vector3(x * cmp.scaleFactor, 
                                                                                  cmp.heightData[x, y], 
                                                                                  -y * cmp.scaleFactor);
                    cmp.vertices[x + y * cmp.terrainWidth].Normal = new Vector3(0, 0, 1); //+Z
                    cmp.vertices[x + y * cmp.terrainWidth].TextureCoordinate = new Vector2(0, 0);
                }
            }
        }

        //private void SetUpIndices()
        //{
        //    indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
        //    int counter = 0;
        //    for (int y = 0; y < terrainHeight - 1; y++)
        //    {
        //        for (int x = 0; x < terrainWidth - 1; x++)
        //        {
        //            int lowerLeft = x + y * terrainWidth;
        //            int lowerRight = (x + 1) + y * terrainWidth;
        //            int topLeft = x + (y + 1) * terrainWidth;
        //            int topRight = (x + 1) + (y + 1) * terrainWidth;

        //            indices[counter++] = topLeft;
        //            indices[counter++] = lowerRight;
        //            indices[counter++] = lowerLeft;

        //            indices[counter++] = topLeft;
        //            indices[counter++] = topRight;
        //            indices[counter++] = lowerRight;
        //        }
        //    }
        //}

        private void LoadHeightData()
        {
            foreach (HeightmapComponent cmp in heightmapComponents)
                for (int x = 0; x < cmp.terrainWidth; x++)
                {
                    for (int y = 0; y < cmp.terrainHeight; y++)
                    {
                        System.Drawing.Color color = cmp.bmp.GetPixel(y, x);
                        cmp.heightData[y, x] = ((color.R + color.G + color.B) / 3);
                    }
                }
           
        }
    }

}
