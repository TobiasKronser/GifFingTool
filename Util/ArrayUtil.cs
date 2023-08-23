using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Util
{
    public static class ArrayUtil
    {
        public static ImmutableArray<T> ToImmutableArray<T>(params T[] values)
        {
            ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>();
            foreach (T value in values)
            {
                builder.Add(value);
            }
            return builder.ToImmutable();
        }

        public static T[] CreateAndInitialize<T>(int size, T initialValue)
        {
            T[] result = new T[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = initialValue;
            }
            return result;
        }
    }
}
