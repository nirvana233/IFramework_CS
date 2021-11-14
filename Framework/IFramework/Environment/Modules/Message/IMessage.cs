using System;

namespace IFramework.Modules.Message
{
    /// <summary>
    /// 消息
    /// </summary>
    [Tip("如果需要缓存此类型的引用，必须绑定回收事件OnCompelete，并制空引用,以防出错")]
    [Tip("部分 Api 只在特殊时期有效")]
    public interface IMessage:IAwaitable<MessageAwaiter>
    {
        /// <summary>
        /// 发送消息类型
        /// </summary>
        Type subject { get; }
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
        /// 设置Code，
        /// 仅在 state 为 MessageState.Wait时有效
        /// </summary>
        /// <param name="code"></param>
        IMessage SetCode(int code);


        /// <summary>
        /// 仅在 state 为 MessageState.Wait时有效
        /// 消息发布完成时的引用
        /// </summary>
        /// <param name="action"></param>
        IMessage OnCompelete(Action<IMessage> action);
        #endregion
    }
}
