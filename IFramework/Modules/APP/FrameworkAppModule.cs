using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Modules.APP
{
    /// <summary>
    /// 一个app的总管理，需要自己写
    /// </summary>
    public abstract class FrameworkAppModule:FrameworkModule
    {
        /// <summary>
        /// app 版本
        /// </summary>
        public virtual string appVersion { get { return "0.0.0.1"; } }
        /// <summary>
        /// app名称
        /// </summary>
        public virtual string appName { get { return name; } }

       

    }
}
