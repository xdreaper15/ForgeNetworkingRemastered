using BeardedManStudios;
using System;
using System.Collections.Generic;
using System.IO;

namespace MasterServer
{
	class Program
	{
		private const string SIG_FILE = "forge-master.sig";
		private const string HOST = "0.0.0.0";
		private const ushort PORT = 15940;

		static void Main(string[] args)
		{
			string host = HOST;
			ushort port = PORT;
			string read = string.Empty;
			int eloRange = 0;
			int seed = GetOrCreateSeed();

			Dictionary<string, string> arguments = ArgumentParser.Parse(args);

			if (args.Length > 0)
			{
				if (arguments.ContainsKey("host"))
					host = arguments["host"];

				if (arguments.ContainsKey("port"))
					ushort.TryParse(arguments["port"], out port);

				if (arguments.ContainsKey("elorange"))
					int.TryParse(arguments["elorange"], out eloRange);
			}
			else
			{
				Console.WriteLine("Entering nothing will choose defaults.");
				Console.WriteLine("Enter Host IP (Default: " + HOST + "):");
				read = Console.ReadLine();

				if (string.IsNullOrEmpty(read))
					host = HOST;

				Console.WriteLine("Enter Port (Default: " + PORT + "):");
				read = Console.ReadLine();

				if (string.IsNullOrEmpty(read))
					port = PORT;
				else
					ushort.TryParse(read, out port);
			}

			Console.WriteLine(string.Format("Hosting ip [{0}] on port [{1}]", host, port));
			ShowHelp();
			MasterServer server = new MasterServer(host, port, seed);
			server.EloRange = eloRange;

			while (true)
			{
				read = Console.ReadLine().ToLower();
				if (read == "s" || read == "stop")
				{
					lock (server)
					{
						Console.WriteLine("Server stopped.");
						server.Dispose();
					}
				}
				else if (read == "r" || read == "restart")
				{
					lock (server)
					{
						if (server.IsRunning)
						{
							Console.WriteLine("Server stopped.");
							server.Dispose();
						}
					}

					Console.WriteLine("Restarting...");
					Console.WriteLine(string.Format("Hosting ip [{0}] on port [{1}]", host, port));
					server = new MasterServer(host, port, seed);
				}
				else if (read == "q" || read == "quit")
				{
					lock (server)
					{
						Console.WriteLine("Quitting...");
						server.Dispose();
					}

					break;
				}
				else if (read == "h" || read == "help")
					ShowHelp();
				else if (read == "e" || read.StartsWith("elo"))
				{
					int index = read.IndexOf("=");
					string val = read.Substring(index + 1, read.Length - (index + 1));
					if (int.TryParse(val.Replace(" ", string.Empty), out index))
					{
						Console.WriteLine(string.Format("Elo range set to {0}", index));
						if (index == 0)
							Console.WriteLine("Elo turned off");
						server.EloRange = index;
					}
					else
						Console.WriteLine("Invalid elo range provided (Must be an integer)\n");
				}
				else if (read == "l" || read == "list")
					server.List(Console.WriteLine);
			}
		}

		private static int GetOrCreateSeed()
		{
			int seed = 0;
			if (!File.Exists(SIG_FILE) || !int.TryParse(File.ReadAllText(SIG_FILE), out seed))
			{
				Console.WriteLine("Failed to find the master server signature file " + SIG_FILE);
				seed = new Random().Next();
				Console.WriteLine("New signature has been generated for your game: " + seed);
				File.WriteAllText(SIG_FILE, seed.ToString());
				Console.WriteLine("New sig file created at " + Path.GetFullPath(SIG_FILE));
			}

			return seed;
		}

		private static void ShowHelp()
		{
			Console.WriteLine(@"[s] - Stops hosting
[r] - Restarts the hosting service even when stopped
[e] - Set the elo range to accept in difference [i.e. ""elorange = 10""]
[q] - Quits the application
[h] - Get a full list of comands
[l] - List the currently registered servers");
		}
	}
}