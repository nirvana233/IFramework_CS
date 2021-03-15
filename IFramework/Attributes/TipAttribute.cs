using System;

namespace IFramework
{
    /// <summary>
    /// 小提示
    /// </summary>
    [AttributeUsage(AttributeTargets.All,AllowMultiple =true,Inherited =false)]
    public class TipAttribute : Attribute, IDescription
    {
        private string _description;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="description"></param>
        public TipAttribute(string description)
        {
            _description = description;
        }
        /// <summary>
        /// 描述
        /// </summary>
        public string description { get { return _description; } }
    }
}
