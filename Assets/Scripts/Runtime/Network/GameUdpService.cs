using HappyCard.Managers;
using HayypCard.Network.Entities;
using HayypCard.Network.Handlers;
using HayypCard.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace HappyCard.Network
{
    /// <summary>
    /// 局域网服务
    /// </summary>
   // [DisallowMultipleComponent]
    public sealed class GameUdpService : MonoBehaviour
    {
        private volatile Queue<SyncInfo> _receiveQueue;
        private volatile Queue<SyncInfo> _sendQueue;
        private Socket _socket;
        private byte[] _receiveBuffer;
        private bool _idel = true; //指示当前发送数据的线程是否处于空闲
        //记录房间中其它玩家的IP、端口信息, key=playerID、value=EndPoint
        private Dictionary<string, EndPoint> _hostInfos;

        /// <summary>
        /// 当接受到同步消息时进行回调
        /// </summary>
        public Action<SyncInfo> OnReceiveSyncInfo { get; set; }

        /// <summary>
        /// 当前玩家的主机信息
        /// </summary>
        public EndPoint HostEndPoint { get; set; }

        private void Awake()
        {
            _receiveBuffer = new byte[1024];
            _receiveQueue = new Queue<SyncInfo>();
            _sendQueue = new Queue<SyncInfo>();
            _hostInfos = new();

            Initialize();
        }

        private void Initialize()
        {
            //使用IPv4
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //获取本机的一个随机临时端口
            HostEndPoint = new IPEndPoint(NetworkManager.Instance.GetDhcpIPv4AddressInTheSameNet(), UnityEngine.Random.Range(49152, 65535));
            try
            {
                _socket.Bind(HostEndPoint);

                EndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 1111); //接受到消息后会自动赋值
                _socket.BeginReceiveFrom(_receiveBuffer, 0, 1024, SocketFlags.None, ref endPoint, ReceiveSyncInfo, endPoint);

                LogHelper.Info($"UDP服务启动成功,EndPoint[{HostEndPoint}]");
            }
            catch (Exception e)
            {
                LogHelper.Warn($"UDP服务启动失败！异常原因:{e.Message}");
            }
        }

        //---------------------------------------------------------------------------------------------------

        private void Update()
        {
            if (_receiveQueue.Count > 0)
            {
                OnReceiveSyncInfo(_receiveQueue.Dequeue());
            }
        }

        //---------------------------------------------------------------------------------------------------

        //当还没有与玩家形成连接时，主动发送消息
        public void SendSyncInfo(SyncInfo syncInfo, EndPoint endPoint)
        {
            if (_idel)
            {
                byte[] data = ProtocolCodecHelper.Encode(syncInfo);
                _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, SendSyncInfo, null);
            }
            else
            {
                _sendQueue.Enqueue(syncInfo);
            }
        }

        //---------------------------------------------------------------------------------------------------

        //广播房间中的所有玩家
        public void SendSyncInfo(SyncInfo syncInfo)
        {
            if (_idel)
            {
                byte[] data = ProtocolCodecHelper.Encode(syncInfo);

                foreach (var receiveEndPoint in _hostInfos.Values)
                {
                    _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, receiveEndPoint, SendSyncInfo, null);
                }
            }
            else
            {
                _sendQueue.Enqueue(syncInfo);
            }
        }

        //---------------------------------------------------------------------------------------------------

        private void ReceiveSyncInfo(IAsyncResult result)
        {
            try
            {
                int len = _socket.EndReceive(result);
                //解码
                SyncInfo udpSyncInfo = ProtocolCodecHelper.Decode(_receiveBuffer);
                _receiveQueue.Enqueue(udpSyncInfo); //入队

                EndPoint endPoint = (EndPoint) result.AsyncState;

                //记录该玩家的主机信息
                if (!_hostInfos.ContainsKey(udpSyncInfo.SenderID))
                {
                    _hostInfos.Add(udpSyncInfo.SenderID, endPoint);
                }

                //继续接受
                _socket.BeginReceiveFrom(_receiveBuffer, 0, 1024, SocketFlags.None,ref endPoint ,ReceiveSyncInfo, endPoint);
            }
            catch(Exception e)
            {
                LogHelper.Warn($"UDP服务异常:{e.Message}");
            }
        }

        //---------------------------------------------------------------------------------------------------

        private void SendSyncInfo(IAsyncResult result)
        {
            try
            {
                _socket.EndSend(result);
                _idel = _sendQueue.Count == 0;

                if (_sendQueue.Count > 0)
                {
                    byte[] data = ProtocolCodecHelper.Encode(_sendQueue.Dequeue());

                    foreach (var receiverID in _hostInfos.Values)
                    {
                        _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, receiverID, SendSyncInfo, null);
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.Warn($"发送UDP数据异常:{e.Message}");
            }
        }

        //---------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            _socket.Dispose();
            _socket = null;
            _receiveQueue.Clear();
            _receiveQueue = null;
            _hostInfos.Clear();
            _hostInfos = null;
            OnReceiveSyncInfo = null;
        }
    }
}
