using System;

namespace IFramework
{
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
        public static int RoundToInt(this float self)
        {
            return (int)Math.Round((double)self);
        }
        public static float Lerp(this float self, float end, float pecent)
        {
            return self + (end - self) * pecent.Clamp01();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
