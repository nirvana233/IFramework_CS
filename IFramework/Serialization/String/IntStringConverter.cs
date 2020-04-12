﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Text;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

    public class IntStringConverter : StringConverter<int>
    {
        public override bool TryConvert(string self, out int result)
        {
            return int.TryParse(self, out result);
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
