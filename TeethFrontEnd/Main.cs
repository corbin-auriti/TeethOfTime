using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Timers;

using Substrate;

namespace TeethFrontEnd
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Run();
        }

        public bool aDisplayReady = false;
        public NbtWorld aWorld;
        public RegionChunkManager aChunkMan;
        public int[] aMinXZ = { 0, 0 };
        public Graphics aMapSurface;
        public System.Drawing.Drawing2D.GraphicsState aMapSave;
        public SolidBrush aChunkBrush = new SolidBrush(Color.Gold);
        public Pen aPen = new Pen(Color.Gold, 2);
        public List<ChunkRef> aSelectedChunks = new List<ChunkRef>();
        public string selectedWorldPath = "";

        public void Run()
        {
            // Load configs into the world selection box

            List<string> names = new List<string>();
            List<string> paths = new List<string>();

            string path = Environment.CurrentDirectory + @"\configs\";

            foreach (string conf in System.IO.Directory.GetFiles(path, "*.cfg"))
            {
                string name = conf.Substring(path.Length);
                if (name != "default.cfg")
                {
                    names.Add(name);
                }
            }

            configBox.DisplayMember = "Key";
            configBox.Items.AddRange(names.ToArray());
            configBox.SelectedIndex = 0;

            // Create the display update timer

            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnDisplayUpdate);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;
        }

        private void OnDisplayUpdate(object source, ElapsedEventArgs e)
        {
            if (aDisplayReady)
            {
                resetMapSurface();

                float zoom = float.Parse(zoomLabel.Text);

                foreach (ChunkRef chunk in aSelectedChunks)
                {
                    Rectangle chunkRect = new Rectangle((int)((-aMinXZ[0] + chunk.X * 16) * zoom) - 1, (int)((-aMinXZ[1] + chunk.Z * 16) * zoom) - 1, (int)(16 * zoom), (int)(16 * zoom));
                    aMapSurface.DrawRectangle(aPen, chunkRect);
                }
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            if (selectedWorldPath != "")
            {
                string ToTPath = System.IO.Directory.GetCurrentDirectory() + @"\TeethOfTime.exe";

                char quote = '"';
                string worldPath = selectedWorldPath;
                worldPath = quote + worldPath + quote;

                int[,] selectedChunks = chunkArrayToIntArray(aSelectedChunks);
                string selectedChunksArg = intArrayToTrueString(selectedChunks);

                ProcessStartInfo TeethInfo = new ProcessStartInfo(ToTPath);
                if (selectedChunks.Length > 0)
                {
                    TeethInfo.Arguments = worldPath + " " + configBox.SelectedItem + " " + selectedChunksArg;
                }
                else
                {
                    TeethInfo.Arguments = worldPath + " " + configBox.SelectedItem;
                }

                Process.Start(TeethInfo);
            }
        }

        private void zoomInButton_Click(object sender, EventArgs e)
        {
            float zoom = float.Parse(zoomLabel.Text);
            zoom *= 2;
            if (zoom > 32)
                zoom = 32;
            else
                zoomChange(0.5f);

            zoomLabel.Text = zoom.ToString();
        }

        private void zoomOutButton_Click(object sender, EventArgs e)
        {
            float zoom = float.Parse(zoomLabel.Text);
            zoom /= 2;
            if (zoom < 1)
                zoom = 1;
            else
                zoomChange(2);

            zoomLabel.Text = zoom.ToString();
        }

        public Bitmap GenerateMap(NbtWorld world, RegionChunkManager chunkMan)
        {
            int[,] colors = { { 0, 255, 0, 255 }, { 1, 128, 128, 128 }, { 2, 64, 192, 64 }, { 3, 128, 48, 0 }, { 4, 64, 64, 64 }, { 5, 255, 128, 64 }, { 6, 0, 192, 0 }, { 7, 32, 32, 32 }, { 8, 16, 16, 128 }, { 9, 16, 16, 128 }, { 10, 255, 0, 0 }, { 11, 255, 0, 0 }, { 12, 255, 255, 192 }, { 13, 156, 128, 128 }, { 14, 192, 192, 128 }, { 15, 192, 128, 96 }, { 16, 48, 48, 32 }, { 17, 192, 96, 48 }, { 18, 0, 128, 0 }, { 19, 255, 255, 0 }, { 20, 224, 224, 224 }, { 21, 128, 128, 192 }, { 22, 64, 64, 255 }, { 23, 64, 64, 64 }, { 24, 128, 128, 96 }, { 25, 192, 96, 48 }, { 26, 255, 128, 128 }, { 27, 224, 128, 128 }, { 28, 224, 128, 128 }, { 29, 128, 192, 48 }, { 30, 224, 224, 224 }, { 31, 96, 32, 0 }, { 32, 128, 64, 32 }, { 33, 255, 192, 96 }, { 34, 255, 192, 96 }, { 35, 224, 224, 224 }, { 36, 0, 0, 0 }, { 37, 255, 255, 0 }, { 38, 255, 48, 48 }, { 39, 192, 128, 96 }, { 40, 255, 192, 96 }, { 41, 255, 255, 64 }, { 42, 192, 192, 192 }, { 43, 140, 140, 140 }, { 44, 150, 150, 150 }, { 45, 192, 64, 64 }, { 46, 128, 0, 0 }, { 47, 255, 128, 64 }, { 48, 64, 128, 64 }, { 49, 16, 16, 24 }, { 50, 255, 255, 0 }, { 51, 255, 224, 0 }, { 52, 96, 128, 192 }, { 53, 255, 192, 96 }, { 54, 224, 128, 64 }, { 55, 128, 0, 0 }, { 56, 128, 138, 92 }, { 57, 128, 192, 255 }, { 58, 192, 96, 48 }, { 59, 192, 255, 0 }, { 60, 192, 96, 0 }, { 61, 96, 96, 96 }, { 62, 96, 96, 96 }, { 63, 255, 128, 64 }, { 64, 255, 128, 64 }, { 65, 255, 128, 64 }, { 66, 224, 224, 192 }, { 67, 78, 78, 78 }, { 68, 255, 128, 64 }, { 69, 128, 96, 64 }, { 70, 133, 133, 133 }, { 71, 192, 192, 192 }, { 72, 255, 192, 128 }, { 73, 224, 128, 128 }, { 74, 224, 128, 128 }, { 75, 255, 64, 64 }, { 76, 255, 64, 64 }, { 77, 255, 64, 64 }, { 78, 255, 255, 255 }, { 79, 0, 192, 224 }, { 80, 255, 255, 255 }, { 81, 0, 192, 0 }, { 82, 170, 160, 170 }, { 83, 96, 255, 32 }, { 84, 192, 96, 48 }, { 85, 255, 128, 64 }, { 86, 255, 128, 0 }, { 87, 128, 24, 48 }, { 88, 128, 96, 64 }, { 89, 255, 224, 96 }, { 90, 128, 0, 255 }, { 91, 255, 128, 0 }, { 92, 255, 192, 192 }, { 93, 255, 96, 96 }, { 94, 255, 96, 96 }, { 95, 0, 0, 0 }, { 96, 255, 128, 64 } };

            int[] minXZ = { 0, 0 };
            int[] maxXZ = { 0, 0 };

            foreach (ChunkRef chunk in chunkMan)
            {
                minXZ[0] = Math.Min(chunk.X * 16, minXZ[0]);
                minXZ[1] = Math.Min(chunk.Z * 16, minXZ[1]);

                maxXZ[0] = Math.Max(16 + chunk.X * 16, maxXZ[0]);
                maxXZ[1] = Math.Max(16 + chunk.Z * 16, maxXZ[1]);
            }

            aMinXZ = minXZ;

            int[] worldSize = { maxXZ[0] - minXZ[0], maxXZ[1] - minXZ[1] };

            Bitmap map = new Bitmap(worldSize[0], worldSize[1]);

            int y;
            int block;
            int[] pixelpos = { 0, 0 };
            Color color;

            long chunkCount = chunkMan.Count();
            long chunkCurrent = 0;

            foreach (ChunkRef chunk in chunkMan)
            {
                for (int x = 0; x < 16; x++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        y = chunk.Blocks.GetHeight(x, z);

                        if (y > 0 && y < 256)
                        {
                            block = chunk.Blocks.GetID(x, y - 1, z);

                            int blockColor = block;

                            if (blockColor < colors.GetUpperBound(0) + 1)
                            {
                                color = color = Color.FromArgb(colors[blockColor, 1], colors[blockColor, 2], colors[blockColor, 3]);
                            }
                            else
                            {
                                color = color = Color.FromArgb(255, 0, 255);
                            }

                            if (x == 0 || z == 0)
                            {
                                color = Color.FromArgb(color.R / 2, color.G / 2, color.B / 2);
                            }
                        }
                        else
                        {
                            color = Color.Black;
                        }

                        pixelpos[0] = Math.Abs(minXZ[0]) + (chunk.X * 16) + x;
                        pixelpos[1] = Math.Abs(minXZ[1]) + (chunk.Z * 16) + z;

                        map.SetPixel(pixelpos[0], pixelpos[1], color);
                    }
                }
                chunkCurrent++;
                pbProgress.Value = (int)(100.0 * ((double)chunkCurrent / (double)chunkCount));
                pbProgress.PerformStep();
            }
            pbProgress.Value = 0;
            pbProgress.PerformStep();

            return map;
        }

        public void zoomChange(float zoom)
        {
            mapBox.Width = (int)(mapBox.Width / zoom);
            mapBox.Height = (int)(mapBox.Height / zoom);

            handleGraphics();
        }

        public void handleGraphics()
        {
            aMapSurface = mapBox.CreateGraphics();
        }

        private void mapBox_Click(object sender, MouseEventArgs e)
        {
            if (aWorld != null)
            {
                ChunkRef chunk = grabChunk(e.X, e.Y);

                if (chunk != null)
                {
                    if (aSelectedChunks.Contains(chunk))
                    {
                        aSelectedChunks.Remove(chunk);
                    }
                    else
                    {
                        aSelectedChunks.Add(chunk);
                    }
                }
            }
        }

        delegate void resetMapSurfaceDel();

        private void resetMapSurface()
        {
            if (!InvokeRequired)
            {
                mapBox.Enabled = false;
                mapBox.Enabled = true;
            }
            else
            {
                Invoke(new resetMapSurfaceDel(resetMapSurface));
            }
        }

        public ChunkRef grabChunk(int X, int Z)
        {
            int[] position = { 0, 0 };

            position = getChunkPos(X, Z);

            ChunkRef chunk = null;

            if (aChunkMan.ChunkExists(position[0],position[1]))
            {
                chunk = aChunkMan.GetChunkRef(position[0], position[1]);
            }

            return chunk;
        }

        public int[] getChunkPos(int X, int Z)
        {
            int[] position = { 0, 0 };

            float zoom = float.Parse(zoomLabel.Text);

            int[] minXZ = { 0, 0 };
            minXZ[0] = (int)(aMinXZ[0] * zoom);
            minXZ[1] = (int)(aMinXZ[1] * zoom);

            position[0] = (minXZ[0] + X) / (int)(16 * zoom);
            position[1] = (minXZ[1] + Z) / (int)(16 * zoom);

            if ((minXZ[0] + X) < 0) //Rounding error fix
                position[0]--;

            if ((minXZ[1] + Z) < 0)
                position[1]--;

            return position;
        }

        public int[,] chunkArrayToIntArray(List<ChunkRef> chunkList)
        {
            ChunkRef[] chunkArray = chunkList.ToArray();

            int[,] result = new int[chunkArray.GetUpperBound(0)+1,2];
            int i = 0;

            foreach (ChunkRef chunk in chunkArray)
            {
                result[i, 0] = chunk.X;
                result[i, 1] = chunk.Z;
                i++;
            }

            return result;
        }

        public string intArrayToTrueString (int[,] input)
        {
            string result = "{";

            for (int i = 0; i < input.GetUpperBound(0)+1; i++)
            {
                result += "{" + input[i, 0] + "," + input[i, 1] + "}";
                if (i < input.GetUpperBound(0))
                {
                    result += ",";
                }
            }

            result += "};";

            return result;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if(folderBrowser.ShowDialog() == DialogResult.OK)
            {
                selectedWorldPath = folderBrowser.SelectedPath;
                aDisplayReady = false;

                string worldPath = selectedWorldPath;

                aSelectedChunks.Clear();
                zoomLabel.Text = "1";

                NbtWorld world = NbtWorld.Open(worldPath);
                string worldName = world.Level.LevelName;
                RegionChunkManager chunkMan = (RegionChunkManager)world.GetChunkManager(0);

                aWorld = world;
                aChunkMan = chunkMan;

                Bitmap map = GenerateMap(world, chunkMan);

                mapBox.Image = map;
                mapBox.Width = map.Width;
                mapBox.Height = map.Height;

                handleGraphics();

                /*string MapDirPath = System.IO.Directory.GetCurrentDirectory() + @"\MapData\";
                if (!Directory.Exists(MapDirPath))
                {
                    Directory.CreateDirectory(MapDirPath);
                }

                map.Save(MapDirPath + worldName + ".png");*/

                aDisplayReady = true;
            }
        }
    }
}