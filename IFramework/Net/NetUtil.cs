/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Net;
using System.Linq;
using System.Net.Sockets;

namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class NetUtil
    {
        public static IPAddress[] GetLoacalIpv4()
        {
           IPAddress[] addresses= Dns.GetHostAddresses("localhost");
           return  (from x in addresses where x.AddressFamily == AddressFamily.InterNetwork select x).ToArray();
        }
        public static IPAddress[] GetLoacalIpv6()
        {
            IPAddress[] addresses = Dns.GetHostAddresses("localhost");
            return (from x in addresses where x.AddressFamily == AddressFamily.InterNetworkV6 select x).ToArray();
        }
        public static string GetOutSideIP()
        {
            using (WebClient wc = new WebClient())
            {
                return wc.DownloadString(@"http://icanhazip.com/").Replace("\n", "");
            }
        }
    }

#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
