/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    [System.Runtime.InteropServices.ComVisible(false)]

    public class Point2StringConverter : StringConverter<Point2>
    {
        private static StringConverter<float> _innner = Get<float>();

        public override bool TryConvert(string self, out Point2 result)
        {
            var bo = self.StartsWith(leftBound.ToString()) && self.EndsWith(rightBound.ToString()) && self.Contains(dot.ToString());

            if (!bo) goto ERR;
            var strs = self.Replace(leftBound.ToString(), "")
                    .Replace(rightBound.ToString(), "")
                    .Split(dot);
            if (strs.Length!=2) goto ERR;
            
            bo= _innner.TryConvert(strs[0], out result.x);
            if (!bo) goto ERR;
            bo = _innner.TryConvert(strs[1], out result.y);
            if (!bo) goto ERR;

            ERR:
            result = MakeDefault();
            return false;
        }
        public override string ConvertToString(Point2 t)
        {
            return string.Format("{0}{1}{2}{3}{4})",
                leftBound,
                 t.x,
                 dot,
                 t.y,
                 rightBound
                 );
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}