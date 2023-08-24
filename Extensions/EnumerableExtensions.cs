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

        public static double Sum(this IEnumerable<double> values)
        {
            double result = 0;
            foreach(double value in values)
            {
                result += value;
            }
            return result;
        }
        public static int Sum(this IEnumerable<int> values)
        {
            int result = 0;
            foreach (int value in values)
            {
                result += value;
            }
            return result;
        }
    }
}
