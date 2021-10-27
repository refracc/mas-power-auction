using ActressMas;
using System;
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
            List<HouseAgent> agents = new();

            for (int i = 0; i < 10; i++)
            {
                var a = new HouseAgent();
                agents.Add(a);
                env.Add(a, $"house{i}");
            }

            env.Start();
            Console.ReadLine();
        }
    }
}
