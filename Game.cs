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
        public int StartingHandSize = 13;
        public int DeadWallSize = 14;
        public GameProperties()
        {}
    }

    public class Game
    {
        public Tiles TilePool;
        protected int NumPlayers = 4;
        public List<Player> Players;
        protected List<string> PlayerNames;
        protected RuleSet RuleSet;
        public GameProperties Properties;
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
            Players.ForEach(x => Console.WriteLine($"{x.Id}: {x.SeatingWind.ToString()}, {x.SeatWind.ToString()}. {x.NextPlayer.Id}")); 
        }

        public virtual void Execute()
        {
            Round round = GenerateRound();
            round.Initialize();
            round.Execute();
        }

        public virtual Round GenerateRound()
        {
            Round round = new Round(this);
            FourPlayerGame fourplayergame = new FourPlayerGame(round, this);
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

            foreach(string name in PlayerNames)
            {
                Players.Add(new Player(name));
            }

            Utils.Shuffle(Players);

            Wind currentWind = Wind.East;
            for(int iterations = 0; iterations < PlayerNames.Count; iterations++)
            {
                Players[iterations].SetSeatingWind(currentWind);
                Players[iterations].NextPlayer = Players[(iterations + 1) % Players.Count()];
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

    public class Round
    {
        private enum RoundPhase
        {
            Unknown,
            PlayerDraw,
            PlayerRinshan,
            PlayerSelfKan,
            PlayerDiscard,
            PlayerCall,
            RoundEnd
        }
        public Game Game;
        public GameProperties GameProperties;
        public Player CurrentDiscard;
        public Tiles Wall;
        private RoundResult Result; 
        public Round(Game game)
        {
            Game = game;
            GameProperties = Game.Properties;
            Wall = new Tiles();
        }

        public void Initialize()
        {
            Reset();
            DistributeTiles();
            Debug.Assert(Wall != null && Wall.Count() != 0);
        }

        public RoundResult Execute()
        {
            Result = RoundResult.Unknown;
            RoundPhase phase = RoundPhase.PlayerDraw;
            CurrentDiscard = Game.Players.Where(x => x.IsDealer).FirstOrDefault();
            Debug.Assert(CurrentDiscard != null);

            while(Result == RoundResult.Unknown)
            {
                phase = ExecuteInternal(phase);
            }

            return Result;
        }

        private RoundPhase ExecuteInternal(RoundPhase phase)
        {
            RoundPhase nextPhase = phase;
            switch(phase)
            {
                case RoundPhase.PlayerDraw:
                    nextPhase = RoundPhase.PlayerDiscard;
                    CurrentDiscard.Hand.Draw(Wall.Take(1).FirstOrDefault());
                    break;
                case RoundPhase.PlayerDiscard:
                    break;
                case RoundPhase.PlayerSelfKan:
                    break;
                case RoundPhase.PlayerRinshan:
                    break;
                case RoundPhase.PlayerCall:
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
            return nextPhase;
        }

        public virtual void Reset()
        {}
        public virtual void DistributeTiles()
        {

        }

        public Tile PlayerDiscardDecision(Tile newTile)
        {
            return null;
        }

        public Tile PlayerCallDiscardDecision()
        {
            return null;
        }

        public void PlayerCallDecision()
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

    public class FourPlayerGame : RoundDecorator
    {
        DeadWall DeadWall;
        public FourPlayerGame(Round round, Game game) : base(round, game)
        {
            DeadWall = new DeadWall(GameProperties.DeadWallSize);
        }

        public override void Reset()
        {
            DeadWall = new DeadWall(GameProperties.DeadWallSize);
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

            DeadWall.SetNewDeadWall(pool.TakeTiles(GameProperties.DeadWallSize));

            foreach(Player player in Game.Players)
            {
                player.Hand.AddClosedTiles(pool.TakeTiles(GameProperties.StartingHandSize));
                Console.WriteLine($"Player:{player.Id}: {player.Hand.ClosedHand.ToString()}");
            }

            Console.WriteLine($"Wall({pool.Count()}):{pool.ToString()}");
            Console.WriteLine($"DeadWall({DeadWall.Count()}):{DeadWall.ToString()}");
            Wall = pool;
        }
    }
}