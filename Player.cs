using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongEngine
{
    public delegate PlayerDiscardDecision ExecutePlayerDiscardDecision(Player player);
    public delegate CallDecisionResult ExecutePlayerCallDecision(Player player, Tile discardedTile);

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

        public override string ToString()
        {
            string str = "";
            str += "Closed:" + ClosedHand.ToString();
            str += "\tOpen:" + String.Join(",", OpenHand.Select(x => x.ToString()));

            return str;
        }
    }


    public enum DiscardDecisionResult
    {
        Unknown,
        Discard,
        Tsumo,
        OpenKan,
        ClosedKan
    }
    public struct PlayerDiscardDecision
    {
        public Tile Discard;
        public DiscardDecisionResult Decision;
    }

    public enum CallDecisionResult
    {
        Unknown,
        NoAction,
        Chii,
        Pon,
        Kan,
        Ron
    }

    public class Player : IEquatable<Player>
    {
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
        public Game Game{get; set;}

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

        public PlayerDiscardDecision DiscardOrCall()
        {
            //TODO: validate that the decision is valid.
            return Game.Callbacks.DiscardDecisionCallback(this);
        }
    }
}