/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class WSConnectionItem
    {
        public WSConnectionItem()
        { }

        public WSConnectionItem(string wsUrl)
        {
            string[] urlParams = wsUrl.Split(':');
            if (urlParams.Length < 3)
                throw new Exception("wsUrl is error format.for example as ws://localhost:80");

            proto = urlParams[0];
            domain = urlParams[1].Replace("//", "");
            port = int.Parse(urlParams[2]);

            host = domain + ":" + port;
        }
        private string _proto = "ws";
        public string proto { get { return _proto; } set { _proto = value; } }

        public string domain { get; set; }
        private int _port = 65531;

        public int port { get { return _port; } set { _port = value; } }

        public string host { get; private set; }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}