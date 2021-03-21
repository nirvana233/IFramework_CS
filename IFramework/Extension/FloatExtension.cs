using System;

namespace IFramework
{
    /// <summary>
    /// 静态扩展
    /// </summary>
    public static class FloatExtension
    {
        /// <summary>
        ///         取绝对值
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float Abs(this float self)
        {
            return Math.Abs(self);
        }
        /// <summary>
        /// 限制在 min-max之间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float Clamp(this float self, float min, float max)
        {
            return self < min ? min : self > max ? max : self;
        }
        /// <summary>
        /// 限制在0-1
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static float Clamp01(this float self)
        {
            return Clamp(self, 0f, 1f);
        }
        /// <summary>
        /// 获取比self小的最大整数
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int Floor(this float self)
        {
            return (int)Math.Floor(self);
        }
        /// <summary>
        /// 从0-length 重复
        /// </summary>
        /// <param name="self"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static float Repeat(this float self, float length)
        {
            return Clamp(self - (float)Math.Floor(self / length) * length, 0f, length);
        }
        /// <summary>
        /// 找到最近的 int
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int RoundToInt(this float self)
        {
            return (int)Math.Round((double)self);
        }
        /// <summary>
        /// 插值
        /// </summary>
        /// <param name="self"></param>
        /// <param name="end"></param>
        /// <param name="pecent"></param>
        /// <returns></returns>
        public static float Lerp(this float self, float end, float pecent)
        {
            return self + (end - self) * pecent.Clamp01();
        }
        /// <summary>
        /// 强制转换
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static int AsInt(this float self)
        {
            return (int)self;
        }
    }

}
