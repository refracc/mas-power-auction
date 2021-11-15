using ActressMas;
using System;
using System.IO;
using System.Collections.Generic;

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

            List<HouseAgent> agents = new();

            for (int i = 1; i <= 5; i++)
            {
                var a = new HouseAgent();
                agents.Add(a);
                env.Add(a, $"house{i}");
            }

            env.Start();
        }
    }
}
