using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongEngine
{
    public class Controller
    {
        public Game Game;
        public Viewer Viewer;

        public Controller()
        {
            StartGame();
        }

        public void StartGame()
        {
            DecisionCallbacks callbacks = new DecisionCallbacks();
            RuleSet ruleset; 
            
            callbacks.CallDecisionCallback    = GetPlayerCallDecision;
            callbacks.DiscardDecisionCallback = GetPlayerDiscardDecision;
            callbacks.RenderCallback          = RequestRender;
            ruleset = new RuleSet();
            Game    = new Game(ruleset, callbacks);
            Viewer  = new ConsoleViewer(Game);
 
            Game.Initialize();
            Viewer.CurrentPlayer = Game.Players.First();
            Game.Execute();
        }

        public PlayerDiscardDecision GetPlayerDiscardDecision(Player player)
        {
            PlayerDiscardDecision decision;
            Console.ReadLine();

            decision.Decision = DiscardDecisionResult.Discard;
            decision.Discard  = Utils.GetRandom(player.Hand.ClosedHand);

            return decision;
        }

        public CallDecisionResult GetPlayerCallDecision(Player player, Tile discardedTile)
        {
            return CallDecisionResult.NoAction;
        }

        public void RequestRender()
        {
            Viewer.Render();
        }
    }

}