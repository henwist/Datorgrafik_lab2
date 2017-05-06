using Datorgrafik_lab2.CreateModels;
using GameEngine.Components;
using GameEngine.Managers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static Datorgrafik_lab2.Enums.Enums;

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

        private float angle = 0f;

        private Matrix _view, _projection;

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

        private Managers.SceneManager sceneManager;

        public CameraComponent camera { get; protected set; }
        private int CAMERA_MOVE_SCALE = 10;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.ApplyChanges();

            figure = new Figure();

            base.Initialize();
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
            sceneManager = new Managers.SceneManager(this, Matrix.Identity);

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


            //if (Keyboard.GetState().IsKeyDown(Keys.Right))
            //    cameraPosition.X += 1.0f;

            //if (Keyboard.GetState().IsKeyDown(Keys.Left))
            //    cameraPosition.X -= 1.0f;

            //if (Keyboard.GetState().IsKeyDown(Keys.Up))
            //    cameraPosition.Y += 1.0f;

            //if (Keyboard.GetState().IsKeyDown(Keys.Down))
            //    cameraPosition.Y -= 1.0f;

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




            TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(sceneManager.cameraID);
            //CameraComponent curCam = ComponentManager.GetComponent<CameraComponent>();

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
                transform.position.X += 1f * CAMERA_MOVE_SCALE;

            if (Keyboard.GetState().IsKeyDown(Keys.X))
                transform.position.Y += 1f * CAMERA_MOVE_SCALE;

            if (Keyboard.GetState().IsKeyDown(Keys.C))
                transform.position.Z += 1f * CAMERA_MOVE_SCALE;


            if (Keyboard.GetState().IsKeyDown(Keys.V))
                transform.position.X -= 1f * CAMERA_MOVE_SCALE;

            if (Keyboard.GetState().IsKeyDown(Keys.B))
                transform.position.Y -= 1f * CAMERA_MOVE_SCALE;

            if (Keyboard.GetState().IsKeyDown(Keys.N))
                transform.position.Z -= 1f * CAMERA_MOVE_SCALE;

            if (Keyboard.GetState().IsKeyDown(Keys.M))
                transform.position.Z = transform.position.Y = transform.position.X = 0f;

            sceneManager.Update(gameTime);

        }

        float posX = 0.01f;

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
            myeffect.Parameters["World"].SetValue(
                            Matrix.CreateRotationX(radx)
                            * Matrix.CreateRotationY(rady)
                            * Matrix.CreateRotationZ(radz)
                            * Matrix.CreateScale(scale)
                            * Matrix.CreateTranslation(translatex, translatey, translatez));


            //myeffect.Parameters["View"].SetValue(_view);
            //myeffect.Parameters["Projection"].SetValue(_projection);

            //Matrix viewMatrix = ComponentManager.GetComponent<CameraComponent>(sceneManager.cameraID).viewMatrix;
            //Matrix projMatrix = ComponentManager.GetComponent<CameraComponent>(sceneManager.cameraID).projectionMatrix;
            //myeffect.Parameters["View"].SetValue(viewMatrix);
            //myeffect.Parameters["Projection"].SetValue(projMatrix);

            myeffect.Parameters["Texture"].SetValue(grass);
        }


        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkSlateBlue);

            Window.Title = "Datorgrafik_lab2 av: Rasmus Lundquist(S142465) och Henrik Wistbacka(S142066) - a,s,d,w,e tangenter fungerar.";


            setCustomShaderParameters();

            foreach (EffectPass pass in myeffect.Techniques[(int)EnumTechnique.CurrentTechnique].Passes)
            {
                pass.Apply();

                figureBuffer.SetData(figure.vertices.ToArray());
                figureIndices.SetData(figure.indices);

                graphics.GraphicsDevice.SetVertexBuffer(figureBuffer);
                graphics.GraphicsDevice.Indices = figureIndices;

                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, figure.vertices.Count / 3);


                sceneManager.Draw(myeffect, gameTime);

            }

            base.Draw(gameTime);
        }


        //private void CreateControllerBindings()
        //{
        //    controller.AddBinding(Keys.A, new Vector2(-10, 0));
        //    controller.AddBinding(Keys.D, new Vector2(10, 0));
        //    controller.AddBinding(Keys.W, new Vector2(0, -10));
        //    controller.AddBinding(Keys.S, new Vector2(0, 10));
        //}

        //private void MoveCamera()
        //{
        //    ComponentManager.GetComponent<Transform>(player1.PlayerId).XVel = controller.GetNextMove().X;
        //    ComponentManager.GetComponent<Transform>(player1.PlayerId).YVel = controller.GetNextMove().Y;

        //}


        //private enum Constants : int
        //{
        //    Pristine = 0,
        //    GameLength = 6000
        //}
    }
}