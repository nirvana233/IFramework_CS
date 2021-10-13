/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface IReference
    {
        int count { get; }
        void Retain(object user = null);
        void Release(object user = null);
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
