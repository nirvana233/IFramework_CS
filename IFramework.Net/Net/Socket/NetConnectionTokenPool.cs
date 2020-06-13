/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class NetConnectionTokenPool
    {
        private LinkedList<NetConnectionToken> _list = null;
        private int _timerSpace = 60;//s
        private Timer _timer;
        private LockParam _lockParam;

        public int connectionTimeout = 60;//s

        public int count { get { return _list.Count; } }

        public NetConnectionTokenPool(int timerSpace)
        {
            this._timerSpace = timerSpace < 2 ? 2 : timerSpace;
            _lockParam = new LockParam();
            int _period = TimerSpaceToSeconds();
            _list = new LinkedList<NetConnectionToken>();
            _timer = new Timer(new TimerCallback(TimerLoop), null, _period, _period);
        }
        private int TimerSpaceToSeconds()
        {
            return (_timerSpace * 1000) >> 1;
        }
        private void TimerLoop(object obj)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                var items = _list.Where(x => x.verification == false ||
                                        DateTime.Now.Subtract(x.connectionTime).TotalSeconds >= connectionTimeout)
                                        .ToArray();
                for (int i = 0; i < items.Length; ++i)
                {
                    items[i].token.Close();
                    _list.Remove(items[i]);
                }
            }
        }

        public void TimerEnable(bool enable)
        {
            if (enable)
            {
                int space = TimerSpaceToSeconds();
                _timer.Change(space, space);
            }
            else _timer.Change(-1, -1);
        }
        public void ChangeTimerSpace(int space)
        {
            this._timerSpace = space < 2 ? 2 : space;
            int _p = TimerSpaceToSeconds();
            _timer.Change(_p, _p);
        }

        public NetConnectionToken GetTop()
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                if (_list.Count > 0)
                    return _list.First();
                return null;
            }
        }

        public IEnumerable<NetConnectionToken> ReadNext()
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                foreach (var l in _list)
                {
                    yield return l;
                }
            }
        }

        public void AddToken(NetConnectionToken ncToken)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                _list.AddLast(ncToken);
            }
        }

        public bool RemoveToken(NetConnectionToken ncToken, bool isClose)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                if (isClose) ncToken.token.Close();
                return _list.Remove(ncToken);
            }
        }

        public bool RemoveToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                var item = _list.Where(x => x.token.CompareTo(sToken) == 0).FirstOrDefault();
                if (item != null)
                {
                    return _list.Remove(item);
                }
            }
            return false;
        }

        public NetConnectionToken GetTokenById(int Id)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                return _list.Where(x => x.token.tokenId == Id).FirstOrDefault();
            }
        }

        public NetConnectionToken GetTokenBySocketToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                return _list.Where(x => x.token.CompareTo(sToken) == 0).FirstOrDefault();
            }
        }

        public void Clear(bool isClose)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                while (_list.Count > 0)
                {
                    var item = _list.First();
                    _list.RemoveFirst();

                    if (isClose)
                    {
                        if (item.token != null)
                            item.token.Close();
                    }
                }
            }
        }

        public bool RefreshConnectionToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref _lockParam))
            {
                var rt = _list.Find(new NetConnectionToken(sToken));
                if (rt == null) return false;
                rt.Value.connectionTime = DateTime.Now;
                return true;
            }
        }

    }
    public class NetConnectionToken : IComparable<NetConnectionToken>
    {
        public NetConnectionToken() { }
        public NetConnectionToken(SocketToken sToken)
        {
            this.token = sToken;
            verification = true;
            connectionTime = DateTime.Now;
        }
        public SocketToken token { get; set; }
        public DateTime connectionTime { get; set; }
        public bool verification { get; set; }
        public int CompareTo(NetConnectionToken item)
        {
            return token.CompareTo(item.token);
        }
        public override bool Equals(object obj)
        {
            var nc = obj as NetConnectionToken;
            if (nc == null) return false;
            return this.CompareTo(nc) == 0;
        }
        public override int GetHashCode()
        {
            return token.tokenId.GetHashCode() | token.sock.GetHashCode();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
