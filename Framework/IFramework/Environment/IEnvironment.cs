using System;
using System.Collections.Generic;
using IFramework.Injection;
using IFramework.Modules;

namespace IFramework
{
    /// <summary>
    /// 环境
    /// </summary>
    [ScriptVersionAttribute(35)]
    [VersionUpdateAttribute(21, "替换可回收对象池子类型")]
    [VersionUpdateAttribute(22, "调整释放时候的成员顺序")]
    [VersionUpdateAttribute(23, "增加数据绑定器")]
    [VersionUpdateAttribute(35, "抽象化环境")]
    public interface IEnvironment: IDisposable
    {
        /// <summary>
        /// 数据容器
        /// </summary>
        IValuesContainer container { get; set; }

        /// <summary>
        /// 环境类型
        /// </summary>
        EnvironmentType envType { get; }
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        bool inited { get; }
        /// <summary>
        /// 模块容器
        /// </summary>
        IFrameworkModules modules { get; }
        /// <summary>
        /// 时间
        /// </summary>
        ITimeCalculator time { get; }
        /// <summary>
        /// 绑定 Dispose
        /// </summary>
        /// <param name="action"></param>
        void BindDispose(Action action);
        /// <summary>
        /// 绑定 Update
        /// </summary>
        /// <param name="action"></param>
        void BindUpdate(Action action);
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="types"></param>
        void Init(IEnumerable<Type> types);
        /// <summary>
        /// 初始化
        /// </summary>
        void InitWithAttribute();
        /// <summary>
        /// 解绑 Dispose
        /// </summary>
        /// <param name="action"></param>
        void UnBindDispose(Action action);
        /// <summary>
        /// 解绑 Update
        /// </summary>
        /// <param name="action"></param>
        void UnBindUpdate(Action action);
        /// <summary>
        /// 刷新
        /// </summary>
        void Update();
        /// <summary>
        /// 等待环境刷新
        /// </summary>
        /// <param name="action"></param>
        void WaitEnvironmentFrame(Action action);
    }
}