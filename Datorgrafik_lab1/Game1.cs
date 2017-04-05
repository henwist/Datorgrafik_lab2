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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

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
