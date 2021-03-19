using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host
{
    public static class ListShuffle
    {
        /// <summary>
        /// Shuffles a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            // you could use a crypto random generator but this is a card game so this is fine
            var rng = new Random(); 

            int numItems = list.Count;
            while (numItems > 1)
            {
                numItems--;
                int spot = rng.Next(numItems + 1);
                var item = list[spot];
                list[spot] = list[numItems];
                list[numItems] = item;
            }

            return list;
        }
    }
}
