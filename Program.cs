using System;
using System.Collections.Generic;
using System.Text;
using ActressMas;

namespace Coursework
{
    public class Program
    {
        public static void Main(string[] _)
        {
            var env = new EnvironmentMas();
            var e = new EnvironmentAgent();
            env.Add(e, "environment");

            for (int i = 0; i < 6; i++)
            {
                var a = new HouseAgent();
                env.Add(a, $"house{i}");
            }

            env.Start();
        }
    }
}
