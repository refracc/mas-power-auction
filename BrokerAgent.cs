using System;
using System.Collections.Generic;
using System.Linq;
using ActressMas;

namespace Coursework
{
    public class BrokerAgent : Agent
    {
        private static Dictionary<string, int> _sellingAgents;
        public static List<string> BuyingAgents;

        public BrokerAgent()
        {
            _sellingAgents = new Dictionary<string, int>();
            BuyingAgents = new List<string>();
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            var msg = message.Content.Split(' '); // Prepare message as arguments

            if (message.Content.StartsWith("register")) // If an agent is registering as a selling agent
            {
                _sellingAgents.Add(msg[1], int.Parse(msg[7])); // Add name and price to dictionary
            }
            else if (message.Content.StartsWith("buying")) // If an agent is registering as a buying agent
            {
                BuyingAgents.Add(message.Sender); // Add name to collection
            }
            else if (message.Content.StartsWith("search")) // If the buying agent is searching for power
            {
                if (_sellingAgents.Count > 0)
                {
                    var name = "";
                    var lowest = int.MaxValue;
                    foreach (var (key, value) in
                        _sellingAgents.Where(pair => pair.Value < lowest)) // Finding minimum using LINQ
                    {
                        name = key;
                        lowest = value;
                    }

                    Send(message.Sender, $"seller {name} {lowest}"); // Send name and lowest price to agent requesting
                }
                else
                {
                    Send(message.Sender, "utility"); // Otherwise send to utility.
                }
            }
            else if (message.Content.StartsWith("unregister")) // If the agent is looking to unregister
            {
                // Remove the agent from both collections (it should only be in 1 anyway).
                if (BuyingAgents.Contains(message.Sender)) BuyingAgents.Remove(message.Sender);
                if (_sellingAgents.ContainsKey(message.Sender)) _sellingAgents.Remove(message.Sender);

                // If the agent has no more selling or buying agents, inform the environment to stop.
                if (_sellingAgents.Count != 0 || BuyingAgents.Count != 0) return;
                Send("environment", "stop");
                Stop(); // Cease execution.
            }
        }
    }
}