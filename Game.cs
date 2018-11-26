using System;
using System.Collections.Generic;

namespace MahjongEngine
{
    public class RuleSet
    {
        public bool AkaDora = true;
        public bool AgariYame = true;
        public bool Shanyu = true;


        public RuleSet()
        {}
    }
    public abstract class Game
    {
        private Tiles TilePool;
        protected int NumDuplicatesPerTile;
        protected int NumPlayers;
        protected List<Player> Players;
        protected List<string> PlayerNames;
        protected RuleSet RuleSet;
        public Game(RuleSet ruleSet, List<string> playerNames = null)
        {
            TilePool = new Tiles();
            Players  = new List<Player>();
            RuleSet  = ruleSet;
            PlayerNames = playerNames;
        }

        public virtual void Initialize()
        {
            GenerateTiles();
            GeneratePlayers();
        }

        public virtual void GenerateTiles()
        {
            for(int iterations = 0; iterations < NumDuplicatesPerTile; iterations++)
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

        public virtual void GeneratePlayers()
        {
            if(PlayerNames == null)
            {
                PlayerNames = new List<string>();
            }

            if(PlayerNames.Count > NumPlayers)
            {
                throw new InvalidOperationException($"Player Names greater than player count.  Expected:{NumPlayers} Actual:{PlayerNames.Count}:{String.Join(",", PlayerNames)}");
            }
            else if(PlayerNames.Count < NumPlayers)
            {
                int playersToCreate = NumPlayers - PlayerNames.Count;
                for(int iterations = 0; iterations < playersToCreate; iterations++)
                {
                    PlayerNames.Add(iterations.ToString());
                }
            }

            Utils.Shuffle(PlayerNames);

            Wind currentWind = Wind.East;
            for(int iterations = 0; iterations < PlayerNames.Count; iterations++)
            {
                Players.Add(new Player(PlayerNames[iterations], currentWind));
                currentWind++;
            }
        }

    }

    public abstract class ScoredGame : Game
    {
        public ScoredGame(RuleSet ruleSet, List<string> playerNames = null) :
            base(ruleSet, playerNames)
        {
            NumDuplicatesPerTile = 4;
        }
    }

    public class FourPlayerGame : ScoredGame
    {
        public FourPlayerGame(RuleSet ruleSet, List<string> playerNames = null) : 
            base(ruleSet, playerNames)
        {
            NumPlayers = 4;
        }
    }
}