using System;
namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息
    /// </summary>
    [Tip("如果需要缓存此类型的引用，必须绑定回收事件OnRecycle，并制空引用,以防出错")]
    [Tip("部分 Api 只在特殊时期有效")]
    public interface IMessage
    {
        /// <summary>
        /// 发送消息类型
        /// </summary>
        Type type { get; }
        /// <summary>
        /// 承载消息内容
        /// </summary>
        IEventArgs args { get; }
        /// <summary>
        /// code,帮助区分 args
        /// </summary>
        int code { get; }

        /// <summary>
        /// 消息状态
        /// </summary>
        MessageState state { get; }

        /// <summary>
        /// 消息发送结果
        /// </summary>
        MessageErrorCode errorCode { get; }



        #region 仅在 state 为 MessageState.Wait时有效
        /// <summary>
        /// 优先级 
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        float priority { get; }
        /// <summary>
        /// 所在位置 
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        int position { get; }
        /// <summary>
        /// 设置Code，
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        /// <param name="code"></param>
        IMessage SetCode(int code);
        /// <summary>
        /// 设置优先级 
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        /// <param name="priority"></param>
        /// <returns></returns>
        IMessage SetPriority(int priority);

        /// <summary>
        /// 仅在 state 为 MessageState.Wait时有效
        /// 实例被回收时的引用
        /// 消息发布完成，回收时候触发
        /// 回收后，此回调内部会制空，即只会触发一次
        /// 如果缓存了引用，记得制空引用
        /// </summary>
        /// <param name="action"></param>
        IMessage OnRecycle(Action<IMessage> action);
        #endregion
    }
}
