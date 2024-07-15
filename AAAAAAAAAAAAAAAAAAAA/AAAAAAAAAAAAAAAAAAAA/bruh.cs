using AAAAAAAAAAAAAAAAAAAA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace AAAAAAAAAAAAAAAAAAAA {
    internal class World {

        public Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>> chunk = new Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>>();
        public Dictionary<int, Dictionary<int, Dictionary<int, VertexPositionTexture[]>>> vertices = new Dictionary<int, Dictionary<int, Dictionary<int, VertexPositionTexture[]>>>();
        public Dictionary<int, Dictionary<int, Dictionary<int, int[]>>> indices = new Dictionary<int, Dictionary<int, Dictionary<int, int[]>>>();

        public Dictionary<int, Dictionary<int, Dictionary<int, int>>> primitiveCount = new Dictionary<int, Dictionary<int, Dictionary<int, int>>>();
        public int chunkSize { get; set; }
        public Texture2D texture { get; set; }
        public BasicEffect effect { get; set; }
        public Player player { get; set; }

        private Random rand = new Random();

        FastNoiseLite noise = new FastNoiseLite();
        private float noiseScale = 3.3f; //0.3f
        private float vertScale = 5f;
        private float threshold = 0.3f;

        public World() {
            noise.SetSeed(rand.Next());
            chunk = new Dictionary<int, Dictionary<int, Dictionary<int, List<Block>>>>();
            chunkSize = 2;
            player = new Player(new Vector3(0, 0, 0), 10f);

        }

        /// <summary>
        /// Takes in Player, Block, and Deltatime data
        /// and updates player position and rotation
        /// </summary>
        /// <param name="player">Player object</param>
        /// <param name="blocks">List of blocks</param>
        /// <param name="deltaTime">Deltatime float</param>
        public void updatePlayer(float deltaTime) {

            Microsoft.Xna.Framework.Point screenCenter = new Microsoft.Xna.Framework.Point(ResourceManager.GraphicsDevice.Viewport.Width / 2, ResourceManager.GraphicsDevice.Viewport.Height / 2);

            player.bruh(deltaTime);

            if (player.canMove) {
                if (player.mouseLock) {
                    // Get the current mouse state
                    var mouseState = Mouse.GetState();

                    // Calculate the difference between the current mouse position and the center of the screen
                    int deltaX = mouseState.X - screenCenter.X;
                    int deltaY = mouseState.Y - screenCenter.Y;

                    // Update yaw and pitch based on mouse movement
                    player.r -= deltaX * player.mouseSensitivity;
                    player.t -= deltaY * player.mouseSensitivity;

                    // Clamp the pitch to prevent flipping
                    player.t = MathHelper.Clamp(player.t, -MathHelper.PiOver2, MathHelper.PiOver2);
                }

                // Get keyboard state
                var kstate = Keyboard.GetState();

                // Move player
                /*
                if (kstate.IsKeyDown(Keys.W)) {
                    player.xVel += (float)Math.Sin(player.r) * -player.movementSpeed * deltaTime;
                    player.zVel += (float)Math.Cos(player.r) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.A)) {
                    player.xVel += (float)Math.Sin(player.r + MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                    player.zVel += (float)Math.Cos(player.r + MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.S)) {
                    player.xVel += (float)Math.Sin(player.r + MathHelper.Pi) * -player.movementSpeed * deltaTime;
                    player.zVel += (float)Math.Cos(player.r + MathHelper.Pi) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.D)) {
                    player.xVel += (float)Math.Sin(player.r - MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                    player.zVel += (float)Math.Cos(player.r - MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.Space) && player.onGround) {
                    player.yVel = player.jumpHeight;
                }
                */

                if (kstate.IsKeyDown(Keys.W)) {
                    player.x += (float)Math.Sin(player.r) * -player.movementSpeed * deltaTime;
                    player.z += (float)Math.Cos(player.r) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.A)) {
                    player.x += (float)Math.Sin(player.r + MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                    player.z += (float)Math.Cos(player.r + MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.S)) {
                    player.x += (float)Math.Sin(player.r + MathHelper.Pi) * -player.movementSpeed * deltaTime;
                    player.z += (float)Math.Cos(player.r + MathHelper.Pi) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.D)) {
                    player.x += (float)Math.Sin(player.r - MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                    player.z += (float)Math.Cos(player.r - MathHelper.PiOver2) * -player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.Space)) {
                    player.y += player.movementSpeed * deltaTime;
                }
                if (kstate.IsKeyDown(Keys.LeftShift)) {
                    player.y -= player.movementSpeed * deltaTime;
                }
            }

            player.yVel -= player.gravity * deltaTime;

            player.onGround = false;

            /*

            foreach (string key in blocks.Keys) {
                floorCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) - 1);
                floorCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize));
                floorCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) + 1);
                floorCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) - 1);
                floorCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize));
                floorCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) + 1);
                floorCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) - 1);
                floorCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize));
                floorCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) + 1);
            }
            player.prevVelocity = player.velocity;
            player.nextVelocity = player.velocity;
            player.debugCanStepX = true;
            player.debugCanStepZ = true;
            foreach (string key in blocks.Keys) {

                wallCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) - 1);
                wallCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize));
                wallCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) + 1);
                wallCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) - 1);
                wallCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize));
                wallCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) + 1);
                wallCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) - 1);
                wallCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize));
                wallCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) + 1);
            }
            foreach (string key in blocks.Keys) {
                stepCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) - 1);
                stepCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize));
                stepCheck(key, Math.Round(player.x / chunkSize) - 1, Math.Round(player.z / chunkSize) + 1);
                stepCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) - 1);
                stepCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize));
                stepCheck(key, Math.Round(player.x / chunkSize), Math.Round(player.z / chunkSize) + 1);
                stepCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) - 1);
                stepCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize));
                stepCheck(key, Math.Round(player.x / chunkSize) + 1, Math.Round(player.z / chunkSize) + 1);
            }

            player.velocity = player.nextVelocity;

            player.position += player.velocity * deltaTime;
            */

            player.cameraPosition = player.position + new Microsoft.Xna.Framework.Vector3(0, player.halfHeight / 2f, 0);

            player.xVel = MathHelper.LerpPrecise(player.xVel, 0, Math.Min(player.damping * deltaTime, 1));
            player.zVel = MathHelper.LerpPrecise(player.zVel, 0, Math.Min(player.damping * deltaTime, 1));

            player.prevVelocity = player.velocity;

            if (player.mouseLock) {
                Mouse.SetPosition(screenCenter.X, screenCenter.Y);
            }

        }

        public void generateChunk(int x, int y, int z) {
            bool alreadyExists = false;
            if (chunk.ContainsKey(x)) {
                if (chunk[x].ContainsKey(y)) {
                    if (chunk[x][y].ContainsKey(z)) {
                        alreadyExists = true;
                    }
                }
            }

            if(!alreadyExists) {
                if (!chunk.ContainsKey(x)) {
                    chunk[x] = new Dictionary<int, Dictionary<int, List<Block>>>();
                }

                if (!chunk[x].ContainsKey(y)) {
                    chunk[x][y] = new Dictionary<int, List<Block>>();
                }

                if (!chunk[x][y].ContainsKey(z)) {
                    chunk[x][y][z] = new List<Block>();
                }

                for(int cX = x * chunkSize; cX < (x * chunkSize) + chunkSize; cX++) {
                    for (int cY = y * chunkSize; cY < (y * chunkSize) + chunkSize; cY++) {
                        for (int cZ = z * chunkSize; cZ < (z * chunkSize) + chunkSize; cZ++) {

                            if (cY == (int)Math.Round(noise.GetNoise(cX * noiseScale, cZ * noiseScale)*vertScale)) {
                                this.addBlock(cX, cY, cZ, 0);
                            } else if (cY < (int)Math.Round(noise.GetNoise(cX * noiseScale, cZ * noiseScale)*vertScale)){
                                this.addBlock(cX, cY, cZ, 1);
                            }

                        }
                    }
                }

            }
            regenerateChunk(x, y, z);
        }

            // Generalize gc function to handle both (x, y, z) and single num
            public int[] gc(int ix, int iy, int iz) {

            float x = ix + 0.5f;
            float y = iy + 0.5f;
            float z = iz + 0.5f;

            int[] returnData = new int[3];
            returnData[0] = (int)Math.Floor(x / this.chunkSize);
            returnData[1] = (int)Math.Floor(y / this.chunkSize);
            returnData[2] = (int)Math.Floor(z / this.chunkSize);

            return returnData;
        }

        public void addBlock(int x, int y, int z, int tex) {
            
            int[] currentChunk = this.gc(x, y, z);

            if (!chunk.ContainsKey(currentChunk[0])) {
                chunk[currentChunk[0]] = new Dictionary<int, Dictionary<int, List<Block>>>();
            }

            if (!chunk[currentChunk[0]].ContainsKey(currentChunk[1])) {
                chunk[currentChunk[0]][currentChunk[1]] = new Dictionary<int, List<Block>>();
            }

            if (!chunk[currentChunk[0]][currentChunk[1]].ContainsKey(currentChunk[2])) {
                chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]] = new List<Block>();
            }

            chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]].Add(new Block(x, y, z, tex));

        }

        public bool checkForBlock(int x, int y, int z) {
            int[] currentChunk = this.gc(x, y, z);
            if (chunk.ContainsKey(currentChunk[0])) {
                if (chunk[currentChunk[0]].ContainsKey(currentChunk[1])) {
                    if (chunk[currentChunk[0]][currentChunk[1]].ContainsKey(currentChunk[2])) {
                        bool exists = false;
                        for(int i = 0; i < chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]].Count; i++) {
                            if (chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]][i].x == x && chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]][i].y == y && chunk[currentChunk[0]][currentChunk[1]][currentChunk[2]][i].z == z) {
                                exists = true;
                                break;
                            }
                        }
                        if (exists) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool checkForBlockChunk(int cX, int cY, int cZ, int x, int y, int z) {
            bool exists = false;
            for (int i = 0; i < chunk[cX][cY][cZ].Count; i++) {
                if (chunk[cX][cY][cZ][i].x == x && chunk[cX][cY][cZ][i].y == y && chunk[cX][cY][cZ][i].z == z) {
                    exists = true;
                    break;
                }
            }
            if (exists) {
                return true;
            }
            return false;
        }

        public void regenerateChunk(int x, int y, int z) {

            //vertices = VertexPositionTexture[];
            //indices = int[];


                        if (!primitiveCount.ContainsKey(x)) {
                            primitiveCount[x] = new Dictionary<int, Dictionary<int, int>>();
                        }
                        if (!primitiveCount[x].ContainsKey(y)) {
                            primitiveCount[x][y] = new Dictionary<int, int>();
                        }

                        primitiveCount[x][y][z] = 0;

                        List<Vector3> positions = new List<Vector3>();
                        List<Vector2> UVs = new List<Vector2>();
                        List<int> listIndices = new List<int>();

                        for (int b = 0; b < chunk[x][y][z].Count; b++) {

                            int cX = chunk[x][y][z][b].x;
                            int cY = chunk[x][y][z][b].y;
                            int cZ = chunk[x][y][z][b].z;
                            float cLX = chunk[x][y][z][b].lx;
                            float cHX = chunk[x][y][z][b].hx;
                            float cLY = chunk[x][y][z][b].ly;
                            float cHY = chunk[x][y][z][b].hy;

                            int totalIndices = 0;

                            // Back (z-)
                            if (!checkForBlockChunk(x, y, z, cX, cY, cZ - 1)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 2 + totalIndices, 1 + totalIndices, 0 + totalIndices, 3 + totalIndices, 2 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Front (z+)
                            if (!checkForBlockChunk(x, y, z, cX, cY, cZ + 1)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 1 + totalIndices, 2 + totalIndices, 0 + totalIndices, 2 + totalIndices, 3 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Left (x-)
                            if (!checkForBlockChunk(x, y, z, cX - 1, cY, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    2 + totalIndices, 3 + totalIndices, 0 + totalIndices, 2 + totalIndices, 0 + totalIndices, 1 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Right (x+)
                            if (!checkForBlockChunk(x, y, z, cX + 1, cY, cZ)) {
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 1 + totalIndices, 2 + totalIndices, 0 + totalIndices, 2 + totalIndices, 3 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Bottom (y-)
                            if (!checkForBlockChunk(x, y, z, cX, cY - 1, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    1 + totalIndices, 2 + totalIndices, 3 + totalIndices, 1 + totalIndices, 3 + totalIndices, 0 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Top (y+)
                            if (!checkForBlockChunk(x, y, z, cX, cY + 1, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));
                                UVs.Add(new Vector2(cHX, cHY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 2 + totalIndices, 3 + totalIndices, 0 + totalIndices, 1 + totalIndices, 2 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                        }

                        if (!vertices.ContainsKey(x)) {
                            vertices[x] = new Dictionary<int, Dictionary<int, VertexPositionTexture[]>>();
                        }
                        if (!vertices[x].ContainsKey(y)) {
                            vertices[x][y] = new Dictionary<int, VertexPositionTexture[]>();
                        }

                        if (!indices.ContainsKey(x)) {
                            indices[x] = new Dictionary<int, Dictionary<int, int[]>>();
                        }
                        if (!indices[x].ContainsKey(y)) {
                            indices[x][y] = new Dictionary<int, int[]>();
                        }

                        vertices[x][y][z] = new VertexPositionTexture[positions.Count];
                        indices[x][y][z] = new int[listIndices.Count];

                        for (int i = 0; i < positions.Count; i++) {
                            vertices[x][y][z][i] = new VertexPositionTexture(new Vector3(positions[i].X, positions[i].Y, positions[i].Z), new Vector2(UVs[i].X, UVs[i].Y));
                        }

                        for (int i = 0; i < listIndices.Count; i++) {
                            indices[x][y][z][i] = listIndices[i];
                        }

        }

        public void regenerate() {

            //vertices = VertexPositionTexture[];
            //indices = int[];

            foreach (int x in chunk.Keys) {
                foreach (int y in chunk[x].Keys) {
                    foreach (int z in chunk[x][y].Keys) {

                        if (!primitiveCount.ContainsKey(x)) {
                            primitiveCount[x] = new Dictionary<int, Dictionary<int, int>>();
                        }
                        if (!primitiveCount[x].ContainsKey(y)) {
                            primitiveCount[x][y] = new Dictionary<int, int>();
                        }

                        primitiveCount[x][y][z] = 0;

                        List<Vector3> positions = new List<Vector3>();
                        List<Vector2> UVs = new List<Vector2>();
                        List<int> listIndices = new List<int>();

                        for (int b = 0; b < chunk[x][y][z].Count; b++) {

                            int cX = chunk[x][y][z][b].x;
                            int cY = chunk[x][y][z][b].y;
                            int cZ = chunk[x][y][z][b].z;
                            float cLX = chunk[x][y][z][b].lx;
                            float cHX = chunk[x][y][z][b].hx;
                            float cLY = chunk[x][y][z][b].ly;
                            float cHY = chunk[x][y][z][b].hy;

                            int totalIndices = 0;

                            // Back (z-)
                            if (!checkForBlockChunk(x, y, z, cX, cY, cZ - 1)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 2 + totalIndices, 1 + totalIndices, 0 + totalIndices, 3 + totalIndices, 2 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Front (z+)
                            if (!checkForBlockChunk(x, y, z, cX, cY, cZ + 1)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 1 + totalIndices, 2 + totalIndices, 0 + totalIndices, 2 + totalIndices, 3 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Left (x-)
                            if (!checkForBlockChunk(x, y, z, cX - 1, cY, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    2 + totalIndices, 3 + totalIndices, 0 + totalIndices, 2 + totalIndices, 0 + totalIndices, 1 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Right (x+)
                            if (!checkForBlockChunk(x, y, z, cX + 1, cY, cZ)) {
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 1 + totalIndices, 2 + totalIndices, 0 + totalIndices, 2 + totalIndices, 3 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Bottom (y-)
                            if (!checkForBlockChunk(x, y, z, cX, cY - 1, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY - 0.5f, cZ - 0.5f));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cHX, cHY));
                                UVs.Add(new Vector2(cHX, cLY));

                                int[] addedIndices = new int[]{
                                    1 + totalIndices, 2 + totalIndices, 3 + totalIndices, 1 + totalIndices, 3 + totalIndices, 0 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                            // Top (y+)
                            if (!checkForBlockChunk(x, y, z, cX, cY + 1, cZ)) {
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ + 0.5f));
                                positions.Add(new Vector3(cX - 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ - 0.5f));
                                positions.Add(new Vector3(cX + 0.5f, cY + 0.5f, cZ + 0.5f));
                                UVs.Add(new Vector2(cLX, cHY));
                                UVs.Add(new Vector2(cLX, cLY));
                                UVs.Add(new Vector2(cHX, cLY));
                                UVs.Add(new Vector2(cHX, cHY));

                                int[] addedIndices = new int[]{
                                    0 + totalIndices, 2 + totalIndices, 3 + totalIndices, 0 + totalIndices, 1 + totalIndices, 2 + totalIndices
                                };

                                listIndices.AddRange(addedIndices);

                                totalIndices += 4;
                                primitiveCount[x][y][z] += 2;
                            }

                        }

                        if (!vertices.ContainsKey(x)) {
                            vertices[x] = new Dictionary<int, Dictionary<int, VertexPositionTexture[]>>();
                        }
                        if (!vertices[x].ContainsKey(y)) {
                            vertices[x][y] = new Dictionary<int, VertexPositionTexture[]>();
                        }

                        if (!indices.ContainsKey(x)) {
                            indices[x] = new Dictionary<int, Dictionary<int, int[]>>();
                        }
                        if (!indices[x].ContainsKey(y)) {
                            indices[x][y] = new Dictionary<int, int[]>();
                        }

                        vertices[x][y][z] = new VertexPositionTexture[positions.Count];
                        indices[x][y][z] = new int[listIndices.Count];

                        for (int i = 0; i < positions.Count; i++) {
                            vertices[x][y][z][i] = new VertexPositionTexture(new Vector3(positions[i].X, positions[i].Y, positions[i].Z), new Vector2(UVs[i].X, UVs[i].Y));
                        }

                        for (int i = 0; i < listIndices.Count; i++) {
                            indices[x][y][z][i] = listIndices[i];
                        }

                    }
                }
            }

        }

        public void render() {

            ResourceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ResourceManager.GraphicsDevice.BlendState = BlendState.Opaque;
            ResourceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Microsoft.Xna.Framework.Point screenCenter = new Microsoft.Xna.Framework.Point(ResourceManager.GraphicsDevice.Viewport.Width / 2, ResourceManager.GraphicsDevice.Viewport.Height / 2);

            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(player.r, player.t, 0);
            Vector3 lookDirection = Vector3.Transform(Vector3.Forward, rotationMatrix);
            Vector3 upDirection = Vector3.Transform(Vector3.Up, rotationMatrix);
            Matrix viewMatrix = Matrix.CreateLookAt(player.cameraPosition, player.cameraPosition + lookDirection, upDirection);

            List<int[]> tmp = new List<int[]>();

            foreach (int x in chunk.Keys) {
                foreach (int y in chunk[x].Keys) {
                    foreach (int z in chunk[x][y].Keys) {
                        if (primitiveCount[x][y][z] >= 1) {
                            tmp.Add(new int[] { x, y, z });
                        }
                    }
                }
            }

            effect.View = viewMatrix;
            ResourceManager.GraphicsDevice.SamplerStates[0] = new SamplerState { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap };
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                Console.WriteLine(tmp.Count);
                for(int i = 0; i < tmp.Count; i++) {
                    ResourceManager.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices[tmp[i][0]][tmp[i][1]][tmp[i][2]], 0, vertices[tmp[i][0]][tmp[i][1]][tmp[i][2]].Length, indices[tmp[i][0]][tmp[i][1]][tmp[i][2]], 0, (indices[tmp[i][0]][tmp[i][1]][tmp[i][2]].Length)/3);
                }
            }

            ResourceManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ResourceManager.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            ResourceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

    }

    internal class Block {

        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public int tex { get; set; }
        public float lx { get; set; }
        public float hx { get; set; }
        public float ly { get; set; }
        public float hy { get; set; }

        public Block(int x, int y, int z, int tex) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.tex = tex;

            switch (tex) {
                case 0:
                    this.lx = 0.0f;
                    this.ly = 0.9f;
                    break;
                case 1:
                    this.lx = 0.1f;
                    this.ly = 0.9f;
                    break;
                case 2:
                    this.lx = 0.2f;
                    this.ly = 0.9f;
                    break;
            }

            this.hx = this.lx + 0.1f - 0.033333333f;
            this.hy = this.ly + 0.1f - 0.033333333f;

            this.lx += 0.033333333f;
            this.ly += 0.033333333f;

            this.lx = 0;
            this.ly = 0;
            this.hx = 1;
            this.hy = 1;

        }

        public float xMin {
            get { return this.x - 0.5f; }
        }
        public float xMax {
            get { return this.x + 0.5f; }
        }
        public float yMin {
            get { return this.y - 0.5f; }
        }
        public float yMax {
            get { return this.y + 0.5f; }
        }
        public float zMin {
            get { return this.z - 0.5f; }
        }
        public float zMax {
            get { return this.z + 0.5f; }
        }

        public void collideFloor(Player player) {
            bool insideNext = false;
            if (player.xMaxNext > this.xMin && player.xMinNext < this.xMax && player.zMaxNext > this.zMin && player.zMinNext < this.zMax) {
                insideNext = true;
            }
            if (insideNext && player.yMin + 0.0001f >= this.yMax && player.yMinNext < this.yMax) {
                player.y = this.yMax + player.halfHeight;
                player.yVel = 0;
                player.onGround = true;
            }
            if (insideNext && player.yMax < this.yMin && player.yMaxNext > this.yMin) {
                player.y = this.yMin - player.halfHeight;
                player.yVel = 0;
            }
        }
        public void step(Player player) {
            bool insideYNext = false;
            bool insideX = false;
            bool insideXNext = false;
            bool insideZ = false;
            bool insideZNext = false;

            if (player.yMaxNextPrev > this.yMin && player.yMinNextPrev < this.yMax) {
                insideYNext = true;
            }
            if (player.zMax > this.zMin && player.zMin < this.zMax) {
                insideZ = true;
            }
            if (player.zMaxNextPrev > this.zMin && player.zMinNextPrev < this.zMax) {
                insideZNext = true;
            }
            if (player.xMax > this.xMin && player.xMin < this.xMax) {
                insideX = true;
            }
            if (player.xMaxNextPrev > this.xMin && player.xMinNextPrev < this.xMax) {
                insideXNext = true;
            }

            if (insideXNext && insideYNext && insideZNext && player.yMin > this.yMax - player.stepHeight && player.onGround) {
                if (insideX && player.debugCanStepZ) {
                    player.nextVelocity = player.prevVelocity;
                    player.y = this.yMax + player.halfHeight;
                    player.nextyVel = 0;
                    player.onGround = true;
                }
                if (insideZ && player.debugCanStepX) {
                    player.nextVelocity = player.prevVelocity;
                    player.y = this.yMin + player.halfHeight;
                    player.nextyVel = 0;
                    player.onGround = true;
                }
            }

        }
        public void collide(Player player) {
            bool insideYNext = false;
            bool insideX = false;
            bool insideXNext = false;
            bool insideZ = false;
            bool insideZNext = false;

            if (player.yMaxNextPrev > this.yMin && player.yMinNextPrev < this.yMax) {
                insideYNext = true;
            }
            if (player.zMax > this.zMin && player.zMin < this.zMax) {
                insideZ = true;
            }
            if (player.zMaxNextPrev > this.zMin && player.zMinNextPrev < this.zMax) {
                insideZNext = true;
            }
            if (player.xMax > this.xMin && player.xMin < this.xMax) {
                insideX = true;
            }
            if (player.xMaxNextPrev > this.xMin && player.xMinNextPrev < this.xMax) {
                insideXNext = true;
            }
            if (insideYNext) {
                if (insideZNext && insideZ && player.xMin >= this.xMax && player.xMinNext < this.xMax) {
                    player.x = this.xMax + player.halfWidth;
                    player.nextxVel = 0;
                    if (player.yMin <= this.yMax - player.stepHeight) {
                        player.debugCanStepX = false;
                    }
                }
                if (insideZNext && insideZ && player.xMax <= this.xMin && player.xMaxNext > this.xMin) {
                    player.x = this.xMin - player.halfWidth;
                    player.nextxVel = 0;
                    if (player.yMin <= this.yMax - player.stepHeight) {
                        player.debugCanStepX = false;
                    }
                }
                if (insideXNext && insideX && player.zMin >= this.zMax && player.zMinNext < this.zMax) {
                    player.z = this.zMax + player.halfWidth;
                    player.nextzVel = 0;
                    if (player.yMin <= this.yMax - player.stepHeight) {
                        player.debugCanStepZ = false;
                    }
                }
                if (insideXNext && insideX && player.zMax <= this.zMin && player.zMaxNext > this.zMin) {
                    player.z = this.zMin - player.halfWidth;
                    player.nextzVel = 0;
                    if (player.yMin <= this.yMax - player.stepHeight) {
                        player.debugCanStepZ = false;
                    }
                }
            }
            // corner bug fix oh god my brain
            if (insideXNext && insideYNext && insideZNext) {
                if (!insideX && !insideZ) {
                    if (Math.Abs(player.prevxVel) < Math.Abs(player.prevzVel)) {
                        if (player.x > this.x) {
                            player.x = this.xMax + player.halfWidth;
                            player.nextxVel = 0;
                            if (player.yMin <= this.yMax - player.stepHeight) {
                                player.debugCanStepX = false;
                            }
                        } else {
                            player.x = this.xMin - player.halfWidth;
                            player.nextxVel = 0;
                            if (player.yMin <= this.yMax - player.stepHeight) {
                                player.debugCanStepX = false;
                            }
                        }
                    } else {
                        if (player.z > this.z) {
                            player.z = this.zMax + player.halfWidth;
                            player.nextzVel = 0;
                            if (player.yMin <= this.yMax - player.stepHeight) {
                                player.debugCanStepZ = false;
                            }
                        } else {
                            player.z = this.zMin - player.halfWidth;
                            player.nextzVel = 0;
                            if (player.yMin <= this.yMax - player.stepHeight) {
                                player.debugCanStepZ = false;
                            }
                        }
                    }
                }
            }

        }

    }

    internal class Player {

        private Microsoft.Xna.Framework.Vector3 pos;
        private Microsoft.Xna.Framework.Vector3 vel;
        private Microsoft.Xna.Framework.Vector2 rot;
        private Microsoft.Xna.Framework.Vector3 prevVel;
        private Microsoft.Xna.Framework.Vector3 nextVel;
        public float deltaTime { get; private set; }

        private float fov;

        public float mouseSensitivity { get; set; }
        public float movementSpeed { get; set; }
        public bool onGround { get; set; }
        public bool mouseLock { get; set; }
        public bool canMove { get; set; }
        public float gravity { get; set; }
        public float width { get; set; }
        public float height { get; set; }
        public float halfWidth { get; set; }
        public float halfHeight { get; set; }
        public float stepHeight { get; set; }
        public bool debugCanStepX { get; set; }
        public bool debugCanStepZ { get; set; }
        public float jumpHeight { get; set; }
        public float damping { get; set; }
        public Microsoft.Xna.Framework.Vector3 cameraPosition { get; set; }

        public Player(Microsoft.Xna.Framework.Vector3 position, float moveSpeed) {
            this.pos = position;
            this.movementSpeed = moveSpeed;
            this.mouseSensitivity = 0.002f;
            this.mouseLock = true;
            this.canMove = true;
            this.rot = new Microsoft.Xna.Framework.Vector2(0, 0);
            this.fov = 90f;
            this.gravity = 15f;
            this.width = 0.6f;
            this.height = 1.8f;
            this.halfWidth = this.width / 2;
            this.halfHeight = this.height / 2;
            this.stepHeight = 0.6f;
            this.jumpHeight = 6f;
            this.damping = 10f;
            this.onGround = false;
            this.cameraPosition = position + new Microsoft.Xna.Framework.Vector3(0, this.halfHeight / 2f, 0);
            this.debugCanStepX = false;
            this.debugCanStepZ = false;
        }

        public void bruh(float buh) {
            deltaTime = buh;
        }

        public Microsoft.Xna.Framework.Vector3 position {
            get { return pos; }
            set { pos = value; }
        }
        public float x {
            get { return pos.X; }
            set { pos.X = value; }
        }
        public float y {
            get { return pos.Y; }
            set { pos.Y = value; }
        }
        public float z {
            get { return pos.Z; }
            set { pos.Z = value; }
        }

        public Microsoft.Xna.Framework.Vector3 velocity {
            get { return vel; }
            set { vel = value; }
        }
        public float xVel {
            get { return vel.X; }
            set { vel.X = value; }
        }
        public float yVel {
            get { return vel.Y; }
            set { vel.Y = value; }
        }
        public float zVel {
            get { return vel.Z; }
            set { vel.Z = value; }
        }

        public Microsoft.Xna.Framework.Vector3 prevVelocity {
            get { return prevVel; }
            set { prevVel = value; }
        }
        public float prevxVel {
            get { return prevVel.X; }
            set { prevVel.X = value; }
        }
        public float prevyVel {
            get { return prevVel.Y; }
            set { prevVel.Y = value; }
        }
        public float prevzVel {
            get { return prevVel.Z; }
            set { prevVel.Z = value; }
        }

        public Microsoft.Xna.Framework.Vector3 nextVelocity {
            get { return nextVel; }
            set { nextVel = value; }
        }
        public float nextxVel {
            get { return nextVel.X; }
            set { nextVel.X = value; }
        }
        public float nextyVel {
            get { return nextVel.Y; }
            set { nextVel.Y = value; }
        }
        public float nextzVel {
            get { return nextVel.Z; }
            set { nextVel.Z = value; }
        }

        public float xMax {
            get { return pos.X + halfWidth; }
        }
        public float xMin {
            get { return pos.X - halfWidth; }
        }
        public float yMax {
            get { return pos.Y + halfHeight; }
        }
        public float yMin {
            get { return pos.Y - halfHeight; }
        }
        public float zMax {
            get { return pos.Z + halfWidth; }
        }
        public float zMin {
            get { return pos.Z - halfWidth; }
        }

        public float xMaxNext {
            get { return pos.X + halfWidth + (vel.X * deltaTime); }
        }
        public float xMinNext {
            get { return pos.X - halfWidth + (vel.X * deltaTime); }
        }
        public float yMaxNext {
            get { return pos.Y + halfHeight + (vel.Y * deltaTime); }
        }
        public float yMinNext {
            get { return pos.Y - halfHeight + (vel.Y * deltaTime); }
        }
        public float zMaxNext {
            get { return pos.Z + halfWidth + (vel.Z * deltaTime); }
        }
        public float zMinNext {
            get { return pos.Z - halfWidth + (vel.Z * deltaTime); }
        }

        public float xMaxNextPrev {
            get { return pos.X + halfWidth + (prevVel.X * deltaTime); }
        }
        public float xMinNextPrev {
            get { return pos.X - halfWidth + (prevVel.X * deltaTime); }
        }
        public float yMaxNextPrev {
            get { return pos.Y + halfHeight + (prevVel.Y * deltaTime); }
        }
        public float yMinNextPrev {
            get { return pos.Y - halfHeight + (prevVel.Y * deltaTime); }
        }
        public float zMaxNextPrev {
            get { return pos.Z + halfWidth + (prevVel.Z * deltaTime); }
        }
        public float zMinNextPrev {
            get { return pos.Z - halfWidth + (prevVel.Z * deltaTime); }
        }

        public Microsoft.Xna.Framework.Vector2 rotation {
            get { return rot; }
            set { rot = value; }
        }
        public float r {
            get { return rot.Y; }
            set { rot.Y = value; }
        }
        public float t {
            get { return rot.X; }
            set { rot.X = value; }
        }

        public float fieldOfView {
            get { return fov * (MathHelper.Pi / 180f); }
            set { fov = value; }
        }

    }
}
