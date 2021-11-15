using ActressMas;
using System;
using System.Collections.Generic;

namespace Coursework
{
    public class BrokerAgent : Agent
    {
        public static Dictionary<string, int> SellingAgents;
        public static List<string> BuyingAgents;

        public BrokerAgent()
        {
            SellingAgents = new();
            BuyingAgents = new();
        }

        public override void Act(Message message)
        {
            Console.WriteLine($"\t{message.Format()}");
            string[] msg = message.Content.Split(' ');

            if (message.Content.StartsWith("register"))
            {
                SellingAgents.Add(msg[1], int.Parse(msg[7]));
            } else if (message.Content.StartsWith("buying"))
            {
                BuyingAgents.Add(message.Sender);
            } 
            else if (message.Content.StartsWith("search"))
            {
                if (SellingAgents.Count > 0)
                {
                    string name = "";
                    int lowest = int.MaxValue;
                    foreach (KeyValuePair<string, int> pair in SellingAgents)
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
                if (BuyingAgents.Contains(message.Sender)) BuyingAgents.Remove(message.Sender);
                if (SellingAgents.ContainsKey(message.Sender)) SellingAgents.Remove(message.Sender);
            }
         }
    }
}
