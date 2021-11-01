using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using ActressMas;

namespace Coursework
{
    public class BrokerAgent : Agent
    {
        private Dictionary<string, int> _sellingAgents;

        public BrokerAgent()
        {
            _sellingAgents = new();
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            string[] msg = message.Content.Split(' ');

            if (message.Content.StartsWith("register"))
            {
                _sellingAgents.Add(msg[1], int.Parse(msg[7]));
            } else if (message.Content.StartsWith("search"))
            {
                if (_sellingAgents.Count > 0)
                {
                    string name = "";
                    int lowest = int.MaxValue;
                    foreach (KeyValuePair<string, int> pair in _sellingAgents)
                    {
                        if (pair.Value < lowest)
                        {
                            name = pair.Key;
                            lowest = pair.Value;
                        }
                    }

                    Send(message.Sender, $"seller {name} {lowest}");
                } else
                {
                    Send(message.Sender, "utility");
                }
            } else if (message.Content.StartsWith("unregister"))
            {
                _sellingAgents.Remove(message.Sender);
            }
         }
    }
}
