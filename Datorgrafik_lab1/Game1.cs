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
        int[] indices;

        private float angle = 0f;
        private int terrainWidth = 4;
        private int terrainHeight = 3;
        private float[,] heightData;

        private Matrix _view, _projection;

        private Vector3 cameraPosition = new Vector3(200.0f, 200.0f, 100.0f);


        float cameraMovex = 0f;
        float radx = 0f;
        float rady = 0f;
        float radz = 0f;
        float scale = 1f;

        Texture2D cross;


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
            Window.Title = "Riemer's XNA Tutorials -- 3D Series 1";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            device = graphics.GraphicsDevice;

            effect = new BasicEffect(graphics.GraphicsDevice);

            cross = Content.Load<Texture2D>("Textures/grass");

            RasterizerState rstate = new RasterizerState();
            rstate.CullMode = CullMode.None;
            rstate.FillMode = FillMode.Solid;

            graphics.GraphicsDevice.RasterizerState = rstate;

            _view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1, 3000);

            effect.World = Matrix.Identity;
            effect.View = _view;
            effect.Projection = _projection;
            effect.PreferPerPixelLighting = true;

            LoadHeightData();
            SetUpVertices();
            SetUpIndices();
        }

        protected override void UnloadContent()
        {
        }

        private void SetUpVertices()
        {
            vertices = new VertexPositionNormalTexture[terrainWidth * terrainHeight];
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
                cameraPosition.X += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraPosition.X -= 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                radx += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                rady += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                radz += 0.1f;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                scale += 0.5f;


            if (Keyboard.GetState().IsKeyDown(Keys.E))
                scale -= 0.5f;
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
            effect.Texture = cross;

            /*effect.CurrentTechnique = effect.Techniques["ColoredNoShading"];
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);*/

            Matrix worldMatrix = Matrix.Identity;
            //ffect.Parameters["xWorld"].SetValue(worldMatrix);

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

                effect.World = Matrix.CreateRotationX(radx)
                                    * Matrix.CreateRotationY(rady)
                                    * Matrix.CreateRotationZ(radz)
                                    * Matrix.CreateScale(scale);

                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);


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



        private void CreatePlayer()
        {
            //Transform trans = new Transform();

            //Collide coll = new Collide();

            //MySprite sprite = new MySprite();
            //sprite.Texture = Content.Load<Texture2D>("runningcat");
            //sprite.Width = 512;
            //sprite.Height = 256;

            //Score score = new Score();
            //score.Font = Content.Load<SpriteFont>("font");

            //Sound sound = new Sound();
            //sound.sound = Content.Load<SoundEffect>("laserfire");
            //sound.soundInstance = sound.sound.CreateInstance();

            //ComponentManager.StoreComponent(player1.PlayerId, trans);
            //ComponentManager.StoreComponent(player1.PlayerId, coll);
            //ComponentManager.StoreComponent(player1.PlayerId, sprite);
            //ComponentManager.StoreComponent(player1.PlayerId, score);
            //ComponentManager.StoreComponent(player1.PlayerId, sound);

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
