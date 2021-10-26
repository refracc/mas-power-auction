using System;
using System.Collections.Generic;
using System.Text;
using ActressMas;

namespace Coursework
{
    class HouseAgent : Agent
    {
        private int _demand = 0;
        private int _generation = 0;
        private int _purchaseFromUtility = 0;
        private int _sellToUtility = 0;
        private int _sellToNeighbour = new Random().Next(1, 3);
        private int _balance = new Random().Next(44, 200);
        private int _sellable = 0;
        private State _state;

        public override void Setup()
        {
            Send("environment", "start");
        }

        public override void Act(Message message)
        {
            if (message.Sender == "environment")
            {
                if (message.Content.StartsWith("inform"))
                {
                    string[] msg = message.Content.Split(' ');

                    _demand = int.Parse(msg[1]);
                    _generation = int.Parse(msg[2]);
                    _purchaseFromUtility = int.Parse(msg[3]);
                    _sellToUtility = int.Parse(msg[4]);
                    _state = (_demand > _generation) ? State.BUY : (_demand == _generation) ? State.USE : State.SELL;
                    _sellable = (_generation > _demand) ? (_generation - _demand) : 0;
                    //Console.WriteLine($"{Name} {_demand} {_generation} {_purchaseFromUtility} {_sellToUtility} {_state} {_sellToNeighbour} {_balance} {_sellable}");
                }
            } else if (message.Sender.Contains("house"))
            {
                if (message.Content.StartsWith("request") && _state == State.SELL)
                {
                    string[] msg = message.Content.Split(' ');
                    int requested = int.Parse(msg[1]);
                    
                    if (requested < _sellable) 
                    {
                        Send(message.Sender, $"power {requested} {_sellToNeighbour}");
                        _sellable -= requested;
                    } else if (requested == _sellable)
                    {
                        Send(message.Sender, $"power {(requested - 1)} {_sellToNeighbour}");
                        _sellable -= (requested - 1);
                    } else if (requested > _sellable)
                    {
                        do
                        {
                            requested /= 2;
                        } while (requested > _sellable);

                    }
                } 
                else if (message.Content.StartsWith("power") && _state == State.BUY)
                {
                }
            }
        }
    }
}
