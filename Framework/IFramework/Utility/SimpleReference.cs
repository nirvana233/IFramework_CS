/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.Utility
{
    public class SimpleReference : IReference
    {
        public SimpleReference()
        {
            count = 0;
        }

        public int count { get; private set; }

        public virtual void Retain(object user = null)
        {
            ++count;
        }

        public virtual void Release(object user = null)
        {
            --count;
            if (count == 0)
            {
                OnZero();
            }
        }

        protected virtual void OnZero()
        {
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
