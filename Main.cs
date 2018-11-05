using System;

namespace MahjongEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.Initialize();
            Console.ReadLine();
        }
    }
}
