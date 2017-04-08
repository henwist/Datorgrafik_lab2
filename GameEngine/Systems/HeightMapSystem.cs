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

        private List<Component> heightmapComponents;
        private List<HeightmapObject> hmobjects;
        private GraphicsDevice gd;


        public HeightmapSystem(GraphicsDevice gd, List<HeightmapObject> hmobjects)
        {
            this.gd = gd;
            this.hmobjects = hmobjects;

            heightmapComponents = new List<Component>();

            CreateHeightmapComponents();

            LoadHeightData();
            SetUpVertices();
            SetUpIndices();

            TransferToGraphicsCard();

        }

        public void Draw()
        {
            foreach (HeightmapComponent cmp in heightmapComponents)
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cmp.indexCount/3);

        }


        private void TransferToGraphicsCard()
        {
            foreach (HeightmapComponent cmp in heightmapComponents)
            {
                cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices, 0, cmp.vertexCount);
                gd.SetVertexBuffer(cmp.vertexBuffer);

                cmp.indexBuffer.SetData<int>(cmp.indices, 0, cmp.indexCount);
                gd.Indices = cmp.indexBuffer;

            }
        }


        private void CreateHeightmapComponents()
        {
            foreach(HeightmapObject hmobj in hmobjects)
            {
                ComponentManager.StoreComponent(ComponentManager.GetNewId(),
                    new HeightmapComponent(gd, hmobj.scaleFactor, hmobj.terrainMapName));
            }

            heightmapComponents = ComponentManager.GetComponents<HeightmapComponent>();
        }


        private void SetUpVertices()
        {
            Random rnd = new Random();

            foreach(HeightmapComponent cmp in heightmapComponents)
            for (int x = 0; x < cmp.terrainWidth; x++)
            {
                for (int y = 0; y < cmp.terrainHeight; y++)
                {
                    cmp.vertices[x + y * cmp.terrainWidth].Position = new Vector3(x, 
                                                                                  cmp.heightData[x, y], 
                                                                                  -y);

                    cmp.vertices[x + y * cmp.terrainWidth].Position = Vector3.Transform(cmp.vertices[x + y * cmp.terrainWidth].Position, 
                                                                                            Matrix.CreateScale(cmp.scaleFactor));

                    cmp.vertices[x + y * cmp.terrainWidth].Normal = new Vector3(rnd.Next(0,101)/100f, rnd.Next(0, 101) / 100f, rnd.Next(0, 101) / 100f); //+Z
                    cmp.vertices[x + y * cmp.terrainWidth].TextureCoordinate = new Vector2(0, 0);
                }
            }
        }


        private void SetUpIndices()
        {

            int counter = 0;

            foreach (HeightmapComponent cmp in heightmapComponents)
                for (int y = 0; y < cmp.terrainHeight - 1; y++)
                {
                    for (int x = 0; x < cmp.terrainWidth - 1; x++)
                    {
                        int lowerLeft = x + y * cmp.terrainWidth;
                        int lowerRight = (x + 1) + y * cmp.terrainWidth;
                        int topLeft = x + (y + 1) * cmp.terrainWidth;
                        int topRight = (x + 1) + (y + 1) * cmp.terrainWidth;

                        cmp.indices[counter++] = topLeft;
                        cmp.indices[counter++] = lowerRight;
                        cmp.indices[counter++] = lowerLeft;

                        cmp.indices[counter++] = topLeft;
                        cmp.indices[counter++] = topRight;
                        cmp.indices[counter++] = lowerRight;
                    }
                }
        }


        private void LoadHeightData()
        {
            foreach (HeightmapComponent cmp in heightmapComponents)
                for (int x = 0; x < cmp.terrainWidth; x++)
                {
                    for (int y = 0; y < cmp.terrainHeight; y++)
                    {
                        System.Drawing.Color color = cmp.bmp.GetPixel(x, y);
                        cmp.heightData[x, y] = ((color.R + color.G + color.B) / 3);
                    }
                }

        }
    }

}
