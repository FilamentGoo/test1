using System;
using System.Collections.Generic;

namespace MahjongEngine
{
    public class Hand
    {

    }

    public class Player : IEquatable<Player>
    {
        public string Id {get; protected set;}
        // this determines player order.
        public Wind SeatingWind{ get; protected set;}
        public Hand Hand {get;}
        public int Score { get; set;}
        public bool IsDealer 
        { 
            get 
            {
                return SeatWind == Wind.East;
            }
        }
        public Wind SeatWind{ get; set;}

        public Player(string id, Wind seatingWind)
        {
            Id = id;
            SeatingWind = seatingWind;
            SeatWind = seatingWind;
        }

        public bool Equals(Player other)
        {
            return Id == other.Id;
        }
    }
}