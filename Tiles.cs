using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MahjongEngine
{
    public class TileVisibility
    {
        public bool IsKnown;
        public bool IsOpen;
        public Player AssignedPlayer;

        public TileVisibility(Player assignedPlayer)
        {
            IsKnown = true;
            AssignedPlayer = assignedPlayer;
            IsOpen = false;
        }

        public TileVisibility(bool isKnown, bool isOpen)
        {
            IsKnown = isKnown;
            IsOpen = isOpen;
            AssignedPlayer = null;
        }
    }

    public class Tiles : IEnumerable<Tile>
    {
        List<Tile> TileCollection = new List<Tile>();
        public TileVisibility visibility;

        public Tiles()
        {
            visibility = new TileVisibility(false, false);
        }

        public Tiles(Player player)
        {
            visibility = new TileVisibility(player);
        }
        
        public Tiles(bool isKnown, bool isOpen)
        {
            visibility = new TileVisibility(isKnown, isOpen);
        }

        public Tiles(Tiles other)
        {
            Add(other);
        }

        public void Clear()
        {
            TileCollection.Clear();
        }

        public void Add(Tile newTile)
        {
            TileCollection.Add(newTile);
        }

        public void Add(Tiles newTiles)
        {
            TileCollection.AddRange(newTiles);
        }

        public void Add(IEnumerable<Tile> newTiles)
        {
            TileCollection.AddRange(newTiles);
        }

        public Tiles TakeTiles(int count)
        {
            Tiles takenTiles = new Tiles();
            if(count >= 0)
            {
                takenTiles.Add(this.Take(count));
                TileCollection.RemoveRange(0, count);
            }
            else
            {
                count *= -1;
                takenTiles.Add(this.TakeLast(count));
                TileCollection.RemoveRange(TileCollection.Count() - count, count);
            }

            return takenTiles;
        }

        public void Sort()
        {
            TileCollection.Sort();
        }

        public void Shuffle()
        {
            Utils.Shuffle(TileCollection);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Suit currentSuit;
            
            if(TileCollection.Count == 0)
            {
                return String.Empty;
            }

            currentSuit = TileCollection[0].Suit;
            for(int i = 0; i < TileCollection.Count; i++)
            {
                bool outputSuit = true;
                if(i + 1 < TileCollection.Count &&
                   SuitExtensions.IsSameSuit(TileCollection[i].Suit, TileCollection[i + 1].Suit))
                {
                    outputSuit = false;
                }

                sb.Append(TileCollection[i].TileValueToString());
                if(outputSuit)
                {
                    sb.Append(SuitExtensions.ToShortenedString(TileCollection[i].Suit));
                }
            }

            return sb.ToString();
        }

        public IEnumerator<Tile> GetEnumerator()
        {
            return TileCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }


    public class DeadWall
    {
        public enum DeadWallFormat
        {
            Unknown,
            Verbose,
            Condensed
        }

        private int DeadWallSize;
        public Tiles Wall {get; set;}
        public Tiles PublicDoraIndicators { get;}
        private Tiles AllDoraIndicators;
        public Tiles RinShanTiles{ get; }
        public Tiles UraDoraTiles {get;}
        public DeadWall(int deadWallSize)
        {
            DeadWallSize = deadWallSize;
            Wall = new Tiles();
            PublicDoraIndicators = new Tiles();
            AllDoraIndicators = new Tiles();
            RinShanTiles = new Tiles();
            UraDoraTiles = new Tiles();
        }

        private void Clear()
        {
            Wall.Clear();
            PublicDoraIndicators.Clear();
            AllDoraIndicators.Clear();
            RinShanTiles.Clear();
            UraDoraTiles.Clear();
        }

        public void SetNewDeadWall(Tiles tiles)
        {
            Debug.Assert(tiles.Count() == DeadWallSize);
            Debug.Assert(DeadWallSize > 12);

            Clear();

            Wall.Add(tiles);

            for(int i = 0; i < 12; i++)
            {
                if(i < 4)
                {
                    RinShanTiles.Add(tiles.ElementAt(i));
                }
                else
                {
                    if(i % 2 == 0)
                    {
                        AllDoraIndicators.Add(tiles.ElementAt(i));
                    }
                    else
                    {
                        UraDoraTiles.Add(tiles.ElementAt(i));
                    }
                }
            }
        }

        public Tile TakeRinShanTile(Tiles LiveWall)
        {
            if(LiveWall.Count() == 0)
            {
                throw new InvalidOperationException("Cannot take a rinshan tile with no tiles left in wall");
            }
            Wall.Add(LiveWall.TakeTiles(-1));
            Wall.TakeTiles(1);
            return RinShanTiles.TakeTiles(1).First();
        }

        public Tile RevealDoraTile()
        {
            if(PublicDoraIndicators.Count() == AllDoraIndicators.Count())
            {
                throw new InvalidOperationException("Cannot reveal more dora tiles, please handle");
            }
            PublicDoraIndicators.Add(AllDoraIndicators.ElementAt(PublicDoraIndicators.Count()));

            return PublicDoraIndicators.Last();
        }

        public Tiles RevealUraDoraTiles()
        {
            Tiles UraDora = new Tiles(true, true);
            UraDora.Add(UraDoraTiles.Take(PublicDoraIndicators.Count()));

            return UraDora;
        }

        public int Count()
        {
            Debug.Assert(Wall.Count() == DeadWallSize);
            return Wall.Count();
        }

        public override string ToString()
        {
            return ToString(DeadWallFormat.Verbose);
        }

        public bool CheckDeadWallSanity()
        {
            return true;
        }

        public string ToString(DeadWallFormat format)
        {
            string str = "";
            switch(format)
            {
                case DeadWallFormat.Condensed:
                    str += $"Wall:{Wall.ToString()}\n";
                    break;
                case DeadWallFormat.Verbose:
                default:
                    str += $"Wall:{Wall.ToString()}\n";
                    str += $"RinShanTiles:{RinShanTiles.ToString()}\n";
                    str += $"PublicDoraIndicators:{PublicDoraIndicators.ToString()}\n";
                    str += $"AllDoraIndicators:{AllDoraIndicators.ToString()}\n";
                    str += $"UraDoraTiles:{UraDoraTiles.ToString()}\n";
                    break;
            }
            return str;
        }
    }
}