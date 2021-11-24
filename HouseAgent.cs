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
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            Console.WriteLine(message.Format());

            var msg = message.Content.Split(' ');
            switch (message.Sender)
            {
                case "environment":
                {
                    if (message.Content.StartsWith("inform"))
                    {
                        _demand = int.Parse(msg[1]);
                        _generation = int.Parse(msg[2]);
                        _initialPower = _generation;
                        _initialDemand = _demand;
                        _purchaseFromUtility = int.Parse(msg[3]);
                        _sellToUtility = int.Parse(msg[4]);
                        _state = _demand > _generation ? State.BUY : State.SELL;
                        _sellable = _generation - _demand;
                        Console.WriteLine(ToString());

                        if (_state == State.SELL)
                        {
                            if (_demand != _generation)
                                Send("broker", $"register {ToString()}");
                            else HandleStop();
                        }
                        else
                        {
                            Send("broker", "buying");
                        }
                    }

                    break;
                }
                case "broker" when message.Content.StartsWith("seller"):
                {
                    Send(msg[1], "purchase");
                    _balance -= int.Parse(msg[2]);
                    _generation++;

                    if (_generation == _demand) HandleStop();
                    break;
                }
                case "broker":
                {
                    if (message.Content.Contains("utility"))
                    {
                        _generation++;
                        _balance -= _purchaseFromUtility;

                        if (_generation == _demand) HandleStop();
                    }

                    break;
                }
                default:
                {
                    if (message.Sender.Contains("house"))
                        if (message.Content.StartsWith("purchase"))
                        {
                            _balance += _sellToNeighbour;
                            _generation--;

                            if (_demand == _generation) HandleStop();
                        }

                    break;
                }
            }
        }

        public override void ActDefault()
        {
            if (!_initialised)
            {
                Thread.Sleep(250);
                _initialised = !_initialised;
            }

            if (_initialised)
            {
                if (_state == State.SELL && _generation == _demand)
                {
                    HandleStop();
                }
                else if (_state == State.SELL)
                {
                    if (_generation > _demand)
                    {
                        if (BrokerAgent.BuyingAgents.Count == 0)
                        {
                            _balance += _sellToUtility;
                            _generation--;

                            if (_generation == _demand) HandleStop();
                        }
                    }
                    else if (_generation == _demand) HandleStop();
                }
                else if (_state == State.BUY)
                {
                    Send("broker", "search");
                }
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