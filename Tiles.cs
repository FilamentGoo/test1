using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MahjongEngine
{
    public class Tiles : IEnumerable<Tile>
    {
        List<Tile> TileCollection = new List<Tile>();

        public Tiles()
        {
        }

        public Tiles(Tiles other)
        {
            Add(other);
        }

        public void Add(Tile newTile)
        {
            TileCollection.Add(newTile);
            TileCollection.Sort();
        }

        public void Add(Tiles newTiles)
        {
            TileCollection.AddRange(newTiles);
            TileCollection.Sort();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Suit currentSuit = TileCollection[0].Suit;
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
}