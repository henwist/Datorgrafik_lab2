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
        int terrainWidth, terrainHeight;
        VertexPositionNormalTexture[] vertices;
        int[] indices;
        float[,] heightData;

        public HeightmapSystem()
        {
            vertices = new VertexPositionNormalTexture[terrainWidth * terrainHeight];
        }

        private void SetUpVertices()
        {
            
            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    vertices[x + y * terrainWidth].Position = new Vector3(x * 10, heightData[x, y], -y * 10);
                    vertices[x + y * terrainWidth].Normal = new Vector3(0, 0, 1); //+Z
                    vertices[x + y * terrainWidth].TextureCoordinate = new Vector2(1, 0);
                }
            }
        }

        private void SetUpIndices()
        {
            indices = new int[(terrainWidth - 1) * (terrainHeight - 1) * 6];
            int counter = 0;
            for (int y = 0; y < terrainHeight - 1; y++)
            {
                for (int x = 0; x < terrainWidth - 1; x++)
                {
                    int lowerLeft = x + y * terrainWidth;
                    int lowerRight = (x + 1) + y * terrainWidth;
                    int topLeft = x + (y + 1) * terrainWidth;
                    int topRight = (x + 1) + (y + 1) * terrainWidth;

                    indices[counter++] = topLeft;
                    indices[counter++] = lowerRight;
                    indices[counter++] = lowerLeft;

                    indices[counter++] = topLeft;
                    indices[counter++] = topRight;
                    indices[counter++] = lowerRight;
                }
            }
        }

        private void LoadHeightData()
        {
            heightData = new float[terrainWidth, terrainHeight];
            heightData[0, 0] = 0;
            heightData[1, 0] = 0;
            heightData[2, 0] = 0;
            heightData[3, 0] = 0;

            heightData[0, 1] = 0.5f;
            heightData[1, 1] = 0;
            heightData[2, 1] = -1.0f;
            heightData[3, 1] = 0.2f;

            heightData[0, 2] = 1.0f;
            heightData[1, 2] = 1.2f;
            heightData[2, 2] = 0.8f;
            heightData[3, 2] = 0;
        }
    }

    
}
