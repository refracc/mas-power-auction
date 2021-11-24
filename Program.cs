using ActressMas;
using System.Collections.Generic;
using System;

namespace Coursework
{
    public class Program
    {
        public static void Main(string[] _)
        {
            var env = new EnvironmentMas();
            var e = new EnvironmentAgent();
            env.Add(e, "environment");

            var b = new BrokerAgent();
            env.Add(b, "broker");

            List<HouseAgent> _agents = new List<HouseAgent>();

            for (int i = 1; i <= 5; i++)
            {
                var a = new HouseAgent();
                _agents.Add(a);
                env.Add(a, $"house{i}");
            }

            env.Start();

            foreach (Agent a in _agents)
            {
                Console.WriteLine(a.ToString());
            }

            Console.ReadLine();
        }
    }
}
