using System;
using System.Collections;
using System.Collections.Generic;

namespace RedRunner.Utilities
{

	public static class Randomize
	{
        private static Random rng = new Random();

        /*
         * Implements a Fisher-Yates Shuffle: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
         * 
         */
        public static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }

}