using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IFramework.Modules.APP
{
    public enum AppStateType
    {
        Stop,Pause,Runing
    }
    
    public abstract class FrameworkAppModule:FrameworkModule
    {
        public virtual string AppVersion { get { return "0.0.0.1"; } }
        public virtual string AppName { get { return name; } }

        //private AppStateType curStateType;
        //public AppStateType CurStateType { get { return curStateType; } set { curStateType = value; } }


    }
}
