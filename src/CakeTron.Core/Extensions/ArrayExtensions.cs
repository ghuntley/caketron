using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace CakeTron
{
    internal static class ArrayExtensions
    {
        public static T GetRandom<T>(this T[] array, Random random)
        {
            return array[random.Next(0, array.Length)];
        }
    }
}
