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
        /// <summary>
        /// 约束数值大小
        /// </summary>
        /// <param name="self"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int Clamp(this int self, int min , int max)
        {
            return self < min ? min : self > max ? max : self;
        }
    }
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class FloatExtension
    {
        public static float Clamp(this float self,float min, float max)
        {
            return self < min ? min : self > max ? max : self;
        }
        public static float Clamp01(this float self)
        {
            return Clamp(self,0f,1f);
        }
        public static float Repeat(this float self, float length)
        {
            return Clamp(self - (float)Math.Floor(self/ length) * length, 0f, length);
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
