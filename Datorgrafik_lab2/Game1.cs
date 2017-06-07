using Datorgrafik_lab2.CreateModels;
using GameEngine.Components;
using GameEngine.Managers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using static Datorgrafik_lab2.Enums.Enums;

namespace Datorgrafik_lab2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        GraphicsDevice device;

        Figure figure;

        VertexBuffer figureBuffer;
        IndexBuffer figureIndices;

        private float angle = 0f;

        float radx = 0f;
        float rady = 0f;
        float radz = 0f;
        float scale = 1f;
        float radObj = 0.001f;
        float translatex = 1f;
        float translatey = 1f;
        float translatez = 1f;

        private Managers.SceneManager sceneManager;

        private int CAMERA_MOVE_SCALE = 10;

        public static ContentManager ContentManager;

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

            base.Initialize();
        }


        protected override void LoadContent()
        {
            Game1.ContentManager = Content;

            device = graphics.GraphicsDevice;

            figure = new Figure(device);
            figureBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionNormalTexture), figure.vertices.Count, BufferUsage.None);
            figureIndices = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, figure.indices.Length, BufferUsage.None);

            sceneManager = new Managers.SceneManager(this, Matrix.Identity);

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


            angle += 0.01f;

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
                radx += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                rady += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                radz += 0.02f;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
                scale += 0.01f;


            if (Keyboard.GetState().IsKeyDown(Keys.E))
                scale -= 0.01f;

            if (Keyboard.GetState().IsKeyDown(Keys.O))
                radObj += 0.0001f;

            figure.Update();


            //TransformComponent transform = ComponentManager.GetComponent<TransformComponent>(sceneManager.cameraID);
            //CameraComponent curCam = ComponentManager.GetComponent<CameraComponent>();

            //if (Keyboard.GetState().IsKeyDown(Keys.Z))
            //    transform.position.X += 1f * CAMERA_MOVE_SCALE;

            //if (Keyboard.GetState().IsKeyDown(Keys.X))
            //    transform.position.Y += 1f * CAMERA_MOVE_SCALE;

            //if (Keyboard.GetState().IsKeyDown(Keys.C))
            //    transform.position.Z += 1f * CAMERA_MOVE_SCALE;


            //if (Keyboard.GetState().IsKeyDown(Keys.V))
            //    transform.position.X -= 1f * CAMERA_MOVE_SCALE;

            //if (Keyboard.GetState().IsKeyDown(Keys.B))
            //    transform.position.Y -= 1f * CAMERA_MOVE_SCALE;

            //if (Keyboard.GetState().IsKeyDown(Keys.N))
            //    transform.position.Z -= 1f * CAMERA_MOVE_SCALE;

            //if (Keyboard.GetState().IsKeyDown(Keys.M))
            //    transform.position.Z = transform.position.Y = transform.position.X = 0f;

            sceneManager.Update(gameTime);

            //figure.RotateBodyPart(0.7f,);

        }


        private void setShaderParameters()
        {
            ComponentManager.GetComponents<WorldMatrixComponent>().Cast<WorldMatrixComponent>().Select(x => x).ElementAt(0).WorldMatrix =
                                            Matrix.CreateRotationX(radx)
                            * Matrix.CreateRotationY(rady)
                            * Matrix.CreateRotationZ(radz)
                            * Matrix.CreateScale(scale)
                            * Matrix.CreateTranslation(translatex, translatey, translatez);
        }


        protected override void Draw(GameTime gameTime)
        {
            device.Clear(Color.DarkSlateBlue);

            Window.Title = "Datorgrafik_lab2 av: Rasmus Lundquist(S142465) och Henrik Wistbacka(S142066) - a,s,d,w,e,arrows. Gubbe gömmer sig mitt på mappen bland träden, W zoomar.";


            setShaderParameters();

            figure.Draw(gameTime);

            sceneManager.Draw(gameTime);


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