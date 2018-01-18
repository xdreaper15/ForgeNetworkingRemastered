using System;
using System.Linq;

namespace Benchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quit = false;
            ForgeBenchmark benchmark = new ForgeBenchmark();

            if (args.Contains("server"))
            {
                benchmark.Server();
            }
            else if (args.Contains("client"))
            {
                benchmark.Client();
            }

            while (!quit)
            {
                switch (Console.ReadLine())
                {
                    case "quit":
                    case "exit":
                        benchmark.Disconnect();
                        quit = true;
                        break;
                }
            }
        }
    }
}