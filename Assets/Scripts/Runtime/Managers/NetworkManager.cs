using HappyCard.Enums;
using HappyCard.Network;
using HappyCard.Network.Entities;
using HappyCard.Utils;
using HayypCard.Network.Entities;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UniVue.Utils;

namespace HappyCard.Managers
{
    public sealed class NetworkManager : Singleton<NetworkManager>
    {
        #region WAN
        private GameHttpService _httpService;
        private GameTcpService _tcpService;
        #endregion

        #region LAN
        private GameOfflineService _offlineService;
        private GameUdpService _udpService;
        #endregion

        /// <summary>
        /// 接受事件回调
        /// </summary>
        private List<SyncInfoResponseHandle> _handles;

        public NetworkManager()
        {
            _handles = new List<SyncInfoResponseHandle>();
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 清空同步消息响应处理，应该在场景卸载时调用此函数
        /// </summary>
        public void ClearHandles()
        {
            for (int i = 0; i < _handles.Count; i++)
            {
                _handles[i] = default;
            }

            _handles.Clear();
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 添加同步消息响应事件处理
        /// </summary>
        public void RegisterSyncInfoResponseHandle(ref SyncInfoResponseHandle handle)
        {
            _handles.Add(handle);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 处理接受到的同步消息
        /// </summary>
        private void OnReceiveSyncInfo(SyncInfo syncInfo)
        {
            for (int i = 0; i < _handles.Count; i++)
            {
                if (_handles[i].Handle(syncInfo))
                {
                    _handles[i] = default;
                    ListUtil.TrailDelete(_handles, i--);
                }
            }
        }

        //----------------------------------------------------------------------------------------------------

        # region 设置游戏中网络服务模式
        /// <summary>
        /// 设置游戏中网络服务模式
        /// </summary>
        public void SetGameNetworkServiceMode(NetworkServiceMode mode)
        {
            GameDataManager.Instance.NetworkInfo.mode = mode;

            if(mode == NetworkServiceMode.WAN)
            {
                if(_tcpService == null)
                {
                    _tcpService = GameRunner.Instance.gameObject.AddComponent<GameTcpService>();
                }

                _httpService = GameRunner.Instance.GetComponent<GameHttpService>();
                if (_httpService == null)
                {
                    _httpService = GameRunner.Instance.gameObject.AddComponent<GameHttpService>();
                }

                if (_udpService != null)
                {
                    GameObject.Destroy(_udpService);
                    _udpService = null;
                    _offlineService = null;
                }

                _tcpService.OnReceiveSyncInfo = OnReceiveSyncInfo;
            }
            else if(mode == NetworkServiceMode.LAN)
            {
                if(_offlineService == null)
                {
                    _offlineService = new GameOfflineService();
                }

                if(_udpService == null)
                {
                    _udpService = GameRunner.Instance.gameObject.AddComponent<GameUdpService>();
                }

                if(_tcpService != null)
                {
                    GameObject.Destroy(_tcpService);
                    _tcpService = null;
                }

                if(_httpService != null)
                {
                    GameObject.Destroy(_httpService);
                    _httpService = null;
                }

                _udpService.OnReceiveSyncInfo = OnReceiveSyncInfo;
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取游戏中网络服务模式
        /// </summary>
        public NetworkServiceMode GetGameNetworkServiceMode()
        {
            return GameDataManager.Instance.NetworkInfo.mode;
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取指定游戏事件的HTTP的接口信息
        /// </summary>
        public URLInfo GetHttpURL(GameEvent gameEvent)
        {
            return GameDataManager.Instance.NetworkInfo.GetHttpURL(gameEvent);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        public void SendHttpRequest<R1,R2>(HttpInfo<R1,R2> httpInfo,GameEvent gameEvent) where R1 :class where R2:class
        {
            if (GetGameNetworkServiceMode() == NetworkServiceMode.WAN)
                _httpService.AsyncSendRequest(httpInfo);
            else
                _offlineService.SimulateHttpServer(httpInfo, gameEvent);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送同步信息
        /// </summary>
        public void SendSyncInfo(SyncInfo syncInfo,ref SyncInfoResponseHandle handle)
        {
            if(GetGameNetworkServiceMode() == NetworkServiceMode.WAN)
                _tcpService.SendSyncInfo(syncInfo);
            else
                _udpService.SendSyncInfo(syncInfo);
            if (handle.Calls != null) { _handles.Add(handle); }
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送UDP单播消息（尚未与玩家形成连接）
        /// </summary>
        /// <param name="syncInfo"></param>
        /// <param name="endPoint"></param>
        public void SendSyncInfo(SyncInfo syncInfo, EndPoint endPoint,ref SyncInfoResponseHandle handle)
        {
            if (GetGameNetworkServiceMode() == NetworkServiceMode.LAN)
                _udpService.SendSyncInfo(syncInfo, endPoint);

            if (handle.Calls != null)
                _handles.Add(handle); 
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送同步信息
        /// </summary>
        public void SendSyncInfo(SyncInfo syncInfo)
        {
            NetworkServiceMode mode = GetGameNetworkServiceMode();
            if (mode == NetworkServiceMode.WAN)
                _tcpService.SendSyncInfo(syncInfo);

            else if(mode == NetworkServiceMode.LAN)
                _udpService.SendSyncInfo(syncInfo);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 发送UDP单播消息（尚未与玩家形成连接）
        /// </summary>
        /// <param name="syncInfo"></param>
        /// <param name="endPoint"></param>
        public void SendSyncInfo(SyncInfo syncInfo, EndPoint endPoint)
        {
            if (GetGameNetworkServiceMode() == NetworkServiceMode.LAN)
                _udpService.SendSyncInfo(syncInfo, endPoint);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取当前玩家主机上的UDP服务IPEndPoint，如果当前是UDP服务则是本机的IP+端口，否则是回环地址+随机临时端口
        /// </summary>
        /// <returns></returns>
        public EndPoint GetHostEndPoint()
        {
            if (_udpService != null) { return _udpService.HostEndPoint; }
            return new IPEndPoint(IPAddress.Loopback, Random.Range(14592, 65535));
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取本机上DHCP协议的IP地址，同时与目标IP处于同一个网络
        /// 注：玩家处于同一个局域网下，连接同一个wifi，玩家的ip地址都是处于同一个网段
        /// </summary>
        public IPAddress GetDhcpIPv4AddressInTheSameNet()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                NetworkInterface @interface = interfaces[i];

                IPInterfaceProperties properties = @interface.GetIPProperties();

                if (properties == null) { continue; }

                var unicastIPs = properties.UnicastAddresses;

                foreach (var ip in unicastIPs)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.PrefixOrigin==PrefixOrigin.Dhcp)
                    {
                        return ip.Address;
                    }
                }
            }

            return null;
        }
    }
}
