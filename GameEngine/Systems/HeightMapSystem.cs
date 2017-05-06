using CollisionSample;
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

        private DebugDraw debugDraw;

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

            debugDraw = new DebugDraw(gd);

            heightmapComponents = new List<Component>();

            CreateHeightmapComponents();

            LoadHeightData();
            SetUpVertices();
            SetUpIndices();

            SetUpNormals();

            SplitHeightmap();

            SetupDebugDraw();
        }

        private void SetupDebugDraw()
        {
            CameraComponent camera = ComponentManager.GetComponents<CameraComponent>().Cast<CameraComponent>().Select(x => x).Where(y => y.isActive == true).ElementAt(0);

            debugDraw.Begin(camera.viewMatrix, camera.projectionMatrix);
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

                indexBuffer = new IndexBuffer(gd, typeof(int), iCount, BufferUsage.None),

                indices = new int[iCount],

                indexCount = iCount,

                texture = cmp.texture,
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
                    partCmp.vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionNormalTexture), vertices.Count(), BufferUsage.None);
                    partCmp.vertexCount = vertices.Count();

                    partCmp.vertexBuffer.SetData(partCmp.vertices);
                    partCmp.indexBuffer.SetData(partCmp.indices);

                    partCmp.texture = Texture2D.FromStream(gd, new StreamReader(textureName).BaseStream);
                    partCmp.texture.Name = textureName;

                    partCmp.spacingBetweenParts = counter++ * cmp.spacingBetweenParts;

                    partCmp.terrainFileName = cmp.terrainFileName;

                    createComponents(partCmp);
                }
            }

            saveIndicesToDisc();
        }


        private void createComponents(HeightmapComponent partCmp)
        {
            ComponentManager.StoreComponent(ComponentManager.GetNewId(), createBoundingBox(partCmp));
            ComponentManager.StoreComponent(ComponentManager.GetCurrentId(), partCmp);
        }


        private BoundingVolumeComponent createBoundingBox(HeightmapComponent partCmp)
        {
            BoundingVolumeComponent bvCmp = new BoundingVolumeComponent();
            bvCmp.bbox = BoundingBox.CreateFromPoints(partCmp.vertices.Select(x => x.Position));

            return bvCmp;
        }


        void saveIndicesToDisc()
        {
            int counter = 1;
            using (StreamWriter outstream = new StreamWriter("./indices.txt"))
            {
                foreach(HeightmapComponent cmp in ComponentManager.GetComponents<HeightmapComponent>())
                {
                    foreach (int num in cmp.indices)
                    {
                        outstream.Write(num.ToString());
                        outstream.Write(",");

                        if(counter++ >= 40 )
                        {
                            outstream.Write('\n');
                            counter = 1;
                        }

                    }

                    outstream.Write('\n');
                    outstream.Write('\n');
                }
            }

        }


        float rot = 0.001f;
        public void Draw(Effect effect)
        {
            Matrix currentWorldMatrix = effect.Parameters["World"].GetValueMatrix();

            CameraComponent camera = ComponentManager.GetComponents<CameraComponent>()
                                                                   .Cast<CameraComponent>().Select(x => x).Where(y => y.isActive == true).ElementAt(0);


            foreach (ulong entId in ComponentManager.GetAllIds<HeightmapComponent>() )
            {
                HeightmapComponent cmp = ComponentManager.GetComponent<HeightmapComponent>(entId);
                BoundingVolumeComponent bvCmp = ComponentManager.GetComponent<BoundingVolumeComponent>(entId);

                effect.Parameters["Texture"].SetValue(cmp.texture);

                //cmp.vertexBuffer.SetData<VertexPositionNormalTexture>(cmp.vertices, 0, cmp.vertexCount);
                //gd.SetVertexBuffer(cmp.vertexBuffer);

                ////cmp.indexBuffer.SetData<int>(cmp.indices, 0, cmp.indexCount);
                //gd.Indices = cmp.indexBuffer;

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

                debugDraw.DrawWireBox(bvCmp.bbox, Color.White);

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    gd.SetVertexBuffer(cmp.vertexBuffer);

                    //cmp.indexBuffer.SetData<int>(cmp.indices, 0, cmp.indexCount);
                    gd.Indices = cmp.indexBuffer;
                        
                
                    if (bvCmp.bbox.Intersects(camera.bFrustum)) //Just draw all parts of heightmap that is anyhow inside the camera frustum.
                        gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cmp.indexCount / 3);
                }
            }
           

            effect.Parameters["World"].SetValue(currentWorldMatrix);

            debugDraw.DrawWireFrustum(camera.bFrustum, Color.White);

            
        }


        private void CreateHeightmapComponents()
        {
            foreach (HeightmapObject hmobj in hmobjects)
            {
                HeightmapComponent cmp = new HeightmapComponent(gd, hmobj.scaleFactor, hmobj.terrainFileName, hmobj.textureFileNames, hmobj.world);
                cmp.breakUpInNumParts = hmobj.breakUpInNumParts;
                cmp.spacingBetweenParts = hmobj.spacingBetweenParts;

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