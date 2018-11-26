using System;

namespace MahjongEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            RuleSet ruleset = new RuleSet();
            FourPlayerGame game = new FourPlayerGame(ruleset);
            game.Initialize();
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
