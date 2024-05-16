using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System;
using System.Text;

namespace HayypCard.Network.Handlers
{
    public sealed class ProtocolCodecHelper
    {
        public static byte[] Encode(SyncInfo syncInfo)
        {
            string json = JsonConvert.SerializeObject(syncInfo);
            
            //计算出编码需要的字节长度
            int dataLen = Encoding.UTF8.GetMaxByteCount(json.Length);

            //前四个字节写入数据长度
            byte[] bytes = BitConverter.GetBytes(dataLen);
            //如果当前是小端系统
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            byte[] data = new byte[dataLen + 4];

            for (int i = 0; i < bytes.Length; i++)
            {
                data[i] = bytes[i];
            }

            Encoding.UTF8.GetBytes(json, 0, json.Length, data, 4);

            return data;
        }

        public static SyncInfo Decode(byte[] message)
        {
            //读取数据的长度
            byte[] bytes = new byte[] { message[0], message[1], message[2], message[3] };
            
            //如果当前是小端系统
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            int dataLen = BitConverter.ToInt32(bytes);

            LogHelper.Info("解码数据长度: " + dataLen);

            //编码为字符串
            string json = Encoding.UTF8.GetString(message, 4, dataLen);
            return JsonConvert.DeserializeObject<SyncInfo>(json);
        }
    }
}
