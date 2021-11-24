﻿using System;
using System.Collections.Generic;
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

            Console.ReadLine();
        }
    }
}