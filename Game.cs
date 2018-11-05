using System;
using System.Collections.Generic;

namespace MahjongEngine
{
    public class Game
    {
        private Tiles TilePool;

        public Game()
        {
            TilePool = new Tiles();
        }

        public virtual void Initialize()
        {
            GenerateTiles();
            Console.WriteLine(TilePool.ToString());
        }

        public virtual void GenerateTiles()
        {
            for(int iterations = 0; iterations < 4; iterations++)
            {
                for(Suit suit = Suit.Unknown + 1; suit <= Suit.Max; suit++)
                {
                    for(int tileValue = Tile.TileMinValue; tileValue <= Tile.TileMaxValue; tileValue ++)
                    {
                        if(suit == Suit.Jihai && tileValue > (int)Honor.Max)
                        {
                            break;
                        }

                        Tile newTile = new Tile(suit, tileValue, false);
                        TilePool.Add(newTile);
                    }
                }
            }
        }
    }
}