using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using AForge.Math;

namespace TeethOfTime
{
    using Substrate;

    public class Destroy
    {
        private static Random rand = new Random();

        // Initialize defaults in case the config file is missing them
        public int[] fallBlock = { 4, 5, 17, 20, 22, 23, 24, 25, 35, 40, 41, 42, 43, 44, 45, 46, 47, 48, 53, 54, 55, 57, 58, 61, 62, 67, 84, 85, 86, 87, 88, 89, 91 };
        public int[] smashBlock = { 50, 78, 44, 31, 6, 65, 66, 55, 59, 85, 93, 94, 75, 76, 63, 68, 69, 70, 72, 77, 81, 30, 83, 26, 51, 92, 37, 38, 39, 40 };
        public int[] fallThroughBlock = { 8, 9, 10, 11 };
        public int[,] mutateBlock = { { 50, 0, 70, 90 }, { 4, 48, 15, 30 }, { 60, 2, 20, 40 } };
        public int passN = 4;
        public int sidewaysPassN = 2;
        public int[] fallChance = { 10, 30 };
        public int[] fallKillChance = { 3, 15 };
        public int[] sidewaysFallChance = { 15, 40 };
        public int stonePassN = 2;
        public int stoneSidewaysPassN = 1;
        public int[] stoneFallChance = { 20, 50 };
        public int[] stoneFallKillChance = { 7, 20 };
        public int[] stoneSidewaysFallChance = { 60, 100 };
        public bool stoneAdvanced = true;
        public int fallSpread = 2;
        public int[] natureKillChance = { 5, 15 };
        public int[] natureKillBlock = { 3, 5, 17, 25, 26, 35, 47, 53, 58, 63, 72, 84, 85, 92, 96 };
        public int natureKillTo = 2;
        public bool natureRepopulation = true;
        public int[] natureLivingReplace = { 6, 18, 19, 31, 32, 37, 38, 39, 40, 59, 81, 83, 86, 91 };
        public int[] natureOresReplace = { 14, 15, 16, 21, 56, 73, 74 };
        public bool globalRelight = true;

        public int[] supportedBlocks = { 6, 26, 27, 28, 37, 38, 39, 40, 51, 55, 59, 63, 66, 70, 72, 78, 83, 92, 93, 94 };

        public void Run(string worldPath, string config, string selectedChunksArg)
        {
            // Config loading sequence
            Console.WriteLine("Loading config file " + config);
            Console.WriteLine();

            // Open the config file
            if (System.IO.File.Exists("configs/" + config))
            {
                StreamReader cfgReader = new StreamReader("configs/" + config);

                string cfgLine = "";
                ArrayList cfgArray = new ArrayList();

                // Read the whole file, then close it
                while (cfgLine != null)
                {
                    cfgLine = cfgReader.ReadLine();
                    if (cfgLine != null)
                        cfgArray.Add(cfgLine);
                }
                cfgReader.Close();

                // Parse the config file
                foreach (string cfgOutput in cfgArray)
                {
                    // Ignore comment lines
                    int cfgComm = cfgOutput.IndexOf("#");
                    if (cfgComm == 0 || cfgOutput.Length == 0)
                        continue;

                    // Get rid of all spaces
                    string cfgParse = cfgOutput.Replace(" ", "");

                    if (cfgParse.IndexOf("fallBlock") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 9 + 1);
                        fallBlock = StringToArray(cfgParse);
                        cfgParse = "New " + "fallBlock" + " = {";
                        for (int m = 0; m < fallBlock.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("smashBlock") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 10 + 1);
                        smashBlock = StringToArray(cfgParse);
                        cfgParse = "New " + "smashBlock" + " = {";
                        for (int m = 0; m < smashBlock.Length; m++)
                        {
                            cfgParse = cfgParse + smashBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("fallThroughBlock") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 16 + 1);
                        fallThroughBlock = StringToArray(cfgParse);
                        cfgParse = "New " + "fallThroughBlock" + " = {";
                        for (int m = 0; m < fallThroughBlock.Length; m++)
                        {
                            cfgParse = cfgParse + fallThroughBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("mutateBlock") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 11 + 1);
                        mutateBlock = StringToMultiArray(cfgParse, 4);
                        cfgParse = "New " + "mutateBlock" + " = {";
                        for (int m = 0; m < mutateBlock.GetUpperBound(0); m++)
                        {
                            cfgParse = cfgParse + "{ " + mutateBlock[m, 0] + ", " + mutateBlock[m, 1] + ", " + mutateBlock[m, 2] + ", " + mutateBlock[m, 3] + "}, ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("passN") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 5);
                        passN = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "passN" + " = " + passN);
                        continue;
                    }
                    else if (cfgParse.IndexOf("sidewaysPassN") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 13);
                        sidewaysPassN = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "sidewaysPassN" + " = " + sidewaysPassN);
                        continue;
                    }
                    else if (cfgParse.IndexOf("fallChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 10 + 1);
                        fallChance = StringToArray(cfgParse);
                        cfgParse = "New " + "fallChance" + " = {";
                        for (int m = 0; m < fallChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("fallKillChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 14 + 1);
                        fallKillChance = StringToArray(cfgParse);
                        cfgParse = "New " + "fallKillChance" + " = {";
                        for (int m = 0; m < fallKillChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("sidewaysFallChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 18 + 1);
                        sidewaysFallChance = StringToArray(cfgParse);
                        cfgParse = "New " + "sidewaysFallChance" + " = {";
                        for (int m = 0; m < sidewaysFallChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stonePassN") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 10);
                        stonePassN = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "stonePassN" + " = " + stonePassN);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stoneSidewaysPassN") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 18);
                        stoneSidewaysPassN = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "stoneSidewaysPassN" + " = " + stoneSidewaysPassN);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stoneFallChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 15 + 1);
                        stoneFallChance = StringToArray(cfgParse);
                        cfgParse = "New " + "stoneFallChance" + " = {";
                        for (int m = 0; m < stoneFallChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stoneFallKillChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 19 + 1);
                        stoneFallKillChance = StringToArray(cfgParse);
                        cfgParse = "New " + "stoneFallKillChance" + " = {";
                        for (int m = 0; m < stoneFallKillChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stoneSidewaysFallChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 23 + 1);
                        stoneSidewaysFallChance = StringToArray(cfgParse);
                        cfgParse = "New " + "stoneSidewaysFallChance" + " = {";
                        for (int m = 0; m < stoneSidewaysFallChance.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("stoneAdvanced") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 13);
                        stoneAdvanced = StringToBool(cfgParse);
                        Console.WriteLine("New " + "stoneAdvanced" + " = " + stoneAdvanced);
                        continue;
                    }
                    else if (cfgParse.IndexOf("fallSpread") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 10);
                        fallSpread = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "fallSpread" + " = " + fallSpread);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureKillChance") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 16 + 1);
                        natureKillChance = StringToArray(cfgParse);
                        cfgParse = "New " + "natureKillChance" + " = {";
                        for (int m = 0; m < fallBlock.Length; m++)
                        {
                            cfgParse = cfgParse + fallBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        Console.WriteLine("New " + "natureKillChance" + " = " + natureKillChance);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureKillBlock") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 15 + 1);
                        natureKillBlock = StringToArray(cfgParse);
                        cfgParse = "New " + "natureKillBlock" + " = {";
                        for (int m = 0; m < natureKillBlock.Length; m++)
                        {
                            cfgParse = cfgParse + natureKillBlock[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureKillTo") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 12);
                        natureKillTo = StringToInteger(cfgParse);
                        Console.WriteLine("New " + "natureKillWith" + " = " + natureKillTo);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureRepopulation") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 18);
                        natureRepopulation = StringToBool(cfgParse);
                        Console.WriteLine("New " + "natureRepopulation" + " = " + natureRepopulation);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureLivingReplace") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 19 + 1);
                        natureLivingReplace = StringToArray(cfgParse);
                        cfgParse = "New " + "natureLivingReplace" + " = {";
                        for (int m = 0; m < natureLivingReplace.Length; m++)
                        {
                            cfgParse = cfgParse + natureLivingReplace[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("natureOresReplace") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 17 + 1);
                        natureOresReplace = StringToArray(cfgParse);
                        cfgParse = "New " + "natureOresReplace" + " = {";
                        for (int m = 0; m < natureOresReplace.Length; m++)
                        {
                            cfgParse = cfgParse + natureOresReplace[m] + ", ";
                        }
                        cfgParse = cfgParse + "}";
                        Console.WriteLine(cfgParse);
                        continue;
                    }
                    else if (cfgParse.IndexOf("globalRelight") != -1)
                    {
                        cfgParse = cfgParse.Remove(0, 13);
                        globalRelight = StringToBool(cfgParse);
                        Console.WriteLine("New " + "globalRelight" + " = " + globalRelight);
                        continue;
                    }
                    Console.WriteLine("Invalid line:" + cfgOutput);
                }
            }
            else
            {
                Console.WriteLine("Config file not found, using internal defaults.");
            }

            if (((natureLivingReplace.Length != 0) || (natureOresReplace.Length != 0)) && natureRepopulation)
            {
                int[] ta = AddArrayToArray(natureLivingReplace, natureOresReplace);
                mutateBlock = AddArrayToMultiArray(mutateBlock, ta, 4);
            }

            Console.WriteLine();
            Console.WriteLine("Config loaded");
            Console.WriteLine();
            //Console.ReadLine();

            Console.WriteLine("Opening world: " + worldPath);
            Console.WriteLine();

            // Open the world
            BetaWorld world = BetaWorld.Open(worldPath);

            BetaChunkManager chunkMan = world.GetChunkManager();

            int[] minXZ = { 0, 0 };
            int[] maxXZ = { 0, 0 };

            int xdim = 16;
            int zdim = 16;

            int affectedChunks = 0;
            int reportedChunks = 0;

            Console.WriteLine("Selecting chunks to be aged");

            int[,] chunkSelTemp;
            List<ChunkRef> selectedChunks = new List<ChunkRef>();

            bool customChunks = false;

            if (selectedChunksArg != "0")
            {
                chunkSelTemp = StringToMultiArray(selectedChunksArg, 2);

                for (int i = 0; i < chunkSelTemp.GetUpperBound(0) + 1; i++)
                {
                    selectedChunks.Add(chunkMan.GetChunkRef(chunkSelTemp[i, 0], chunkSelTemp[i, 1]));
                }

                customChunks = true;
            }
            else
            {
                foreach (ChunkRef chunk in chunkMan)
                {
                    selectedChunks.Add(chunk);
                }
            }

            // See how big the world is, will be used for perlin mask
            Console.WriteLine("Working out world size");

            foreach (ChunkRef chunk in selectedChunks)
            {
                minXZ[0] = Math.Min(chunk.X * 16, minXZ[0]);
                minXZ[1] = Math.Min(chunk.Z * 16, minXZ[1]);

                maxXZ[0] = Math.Max(xdim + chunk.X * 16, maxXZ[0]);
                maxXZ[1] = Math.Max(zdim + chunk.Z * 16, maxXZ[1]);

                reportedChunks++;
            }

            int[] worldSize = { maxXZ[0] - minXZ[0], maxXZ[1] - minXZ[1] };

            Console.WriteLine("World size: " + worldSize[0] + " " + worldSize[1]);
            Console.WriteLine("Chunks total: " + reportedChunks);
            Console.WriteLine();

            // Generate the perlin mask

            Console.WriteLine("Generating perlin mask");

            int maxPerlinSize = 1024;
            double perlinResolution = Math.Max(worldSize[0], worldSize[1]) / maxPerlinSize;
            perlinResolution = (Math.Floor(perlinResolution));

            int perlinResInt = (int)perlinResolution;

            if (perlinResInt == 0)
                perlinResInt = 1;

            Console.WriteLine("Perlin mask size: " + worldSize[0] / perlinResInt + " " + worldSize[1] / perlinResInt);
            Console.WriteLine("Perlin mask resolution: " + perlinResInt);
            Console.WriteLine();

            Bitmap perlinMask = GeneratePerlinMask(worldSize, 4, perlinResInt);

            // ----------------------
            // Start main ageing loop
            // ----------------------

            Console.WriteLine("Running ageing loop");
            Console.WriteLine();

            // Turn off rain to avoid too many chunk updates and crashes
            if (natureRepopulation)
            {
                world.Level.IsRaining = false;
                world.Level.IsThundering = false;
                world.Save();
            }

            reportedChunks = 0;

            // Main ageing loop
            if (customChunks)
            {
                foreach (ChunkRef chunk in selectedChunks)
                {
                    affectedChunks++;

                    if (globalRelight)
                    {
                        chunk.Blocks.AutoLight = false;
                    }

                    AgeChunk(chunk, chunkMan, perlinMask, perlinResInt, minXZ);

                    chunkMan.Save();

                    if (affectedChunks > 10)
                    {
                        reportedChunks = reportedChunks + 10;
                        affectedChunks = affectedChunks - 10;
                        Console.WriteLine(reportedChunks + " chunks aged");
                    }
                }
            }
            else
            {
                foreach (ChunkRef chunk in chunkMan)
                {
                    affectedChunks++;

                    if (globalRelight)
                    {
                        chunk.Blocks.AutoLight = false;
                    }

                    AgeChunk(chunk, chunkMan, perlinMask, perlinResInt, minXZ);

                    chunkMan.Save();

                    if (affectedChunks > 10)
                    {
                        reportedChunks = reportedChunks + 10;
                        affectedChunks = affectedChunks - 10;
                        Console.WriteLine(reportedChunks + " chunks aged");
                    }
                }
            }
            Console.WriteLine((reportedChunks + affectedChunks) + " chunks aged");

            affectedChunks = 0;
            reportedChunks = 0;

            Console.WriteLine();
            Console.WriteLine("Running cleaning loop");

            // Cleaning loop
            if (customChunks)
            {
                foreach (ChunkRef chunk in selectedChunks)
                {
                    affectedChunks++;

                    if (natureRepopulation)
                    {
                        chunk.IsTerrainPopulated = false;
                    }

                    CleanChunk(chunk, chunkMan, perlinMask, perlinResInt, minXZ);

                    chunk.Blocks.RebuildFluid();

                    if (globalRelight)
                    {
                        chunk.Blocks.RebuildHeightMap();
                        chunk.Blocks.ResetBlockLight();
                        chunk.Blocks.ResetSkyLight();

                        chunk.Blocks.RebuildBlockLight();
                        chunk.Blocks.RebuildSkyLight();
                    }

                    chunkMan.Save();

                    if (affectedChunks > 50)
                    {
                        reportedChunks = reportedChunks + 50;
                        affectedChunks = affectedChunks - 50;
                        Console.WriteLine(reportedChunks + " chunks cleaned");
                    }
                }
            }
            else
            {
                foreach (ChunkRef chunk in chunkMan)
                {
                    affectedChunks++;

                    if (natureRepopulation)
                    {
                        chunk.IsTerrainPopulated = false;
                    }

                    CleanChunk(chunk, chunkMan, perlinMask, perlinResInt, minXZ);

                    chunk.Blocks.RebuildFluid();

                    if (globalRelight)
                    {
                        chunk.Blocks.RebuildHeightMap();
                        chunk.Blocks.ResetBlockLight();
                        chunk.Blocks.ResetSkyLight();

                        chunk.Blocks.RebuildBlockLight();
                        chunk.Blocks.RebuildSkyLight();
                    }

                    chunkMan.Save();

                    if (affectedChunks > 50)
                    {
                        reportedChunks = reportedChunks + 50;
                        affectedChunks = affectedChunks - 50;
                        Console.WriteLine(reportedChunks + " chunks cleaned");
                    }
                }
            }

            Console.WriteLine((reportedChunks + affectedChunks) + " chunks cleaned");
        }

        // Function for falling and toppling blocks
        public void AgeChunk(ChunkRef chunk, BetaChunkManager chunkMan, Bitmap perlinMask, int perlinResolution, int[] minXZ)
        {
            int xdim = chunk.Blocks.XDim;
            int ydim = chunk.Blocks.YDim;
            int zdim = chunk.Blocks.ZDim;

            double c = 0;

            int xp;
            int yp;
            int zp;

            float fallChanceMin = (float)fallChance[0] / 100f; // Some lazy coding on my side
            float fallChanceMax = (float)fallChance[1] / 100f - fallChanceMin;
            float fallKillChanceMin = (float)fallKillChance[0] / 100f;
            float fallKillChanceMax = (float)fallKillChance[1] / 100f - fallKillChanceMin;
            float sidewaysFallChanceMin = (float)sidewaysFallChance[0] / 100f;
            float sidewaysFallChanceMax = (float)sidewaysFallChance[1] / 100f - sidewaysFallChanceMin;
            float stoneFallChanceMin = (float)stoneFallChance[0] / 100f;
            float stoneFallChanceMax = (float)stoneFallChance[1] / 100f - stoneFallChanceMin;
            float stoneFallKillChanceMin = (float)stoneFallKillChance[0] / 100f;
            float stoneFallKillChanceMax = (float)stoneFallKillChance[1] / 100f - stoneFallKillChanceMin;
            float stoneSidewaysFallChanceMin = (float)stoneSidewaysFallChance[0] / 100f;
            float stoneSidewaysFallChanceMax = (float)stoneSidewaysFallChance[1] / 100f - stoneSidewaysFallChanceMin;

            int[] airPos = { -9999, -9999, -999, -999 };

            for (int pass = 1; pass <= passN; pass++)
            {
                // Check all blocks in the chunk
                for (int x = 0; x < xdim; x++)
                {
                    for (int z = 0; z < zdim; z++)
                    {
                        // Reset the algorithm limiter
                        int bottomY = ydim - 2;

                        // Get the perlin mask multiplier
                        Color perlinCol = perlinMask.GetPixel(Math.Abs(minXZ[0] / perlinResolution) + (chunk.X * 16 / perlinResolution) + (x / perlinResolution), Math.Abs(minXZ[1] / perlinResolution) + (chunk.Z * 16 / perlinResolution) + (z / perlinResolution));
                        double perlinMulti = (double)perlinCol.R / 255f;

                        int oldBlock;

                        if (stoneAdvanced && pass <= stonePassN) // Stone displacement - advanced method
                        {
                            for (int y = chunk.Blocks.GetHeight(x, z); y > 1; y--)
                            {
                                bottomY = y;

                                // Get the checked block's ID
                                oldBlock = chunk.Blocks.GetID(x, y, z);

                                if (oldBlock == 2 || oldBlock == 3 || oldBlock == 12 || oldBlock == 13) // Stop if encountered grass or dirt, should stop random underground space
                                    break;

                                if (oldBlock == 1)
                                {
                                    airPos = FindFreeLight(chunk, chunkMan, x, y, z);

                                    // Set the most bottom stone to be eroded, stop on grass
                                    if (((airPos[0] == -9999) && (airPos[1] == -9999)))
                                    {
                                        break;
                                    }
                                }
                            }

                            for (int y = bottomY; y < ydim - 2; y++)
                            {
                                // Don't even try if out of limits
                                if (y >= ydim - 2 || y <= 1)
                                {
                                    continue;
                                }

                                // Get the checked block's ID
                                oldBlock = chunk.Blocks.GetID(x, y, z);

                                // Probability test
                                c = rand.NextDouble();
                                if (c < stoneFallChanceMin + (stoneFallChanceMax * perlinMulti))
                                {
                                    if (oldBlock == 1)
                                    {
                                        // Random chance to destroy the block in fall
                                        c = rand.NextDouble();
                                        if ((c < fallKillChanceMin + (fallKillChanceMax * perlinMulti)) && pass == 1)
                                        {
                                            // Kill the initial block
                                            chunk.Blocks.SetID(x, y, z, 0);
                                            continue;
                                        }

                                        airPos = FindFreeAir(chunk, chunkMan, x, y, z);

                                        // Skip to another column if we don't see light anymore
                                        if ((airPos[0] == -9999) && (airPos[1] == -9999))
                                        {
                                            continue;
                                        }

                                        int xf = x;
                                        int zf = z;

                                        ChunkRef tchunk = chunk;

                                        // Random chance to topple
                                        c = rand.NextDouble();
                                        if ((c < sidewaysFallChanceMin + (sidewaysFallChanceMax * perlinMulti)) && pass == 1)
                                        {
                                            xf = airPos[0];
                                            zf = airPos[1];

                                            if (((airPos[2] != chunk.X) || (airPos[3] != chunk.Z)) && ((airPos[2] != -999) && (airPos[3] != -999)))
                                            {
                                                tchunk = chunkMan.GetChunkRef(airPos[2], airPos[3]);
                                            }
                                        }

                                        // Kill the initial block
                                        chunk.Blocks.SetID(x, y, z, 0);

                                        for (int yf = y; yf > 1; yf--)
                                        {
                                            // Incoming!
                                            int destBlock = tchunk.Blocks.GetID(xf, yf - 1, zf);

                                            if (destBlock == 0 || fallThroughBlock.Contains(destBlock))
                                            {
                                                continue;
                                            }

                                            // Smash weak blocks in the way
                                            if (smashBlock.Contains(destBlock))
                                            {
                                                tchunk.Blocks.SetID(xf, yf - 1, zf, 0);
                                                continue;
                                            }

                                            tchunk.Blocks.SetID(xf, yf, zf, 1);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else // Stone displacement - simple method
                        {
                            for (int y = chunk.Blocks.GetHeight(x, z); y > 1; y--)
                            {
                                // Don't even try if out of limits
                                if (y >= ydim - 2 || y <= 1)
                                {
                                    continue;
                                }

                                // Get the checked block's ID
                                oldBlock = chunk.Blocks.GetID(x, y, z);

                                if (oldBlock == 2 || oldBlock == 3) // Stop if encountered grass or dirt, should stop random underground space
                                    break;

                                if (oldBlock == 1)
                                {
                                    // Probability test
                                    c = rand.NextDouble();
                                    if (c < stoneFallChanceMin + (stoneFallChanceMax * perlinMulti))
                                    {
                                        // Random chance to destroy the block in fall
                                        c = rand.NextDouble();

                                        if ((c < stoneFallKillChanceMin + (stoneFallKillChanceMax * perlinMulti)) && pass == 1)
                                        {
                                            // Kill the initial block
                                            chunk.Blocks.SetID(x, y, z, 0);
                                            continue;
                                        }

                                        airPos = FindFreeLight(chunk, chunkMan, x, y, z);

                                        // Skip to another column if we don't see light anymore
                                        if ((airPos[0] == -9999) && (airPos[1] == -9999))
                                        {
                                            break;
                                        }

                                        int xf = x;
                                        int yf = y;
                                        int zf = z;

                                        ChunkRef tchunk = chunk;

                                        // Random chance to topple
                                        c = rand.NextDouble();
                                        if ((c < stoneSidewaysFallChanceMin + (stoneSidewaysFallChanceMin * perlinMulti)) && pass == 1)
                                        {
                                            xf = airPos[0];
                                            zf = airPos[1];
                                            yf = airPos[2];

                                            if (((airPos[3] != chunk.X) || (airPos[4] != chunk.Z)) && ((airPos[3] != -999) && (airPos[4] != -999)))
                                            {
                                                tchunk = chunkMan.GetChunkRef(airPos[3], airPos[4]);
                                            }
                                        }

                                        // Kill the initial block
                                        chunk.Blocks.SetID(x, y, z, 0);

                                        for (yf = y; yf > 1; yf--)
                                        {
                                            // Incoming!
                                            int destBlock = tchunk.Blocks.GetID(xf, yf - 1, zf);

                                            if (destBlock == 0 || fallThroughBlock.Contains(destBlock))
                                            {
                                                continue;
                                            }

                                            // Smash weak blocks in the way
                                            if (smashBlock.Contains(destBlock))
                                            {
                                                tchunk.Blocks.SetID(xf, yf - 1, zf, 0);
                                                continue;
                                            }

                                            tchunk.Blocks.SetID(xf, yf, zf, 1);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        for (int y = 1; y < ydim - 2; y++) // Manmade blocks displacement
                        {
                            // Get the checked block's ID
                            oldBlock = chunk.Blocks.GetID(x, y, z);

                            xp = x;
                            yp = y;
                            zp = z;

                            // Try to look
                            if (pass == 1)
                            {
                                // Loop through all entries in the mutateBlock array
                                for (int i = 0; i < mutateBlock.GetUpperBound(0); i++)
                                {
                                    if (oldBlock == 0)
                                    {
                                        break;
                                    }

                                    // If the checked block is not the what we want, jump to the next mutateBlock entry
                                    if (oldBlock != mutateBlock[i, 0])
                                    {
                                        continue;
                                    }

                                    // Have a random chance of letting the block live
                                    float pMin = (mutateBlock[i, 2]) / 100f;
                                    float pMax = (mutateBlock[i, 3]) / 100f;

                                    c = rand.NextDouble();
                                    if (c > pMin + (pMax * perlinMulti))
                                    {
                                        break;
                                    }

                                    chunk.Blocks.SetID(x, y, z, mutateBlock[i, 1]);
                                    oldBlock = mutateBlock[i, 1];
                                    break;
                                }
                            }

                            foreach (int i in fallBlock)
                            {
                                if (oldBlock == i)
                                {
                                    // Probability test
                                    c = rand.NextDouble();
                                    if (c < fallChanceMin + (fallChanceMax * perlinMulti))
                                    {
                                        int xtemp = x;
                                        int ztemp = z;

                                        // Random chance to destroy the block in fall
                                        c = rand.NextDouble();
                                        if ((c < fallKillChanceMin + (fallKillChanceMin * perlinMulti)) && pass == 1)
                                        {
                                            chunk.Blocks.SetID(x, y, z, 0);
                                            break;
                                        }

                                        ChunkRef tchunk = chunk;

                                        // If we push blocks sideways
                                        if (pass <= sidewaysPassN)
                                        {
                                            // Look if the block has empty space below it
                                            int supportBlock = chunk.Blocks.GetID(x, y - 1, z);

                                            if (supportBlock != 0)
                                            {
                                                c = rand.NextDouble();
                                                if (c < sidewaysFallChanceMin + (sidewaysFallChanceMin * perlinMulti))
                                                {
                                                    //Try to move the block sideways
                                                    airPos = FindFreeAir(chunk, chunkMan, x, y, z);

                                                    if ((airPos[0] == -9999) && (airPos[1] == -9999))
                                                    {
                                                        break;
                                                    }

                                                    xtemp = airPos[0];
                                                    ztemp = airPos[1];

                                                    if (((airPos[2] != chunk.X) || (airPos[3] != chunk.Z)) && ((airPos[2] != -999) && (airPos[3] != -999)))
                                                    {
                                                        tchunk = chunkMan.GetChunkRef(airPos[2], airPos[3]);
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        }

                                        int fallData = chunk.Blocks.GetData(x, y, z);
                                        TileEntity fallTE = chunk.Blocks.GetTileEntity(x, y, z);

                                        chunk.Blocks.SetID(x, y, z, 0);

                                        for (int yf = y; yf > 1; yf--)
                                        {
                                            // It's raining blocks, halelujah, it's raining blocks...
                                            int destBlock = tchunk.Blocks.GetID(xtemp, yf - 1, ztemp);

                                            if (destBlock == 0 || fallThroughBlock.Contains(destBlock))
                                            {
                                                continue;
                                            }

                                            // Smash weak blocks in the way
                                            if (smashBlock.Contains(destBlock))
                                            {
                                                tchunk.Blocks.SetID(xtemp, yf - 1, ztemp, 0);
                                                continue;
                                            }

                                            tchunk.Blocks.SetID(xtemp, yf, ztemp, i);
                                            tchunk.Blocks.SetData(xtemp, yf, ztemp, fallData);
                                            if (fallTE != null && AcceptsTileEntities(i))
                                                tchunk.Blocks.SetTileEntity(xtemp, yf, ztemp, fallTE);

                                            xp = xtemp;
                                            yp = yf;
                                            zp = ztemp;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Function for cleaning clutter left by AgeChunk
        public void CleanChunk(ChunkRef chunk, BetaChunkManager chunkMan, Bitmap perlinMask, int perlinResolution, int[] minXZ)
        {
            int xdim = chunk.Blocks.XDim;
            int ydim = chunk.Blocks.YDim;
            int zdim = chunk.Blocks.ZDim;

            int checkx = 0;
            int checky = 0;
            int checkz = 0;

            int oldBlock;

            float natureKillChanceMin = (float)natureKillChance[0] / 100f;
            float natureKillChanceMax = (float)natureKillChance[1] / 100f - natureKillChanceMin;

            // Check all blocks in the chunk
            for (int x = 0; x < xdim; x++)
            {
                for (int z = 0; z < zdim; z++)
                {
                    Color perlinCol = perlinMask.GetPixel(Math.Abs(minXZ[0]) + (chunk.X * 16) + (x / perlinResolution), Math.Abs(minXZ[1]) + (chunk.Z * 16) + (z / perlinResolution));
                    double perlinMulti = (double)perlinCol.R / 255f;

                    bool eroded = false;
                    bool foundLeaves = false;

                    for (int y = ydim - 2; y > 1; y--)
                    {
                        checkx = x;
                        checky = y;
                        checkz = z;

                        // Attempt to replace block
                        oldBlock = chunk.Blocks.GetID(x, y, z);
                        int oldData = chunk.Blocks.GetData(x, y, z);

                        // Destroy old trees
                        if (natureRepopulation)
                        {
                            if (oldBlock == 18)
                            {
                                foundLeaves = true;
                            }

                            if (foundLeaves && oldBlock == 17)
                            {
                                chunk.Blocks.SetID(x, y, z, 0);
                            }
                        }

                        if (oldBlock != 0 && !eroded && natureKillBlock.Length != 0)
                        {
                            foreach (int i in natureKillBlock)
                            {
                                if (oldBlock == i)
                                {
                                    double c = rand.NextDouble();
                                    if (c < natureKillChanceMin + (natureKillChanceMax * perlinMulti))
                                    {
                                        chunk.Blocks.SetID(x, y, z, natureKillTo);
                                    }
                                    eroded = true;
                                    break;
                                }
                            }
                        }

                        // Check signs, ladders
                        if (oldBlock == 65 || oldBlock == 68)
                        {
                            if (oldData == 2)
                            {
                                checkz = z + 1;
                            }
                            else if (oldData == 3)
                            {
                                checkz = z - 1;
                            }
                            else if (oldData == 4)
                            {
                                checkx = x + 1;
                            }
                            else if (oldData == 5)
                            {
                                checkx = x - 1;
                            }
                            int airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                            if (airCheck == 0)
                            {
                                chunk.Blocks.SetID(x, y, z, 0);
                                continue;
                            }
                        }

                        // Check buttons
                        if (oldBlock == 77)
                        {
                            if (oldData == 1)
                            {
                                checkx = x - 1;
                            }
                            else if (oldData == 2)
                            {
                                checkx = x + 1;
                            }
                            else if (oldData == 3)
                            {
                                checkz = z - 1;
                            }
                            else if (oldData == 4)
                            {
                                checkz = z + 1;
                            }
                            int airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                            if (airCheck == 0)
                            {
                                chunk.Blocks.SetID(x, y, z, 0);
                                continue;
                            }
                        }

                        // Check torches and levers
                        if (oldBlock == 50 || oldBlock == 69 || oldBlock == 75 || oldBlock == 76)
                        {
                            if (oldData == 1)
                            {
                                checkx = x - 1;
                            }
                            else if (oldData == 2)
                            {
                                checkx = x + 1;
                            }
                            else if (oldData == 3)
                            {
                                checkz = z - 1;
                            }
                            else if (oldData == 4)
                            {
                                checkz = z + 1;
                            }
                            else if (oldData == 5 || oldData == 6)
                            {
                                checky = y - 1;
                            }
                            int airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                            if (airCheck == 0)
                            {
                                chunk.Blocks.SetID(x, y, z, 0);
                                continue;
                            }
                        }

                        // Check trapdoors
                        if (oldBlock == 96)
                        {
                            if (oldData == 0)
                            {
                                checkz = z + 1;
                            }
                            else if (oldData == 1)
                            {
                                checkz = z - 1;
                            }
                            else if (oldData == 2)
                            {
                                checkx = x + 1;
                            }
                            else if (oldData == 3)
                            {
                                checkx = x - 1;
                            }

                            int airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                            if (airCheck == 0)
                            {
                                chunk.Blocks.SetID(x, y, z, 0);
                                continue;
                            }
                        }

                        // Check doors
                        if (oldBlock == 64 || oldBlock == 71)
                        {
                            int tchecky = y;
                            if (oldData >= 8)
                            {
                                tchecky = y - 1;
                                checky = y - 2;

                                int airCheck = CheckAir(chunk, chunkMan, checkx, tchecky, checkz);

                                if (airCheck != 64 && airCheck != 71)
                                {
                                    chunk.Blocks.SetID(x, y, z, 0);
                                    continue;
                                }

                                airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                                if (airCheck == 0)
                                {
                                    chunk.Blocks.SetID(x, y, z, 0);
                                    chunk.Blocks.SetID(x, y - 1, z, 0);
                                    continue;
                                }
                            }
                            else
                            {
                                tchecky = y + 1;
                                checky = y - 1;

                                int airCheck = CheckAir(chunk, chunkMan, checkx, tchecky, checkz);

                                if (airCheck != 64 && airCheck != 71)
                                {
                                    chunk.Blocks.SetID(x, y, z, 0);
                                    continue;
                                }

                                airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                                if (airCheck == 0)
                                {
                                    chunk.Blocks.SetID(x, y, z, 0);
                                    continue;
                                }
                            }
                        }

                        // Check the rest of supported blocks
                        foreach (int i in supportedBlocks)
                        {
                            if (oldBlock == i)
                            {
                                checky = y - 1;

                                int airCheck = CheckAir(chunk, chunkMan, checkx, checky, checkz);

                                if (airCheck == 0)
                                {
                                    chunk.Blocks.SetID(x, y, z, 0);
                                }
                            }
                        }
                    }
                }
            }
        }

        public int[] FindFreeAir(ChunkRef chunk, BetaChunkManager chunkMan, int x, int y, int z)
        {
            int cx = chunk.X;
            int cz = chunk.Z;

            int[] position = { -9999, -9999, cx, cz };

            int xdim = chunk.Blocks.XDim;
            int zdim = chunk.Blocks.ZDim;

            int xt;
            int zt;

            int cxt;
            int czt;

            for (int a = 0; a <= 8; a++)
            {
                xt = x + (-fallSpread + rand.Next(((1 + fallSpread) * 2)));
                zt = z + (-fallSpread + rand.Next(((1 + fallSpread) * 2)));

                cxt = cx;
                czt = cz;

                // Jump into neighboring chunks if needed
                if (xt < 0)
                {
                    cxt--;
                    xt = xt + xdim;
                }
                else if (xt >= xdim)
                {
                    cxt++;
                    xt = xt - xdim;
                }

                if (zt < 0)
                {
                    czt--;
                    zt = zt + zdim;
                }
                else if (zt >= zdim)
                {
                    czt++;
                    zt = zt - zdim;
                }

                ChunkRef tchunk = chunkMan.GetChunkRef(cxt, czt);

                if (tchunk == null)
                    continue;

                int resBlock = tchunk.Blocks.GetID(xt, y, zt);
                if (resBlock == 0 || smashBlock.Contains(resBlock))
                {
                    position[0] = xt;
                    position[1] = zt;
                    position[2] = cxt;
                    position[3] = czt;
                    break;
                }
            }
            return position;
        }

        public int[] FindFreeLight(ChunkRef chunk, BetaChunkManager chunkMan, int x, int y, int z)
        {
            int[] position = { -9999, -9999, -9999, -999, -999 };

            int cx = chunk.X;
            int cz = chunk.Z;

            int xdim = chunk.Blocks.XDim;
            int zdim = chunk.Blocks.ZDim;

            int xt = x;
            int zt = z;

            int chance = rand.Next(4);

            if (chance == 0)
                xt = x + 1;
            else if (chance == 1)
                xt = x - 1;
            else if (chance == 2)
                zt = z + 1;
            else
                zt = z - 1;

            int xtt = xt;
            int ztt = zt;

            int cxt = cx;
            int czt = cz;

            for (int a = 0; a <= 4; a++)
            {
                xt = x;
                zt = z;
                cxt = cx;
                czt = cz;

                if (a == 4)
                {
                    y--;
                }
                else if ((xtt == x + 1) && (ztt == z))
                {
                    xt = x;
                    zt = z + 1;
                }
                else if ((xtt == x) && (ztt == z + 1))
                {
                    xt = x - 1;
                    zt = z;
                }
                else if ((xtt == x - 1) && (ztt == z))
                {
                    xt = x;
                    zt = z - 1;
                }
                else if ((xtt == x) && (ztt == z - 1))
                {
                    xt = x + 1;
                    zt = z;
                }

                xtt = xt;
                ztt = zt;

                // Jump into neighboring chunks if needed
                if (xt < 0)
                {
                    cxt--;
                    xt = xt + xdim;
                }
                else if (xt >= xdim)
                {
                    cxt++;
                    xt = xt - xdim;
                }

                if (zt < 0)
                {
                    czt--;
                    zt = zt + zdim;
                }
                else if (zt >= zdim)
                {
                    czt++;
                    zt = zt - zdim;
                }

                ChunkRef tchunk = chunkMan.GetChunkRef(cxt, czt);

                if (tchunk == null)
                { continue; }

                int resBlock = tchunk.Blocks.GetID(xt, y, zt);
                if (resBlock == 0 || smashBlock.Contains(resBlock))
                {
                    position[0] = xt;
                    position[1] = zt;
                    position[2] = y;
                    position[3] = cxt;
                    position[4] = czt;
                    break;
                }
            }
            return position;
        }

        public int CheckAir(ChunkRef chunk, BetaChunkManager chunkMan, int x, int y, int z)
        {
            int cx = chunk.X;
            int cz = chunk.Z;

            int xdim = chunk.Blocks.XDim;
            int zdim = chunk.Blocks.ZDim;

            int result = 0;

            if (x < 0)
            {
                cx--;
                x = x + xdim;
            }
            else if (x >= xdim)
            {
                cx++;
                x = x - xdim;
            }

            if (z < 0)
            {
                cz--;
                z = z + zdim;
            }
            else if (z >= zdim)
            {
                cz++;
                z = z - zdim;
            }

            ChunkRef tchunk = chunkMan.GetChunkRef(cx, cz);

            if (tchunk != null)
            {
                int resBlock = tchunk.Blocks.GetID(x, y, z);
                if (resBlock != 0)
                {
                    result = resBlock;
                }
            }

            return result;
        }

        public bool AcceptsTileEntities(int blockNum)
        {
            bool result = false;

            if ((blockNum == 23 || blockNum == 25) || (blockNum == 29 || blockNum == 33) || (blockNum == 52 || blockNum == 54) || (blockNum == 61 || blockNum == 62) || (blockNum == 63 || blockNum == 68))
            {
                result = true;
            }

            return result;
        }

        // Generic functions down there, all ageing is done above

        public Bitmap GeneratePerlinMask(int[] perlinSize, int perlinMultiplier, int perlinResolution)
        {
            int regionSize = 512;

            perlinSize[0] = perlinSize[0] / perlinResolution;
            perlinSize[1] = perlinSize[1] / perlinResolution;

            perlinSize[0] = Math.Max(perlinSize[0], regionSize);
            perlinSize[1] = Math.Max(perlinSize[1], regionSize);

            int regionMultiplier = Math.Min((perlinSize[0] / regionSize) * perlinMultiplier, (perlinSize[1] / regionSize) * perlinMultiplier);

            Bitmap bitmap = new Bitmap(perlinSize[0], perlinSize[1]);

            PerlinNoise noise = new PerlinNoise(8, 0.3, 1.0 / 256, 1.0);

            for (int y = 0; y < perlinSize[1]; y++)
            {
                for (int x = 0; x < perlinSize[0]; x++)
                {
                    double c = Math.Max(0.0f, Math.Min(1.0f, (float)noise.Function2D(regionMultiplier * x, regionMultiplier * y) * 0.5f + 0.5f));
                    byte b = (byte)(c * 255);

                    bitmap.SetPixel(x, y, Color.FromArgb(b, b, b));
                }
            }

            return bitmap;
        }

        public int[] StringToArray(string input)
        {
            List<int> result = new List<int>();
            int arrNum = 0;
            int add = 0;
            bool finished = false;
            string sub = "";
            string temp = "";
            string progress = input.Remove(0, 1);

            for (int i = 0; i <= 99; i++)
            {
                temp = "";
                add = 0;

                for (int i2 = 0; i2 <= 3; i2++)
                {
                    sub = progress.Substring(0, 1);

                    progress = progress.Remove(0, 1);
                    if (sub == ",")
                        break;
                    else if (sub == "}")
                    {
                        finished = true;
                        break;
                    }

                    if (temp.Length != 0)
                    {
                        temp += sub;
                    }
                    else
                    {
                        temp = sub;
                    }
                }

                add = Int32.Parse(temp);

                result.Add(add);
                arrNum++;

                if (finished)
                    break;
            }
            return result.ToArray();
        }

        public int[,] StringToMultiArray(string input, int subSize)
        {
            int arrNum = 0;
            int curArr = 0;
            string sub = "";
            string subArr = "";

            string progress = input.Remove(0, 1);
            foreach (char c in progress)
                if (c == '{') arrNum++;

            int[,] result = new int[arrNum, subSize];

            for (int i = 0; i <= arrNum - 1; i++)
            {
                while (sub != "}")
                {
                    sub = progress.Substring(0, 1);
                    progress = progress.Remove(0, 1);
                    subArr = subArr + sub;
                }
                progress = progress.Remove(0, 1);

                sub = "";

                int[] subArrT = StringToArray(subArr);

                for (int i2 = 0; i2 <= subSize - 1; i2++)
                {
                    result[curArr, i2] = subArrT[i2];
                }

                curArr++;
                subArr = "";
            }
            return result;
        }

        public int StringToInteger(string input)
        {
            int result = 0;
            int add = 0;
            bool negative = false;
            string sub = "";
            string temp = "";
            string progress = input.Remove(0, 1);

            for (int i = 0; i <= 99; i++)
            {
                sub = progress.Substring(0, 1);
                progress = progress.Remove(0, 1);

                if (sub == ";")
                {
                    break;
                }

                if (sub == "-")
                {
                    negative = true;
                    continue;
                }

                if (temp.Length != 0)
                {
                    temp += sub;
                }
                else
                {
                    temp = sub;
                }
            }

            add = Int32.Parse(temp);
            if (negative)
            {
                result = -add;
            }
            else
            {
                result = add;
            }

            return result;
        }

        public bool StringToBool(string input)
        {
            bool result = false;
            string sub = "";
            string progress = input.Remove(0, 1);

            progress = progress.ToLowerInvariant();
            sub = progress.Substring(0, 1);

            if (sub == "t" || sub == "1")
            {
                result = true;
            }

            return result;
        }

        public int[] AddArrayToArray(int[] input1, int[] input2)
        {
            int[] result = input1;
            int[] temp = new int[input1.Length + input2.Length];

            if ((input1.Length != 0) && (input2.Length != 0))
            {

                for (int a = 0; a < input1.Length; a++)
                {
                    temp[a] = input1[a];
                }

                for (int a = input1.Length; a < input1.Length + input2.Length; a++)
                {
                    temp[a] = input2[a - input1.Length];
                }

                result = new int[input1.Length + input2.Length];
                result = temp;
            }
            return result;
        }

        public int[,] AddArrayToMultiArray(int[,] input1, int[] input2, int inputNum)
        {
            int[,] result = input1;
            int[,] temp = new int[input1.GetUpperBound(0) + input2.GetUpperBound(0) + 1, inputNum];

            if ((input1.Length != 0) && (input2.Length != 0))
            {
                for (int a = 0; a < input1.GetUpperBound(0); a++)
                {
                    for (int b = 0; b < inputNum; b++)
                    {
                        //System.Console.WriteLine(a + " " + b);
                        temp[a, b] = input1[a, b];
                    }
                }

                for (int a = input1.GetUpperBound(0); a < input1.GetUpperBound(0) + input2.GetUpperBound(0); a++)
                {
                    temp[a, 0] = input2[a - input1.GetUpperBound(0)]; // Hacky solution, but it works
                    temp[a, 1] = 0;
                    temp[a, 2] = 10000; //Make sure nothing survives
                    temp[a, 3] = 10000;
                }

                result = new int[input1.GetUpperBound(0) + input2.GetUpperBound(0), inputNum];
                result = temp;
            }
            return result;
        }
    }
}