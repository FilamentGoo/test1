using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MahjongEngine
{
    public enum Suit 
    {
        Unknown,
        Manzu,
        Pinzu,
        Souzu,
        Jihai,
        Max = Jihai,
    }

    public static class SuitExtensions
    {
        public static string ToShortenedString(Suit suit)
        {
            switch(suit)
            {
                case Suit.Manzu:
                    return "m";
                case Suit.Pinzu:
                    return "p";
                case Suit.Souzu:
                    return "s";
                case Suit.Jihai:
                    return "z";
                    default:
                throw new InvalidProgramException($"Invalid suit to be shortened {suit.ToString()}");
            }
        }
    }

    public enum Honor
    {
        Unknown,
        East,
        South,
        West,
        North,
        WindStart = East,
        WindEnd = North,
        White,
        Green,
        Red,
        DragonStart = White,
        DragonEnd = Red,
        Max = Red,
    }

    public enum TileDisplayStyle 
    {
        Tenhou,
        English,
        Japanese
    }

    public class Tile : IEquatable<Tile>, IComparable<Tile>
    {
        public static readonly int TileMinValue = 1;
        public static readonly int TileMaxValue = 9;
        public readonly Suit Suit;
        public readonly int TileValue;
        public readonly bool IsRedDora;
        public bool IsDora { get; set; }
        public static TileDisplayStyle DisplayStyle = TileDisplayStyle.English;

        public bool IsTerminal 
        { 
            get 
            {
                if(this.Suit != Suit.Jihai)
                {
                    return this.TileValue == TileMinValue ||
                           this.TileValue == TileMaxValue;
                }
                return false;
            }
        }
        public bool IsHonorOrTerminal
        {
            get
            {
                return this.Suit == Suit.Jihai;
            } 
        }

        protected bool IsValid()
        {
            if(Suit > Suit.Max ||
               Suit <= Suit.Unknown)
            {
                return false;
            }

            if(Suit == Suit.Jihai)
            {
                Honor honorValue = (Honor)TileValue;
                if(honorValue > Honor.Max ||
                   honorValue <= Honor.Unknown)
                {
                    return false;
                }
            }
            else
            {
                if(TileValue > TileMaxValue ||
                   TileValue < TileMinValue)
                {
                    return false;
                }
            }
            return true;
        }

        public Tile(Suit suit, int tileValue, bool isRedDora = false)
        {
            this.Suit      = suit;
            this.TileValue = tileValue;
            this.IsRedDora = isRedDora;

            if(!IsValid())
            {
                throw new ArgumentException($"Invalid parameters passed to tile creation mechanism Suit:{Suit.ToString()} Value:{tileValue.ToString()}");
            }
        }

        public bool Equals(Tile other)
        {
            return this.Suit == other.Suit &&
                   this.TileValue == other.TileValue;
        }

        public int CompareTo(Tile other)
        {
            //Suits take precedence over tile value, which takes precedence over red dora
            return (int)(this.Suit - other.Suit) * 100 +
                   (int)(this.TileValue - other.TileValue) *10 +
                   (Convert.ToInt32(this.IsRedDora) - Convert.ToInt32(other.IsRedDora));
        }

        public override string ToString()
        {
            return $"{TileValueToString()}{SuitExtensions.ToShortenedString(Suit)}";
        }

        public string TileValueToString()
        {
            if(Suit != Suit.Jihai ||
                DisplayStyle == TileDisplayStyle.Tenhou)
            {
                return TileValue.ToString();
            }
            else
            {
                Honor honorValue = (Honor)TileValue;
                switch(honorValue)
                {
                    case Honor.East:
                        return "E";
                    case Honor.South:
                        return "S";
                    case Honor.West:
                        return "W";
                    case Honor.North:
                        return "N";
                    case Honor.Green:
                        return "G";
                    case Honor.Red:
                        return "R";
                    case Honor.White:
                        return "W";
                    default:
                        throw new InvalidOperationException($"Tried to print invalid honor value {this.ToString()}");
                }
            }
        }
    }


}