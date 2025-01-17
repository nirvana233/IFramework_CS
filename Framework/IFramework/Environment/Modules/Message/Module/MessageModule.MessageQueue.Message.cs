﻿using System;
namespace IFramework.Modules.Message
{
    public partial class MessageModule
    {
        private partial class MessageQueue
        {
            private class Message : IMessage
            {

                private int _code = 0;
                private Type _subject;
                private IEventArgs _args;
                private MessageState _state;
                private MessageErrorCode _errCode;
                private event Action<IMessage> onRecycle;

                public int code { get { return _code; } }
                public Type subject { get { return _subject; } }
                public IEventArgs args { get { return _args; } }
                public MessageState state { get { return _state; } }
                public MessageErrorCode errorCode { get { return _errCode; } }

                private void Reset()
                {
                    this._errCode = MessageErrorCode.None;
                    _code = int.MinValue;
                    _args = null;
                    _subject = null;
                    onRecycle = null;
                }
                internal void Begin()
                {
                    Reset();
                    _state = MessageState.Wait;
                }
                internal Message SetType(Type type)
                {
                    this._subject = type;
                    return this;
                }
                internal Message SetArgs(IEventArgs args)
                {
                    this._args = args;
                    return this;
                }
                public IMessage SetCode(int code)
                {
                    if (_state != MessageState.Wait)
                    {
                        Log.E(string.Format("you can not set the code now, the state is {0}", _state));
                        return this;
                    }
                    this._code = code;
                    return this;
                }

                public IMessage OnCompelete(Action<IMessage> action)
                {
                    if (_state != MessageState.Wait)
                    {
                        Log.E(string.Format("you can not bind the action now, the state is {0}", _state));
                        return this;
                    }
                    onRecycle += action;
                    return this;
                }
                internal void Lock()
                {
                    if (_state == MessageState.Wait)
                    {
                        _state = MessageState.Lock;
                    }
                    else
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                }
                internal void SetErrorCode(MessageErrorCode code)
                {
                    if (_state == MessageState.Lock)
                    {
                        this._errCode = code;
                    }
                    else
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                }
                internal void End()
                {
                    if (_state != MessageState.Lock)
                    {
                        Log.E("unknown Exception occured with this message");
                    }
                    _state = MessageState.Rest;
                    if (onRecycle != null)
                    {
                        onRecycle.Invoke(this);
                    }
                    Reset();
                }

                public MessageAwaiter GetAwaiter()
                {
                    return new MessageAwaiter(this);
                }
            }
        }
    }
}
