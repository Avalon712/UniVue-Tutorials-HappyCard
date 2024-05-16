using HappyCard.Entities.Configs;
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
    /// 广域网服务（与服务器进行tcp长链接）
    /// </summary>
   // [DisallowMultipleComponent]
    public sealed class GameTcpService : MonoBehaviour
    {
        private volatile Queue<SyncInfo> _receiveQueue;
        private volatile Queue<SyncInfo> _sendQueue;
        private Socket _socket;
        private byte[] _receiveBuffer;
        private bool _idel = true; //指示当前发送数据的线程是否处于空闲

        /// <summary>
        /// 当接受到同步消息时进行回调
        /// </summary>
        public Action<SyncInfo> OnReceiveSyncInfo { get; set; }

        private void Awake()
        {
            _receiveBuffer = new byte[1024];
            _receiveQueue = new Queue<SyncInfo>();
            _sendQueue = new Queue<SyncInfo>();
            Initialize();
        }

        private async void Initialize()
        {
            //使用IPv4
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            NetworkInfo networkInfo = GameDataManager.Instance.NetworkInfo;

            try
            {
                await _socket.ConnectAsync(IPAddress.Parse(networkInfo.RoomServerIP), networkInfo.RoomServerPort);

                if (_socket.Connected)
                {
                    _socket.BeginReceive(_receiveBuffer, 0, 1024, SocketFlags.None, ReceiveSyncInfo, null);
                }
            }
            catch(Exception e)
            {
                LogHelper.Warn($"与服务器连接失败！异常原因:{e.Message}");
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

        public void SendSyncInfo(SyncInfo syncInfo)
        {
            if (_idel)
            {
                byte[]  data = ProtocolCodecHelper.Encode(syncInfo);
                _socket.BeginSend(data,0,data.Length, SocketFlags.None, SendSyncInfo, null);
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
                _receiveQueue.Enqueue(ProtocolCodecHelper.Decode(_receiveBuffer));

                _socket.BeginReceive(_receiveBuffer, 0, 1024, SocketFlags.None, ReceiveSyncInfo, null);
            }
            catch(Exception e)
            {
                LogHelper.Warn($"接受TCP数据异常:{e.Message}");
            }
        }


        //---------------------------------------------------------------------------------------------------

        private void SendSyncInfo(IAsyncResult result)
        {
            try
            {
                _socket.EndSend(result);
                if (_sendQueue.Count > 0)
                {
                    byte[] data = ProtocolCodecHelper.Encode(_sendQueue.Dequeue());
                    _socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendSyncInfo, null);
                }
            }
            catch(Exception e)
            {
                LogHelper.Warn($"发送TCP数据异常:{e.Message}");
            }
        }

        //---------------------------------------------------------------------------------------------------

        private void OnDestroy()
        {
            _socket.Dispose();
            _socket = null;
            _receiveQueue.Clear();
            _receiveQueue = null;
            OnReceiveSyncInfo = null;
        }

    }
}
