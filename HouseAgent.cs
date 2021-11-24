using ActressMas;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Coursework
{
    public class HouseAgent : Agent
    {
        private int _demand = 0;
        private int _generation = 0;
        private int _purchaseFromUtility = 0;
        private int _sellToUtility = 0;
        private int _sellToNeighbour = new Random().Next(1, 3);
        private int _balance = new Random().Next(10, 20);
        private int _sellable = 0;
        private State _state = 0;

        public override void Setup()
        {
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            Console.WriteLine(message.Format());

            string[] msg = message.Content.Split(' ');
            if (message.Sender == "environment")
            {
                if (message.Content.StartsWith("inform"))
                {
                    _demand = int.Parse(msg[1]);
                    _generation = int.Parse(msg[2]);
                    _purchaseFromUtility = int.Parse(msg[3]);
                    _sellToUtility = int.Parse(msg[4]);
                    _state = (_demand > _generation) ? State.BUY : State.SELL;
                    _sellable = (_generation > _demand) ? (_generation - _demand) : 0;
                    Console.WriteLine(ToString());

                    if (_state == State.SELL)
                    {
                        if (_demand != _generation)
                        {
                            Send("broker", $"register {ToString()}");
                        }
                        else HandleStop();
                    } else
                    {
                        Send("broker", "buying");
                    }
                }
            } else if (message.Sender == "broker")
            {
                if (message.Content.StartsWith("seller"))
                {
                    Send(msg[1], $"purchase");
                    _balance -= int.Parse(msg[2]);
                    _generation++;

                    if (_generation == _demand) HandleStop();

                } else if (message.Content.Contains("utility"))
                {
                    _generation++;
                    _balance -= _purchaseFromUtility;

                    if (_generation == _demand) HandleStop();
                }
            } else if (message.Sender.Contains("house"))
            {
                if (message.Content.StartsWith("purchase"))
                {
                    _balance += _sellToNeighbour;
                    _generation--;

                    if (_demand == _generation) HandleStop();

                }
            }
        }

        public override void ActDefault()
        {
            Thread.Sleep(15);

            if (_state == State.SELL)
            {
                if (_generation == _demand)
                {
                    HandleStop();
                } else if (_generation > _demand)
                {
                    if (BrokerAgent.BuyingAgents.Count == 0)
                    {
                        _balance += _sellToUtility;
                        _generation--;

                        if (_generation == _demand) HandleStop();
                    }
                }
            } else if (_state == State.BUY && BrokerAgent.SellingAgents.Count > 0)
            {
                Send("broker", "search");
                return;
            }
        }

        public override string ToString()
        {
            return $"{Name} {_demand} {_generation} {_purchaseFromUtility} {_sellToUtility} {_state} {_sellToNeighbour} {_balance} {_sellable}";
        }

        public void HandleStop()
        {
            _state = State.SELL;
            Console.WriteLine($"{Name} has satisifed their energy requirements!");
            Send("broker", "unregister");
            Console.WriteLine(ToString());
            Stop();
        }
    }
}