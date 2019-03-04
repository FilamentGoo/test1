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
        public Tile CurrentTile;
        public Hand(Player player)
        {
            ClosedHand = new Tiles(player);
            OpenHand = new List<Tiles>();
            ExtraTiles = new Tiles(player);
        }

        public void AddClosedTiles(Tiles newTiles)
        {
            ClosedHand.Add(newTiles);
            ClosedHand.Sort();
        }

        public void Draw(Tile newTile)
        {
            CurrentTile = newTile;
        }

        public Tile Discard(Tile discardTile, bool Tsumogiri)
        {
            return null;
        }
    }

    public class Player : IEquatable<Player>
    {
        public enum DiscardDecision
        {
            Discard,
            Tsumo,
            OpenKan,
            ClosedKan
        }
        public struct PlayerDiscardDecision
        {
            public Tile Discard;
            public DiscardDecision Decision;
        }
        
        public string Id {get; protected set;}
        // this determines player order.
        public Wind SeatingWind { get; set;}
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
        public Player NextPlayer {get;set;}

        public Player(string id)
        {
            Id = id;
        }

        public void SetSeatingWind(Wind wind)
        {
            SeatingWind = wind;
            SeatWind = wind;
        }

        public bool Equals(Player other)
        {
            return Id == other.Id;
        }

        public void InitializeHand()
        {
            Hand = new Hand(this);
        }
    }
}