using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
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

        public struct HeightData
        {
            public float[,] heightData;
            public int terrainWidth;
            public int terrainHeight;
        }

        public HeightmapSystem(GraphicsDevice gd, List<HeightmapObject> hmobjects)
        {
            this.gd = gd;
            this.hmobjects = hmobjects;

            heightmapComponents = new List<Component>();

            CreateHeightmapComponents();

            LoadHeightData();
            SetUpVertices();
            SetUpIndices();

            SetUpNormals();

            SplitHeightmap();

           //TransferToGraphicsCard();

        }

        public static HeightData GetHeightData(string terrainFileName)
        {
            HeightData heightData = new HeightData();

            try
            {
                HeightmapComponent cmp = ComponentManager.GetComponents<HeightmapComponent>()
                .Cast<HeightmapComponent>()
                .First(x => x.terrainFileName.Equals(terrainFileName));

                heightData.heightData = cmp.heightData;
                heightData.terrainWidth = cmp.terrainWidth;
                heightData.terrainHeight = cmp.terrainHeight;

            }
            catch(System.InvalidOperationException)
            {
                heightData = new HeightData();
            }

            return heightData;
        }


        private HeightmapComponent CreateHeightmapComponent(HeightmapComponent cmp)
        {
            int vCount = (cmp.terrainWidth * cmp.terrainHeight) / cmp.breakUpInNumParts;
            int iCount = (cmp.terrainWidth-1) * (cmp.terrainHeight - 1) * 6 / cmp.breakUpInNumParts;

            HeightmapComponent partCmp = new HeightmapComponent()
            {
                breakUpInNumParts = 1,
                spacingBetweenParts = cmp.spacingBetweenParts,
                world = cmp.world,
                objectWorld = cmp.objectWorld,
                vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture),
                                                vCount, BufferUsage.None),
                indexBuffer = new IndexBuffer(gd, typeof(int), iCount, BufferUsage.None), 
                vertices = new VertexPositionNormalTexture[vCount],
                indices = new int[iCount],
                texture = cmp.texture,

                indexCount = iCount,
                vertexCount = vCount,

                scaleFactor = cmp.scaleFactor,

                 terrainWidth = cmp.terrainWidth,
                 terrainHeight = cmp.terrainHeight,
                 heightData = cmp.heightData,
                 
                 
            };

            return partCmp;
        }


        private void RebuildArray(int[] indices, VertexPositionNormalTexture[] vertices, 
                                  out int[] outIndices, out VertexPositionNormalTexture[] outVertices)
        {
            Dictionary<int, int> verticePositions = new Dictionary<int, int>();

            int indexCounter = 0;
            int vertexCounter = 0;

            outIndices = new int[indices.Length];
            outVertices = new VertexPositionNormalTexture[indices.Length]; //length not longer than number of indices at least.

            foreach (int index in indices)//just store an index once and give it a new position (indexCounter) in another array.
                if (!verticePositions.ContainsKey(index))
                    verticePositions.Add(index, indexCounter++);

            foreach (int index in verticePositions.Keys.ToArray()) //store the vertices on new positions.
            {
                outVertices.SetValue(vertices.GetValue(index), verticePositions[index]);
                vertexCounter++; //measure length of the built array.
            }

            indexCounter = 0;
            foreach (int index in indices) //Rebuild the indices to match the new positions in outVertices.
                outIndices[indexCounter++] = verticePositions[index];

            Array.Resize<int>(ref outIndices, indexCounter);
            Array.Resize<VertexPositionNormalTexture>(ref outVertices, vertexCounter);
        }


        private void SplitHeightmap()
        {
            int takeIndices;
            int skipIndices = 0;

            string textureName;
            int textureIndex = 0;

            int counter = 0;

            foreach (HeightmapComponent cmp in heightmapComponents)
            {
                for (int i = 0; i < cmp.breakUpInNumParts; i++)
                {
                    int[] indices;
                    VertexPositionNormalTexture[] vertices;

                    textureName = cmp.textureFileNames[0];

                    if (cmp.textureFileNames.Count() == cmp.breakUpInNumParts) //use a new texture for every minor heightmap
                        textureName = cmp.textureFileNames[textureIndex++];    //- if provided, else use the first.

                    HeightmapComponent partCmp = CreateHeightmapComponent(cmp);

                    takeIndices = partCmp.indexCount;

                    Array.Copy(cmp.indices, skipIndices++ * takeIndices, partCmp.indices, 0, takeIndices);

                    RebuildArray(partCmp.indices, cmp.vertices, out indices, out vertices);

                    partCmp.indices = indices;
                    partCmp.vertices = vertices;

                    partCmp.vertexBuffer.SetData(partCmp.vertices, 0, partCmp.vertexCount);
                    partCmp.indexBuffer.SetData(partCmp.indices, 0, partCmp.indexCount);

                    
                    partCmp.texture = Texture2D.FromStream(gd, new StreamReader(textureName).BaseStream);
                    partCmp.texture.Name = textureName;

                    partCmp.spacingBetweenParts = counter++ * cmp.spacingBetweenParts;

                    partCmp.terrainFileName = cmp.terrainFileName;

                    ComponentManager.StoreComponent(ComponentManager.GetNewId(), partCmp);
                }
            }
        }


        float rot = 0.001f;
        public void Draw(Effect effect)
        {
            Matrix currentWorldMatrix = effect.Parameters["World"].GetValueMatrix();

            foreach (HeightmapComponent cmp in ComponentManager.GetComponents<HeightmapComponent>() )
            {
                effect.Parameters["Texture"].SetValue(cmp.texture);

                //cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices, 0, cmp.vertexCount);
                gd.SetVertexBuffer(cmp.vertexBuffer);

                //cmp.indexBuffer.SetData<int>(cmp.indices, 0, cmp.indexCount);
                gd.Indices = cmp.indexBuffer;

                //if (cmp.texture.Name.Contains("fire"))
                //{
                //    cmp.objectWorld = Matrix.CreateScale(cmp.scaleFactor)
                //                    * Matrix.CreateTranslation(cmp.position + (float)System.Math.Sin(rot) *100*  new Vector3(rot, rot, rot));
                //}
                //else
                //{
                //    cmp.objectWorld = Matrix.CreateScale(cmp.scaleFactor)
                //                     * Matrix.CreateTranslation(cmp.position);
                //}
                //rot += 0.002f;


                cmp.objectWorld = Matrix.CreateTranslation(cmp.position + cmp.spacingBetweenParts)
                                * Matrix.CreateScale(cmp.scaleFactor);



                effect.Parameters["World"].SetValue(currentWorldMatrix * cmp.objectWorld);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cmp.indexCount / 3);
                }
            }

            effect.Parameters["World"].SetValue(currentWorldMatrix);
        }


        //private void TransferToGraphicsCard()
        //{
        //    foreach (HeightmapComponent cmp in ComponentManager.GetComponents<HeightmapComponent>())
        //    {
        //        cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices);/*, 0, cmp.vertexCount);*/

        //        cmp.indexBuffer.SetData<int>(cmp.indices);/*, 0, cmp.indexCount);*/
        //    }
        //}


        private void CreateHeightmapComponents()
        {
            foreach (HeightmapObject hmobj in hmobjects)
            {
                HeightmapComponent cmp = new HeightmapComponent(gd, hmobj.scaleFactor, hmobj.terrainFileName, hmobj.textureFileNames, hmobj.world);
                cmp.breakUpInNumParts = hmobj.breakUpInNumParts;
                cmp.spacingBetweenParts = hmobj.spacingBetweenParts;

                //ComponentManager.StoreComponent(ComponentManager.GetNewId(),cmp);
                heightmapComponents.Add(cmp);
            }

            //heightmapComponents = ComponentManager.GetComponents<HeightmapComponent>();
        }


        private void SetUpVertices()
        {
            Random rnd = new Random();
            int index = 0;

            foreach (HeightmapComponent cmp in heightmapComponents)
            {
                for (int x = 0; x < cmp.terrainWidth; x++)
                {
                    for (int y = 0; y < cmp.terrainHeight; y++)
                    {
                        index = x + y * cmp.terrainWidth;

                        cmp.vertices[index].Position = new Vector3(x,
                                                                                      cmp.heightData[x, y],
                                                                                      -y);

                        //cmp.vertices[index].Position = Vector3.Transform(cmp.vertices[index].Position,
                        //                                                                        Matrix.CreateScale(cmp.scaleFactor));

                        cmp.vertices[index].Normal = new Vector3(rnd.Next(0, 101) / 100f, rnd.Next(0, 101) / 100f, rnd.Next(0, 101) / 100f);
                        cmp.vertices[index].TextureCoordinate = new Vector2(0, 0);
                    }
                }

                index = 0;
            }
        }

        private void SetUpNormals()
        {

            int counter = 0;

            Vector3 v1 = Vector3.Zero;
            Vector3 v2 = Vector3.Zero;

            foreach (HeightmapComponent cmp in heightmapComponents)
                for (int y = 0; y < cmp.terrainHeight - 1; y++)
                {
                    for (int x = 0; x < cmp.terrainWidth - 1; x++)
                    {
                        int lowerLeft = x + y * cmp.terrainWidth;
                        int lowerRight = (x + 1) + y * cmp.terrainWidth;
                        int topLeft = x + (y + 1) * cmp.terrainWidth;
                        int topRight = (x + 1) + (y + 1) * cmp.terrainWidth;

                        v1 = Vector3.Cross(cmp.vertices[topLeft].Position, cmp.vertices[lowerLeft].Position);
                        //cmp.indices[counter++] = lowerRight;
                        //cmp.indices[counter++] = lowerLeft;

                        v2 = Vector3.Cross(cmp.vertices[topRight].Position, cmp.vertices[lowerRight].Position);

                        cmp.vertices[lowerLeft].Normal = Vector3.Normalize(Vector3.Add(v1, cmp.vertices[lowerRight].Normal));
                        cmp.vertices[topRight].Normal = Vector3.Normalize(Vector3.Add(v2, cmp.vertices[lowerLeft].Normal));
                        //cmp.indices[counter++] = topLeft;
                        //cmp.indices[counter++] = topRight;
                        //cmp.indices[counter++] = lowerRight;
                    }
                }
        }


        private void SetUpIndices()
        {

            int counter = 0;

            foreach (HeightmapComponent cmp in heightmapComponents)
            {
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

                counter = 0;
           }
        }


        private void LoadHeightData()
        {
            foreach (HeightmapComponent cmp in heightmapComponents)
                for (int x = 0; x < cmp.terrainWidth; x++)
                {
                    for (int y = 0; y < cmp.terrainHeight; y++)
                    {
                        System.Drawing.Color color = cmp.bmpHeightdata.GetPixel(x, y);
                        cmp.heightData[x, y] = ((color.R + color.G + color.B) / 3);
                    }
                }

        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }

}










/////////////////////////////////////////////////////////////////////////////////////////////////////////////
//using GameEngine.Components;
//using GameEngine.Managers;
//using GameEngine.Objects;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GameEngine.Systems
//{

//    public class HeightmapSystem : IUdatable
//    {

//        private List<Component> heightmapComponents;
//        private List<HeightmapObject> hmobjects;
//        private GraphicsDevice gd;


//        public HeightmapSystem(GraphicsDevice gd, List<HeightmapObject> hmobjects)
//        {
//            this.gd = gd;
//            this.hmobjects = hmobjects;

//            heightmapComponents = new List<Component>();

//            CreateHeightmapComponents();

//            LoadHeightData();
//            SetUpVertices();
//            SetUpIndices();

//            SetUpNormals();

//            SplitHeightmap();

//            //TransferToGraphicsCard();

//        }


//        private HeightmapComponent CreateHeightmapComponent(HeightmapComponent cmp)
//        {
//            int vCount = (cmp.terrainWidth * cmp.terrainHeight) / cmp.breakUpInNumParts;
//            int iCount = (cmp.terrainWidth - 1) * (cmp.terrainHeight - 1) * 6 / cmp.breakUpInNumParts;

//            HeightmapComponent partCmp = new HeightmapComponent()
//            {
//                breakUpInNumParts = 1,
//                world = cmp.world,
//                objectWorld = cmp.objectWorld,
//                vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture),
//                                                vCount, BufferUsage.None),
//                indexBuffer = new IndexBuffer(gd, typeof(int), iCount, BufferUsage.None),
//                vertices = new VertexPositionNormalTexture[vCount],
//                indices = new int[iCount],
//                texture = cmp.texture,

//                indexCount = iCount,
//                vertexCount = vCount,

//                scaleFactor = cmp.scaleFactor
//            };

//            return partCmp;
//        }


//        private void RebuildArray(int[] indices, VertexPositionNormalTexture[] vertices,
//                                  out int[] outIndices, out VertexPositionNormalTexture[] outVertices)
//        {
//            Dictionary<int, int> verticePositions = new Dictionary<int, int>();

//            int indexCounter = 0;
//            int vertexCounter = 0;

//            outIndices = new int[indices.Length];
//            outVertices = new VertexPositionNormalTexture[indices.Length]; //length not longer than number of indices at least.

//            foreach (int index in indices)//just store an index once and give it a new position (indexCounter) in another array.
//                if (!verticePositions.ContainsKey(index))
//                    verticePositions.Add(index, indexCounter++);

//            foreach (int index in verticePositions.Keys.ToArray()) //store the vertices on new positions.
//            {
//                outVertices.SetValue(vertices.GetValue(index), verticePositions[index]);
//                vertexCounter++; //measure length of the built array.
//            }

//            indexCounter = 0;
//            foreach (int index in indices) //Rebuild the indices to match the new positions in outVertices.
//                outIndices[indexCounter++] = verticePositions[index];

//            Array.Resize<int>(ref outIndices, indexCounter);
//            Array.Resize<VertexPositionNormalTexture>(ref outVertices, vertexCounter);
//        }


//        private void SplitHeightmap()
//        {
//            int takeIndices;
//            int skipIndices = 0;

//            int takeVertices;
//            int skipVertices = 0;

//            string textureName;
//            int textureIndex = 0;

//            foreach (HeightmapComponent cmp in heightmapComponents)
//            {
//                for (int i = 0; i < cmp.breakUpInNumParts; i++)
//                {
//                    int[] indices;
//                    VertexPositionNormalTexture[] vertices;

//                    textureName = cmp.textureFileNames[0];

//                    if (cmp.textureFileNames.Count() == cmp.breakUpInNumParts) //use a new texture for every minor heightmap
//                        textureName = cmp.textureFileNames[textureIndex++];    //- if provided, else use the first.

//                    HeightmapComponent partCmp = CreateHeightmapComponent(cmp);

//                    takeIndices = partCmp.indexCount;
//                    takeVertices = partCmp.vertexCount;
//                    //partCmp.indices = cmp.indices.Skip(skipIndices++ * takeIndices).Take(takeIndices).ToArray();
//                    //partCmp.vertices =  cmp.vertices.Skip(skipVertices++ * takeVertices).Take(takeVertices).ToArray();
//                    Array.Copy(cmp.indices, skipIndices++ * takeIndices, partCmp.indices, 0, takeIndices);
//                    //Array.Copy(cmp.vertices, skipVertices++ * takeVertices, partCmp.vertices, 0, takeVertices);
//                    //Array.Sort(partCmp.indices);

//                    //Array outIndices = Array.CreateInstance(typeof(int[]), 100);
//                    //Array outVertices = Array.CreateInstance(typeof(VertexPositionNormalTexture[]), 100);


//                    RebuildArray(partCmp.indices, cmp.vertices, out indices, out vertices);

//                    partCmp.indices = indices;
//                    partCmp.vertices = vertices;

//                    partCmp.vertexBuffer.SetData(partCmp.vertices, 0, partCmp.vertexCount);
//                    partCmp.indexBuffer.SetData(partCmp.indices, 0, partCmp.indexCount);


//                    partCmp.texture = Texture2D.FromStream(gd, new StreamReader(textureName).BaseStream);
//                    partCmp.texture.Name = textureName;

//                    ComponentManager.StoreComponent(ComponentManager.GetNewId(), partCmp);
//                }
//            }
//        }


//        float rot = 0.001f;
//        public void Draw(Effect effect)
//        {
//            Matrix currentWorldMatrix = effect.Parameters["World"].GetValueMatrix();

//            foreach (HeightmapComponent cmp in ComponentManager.GetComponents<HeightmapComponent>())
//            {
//                effect.Parameters["Texture"].SetValue(cmp.texture);

//                //cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices, 0, cmp.vertexCount);
//                gd.SetVertexBuffer(cmp.vertexBuffer);

//                //cmp.indexBuffer.SetData<int>(cmp.indices, 0, cmp.indexCount);
//                gd.Indices = cmp.indexBuffer;

//                //if (cmp.texture.Name.Contains("fire"))
//                //{
//                //    cmp.objectWorld = Matrix.CreateScale(cmp.scaleFactor)
//                //                    * Matrix.CreateTranslation(cmp.position + (float)System.Math.Sin(rot) *100*  new Vector3(rot, rot, rot));
//                //}
//                //else
//                //{
//                //    cmp.objectWorld = Matrix.CreateScale(cmp.scaleFactor)
//                //                     * Matrix.CreateTranslation(cmp.position);
//                //}
//                //rot += 0.002f;


//                cmp.objectWorld = Matrix.CreateScale(cmp.scaleFactor)
//                                 * Matrix.CreateTranslation(cmp.position);


//                effect.Parameters["World"].SetValue(currentWorldMatrix * cmp.objectWorld);

//                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
//                {
//                    pass.Apply();
//                    gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cmp.indexCount / 3);
//                }
//            }

//            effect.Parameters["World"].SetValue(currentWorldMatrix);
//        }


//        //private void TransferToGraphicsCard()
//        //{
//        //    foreach (HeightmapComponent cmp in ComponentManager.GetComponents<HeightmapComponent>())
//        //    {
//        //        cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices);/*, 0, cmp.vertexCount);*/

//        //        cmp.indexBuffer.SetData<int>(cmp.indices);/*, 0, cmp.indexCount);*/
//        //    }
//        //}


//        private void CreateHeightmapComponents()
//        {
//            foreach (HeightmapObject hmobj in hmobjects)
//            {
//                HeightmapComponent cmp = new HeightmapComponent(gd, hmobj.scaleFactor, hmobj.terrainMapName, hmobj.textureNames, hmobj.world);
//                cmp.breakUpInNumParts = hmobj.breakUpInNumParts;

//                //ComponentManager.StoreComponent(ComponentManager.GetNewId(),cmp);
//                heightmapComponents.Add(cmp);
//            }

//            //heightmapComponents = ComponentManager.GetComponents<HeightmapComponent>();
//        }


//        private void SetUpVertices()
//        {
//            Random rnd = new Random();
//            int index = 0;

//            foreach (HeightmapComponent cmp in heightmapComponents)
//            {
//                for (int x = 0; x < cmp.terrainWidth; x++)
//                {
//                    for (int y = 0; y < cmp.terrainHeight; y++)
//                    {
//                        index = x + y * cmp.terrainWidth;

//                        cmp.vertices[index].Position = new Vector3(x,
//                                                                                      cmp.heightData[x, y],
//                                                                                      -y);

//                        cmp.vertices[index].Position = Vector3.Transform(cmp.vertices[index].Position,
//                                                                                                Matrix.CreateScale(cmp.scaleFactor));

//                        cmp.vertices[index].Normal = new Vector3(rnd.Next(0, 101) / 100f, rnd.Next(0, 101) / 100f, rnd.Next(0, 101) / 100f);
//                        cmp.vertices[index].TextureCoordinate = new Vector2(0, 0);
//                    }
//                }

//                index = 0;
//            }
//        }

//        private void SetUpNormals()
//        {

//            int counter = 0;

//            Vector3 v1 = Vector3.Zero;
//            Vector3 v2 = Vector3.Zero;

//            foreach (HeightmapComponent cmp in heightmapComponents)
//                for (int y = 0; y < cmp.terrainHeight - 1; y++)
//                {
//                    for (int x = 0; x < cmp.terrainWidth - 1; x++)
//                    {
//                        int lowerLeft = x + y * cmp.terrainWidth;
//                        int lowerRight = (x + 1) + y * cmp.terrainWidth;
//                        int topLeft = x + (y + 1) * cmp.terrainWidth;
//                        int topRight = (x + 1) + (y + 1) * cmp.terrainWidth;

//                        v1 = Vector3.Cross(cmp.vertices[topLeft].Position, cmp.vertices[lowerLeft].Position);
//                        //cmp.indices[counter++] = lowerRight;
//                        //cmp.indices[counter++] = lowerLeft;

//                        v2 = Vector3.Cross(cmp.vertices[topRight].Position, cmp.vertices[lowerRight].Position);

//                        cmp.vertices[lowerLeft].Normal = Vector3.Normalize(Vector3.Add(v1, cmp.vertices[lowerRight].Normal));
//                        cmp.vertices[topRight].Normal = Vector3.Normalize(Vector3.Add(v2, cmp.vertices[lowerLeft].Normal));
//                        //cmp.indices[counter++] = topLeft;
//                        //cmp.indices[counter++] = topRight;
//                        //cmp.indices[counter++] = lowerRight;
//                    }
//                }
//        }


//        private void SetUpIndices()
//        {

//            int counter = 0;

//            foreach (HeightmapComponent cmp in heightmapComponents)
//            {
//                for (int y = 0; y < cmp.terrainHeight - 1; y++)
//                {
//                    for (int x = 0; x < cmp.terrainWidth - 1; x++)
//                    {
//                        int lowerLeft = x + y * cmp.terrainWidth;
//                        int lowerRight = (x + 1) + y * cmp.terrainWidth;
//                        int topLeft = x + (y + 1) * cmp.terrainWidth;
//                        int topRight = (x + 1) + (y + 1) * cmp.terrainWidth;

//                        cmp.indices[counter++] = topLeft;
//                        cmp.indices[counter++] = lowerRight;
//                        cmp.indices[counter++] = lowerLeft;

//                        cmp.indices[counter++] = topLeft;
//                        cmp.indices[counter++] = topRight;
//                        cmp.indices[counter++] = lowerRight;
//                    }
//                }

//                counter = 0;
//            }
//        }


//        private void LoadHeightData()
//        {
//            foreach (HeightmapComponent cmp in heightmapComponents)
//                for (int x = 0; x < cmp.terrainWidth; x++)
//                {
//                    for (int y = 0; y < cmp.terrainHeight; y++)
//                    {
//                        System.Drawing.Color color = cmp.bmpHeightdata.GetPixel(x, y);
//                        cmp.heightData[x, y] = ((color.R + color.G + color.B) / 3);
//                    }
//                }

//        }

//        public void Update(GameTime gameTime)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}

