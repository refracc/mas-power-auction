using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ActressMas;

namespace Coursework
{
    public class HouseAgent : Agent
    {
        private int _demand = 0;
        private int _generation = 0;
        private int _purchaseFromUtility = 0;
        private int _sellToUtility = 0;
        private int _sellToNeighbour = new Random().Next(1, 3);
        private int _balance = new Random().Next(44, 200);
        private int _sellable = 0;
        private State _state = 0;
        private Dictionary<string, int> _prices = new();

        public override void Setup()
        {
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            string[] msg = message.Content.Split(' ');
            if (message.Sender == "environment")
            {
                if (message.Content.StartsWith("inform"))
                {
                    _demand = int.Parse(msg[1]);
                    _generation = int.Parse(msg[2]);
                    _purchaseFromUtility = int.Parse(msg[3]);
                    _sellToUtility = int.Parse(msg[4]);
                    _state = (_demand > _generation) ? State.BUY : (_demand == _generation) ? State.USE : State.SELL;
                    _sellable = (_generation > _demand) ? (_generation - _demand) : 0;
                }
            } else if (message.Sender.Contains("house"))
            {
                switch (_state)
                {
                    case State.SELL:
                        if (message.Content.StartsWith("request"))
                        {
                            Console.WriteLine($"{message.Format()}");
                            int requested = int.Parse(msg[1]);

                            if (requested <= _sellable)
                            {
                                Send(message.Sender, $"power {requested} {_sellToNeighbour}");

                                _sellable -= requested;
                            }
                            else if (requested > _sellable)
                            {
                                do
                                {
                                     requested /= 2;
                                } while (requested > _sellable);

                                Send(message.Sender, $"power {requested} {_sellToNeighbour}");
                                _sellable -= requested;
                            }
                        } else if (message.Content.StartsWith("pay"))
                        {
                            Console.WriteLine($"{message.Format()}");
                            int pay = int.Parse(msg[1]);
                            _balance += pay;
                        }
                        break;
                    case State.BUY:
                        if (message.Content.StartsWith("power"))
                        {
                            Console.WriteLine($"{message.Format()}");
                            int requested = int.Parse(msg[1]);
                            int pay = int.Parse(msg[2]);

                            _prices.Add(message.Sender, pay);

                            int lowest = int.MaxValue;
                            string name = "";
                            foreach (KeyValuePair<string, int> pair in _prices)
                            {
                                if (pair.Value < lowest)
                                {
                                    lowest = pair.Value;
                                    name = pair.Key;
                                }
                            }
                            _prices.Clear();
                            _generation += requested;
                            Send(name, $"pay {lowest}");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        public override void ActDefault()
        {
            _generation--;
            if((_demand > _generation) && _state == State.BUY)
            {
                Broadcast($"request {Math.Abs(_demand - _generation)}");
            }
            Thread.Sleep(25);
        }

        public override string ToString()
        {
            return $"{Name} {_demand} {_generation} {_purchaseFromUtility} {_sellToUtility} {_state} {_sellToNeighbour} {_balance} {_sellable}";
        }
    }
}


/*
} else if (message.Content.StartsWith("pay"))
{
    int pay = int.Parse(msg[1]);
                    
}
else if (message.Content.StartsWith("power") && _state == State.BUY)
{
    int requested = int.Parse(msg[1]);
    int pay = int.Parse(msg[2]);

    if (_balance >= pay)
    {
        _generation += requested;
        Send(message.Sender, $"pay {pay}");
    }
}
*/