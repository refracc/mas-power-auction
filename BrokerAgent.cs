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
            var msg = message.Content.Split(' ');

            if (message.Content.StartsWith("register"))
            {
                _sellingAgents.Add(msg[1], int.Parse(msg[7]));
            }
            else if (message.Content.StartsWith("buying"))
            {
                BuyingAgents.Add(message.Sender);
            }
            else if (message.Content.StartsWith("search"))
            {
                if (_sellingAgents.Count > 0)
                {
                    var name = "";
                    var lowest = int.MaxValue;
                    foreach (var (key, value) in _sellingAgents.Where(pair => pair.Value < lowest))
                    {
                        name = key;
                        lowest = value;
                    }

                    Send(message.Sender, $"seller {name} {lowest}");
                }
                else
                {
                    Send(message.Sender, "utility");
                }
            }
            else if (message.Content.StartsWith("unregister"))
            {
                if (BuyingAgents.Contains(message.Sender)) BuyingAgents.Remove(message.Sender);

                if (_sellingAgents.ContainsKey(message.Sender)) _sellingAgents.Remove(message.Sender);

                if (_sellingAgents.Count != 0 || BuyingAgents.Count != 0) return;
                Send("environment", "stop");
                Stop();
            }
        }
    }
}