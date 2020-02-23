/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net;
using System.Net.Sockets;

namespace IFramework.Net
{
    /// <summary>
    /// 链接实例
    /// </summary>
    public class SocketToken : IDisposable, IComparable<SocketToken>
    {
        private DateTime _connTime=DateTime.MinValue;
        private bool _isDisposed = false;
        private int _tokenId;
        private Socket _sock;
        private IPEndPoint _endPoint=null;
        private SocketAsyncEventArgs _arg=null;
        /// <summary>
        /// 序号
        /// </summary>
        public int tokenId { get { return _tokenId; } internal set { _tokenId = value; } }
        /// <summary>
        /// 套接字
        /// </summary>
        public Socket sock { get { return _sock; } internal set { _sock = value; } }
        /// <summary>
        /// 端口
        /// </summary>
        public IPEndPoint endPoint { get { return _endPoint; } internal set { _endPoint = value; } }
        /// <summary>
        /// 异步消息
        /// </summary>
        public SocketAsyncEventArgs arg { get { return _arg; }internal set { _arg = value; } }
        /// <summary>
        /// 是否释放
        /// </summary>
        public bool isDisposed { get { return _isDisposed; } }
        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime connTime { get { return _connTime; }internal set { _connTime = value; } }
        /// <summary>
        /// 
        /// </summary>
        ~SocketToken()
        {
            Dispose(false);
        }
        /// <summary>
        /// Ctor
        /// </summary>
        public SocketToken() { }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="id"></param>
        public SocketToken(int id) : this()
        {
            this._tokenId = id;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            if (_sock != null)
            {
                try
                {
                    if (_arg.ConnectSocket != null)
                    {
                        _arg.ConnectSocket.Close();
                    }
                    else if (_arg.AcceptSocket != null)
                    {
                        _arg.AcceptSocket.Close();
                    }
                    _sock.Shutdown(SocketShutdown.Send);
                }
                catch (ObjectDisposedException) { return; }
                catch { }
                _sock.Close();
            }
        }
        /// <summary>
        /// 比较==
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public int CompareTo(SocketToken token)
        {
            return this._tokenId.CompareTo(token.tokenId);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 释放时
        /// </summary>
        /// <param name="dispose"></param>
        protected virtual void Dispose(bool dispose)
        {
            if (_isDisposed) return;
            if (dispose)
            {
                _arg.Dispose();
                _sock.Dispose();
                _sock = null;
                _isDisposed = true;
            }
        }
    }

}
