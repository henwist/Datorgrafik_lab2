//using CollisionSample;
using GameEngine.Components;
using GameEngine.Helpers;
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


        private void CreateHeightmapComponent(HeightmapComponent cmp, out HeightmapComponent hmComponent, out BufferComponent buffer)
        {
            int vCount = (cmp.terrainWidth * cmp.terrainHeight) / cmp.breakUpInNumParts;
            int iCount = (cmp.terrainWidth-1) * (cmp.terrainHeight - 1) * 6 / cmp.breakUpInNumParts;

            BufferComponent bufCmp = new BufferComponent()
            {
                IndexBuffer = new IndexBuffer(gd, typeof(int), iCount, BufferUsage.None),
                Indices = new int[iCount],
                Texture = new Texture2D[1],
                PrimitiveType = PrimitiveType.TriangleList,
                PrimitiveCount = iCount / 3,
                
            };


            HeightmapComponent partCmp = new HeightmapComponent()
            {
                breakUpInNumParts = 1,
                spacingBetweenParts = cmp.spacingBetweenParts,

                indexCount = iCount,
                position = cmp.position,
                scaleFactor = cmp.scaleFactor,

                terrainWidth = cmp.terrainWidth,
                terrainHeight = cmp.terrainHeight,
                heightData = cmp.heightData,
            };

            hmComponent = partCmp;
            buffer = bufCmp;
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

                    HeightmapComponent partCmp;
                    BufferComponent buffer;

                    CreateHeightmapComponent(cmp, out partCmp, out buffer);

                    takeIndices = partCmp.indexCount;

                    Array.Copy(cmp.indices, skipIndices++ * takeIndices, buffer.Indices, 0, takeIndices);

                    RebuildArray(buffer.Indices, cmp.vertices, out indices, out vertices);

                    buffer.Indices = indices;
                    buffer.Vertices = vertices;
                    buffer.VertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vertices.Count(), BufferUsage.None);
                    partCmp.vertexCount = vertices.Count();

                    buffer.VertexBuffer.SetData(buffer.Vertices);
                    buffer.IndexBuffer.SetData(buffer.Indices);

                    buffer.Texture[0] = Texture2D.FromStream(gd, new StreamReader(textureName).BaseStream);
                    buffer.Texture[0].Name = textureName;

                    partCmp.spacingBetweenParts = counter++ * cmp.spacingBetweenParts;

                    partCmp.terrainFileName = cmp.terrainFileName;

                    createComponents(partCmp, buffer);
                }
            }

        }


        private void createComponents(HeightmapComponent partCmp, BufferComponent buffer)
        {
            TransformComponent transform = new TransformComponent(partCmp.position, 0f, 0f, 0f, partCmp.scaleFactor.X);

            ComponentManager.StoreComponent(ComponentManager.GetNewId(), BoundingVolume.GetBoundingBoxVolume(buffer.Vertices, transform.ObjectWorld));
            ComponentManager.StoreComponent(ComponentManager.GetCurrentId(), partCmp);
            ComponentManager.StoreComponent(ComponentManager.GetCurrentId(), buffer);
            ComponentManager.StoreComponent(ComponentManager.GetCurrentId(), transform);
        }


        public void Draw(GameTime gameTime)
        {
            //EffectComponent effectCmp = ComponentManager.GetComponents<EffectComponent>().Cast<EffectComponent>().Select(x => x).ElementAt(0);
            //BasicEffect effect = effectCmp.effect;

            //Matrix currentWorldMatrix= ComponentManager.GetComponents<WorldMatrixComponent>().Cast<WorldMatrixComponent>().Select(x => x).ElementAt(0).WorldMatrix;

            //CameraComponent camera = ComponentManager.GetComponents<CameraComponent>() //it should be just one camera that is actice.
            //                                                       .Cast<CameraComponent>().Select(x => x).Where(y => y.isActive == true).ElementAt(0);


            //foreach (ulong entId in ComponentManager.GetAllIds<HeightmapComponent>() )
            //{
            //    HeightmapComponent cmp = ComponentManager.GetComponent<HeightmapComponent>(entId);
            //    BoundingVolumeComponent bvCmp = ComponentManager.GetComponent<BoundingVolumeComponent>(entId);
            //    BufferComponent buffer = ComponentManager.GetComponent<BufferComponent>(entId);
            //    TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(entId);

            //    effect.Texture = buffer.Texture[0];

            //    //cmp.objectWorld = Matrix.CreateTranslation(cmp.position + cmp.spacingBetweenParts)
            //    //                * Matrix.CreateScale(cmp.scaleFactor);



            //    effect.World = transform.ObjectWorld * currentWorldMatrix;
                 
            //    gd.SetVertexBuffer(buffer.VertexBuffer);


            //    gd.Indices = buffer.IndexBuffer;

            //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //    {
            //        pass.Apply();

            //        if (bvCmp.bbox.Intersects(camera.bFrustum)) //Just draw all parts of heightmap that is anyhow inside the camera frustum.
            //            gd.DrawIndexedPrimitives(buffer.PrimitiveType, 0, 0, buffer.PrimitiveCount);
            //    }

            //    BoundingVolume.DrawBoundingVolume(gd, bvCmp, camera, transform.ObjectWorld * currentWorldMatrix);
            //}

        }


        private void CreateHeightmapComponents()
        {
            foreach (HeightmapObject hmobj in hmobjects)
            {
                HeightmapComponent cmp = new HeightmapComponent(gd, hmobj.scaleFactor, hmobj.terrainFileName, hmobj.textureFileNames/*, hmobj.world*/);
                cmp.breakUpInNumParts = hmobj.breakUpInNumParts;
                cmp.spacingBetweenParts = hmobj.spacingBetweenParts;
                cmp.position = hmobj.position;
                cmp.scaleFactor = hmobj.scaleFactor;

                heightmapComponents.Add(cmp);
            }
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

                        cmp.vertices[index].Position = new Vector3(x, cmp.heightData[x, y], -y);

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

                        v2 = Vector3.Cross(cmp.vertices[topRight].Position, cmp.vertices[lowerRight].Position);

                        cmp.vertices[lowerLeft].Normal = Vector3.Normalize(Vector3.Add(v1, cmp.vertices[lowerRight].Normal));
                        cmp.vertices[topRight].Normal = Vector3.Normalize(Vector3.Add(v2, cmp.vertices[lowerLeft].Normal));

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