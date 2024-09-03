using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Reflection.Metadata.BlobBuilder;

namespace AAAAAAAAAAAAAAAAAAAA {

    public static class ResourceManager {
        public static GraphicsDevice GraphicsDevice { get; set; }
    }

    public class Game1: Game {

        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;

        private World world = new World();

        private Random rand = new Random();

        FastNoiseLite noise = new FastNoiseLite();
        private float noiseScale = 0.3f; //0.3f
        private float vertScale = 15f;
        private float threshold = 0.3f;

        private int renderDistance = 5; // 16 - 18 fps on 500 render dist before greedy meshing

        public Game1() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            AllocConsole();

            //world.player.movementSpeed = 500f;
            world.player.gravity = 15f;

            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetFractalType(FastNoiseLite.FractalType.None);
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 144.0);
            IsFixedTimeStep = true;

            graphics.IsFullScreen = true;

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;

            string[] bruh = new string[3];
            bruh[0] = "dark_gray";
            bruh[1] = "gray";
            bruh[2] = "grass";

            noise.SetSeed(rand.Next());

            world.AOEnabled = false;

            for (int x = -renderDistance; x < renderDistance + 1; x++) {
                    Console.Clear();
                    Console.CursorLeft = 0;
                    Console.CursorTop = 0;
                    Console.Write(x);

                for (int y = -renderDistance; y < renderDistance + 1; y++) {
                    for (int z = -renderDistance; z < renderDistance + 1; z++) {
                        world.generateChunk(x, y, z);
                    }
                }
            }
        }

        protected override void Initialize() {
            base.Initialize();
            ResourceManager.GraphicsDevice = GraphicsDevice;
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("ArialFont");
            world.lighting = Content.Load<Effect>("Lighting");

            world.FogEnabled = true;
            world.FogNear = 50;
            world.FogFar = 550;

            world.projectionMatrix = Matrix.CreatePerspectiveFieldOfView(world.player.fieldOfView, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);

            world.texture = Content.Load<Texture2D>("texsheet");

            world.texture.GraphicsDevice.SamplerStates[0] = new SamplerState { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap };

            world.lighting.Parameters[$"Texture"].SetValue(world.texture);

            //world.lighting.Parameters[$"LightDirection"].SetValue(new Microsoft.Xna.Framework.Vector4(0, 0, 0, 0));
            //world.lighting.Parameters[$"AmbientColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1, 1, 1, 1));

            for (int i = 0; i < 4; i++) {
                //world.lighting.Parameters[$"PointLights[{i}].Position"].SetValue(new Microsoft.Xna.Framework.Vector3(0, 0, 0));
                //world.lighting.Parameters[$"PointLights[{i}].Color"].SetValue(new Microsoft.Xna.Framework.Vector3(0, 0, 0));
                //world.lighting.Parameters[$"PointLights[{i}].Attenuation"].SetValue(new Microsoft.Xna.Framework.Vector3(1, 0.1f, 0.01f));
            }

            //world.addLight(new Microsoft.Xna.Framework.Vector3(0, 0, 0), new Microsoft.Xna.Framework.Vector3(0, 1, 0));

            world.regenerate();

        }

        protected override void Update(GameTime gameTime) {

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            IsMouseVisible = !world.player.mouseLock;

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
                world.player.movementSpeed = 150f;
            } else {
                world.player.movementSpeed = 40f;
            }

            world.updatePlayer(deltaTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                world.player.yVel += 0.4f;

            if (Keyboard.GetState().IsKeyDown(Keys.E))
                world.player.r = 0;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            //GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.Clear(Color.SkyBlue);

            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(world.player.r, world.player.t, 0);
            Microsoft.Xna.Framework.Vector3 lookDirection = Microsoft.Xna.Framework.Vector3.Transform(Microsoft.Xna.Framework.Vector3.Forward, rotationMatrix);
            Microsoft.Xna.Framework.Vector3 upDirection = Microsoft.Xna.Framework.Vector3.Transform(Microsoft.Xna.Framework.Vector3.Up, rotationMatrix);
            Matrix viewMatrix = Matrix.CreateLookAt(world.player.cameraPosition, world.player.cameraPosition + lookDirection, upDirection);

            //world.lighting.Parameters["WorldViewProjection"].SetValue(Matrix.CreateTranslation(0, 0, 0) * viewMatrix * world.projectionMatrix);

            world.lighting.Parameters["World"].SetValue(Matrix.CreateTranslation(0, 0, 0));
            world.lighting.Parameters["playerPos"].SetValue(world.player.position);

            //world.lighting.Parameters["AmbientColor"].SetValue(new Microsoft.Xna.Framework.Vector4(1f, 1f, 1f, 1f));
            //world.lighting.Parameters["AmbientIntensity"].SetValue(1f);

            world.lighting.Parameters["View"].SetValue(viewMatrix);
            world.lighting.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(world.player.fieldOfView, GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000f));

            world.render();

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "FPS: " + (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString("0.0"), new Microsoft.Xna.Framework.Vector2(10, 10), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}