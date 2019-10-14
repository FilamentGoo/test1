using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MahjongEngine 
{
    public static class Utils
    {
        private static Random random = null;

        public static int GetRandom(int low, int high)
        {
            if(random == null)
            {
                random = new Random();
            }

            return random.Next(low, high);
        }
        
        public static int GetRandom(int high)
        {
            return GetRandom(0, high);
        }

        public static T GetRandom<T>(IEnumerable<T> Collection)
        {
            List<T> list = Collection.ToList();
            return list[GetRandom(0, list.Count)];
        }

        public static List<T> Shuffle<T>(List<T> collection)
        {
            if(collection == null)
            {
                return null;
            }
            for(int i = 0; i < collection.Count; i++)
            {
                int swapIndex = GetRandom(i, collection.Count);
                T temp = collection[i];
                collection[i] = collection[swapIndex];
                collection[swapIndex] = temp;
            }

            return collection;
        }
    }
}