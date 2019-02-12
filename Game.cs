using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

    public class GameProperties
    {
        public bool HasScore = false;
        public int InitialScorePerPlayer = 25000;
        public bool HasRoundWind = true;
        public Wind MaxRound = Wind.West;
        public int NumDuplicatesPerTile = 4;
        public int DeadWallSize = 14;
        public GameProperties()
        {}
    }

    public class Game
    {
        public Tiles TilePool;
        protected int NumPlayers;
        public List<Player> Players;
        protected List<string> PlayerNames;
        protected RuleSet RuleSet;
        protected GameProperties Properties;
        protected Wind CurrentRoundWind;
        public Game(RuleSet ruleSet, List<string> playerNames = null)
        {
            TilePool = new Tiles();
            Players  = new List<Player>();
            Properties = new GameProperties();
            RuleSet  = ruleSet;
            PlayerNames = playerNames;
        }

        public virtual void Initialize()
        {
            Tile.DisplayStyle = TileDisplayStyle.Tenhou;
            GenerateTiles();
            GeneratePlayers();
            Console.WriteLine(TilePool.ToString());
            Players.ForEach(x => Console.WriteLine($"{x.Id}: {x.SeatingWind.ToString()}, {x.SeatWind.ToString()}")); 
        }

        public virtual void Execute()
        {
            Round round = GenerateRound();
            round.Execute();
        }

        public virtual Round GenerateRound()
        {
            Round round = new Round(this);
            ScoredGame scoredgame = new ScoredGame(round, this);
            FourPlayerGame fourplayergame = new FourPlayerGame(scoredgame, this);
            return fourplayergame;
        }

        public virtual void GenerateTiles()
        {
            for(int iterations = 0; iterations < Properties.NumDuplicatesPerTile; iterations++)
            {
                for(Suit suit = Suit.Unknown + 1; suit <= Suit.Max; suit++)
                {
                    for(int tileValue = SuitExtensions.GetMinValue(suit);
                        tileValue <= SuitExtensions.GetMaxValue(suit);
                        tileValue ++)
                    {
                        Tile newTile = new Tile(suit, tileValue, false);
                        TilePool.Add(newTile);
                    }
                }
            }

            TilePool.Sort();
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

    public enum RoundResult
    {
        Unknown,
        DealerWin,
        DealerLoss,
        AbortiveDraw
    }

    public abstract class Round
    {
        public Game Game;
        public GameProperties GameProperties;
        public Player CurrentDiscard;
        public Tiles Wall;

        public Round(Game game)
        {
            Game = game;
            Wall = new Tiles();
        }

        public RoundResult Execute()
        {
            DistributeTiles();
            Debug.Assert(Wall != null && Wall.Count() != 0);
            
            return RoundResult.Unknown;
        }

        public abstract void Reset();
        public abstract void DistributeTiles();

        public virtual void PlayerDiscardDecision()
        {

        }

        public virtual void PlayerCallDecision()
        {

        }

        public virtual void ResolveDiscard()
        {

        }

        public virtual void CalculateScores()
        {

        }
    }

    public abstract class RoundDecorator : Round
    {
        public Round BaseRound = null;

        protected RoundDecorator(Round round, Game game) : base(game)
        {
            BaseRound = round;
        }
    }

    public abstract class TwoPlayerGame : RoundDecorator
    {
        public TwoPlayerGame(Round round, Game game) : base(round, game)
        {

        }

        public override void DistributeTiles()
        {

        }
    }

    public abstract class FourPlayerGame : RoundDecorator
    {
        Tiles DeadWall;
        public FourPlayerGame(Round round, Game game) : base(round, game)
        {

        }

        public override void Reset()
        {
            DeadWall = new Tiles();
            BaseRound.Reset();
        }

        public override void DistributeTiles()
        {
            foreach(Player player in Game.Players)
            {
                player.InitializeHand();
            }

            Tiles pool = new Tiles();
            pool.Add(Game.TilePool);

            pool.Shuffle();

            DeadWall = pool.TakeTiles(GameProperties.DeadWallSize);
        }
    }
}