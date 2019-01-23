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
        Kazehai,
        Sangenpai,
        Max = Sangenpai,
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
                case Suit.Sangenpai:
                case Suit.Kazehai:
                    return "z";
                default:
                    throw new InvalidProgramException($"Invalid suit to be shortened {suit.ToString()}");
            }
        }

        public static int GetMaxValue(Suit suit)
        {
            switch(suit)
            {
                case Suit.Manzu:
                case Suit.Pinzu:
                case Suit.Souzu:
                    return 9;
                case Suit.Sangenpai:
                    return (int)Dragon.Red;
                case Suit.Kazehai:
                    return (int)Wind.North;
                default:
                    throw new InvalidProgramException($"Invalid suit to retrieve max value {suit.ToString()}");
            }
        }

        public static int GetMinValue(Suit suit)
        {
            switch(suit)
            {
                case Suit.Manzu:
                case Suit.Pinzu:
                case Suit.Souzu:
                    return 1;
                case Suit.Sangenpai:
                    return (int)Dragon.White;
                case Suit.Kazehai:
                    return (int)Wind.East;
                default:
                    throw new InvalidProgramException($"Invalid suit to retrieve max value {suit.ToString()}");
            }
        }

        public static bool IsSameSuit(Suit suit1, Suit suit2)
        {
            if(IsJihai(suit1) && IsJihai(suit2))
            {
                return true;
            }
            else
            {
                return suit1 == suit2;
            }
        }
 
        public static bool IsJihai(Suit suit)
        {
            switch(suit)
            {
                case Suit.Manzu:
                case Suit.Pinzu:
                case Suit.Souzu:
                    return false;
                case Suit.Sangenpai:
                case Suit.Kazehai:
                    return true;
                default:
                    throw new InvalidProgramException($"Invalid suit to determine if jihai {suit.ToString()}");
            }
        }

    }

    public enum Wind
    {
        Unknown,
        East,
        South,
        West,
        North,
        Max = North
    }

    public enum Dragon
    {
        Unknown,
        White,
        Green,
        Red,
        Max = Red
    }

    public enum TileDisplayStyle 
    {
        Tenhou,
        English,
        Japanese
    }

    public class Tile : IEquatable<Tile>, IComparable<Tile>
    {
        public readonly Suit Suit;
        public readonly int TileValue;
        public readonly bool IsRedDora;
        public bool IsDora { get; set; }
        public static TileDisplayStyle DisplayStyle = TileDisplayStyle.English;

        public bool IsHonor
        {
            get
            {
                return SuitExtensions.IsJihai(Suit);
            }
        }
        public bool IsTerminal 
        { 
            get 
            {
                if(!this.IsHonor)
                {
                    return this.TileValue == SuitExtensions.GetMinValue(Suit) ||
                           this.TileValue == SuitExtensions.GetMaxValue(Suit);
                }
                return false;
            }
        }

        protected bool IsValid()
        {
            if(Suit > Suit.Max ||
               Suit <= Suit.Unknown)
            {
                return false;
            }

            if(Suit == Suit.Sangenpai)
            {
                Dragon dragonValue = (Dragon)TileValue;
                if(dragonValue > Dragon.Max ||
                   dragonValue <= Dragon.Unknown)
                {
                    return false;
                }
            }
            else if(Suit == Suit.Kazehai)
            {
                Wind windValue = (Wind)TileValue;
                if(windValue > Wind.Max ||
                   windValue <= Wind.Unknown)
                {
                    return false;
                }
            }
            else
            {
                if(TileValue < SuitExtensions.GetMinValue(Suit) ||
                   TileValue > SuitExtensions.GetMaxValue(Suit))
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
            int tenhouValue = TileValue;
            if(!this.IsHonor ||
                DisplayStyle == TileDisplayStyle.Tenhou)
            {
                if(this.Suit == Suit.Sangenpai)
                {
                    tenhouValue += (int)Wind.Max;
                }
                return tenhouValue.ToString();
            }
            else
            {
                if(Suit == Suit.Sangenpai)
                {
                    switch((Dragon)TileValue)
                    {
                        case Dragon.Green:
                            return "G";
                        case Dragon.Red:
                            return "R";
                        case Dragon.White:
                            return "W";
                        default:
                            throw new InvalidOperationException($"Tried to print invalid honor value Suit:{Suit.ToString()} Value:{TileValue}");
                    }
                }
                else
                {
                    switch((Wind)TileValue)
                    {
                        case Wind.East:
                            return "E";
                        case Wind.South:
                            return "S";
                        case Wind.West:
                            return "W";
                        case Wind.North:
                            return "N";
                        default:
                            throw new InvalidOperationException($"Tried to print invalid honor value Suit:{Suit.ToString()} Value:{TileValue}");
                    }
                }
            }
        }
    }
}