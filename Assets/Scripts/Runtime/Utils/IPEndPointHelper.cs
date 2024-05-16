using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HayypCard.Utils
{
    public sealed class IPEndPointHelper
    {
        private IPEndPointHelper() { }

        private const string REGEX = @"^(\w{2})(\w{2})(\w{2})(\w{2})(\w{1,4})$";

        public static string EndPointToString(EndPoint endPoint)
        {
            IPEndPoint point = endPoint as IPEndPoint;
            if(point != null)
            {
                byte[] bytes = point.Address.GetAddressBytes();
                int port = point.Port;
                StringBuilder builder = new StringBuilder(13);
                builder.Append(BitConverter.ToString(bytes));
                builder.Replace("-", string.Empty);
                builder.Append(port.ToString("X"));
                return builder.ToString();
            }
            return null;
        }

        public static EndPoint StringToEndPoint(string str)
        {
            Match match = Regex.Match(str, REGEX);
            if (match.Success)
            {
                try
                {
                    byte[] bytes = new byte[4];
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] = Convert.ToByte(match.Groups[i+1].Value, 16);
                    }
                    int port = Convert.ToInt32(match.Groups[5].Value, 16);
                    IPEndPoint point = new IPEndPoint(new IPAddress(bytes), port);
                    return point;
                }
                catch(Exception e)
                {
                    LogHelper.Warn($"将字符串{str}解析为IP地址发生错误!原因:{e.Message}");
                    return null;
                }
            }
            return null;
        }
    }
}
