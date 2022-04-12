﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-03
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using System.Linq;

namespace IFramework.Serialization
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class LinkedListStringConverter<T> : StringConverter<LinkedList<T>>
    {
        public override string ConvertToString(LinkedList<T> t)
        {
            ListStringConverter<T> c = Get(typeof(List<T>)) as ListStringConverter<T>;
            return c.ConvertToString(t.ToList());
        }
        public override bool TryConvert(string self, out LinkedList<T> result)
        {
            ListStringConverter<T> c = Get(typeof(List<T>)) as ListStringConverter<T>;
            List<T> list;
            if (!c.TryConvert(self, out list))
            {
                result = MakeDefault();
                return false;
            }
            else
            {
                result = new LinkedList<T>(list);
                return true;
            }
        }

    }

}