﻿
using System;

namespace IFramework.Modules.Fsm
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public interface ICondition
    {
        Type type { get; }
        string name { get; }
        CompareType compareType { get;  }
        bool IsMetCondition();
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
