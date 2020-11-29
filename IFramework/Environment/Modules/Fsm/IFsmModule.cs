using System;

namespace IFramework.Modules.Fsm
{/// <summary>
/// FSM 模块
/// </summary>
    public interface IFsmModule
    {
        /// <summary>
        /// 当前节点
        /// </summary>
        IState currentState { get; }
        /// <summary>
        /// 开始节点
        /// </summary>
        IState enterState { get; set; }
        /// <summary>
        /// 结束节点
        /// </summary>
        IState exitState { get; set; }
        /// <summary>
        /// 是否运行
        /// </summary>
        bool runing { get; }
        /// <summary>
        /// 节点变化时
        /// </summary>
        event Action<IState> onStateChange;

        /// <summary>
        /// 创建条件值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        ConditionValue<T> CreateConditionValue<T>(string name, T value);
        /// <summary>
        /// 移除条件值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void DestoryConditionValue<T>(string name, T value);

        /// <summary>
        /// 创建条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="CompareValue"></param>
        /// <param name="CompareType"></param>
        /// <returns></returns>
        Condition<T> CreateCondition<T>(ConditionValue<T> value, T CompareValue, CompareType CompareType);
        /// <summary>
        /// 创建条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditionValName"></param>
        /// <param name="CompareValue"></param>
        /// <param name="CompareType"></param>
        /// <returns></returns>
        Condition<T> CreateCondition<T>(string conditionValName, T CompareValue, CompareType CompareType);

        /// <summary>
        /// 连线
        /// </summary>
        /// <param name="head"></param>
        /// <param name="trail"></param>
        /// <returns></returns>
        Transition CreateTransition(IState head, IState trail);
        /// <summary>
        /// 移除连线
        /// </summary>
        /// <param name="head"></param>
        /// <param name="trail"></param>
        void DestoryTransition(IState head, IState trail);


        /// <summary>
        /// 注册节点
        /// </summary>
        /// <param name="state"></param>
        void SubscribeState(IState state);
        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="state"></param>
        void UnSubscribeState(IState state);


        /// <summary>
        /// 开始
        /// </summary>
        void Start();
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        /// <summary>
        /// 继续
        /// </summary>
        void UnPause();



        /// <summary>
        /// 设置bool
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetBool(string valName, bool value);
        /// <summary>
        /// 设置float
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetFloat(string valName, float value);
        /// <summary>
        /// 设置int
        /// </summary>
        /// <param name="valName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool SetInt(string valName, int value);
    }
}