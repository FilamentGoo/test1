using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MahjongEngine
{
    public abstract class Viewer
    {
        public Game CurrentGame;
        //If CurrentPlayer is null, we render the entire gamestate
        public Player CurrentPlayer;
        public Viewer(Game game)
        {
            CurrentGame = game;
        }
        public abstract void Render();
    }

    public class ConsoleViewer : Viewer
    {
        public enum Direction
        {
            Bottom,
            Right,
            Top,
            Left
        }

        public ConsoleViewer(Game game) : base(game)
        {
        }

        private void RenderHand()
        {

        }

        private void RenderSeatWind()
        {
            
        }

        private void RenderWalls()
        {

        }

        private void RenderDiscards()
        {

        }

        private void RenderOpponents()
        {

        }

        private void RenderPlayerNameAndScore(Player Player)
        {
            Direction direction = GetDirection(Player.SeatingWind, GetOrientation().SeatingWind);
            string Id = Player.Id.Substring(0, Math.Min(12, Player.Id.Length));
            string Score = $"{Player.Score} {Player.SeatWind.ToString()}";
            if(Player.SeatWind == Wind.East)
            {
                Score += "(Dealer)";
            }

            switch(direction)
            {
                case Direction.Bottom:
                    Console.SetCursorPosition(Console.BufferWidth/2 - 6, Console.BufferHeight/2 + 10 - 2);
                    break;

                case Direction.Right:
                    Console.SetCursorPosition(Console.BufferWidth/2 + 25 - Math.Max(Id.Length, Score.Length), Console.BufferHeight/2 - 2);
                    break;

                case Direction.Top:
                    Console.SetCursorPosition(Console.BufferWidth/2 - 6, Console.BufferHeight/2 - 10 + 1);
                    break;

                case Direction.Left:
                    Console.SetCursorPosition(Console.BufferWidth/2 - 25 + 1, Console.BufferHeight/2 + 1);
                    break;
                default:
                    return;
            }

            Console.Write(Id);
            Console.CursorTop += 1;
            Console.CursorLeft -= Id.Length;
            Console.Write(Score);
        }

        private void RenderRound()
        {
            Console.SetCursorPosition(Console.BufferWidth/2 - 6, Console.BufferHeight/2);
            Console.Write($"{CurrentGame.CurrentRoundWind.ToString()} {CurrentGame.GetRoundNumber()}"); 
        }

        private void RenderTile(Direction orientation, int x, int y, Tile tile)
        {
            string tileString = "  ";

            if(tile != null)
            {
                tileString = tile.ToString();
                if(tileString.Length == 1)
                {
                    tileString += " ";
                }
            }

            switch(orientation)
            {
                case Direction.Bottom:
                    Console.SetCursorPosition(x, y);
                    Console.Write(" -- ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.Write($"|{tileString}|");
                    Console.SetCursorPosition(x, y + 2);
                    Console.Write("|  |");
                    Console.SetCursorPosition(x, y + 3);
                    Console.Write(" -- ");
                    return;
                case Direction.Top:
                    Console.SetCursorPosition(x, y);
                    Console.Write(" -- ");
                    Console.SetCursorPosition(x, y + 1 );
                    Console.Write("|  |");
                    Console.SetCursorPosition(x, y + 2);
                    Console.Write($"|{tileString}|");
                    Console.SetCursorPosition(x, y + 3);
                    Console.Write(" -- ");
                    return;
                case Direction.Right:
                    Console.SetCursorPosition(x, y);
                    Console.Write(" ---- ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.Write($"|{tileString}  |");
                    Console.SetCursorPosition(x, y + 2);
                    Console.Write(" ---- ");
                    return;
                case Direction Left:
                    Console.SetCursorPosition(x, y);
                    Console.Write(" ---- ");
                    Console.SetCursorPosition(x, y + 1);
                    Console.Write($"|  {tileString}|");
                    Console.SetCursorPosition(x, y + 2);
                    Console.Write(" ---- ");
                    return;
            }   
        }

        private void RenderPlayerHand(Player player)
        {
            Direction handDirection = GetDirection(player.SeatingWind, GetOrientation().SeatingWind);
            bool hasVisibility = !(CurrentPlayer != null && player != CurrentPlayer);
            int x;
            int y;

            if(handDirection == Direction.Top ||
               handDirection == Direction.Bottom)
            {
                y = handDirection == Direction.Top ? 0 : (Console.BufferHeight - 10);
                x = Console.BufferWidth / 2 - 25;

                foreach(Tile tile in player.Hand.ClosedHand)
                {
                    Tile tempTile = !hasVisibility ? null : tile;

                    RenderTile(handDirection, x, y, tempTile);
                    x += 4;
                }
            }
            else
            {
                x = handDirection == Direction.Left ? 0 : (Console.BufferWidth - 6);
                y = Console.BufferHeight / 4;

                foreach(Tile tile in player.Hand.ClosedHand)
                {
                    Tile tempTile = !hasVisibility ? null : tile;

                    RenderTile(handDirection, x, y, tempTile);
                    y += 3;
                }
            }
        }

        private Direction GetDirection(Wind Current, Wind Reference)
        {
            int diff = ((int)Current - (int) Reference + 4) % 4;
            switch(diff)
            {
                case 0:
                    return Direction.Bottom;
                case 1:
                    return Direction.Right;
                case 2:
                    return Direction.Top;
                case 3: 
                    return Direction.Left;
                default:
                    throw new InvalidOperationException($"diff of {diff} reported. Current {Current.ToString()} Reference:{Reference.ToString()}");
            }

        }

        //Returns the player that is supposed to be sitting at the bottom of the screen
        private Player GetOrientation()
        {
            if(CurrentPlayer != null)
            {
                return CurrentPlayer;
            }
            else
            {
                Debug.Assert(CurrentGame.Players.Where(x => x.SeatingWind == Wind.East).First() != null);
                return CurrentGame.Players.Where(x => x.SeatingWind == Wind.East).First();
            }
        }

        

        public override void Render()
        {
            Console.Clear();
            Console.BufferHeight = Console.LargestWindowHeight;
            Console.BufferWidth = Console.LargestWindowWidth;
            Console.WindowTop = 0;
            Console.WindowLeft = 0;
            Console.WindowHeight = Console.LargestWindowHeight;
            Console.WindowWidth = Console.LargestWindowWidth;

            Console.SetCursorPosition(Console.BufferWidth/2 - 25 , Console.BufferHeight/2 - 10);
            Console.Write(new String('-', 51));

            Console.SetCursorPosition(Console.BufferWidth/2 - 25 , Console.BufferHeight/2 + 10);
            Console.Write(new String('-' ,51));

            for(int i = -1; i < 2; i+= 2)
            {
                for(int j = Console.BufferHeight/2 - 10 + 1; 
                    j <= Console.BufferHeight/2 + 10 - 1;
                    j++)
                {
                    Console.SetCursorPosition(Console.BufferWidth/2 + i*25, j);
                    Console.Write("|");
                }
            }

            foreach(Player player in CurrentGame.Players)
            {
                RenderPlayerNameAndScore(player);
            }

            foreach(Player player in CurrentGame.Players)
            {
                RenderPlayerHand(player);
            }


            RenderRound();

            Console.SetCursorPosition(0, Console.BufferHeight - 1);

/*
            RenderWalls();
            RenderDiscards();
            RenderMessage();
            RenderError();*/
        }

        private void TestDirection()
        {            
            
            for(Wind i = Wind.East; 
                i < Wind.Max;
                i++)
            {
                for(Wind j = Wind.East;
                j < Wind.Max;
                j++)
                {
                    Console.WriteLine($"Reference:{i} Current:{j} Direction:{GetDirection(j, i).ToString()}");
                }
            }}


    }
}