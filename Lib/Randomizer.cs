using System;

namespace Lib
{
    public static class Randomizer
    {
        private static Random random = new Random();

        internal static void InitSeed(int seed)
        {
            random = new Random(seed);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static double NextDouble(double multiply)
        {
            return random.NextDouble() * multiply;
        }
    }
}
