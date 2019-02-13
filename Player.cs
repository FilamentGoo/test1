using System;
using System.Collections.Generic;

namespace MahjongEngine
{
    public class Hand
    {
        public Tiles ClosedHand;
        public List<Tiles> OpenHand;
        //For sanma, for instance
        public Tiles ExtraTiles;
        public Hand()
        {
            ClosedHand = new Tiles();
            OpenHand = new List<Tiles>();
            ExtraTiles = new Tiles();
        }

        public void AddClosedTiles(Tiles newTiles)
        {
            ClosedHand.Add(newTiles);
            ClosedHand.Sort();
        }
    }

    public class Player : IEquatable<Player>
    {
        public string Id {get; protected set;}
        // this determines player order.
        public Wind SeatingWind { get; protected set;}
        public Hand Hand {get; private set;}
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

        public void InitializeHand()
        {
            Hand = new Hand();
        }
    }
}