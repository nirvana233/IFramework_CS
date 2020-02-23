/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Net
{
    /// <summary>
    /// 链接回调
    /// </summary>
    /// <param name="token"></param>
    /// <param name="connected"></param>
    public delegate void OnConnect(SocketToken token, bool connected);
    /// <summary>
    /// 收到 String
    /// </summary>
    /// <param name="sToken"></param>
    /// <param name="content"></param>
    public delegate void OnReceivedString(SocketToken sToken, string content);
    /// <summary>
    /// 收到消息
    /// </summary>
    /// <param name="token"></param>
    /// <param name="seg"></param>
    public delegate void OnReceieve(SocketToken token,BufferSegment seg);
    /// <summary>
    /// 发送回调
    /// </summary>
    /// <param name="token"></param>
    /// <param name="seg"></param>
    public delegate void OnSendCallBack(SocketToken token, BufferSegment seg);
    /// <summary>
    /// 接收到客户端
    /// </summary>
    /// <param name="token"></param>
    public delegate void OnAccept(SocketToken token);
    /// <summary>
    /// 客户端断线
    /// </summary>
    /// <param name="token"></param>
    public delegate void OnDisConnect(SocketToken token);

}
