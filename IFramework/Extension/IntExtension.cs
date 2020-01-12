using System;

namespace IFramework
{
    public static class IntExtension
    {
        public static bool isPrimeNumber(this int self)
        {
            for (int i = 2; i < Math.Sqrt(self); i++)
            {
                if (self % i == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static int Swap(this int self, ref int other)
        {
            self = self ^ other;
            other = self ^ other;
            return self ^ other;
        }
    }

}
