using System;
using System.Collections.Generic;
using System.IO;
using ActressMas;

namespace Coursework
{
    public class Program
    {
        public static void Main(string[] _)
        {
            for (int j = 0; j < 10; j++)
            {
                var env = new EnvironmentMas();
                var e = new EnvironmentAgent();
                env.Add(e, "environment");

                var b = new BrokerAgent();
                env.Add(b, "broker");

                var agents = new List<HouseAgent>();

                for (var i = 1; i <= 5; i++)
                {
                    var a = new HouseAgent();
                    agents.Add(a);
                    env.Add(a, $"house{i}");
                }

                env.Start();

                foreach (var a in agents) Console.WriteLine(a.ToString());
                WriteToFile(agents);

                Console.ReadLine();
            }
        }

        private static void WriteToFile(List<HouseAgent> agents)
        {
            using var sw = File.AppendText("mas-experiments.csv");
            foreach (var a in agents) sw.WriteLine(a.ToString().Replace(' ', ','));
            sw.WriteLine();
        }
    }
}