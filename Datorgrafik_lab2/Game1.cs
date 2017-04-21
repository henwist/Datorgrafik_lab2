using Datorgrafik_lab2.CreateModels;
using GameEngine.Components;
using GameEngine.Managers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Datorgrafik_lab2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        BasicEffect effect;
        Effect myeffect;
        Figure figure;

        VertexBuffer figureBuffer;
        IndexBuffer figureIndices;

        Matrix objRotation;

        private float angle = 0f;

        private Matrix _view, _projection;

        //private Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 70.0f);
        private Vector3 cameraPosition = new Vector3(0.0f, 40.0f, 1.0f);

        float radx = 0f;
        float rady = 0f;
        float radz = 0f;
        float scale = 1f;
        float radObj = 0.001f;
        float translatex = 1f;
        float translatey = 1f;
        float translatez = 1f;

        Texture2D grass;

        instance_matrice[] objectWorldMatrices;
        instance_pos[] objectWorldPos;

        static readonly int MATRICE_CHANGE_EVERY_X_INSTANCES_FREQUENCY = 5;
        static readonly int OBJECTPOS_CHANGE_EVERY_X_INSTANCES_FREQUENCY = 1;
        static readonly int COUNTOBJECTPOSITIONS = 10; // set to something dividable by MATRICE_CHANGE_EVERY_X_INSTANCES_FREQUENCY.
        static readonly int INSTANCECOUNT = COUNTOBJECTPOSITIONS;
        static readonly int COUNTOBJECTMATRICES = COUNTOBJECTPOSITIONS / MATRICE_CHANGE_EVERY_X_INSTANCES_FREQUENCY;
        VertexBufferBinding[] bindings;
        VertexBuffer matriceIVB;
        VertexDeclaration matriceVD;

        VertexBuffer posIVB;
        VertexDeclaration posVD;

        private SceneManager sceneManager;


        Tree tree;

        public CameraComponent camera { get; protected set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            figure = new Figure();

            tree = new Tree(graphics.GraphicsDevice, 1f, MathHelper.PiOver4 + 0.4f, "F[LF]F[RF]F", 0, 1f, new string[] { "F" });

            GenerateIVD();

            t();



            base.Initialize();
        }


        private void GenerateIVD()
        {

            matriceVD = new VertexDeclaration(
                                             new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0),
                                             new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
                                             new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 2),
                                             new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 3)
                                             );

            posVD = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 0));
        }


        private void t()
        {

            initMatrices();

            matriceIVB = new VertexBuffer(graphics.GraphicsDevice, matriceVD, COUNTOBJECTMATRICES, BufferUsage.None);
            matriceIVB.SetData<instance_matrice>(objectWorldMatrices);

            posIVB = new VertexBuffer(graphics.GraphicsDevice, posVD, COUNTOBJECTPOSITIONS, BufferUsage.None);
            posIVB.SetData<instance_pos>(objectWorldPos);

            bindings = new VertexBufferBinding[3];
            bindings[0] = new VertexBufferBinding(tree.vertexBuffer);
            bindings[1] = new VertexBufferBinding(matriceIVB, 0, INSTANCECOUNT);
            bindings[2] = new VertexBufferBinding(posIVB, 0, OBJECTPOS_CHANGE_EVERY_X_INSTANCES_FREQUENCY);


        }


        struct instance_matrice
        {
            public Matrix matrice;
            //public Vector4 position;

        }

        struct instance_pos
        {
            public Vector4 position;
        }

        private void initMatrices()
        {
            Random rnd = new Random();

            objectWorldMatrices = new instance_matrice[COUNTOBJECTMATRICES];

            float x = 0, y = 0, z = 0;

            float[] localRotation = { .7f, 1.57f };

            int index = 0;
            foreach (instance_matrice m in objectWorldMatrices)
            {
                objectWorldMatrices[index].matrice = Matrix.Identity
                                             * Matrix.CreateRotationY(localRotation[index]) /** Matrix.CreateTranslation(pos = new Vector3((x++) * scale, (x % 2) * scale, (z++) * scale))*/
                                             ;//* Matrix.CreateScale(rnd.Next(100, 140) / 100f);
                //objectWorldMatrices[index++].position = new Vector4((float)Math.Pow(index, 1.3));
            }



            ////////////////
            objectWorldPos = new instance_pos[COUNTOBJECTPOSITIONS];

            index = 0;
            foreach (instance_pos ipos in objectWorldPos)
            {
                objectWorldPos[index++].position = new Vector4((float)Math.Pow(index, 1.3), 0, 0, 0);
            }


        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            figureBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), figure.vertices.Count, BufferUsage.None);
            figureIndices = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, figure.indices.Length, BufferUsage.None);

            grass = Content.Load<Texture2D>("Textures/grass");



            _view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1, 3000);

            myeffect = Content.Load<Effect>("Effects/myeffect");
            sceneManager = new SceneManager(graphics.GraphicsDevice, Matrix.Identity);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;

        }

        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            foreach (Keys k in Keyboard.GetState().GetPressedKeys())
            {
                if (k == Keys.Escape)
                    this.Exit();
            }


            angle += 0.005f;

            base.Update(gameTime);


            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraPosition.X += 1.0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraPosition.X -= 1.0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                cameraPosition.Y += 1.0f;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cameraPosition.Y -= 1.0f;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                radx += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                rady += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                radz += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                scale += 0.01f;


            if (Keyboard.GetState().IsKeyDown(Keys.E))
                scale -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.O))
                radObj += 0.0001f;



            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                translatex += 1f;

            if (Keyboard.GetState().IsKeyDown(Keys.X))
                translatey += 1f;

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                translatez += 1f;


            if (Keyboard.GetState().IsKeyDown(Keys.V))
                translatex -= 1f;

            if (Keyboard.GetState().IsKeyDown(Keys.B))
                translatey -= 1f;

            if (Keyboard.GetState().IsKeyDown(Keys.N))
                translatez -= 1f;


        }

        private void setBasiceffectParameters()
        {


            effect.World = Matrix.Identity;
            effect.View = _view;
            effect.Projection = _projection;
            effect.PreferPerPixelLighting = true;

            effect.EnableDefaultLighting();
            effect.TextureEnabled = true;
            effect.Texture = grass;
            effect.EmissiveColor = new Vector3(0.5f, 0.5f, 0f);

            effect.EnableDefaultLighting();
            effect.LightingEnabled = true;

            effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.DirectionalLight0.Direction = new Vector3(0.5f, -1, 0);
            effect.DirectionalLight0.SpecularColor = new Vector3(1, 1, 1);
            effect.DirectionalLight0.Enabled = true;

            effect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
            effect.PreferPerPixelLighting = true;
            effect.SpecularPower = 100;
            effect.DiffuseColor = new Vector3(0.4f, 0.4f, 0.7f);
            effect.EmissiveColor = new Vector3(1.0f, 1.0f, 1.0f);

            effect.World =
            Matrix.CreateRotationX(radx)
            * Matrix.CreateRotationY(rady)
            * Matrix.CreateRotationZ(radz)
            * Matrix.CreateScale(scale);
        }

        private void setCustomShaderParameters()
        {



            myeffect.Parameters["World"].SetValue(/*objectWorldMatrices[counter++ % countObjectMatrices]*/
                            Matrix.CreateRotationX(radx)
                            * Matrix.CreateRotationY(rady)
                            * Matrix.CreateRotationZ(radz)
                            * Matrix.CreateScale(scale)
                            * Matrix.CreateTranslation(translatex, translatey, translatez));


            //myeffect.Parameters["World"].SetValue(Matrix.Identity);
            myeffect.Parameters["View"].SetValue(_view);
            myeffect.Parameters["Projection"].SetValue(_projection);
            myeffect.Parameters["Texture"].SetValue(grass);


        }

        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkSlateBlue);


            foreach (EffectPass pass in myeffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                figureBuffer.SetData(figure.vertices.ToArray());
                figureIndices.SetData(figure.indices);

                graphics.GraphicsDevice.SetVertexBuffer(figureBuffer);
                graphics.GraphicsDevice.Indices = figureIndices;

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, figure.vertices.Count / 3);

                //Trying rotation of object's world for all objects.
                //objRotation = Matrix.CreateRotationY(radObj) * Matrix.CreateRotationZ(0.0001f);
                //bindings[1].VertexBuffer.GetData<instance_matrice>(objectWorldMatrices);

                //int i = 0;
                //foreach (instance_matrice m in objectWorldMatrices)
                //{
                //    Vector3 translate = m.matrice.Translation;
                //    //translate.Normalize();
                //    objectWorldMatrices[i++].matrice = m.matrice * Matrix.CreateTranslation(-1 * translate) * objRotation * Matrix.CreateTranslation(translate);

                //}

                //bindings[1].VertexBuffer.SetData<instance_matrice>(objectWorldMatrices);
                ////end trying rotation.


                ////setBasiceffectParameters();
                setCustomShaderParameters();

                //graphics.GraphicsDevice.Indices = tree.indexBuffer;

                //graphics.GraphicsDevice.SetVertexBuffers(bindings);

                ////tree
                //graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.LineList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 2, INSTANCECOUNT);

                //boxes
                //graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, tree.vertexBuffer.VertexCount, 0, tree.indexBuffer.IndexCount / 3, INSTANCECOUNT);

            }
            //}

            //sceneManager.Draw(effect, gameTime);

            base.Draw(gameTime);


        }


        private void CreateControllerBindings()
        {
            //controller.AddBinding(Keys.A, new Vector2(-10, 0));
            //controller.AddBinding(Keys.D, new Vector2(10, 0));
            //controller.AddBinding(Keys.W, new Vector2(0, -10));
            //controller.AddBinding(Keys.S, new Vector2(0, 10));
        }

        private void MovePlayer()
        {
            //ComponentManager.GetComponent<Transform>(player1.PlayerId).XVel = controller.GetNextMove().X;
            //ComponentManager.GetComponent<Transform>(player1.PlayerId).YVel = controller.GetNextMove().Y;

        }


        private enum Constants : int
        {
            Pristine = 0,
            GameLength = 6000
        }
    }
}