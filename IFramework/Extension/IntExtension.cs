using System;

namespace IFramework
{
    /// <summary>
    /// int 静态扩展
    /// </summary>
    public static class IntExtension
    {
        /// <summary>
        /// 是不是素数
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 交换两个数
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static int Swap(this int self, ref int other)
        {
            self = self ^ other;
            other = self ^ other;
            return self ^ other;
        }
    }

}
