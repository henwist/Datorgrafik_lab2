using GameEngine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Datorgrafik_lab1
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
        VertexPositionNormalTexture[] vertices;
        Matrix viewMatrix;
        Matrix projectionMatrix;

        private float angle = 0f;

        private Matrix _view, _projection;

        private Vector3 cameraPosition = new Vector3(200.0f, 200.0f, 100.0f);

        float cameraMovex = 0f;
        float radx = 0f;
        float rady = 0f;
        float radz = 0f;
        float scale = 1f;

        Texture2D grass;

        private SceneManager sceneManager;

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

            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            effect = new BasicEffect(graphics.GraphicsDevice);

            grass = Content.Load<Texture2D>("Textures/grass");

            _view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1, 3000);

            effect.World = Matrix.Identity;
            effect.View = _view;
            effect.Projection = _projection;
            effect.PreferPerPixelLighting = true;

            sceneManager = new SceneManager(graphics.GraphicsDevice, effect.World);

        }

        protected override void UnloadContent()
        {
        }


        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 10, 0), new Vector3(0, 0, 0), new Vector3(0, 0, -1));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 300.0f);
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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
        }


        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkSlateBlue);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            device.RasterizerState = rs;


            effect.EnableDefaultLighting();
            effect.TextureEnabled = true;
            //effect.Texture = grass;

            Matrix worldMatrix = Matrix.Identity;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {


                pass.Apply();

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

                _view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1, 3000);



                effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                //* Matrix.CreateRotationX(radx)
                //* Matrix.CreateRotationY(rady)
                //* Matrix.CreateRotationZ(radz);

                //effect.World = Matrix.CreateRotationX(radx)
                //                    * Matrix.CreateRotationY(rady)
                //                    * Matrix.CreateRotationZ(radz)
                //                    * Matrix.CreateScale(scale);


                sceneManager.Draw(effect, gameTime);

            }

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
