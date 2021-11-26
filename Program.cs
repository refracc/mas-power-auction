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
            for (int j = 0; j < 10; j++) // Run code 10 times
            {
                var env = new EnvironmentMas(); // Create agent environment
                var e = new EnvironmentAgent(); // Create environment agent
                env.Add(e, "environment"); // Add environment agent

                var b = new BrokerAgent();
                env.Add(b, "broker"); // Create & Add Broker agent to 

                var agents = new List<HouseAgent>();

                for (var i = 1; i <= 5; i++)
                {
                    var a = new HouseAgent();
                    agents.Add(a); // Add agent to a collection of agents
                    env.Add(a, $"house{i}"); // Add agent to environment
                }

                env.Start();

                foreach (var a in agents) Console.WriteLine(a.ToString()); // Print properties of each agent
                WriteToFile(agents); // Save to file (automation hehehehe)

                Console.ReadLine(); // Wait for input before either next iteration or program termination
            }
        }

        /// <summary>
        /// Write the agents within the system to a file.
        /// </summary>
        /// <param name="agents">The collection of agents.</param>
        private static void WriteToFile(List<HouseAgent> agents)
        {
            using var sw = File.AppendText("mas-experiments.csv");
            foreach (var a in agents) sw.WriteLine(a.ToString().Replace(' ', ','));
            sw.WriteLine();
            sw.Close();
        }
    }
}