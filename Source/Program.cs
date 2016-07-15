using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace TeethOfTime
{
    using Substrate;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                System.Console.WriteLine("Path to an existing world must be entered to continue.");
                System.Console.WriteLine("Example: TeethOfTime /path/to/world (config)");     
            }

            // Just some fancy text

            System.Console.WriteLine();
            System.Console.WriteLine("-=================-");
            System.Console.WriteLine("-= Teeth of Time =-");
            System.Console.WriteLine("-=================-");
            System.Console.WriteLine();

            // Check if world exists

            if (args.Length >= 1)
            {
                if (!(System.IO.Directory.Exists(args[0]))) // World directory does not exist
                {
                    System.Console.WriteLine(args[0] + " is not a valid world path. Path to an existing world must be entered.");
                    System.Console.ReadLine();
                    return;
                }
            }
            else // World not specified
            {
                System.Console.WriteLine("No world path specified. Path to an existing world must be entered.");
                System.Console.ReadLine();
                return;
            }

            // Check if config exists

            string config;

            if (args.Length >= 2)
            {
                config = args[1];
                string see = args[1];

                if (args[1].Length >= 4) // Look if there's the ".cfg" suffix
                {
                    see = args[1].Substring(args[1].Length - 4, 4);
                }

                if (see != ".cfg") // Add it if it's missing
                {
                    config = args[1] + ".cfg";
                }

                if (System.IO.File.Exists("configs/" + config)) // Config file found
                {
                    System.Console.WriteLine("Using config file " + config);
                }
                else // Config file not found, use the default.cfg
                {
                    config = "default.cfg";
                    System.Console.WriteLine(config + " config file does not exist. Using default.");
                    System.Console.ReadLine();
                }
            }
            else // No config file specified, use the default.cfg
            {
                System.Console.WriteLine("No config file specified. Using default.");
                System.Console.ReadLine();
                config = "default.cfg";
            }

            string selectedChunks = "0";

            if (args.Length >= 3)
            {
                selectedChunks = args[2];
            }

            Console.WriteLine();
            Console.WriteLine("-------------------");
            Console.WriteLine();

            // Run the ageing process

            Destroy proc = new Destroy();
            proc.Run(args[0], config, selectedChunks);

            // Wait till it finishes

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.WriteLine();

            //Console.ReadLine();
        }
    }
}
