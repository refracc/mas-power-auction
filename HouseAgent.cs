using System;
using System.Threading;
using ActressMas;

namespace Coursework
{
    public class HouseAgent : Agent
    {
        private readonly int _sellToNeighbour = new Random().Next(1, 3);
        private int _balance;
        private int _demand;
        private int _generation;
        private int _initialDemand;
        private bool _initialised;
        private int _initialPower;
        private int _purchaseFromUtility;
        private int _sellable;
        private int _sellToUtility;
        private State _state = 0;

        public override void Setup()
        {
            // Let the environment know this agent is ready to go.
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            Console.WriteLine(message.Format());

            var msg = message.Content.Split(' '); // Prepare the incoming message as an array of strings
            switch (message.Sender)
            {
                case "environment": // If the message is from the environment
                {
                    if (message.Content.StartsWith("inform")) // Giving us the default data values.
                    {
                        _demand = int.Parse(msg[1]);
                        _generation = int.Parse(msg[2]);
                        _initialPower = _generation;
                        _initialDemand = _demand;
                        _purchaseFromUtility = int.Parse(msg[3]);
                        _sellToUtility = int.Parse(msg[4]);
                        _state = _demand > _generation ? State.BUY : State.SELL; // Ternary Operator :)
                        _sellable = _generation - _demand;
                        Console.WriteLine(ToString());

                        // If the state is selling state
                        if (_state == State.SELL)
                        {
                            if (_demand != _generation)
                                Send("broker", $"register {ToString()}"); // Inform broker this is selling agent
                            else HandleStop();
                        }
                        else
                        {
                            Send("broker", "buying"); // Inform broker this is buying agent
                        }
                    }

                    break;
                }
                // If the message is from the broker and it starts with word "seller"
                case "broker" when message.Content.StartsWith("seller"):
                {
                    Send(msg[1], "purchase"); // Purchase 1 kWh from the house specified (msg[1])
                    _balance -= int
                        .Parse(msg[2]); // Subtract purchaseFromNeighbour (from the house agent) from this house's balance
                    _generation++; // Add 1 kWh to this agent.

                    if (_generation == _demand) HandleStop();
                    break;
                }
                case "broker":
                {
                    if (message.Content
                        .Contains("utility")) // If the broker sends this agent to the utility company for power
                    {
                        _generation++; // Add 1 kWh to this agent
                        _balance -= _purchaseFromUtility; // Subtract cost from balance

                        if (_generation == _demand) HandleStop();
                    }

                    break;
                }
                default: // Everything else...
                {
                    if (message.Sender.Contains("house"))
                        if (message.Content
                            .StartsWith(
                                "purchase")) // If the incoming message is from a house and it starts with the word "purchase"
                        {
                            _balance += _sellToNeighbour; // Add sale to balance
                            _generation--;

                            if (_demand == _generation) HandleStop();
                        }

                    break;
                }
            }
        }

        public override void ActDefault()
        {
            if (!_initialised) // Initialisation flag check, runs once for each agent in the system.
            {
                Thread.Sleep(250);
                _initialised = !_initialised;
                return;
            }

            // Check the state of the agent
            if (_state == State.SELL && _generation == _demand)
            {
                HandleStop();
            }
            else if (_state == State.SELL)
            {
                if (_generation > _demand)
                {
                    // If the agent is generating more power than it needs, and the broker agent has no more selling agents
                    if (BrokerAgent.BuyingAgents.Count == 0)
                    {
                        _balance += _sellToUtility; // Sell power to utility
                        _generation--;

                        if (_generation == _demand) HandleStop();
                    }
                }
                else if (_generation == _demand) HandleStop();
            }
            else if (_state == State.BUY)
            {
                Send("broker", "search"); // Search broker agent for any agents selling power.
            }
        }

        public override string ToString() =>
            $"{Name} {_initialDemand} {_initialPower} {_purchaseFromUtility} {_sellToUtility} {_state} {_sellToNeighbour} {_balance} {_sellable}";

        private void HandleStop()
        {
            _state = State.SELL;
            Console.WriteLine($"{Name} has satisfied their energy requirements!");
            Send("broker", "unregister");
            Stop();
        }
    }
}