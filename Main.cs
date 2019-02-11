using System;

namespace MahjongEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            RuleSet ruleset = new RuleSet();
            Game game = new Game(ruleset);
            game.Initialize();
            game.Execute();
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
