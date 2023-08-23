using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Extensions
{
    public static class EnumerableExtensions
    {

        public static void AddAll<T>(this ICollection<T> collection, IEnumerable<T> elementsToAdd)
        {
            foreach(T element in elementsToAdd)
            {
                collection.Add(element);
            }
        }

    }
}
