using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UniVue.Model;

namespace HappyCard.Entities.Configs
{
    /// <summary>
    /// 游戏网络配置信息
    /// </summary>
    public sealed class NetworkInfo : BaseModel
    {
        //默认为广域网
        private NetworkServiceMode _mode = NetworkServiceMode.WAN;
        private float _delay;

        /// <summary>
        /// 配置文件的文件名，不含文件后缀
        /// </summary>
        public const string fileName = "network";

        /// <summary>
        /// 大厅服务器的IP地址
        /// </summary>
        public string HallServerIP { get; set; }

        /// <summary>
        /// 大厅服务器端口号
        /// </summary>
        public int HallServerPort { get; set; }

        /// <summary>
        /// 房间服务器的IP地址
        /// </summary>
        public string RoomServerIP { get; set; }

        /// <summary>
        /// 房间服务器端口号
        /// </summary>
        public int RoomServerPort { get; set; }

        /// <summary>
        /// 游戏事件对应的大厅服务器的url接口
        /// </summary>
        public Dictionary<GameEvent, URLInfo> Urls { get; set; }

        /// <summary>
        /// 当前游戏网络服务类型
        /// </summary>
        [JsonIgnore]
        public NetworkServiceMode mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    NotifyUIUpdate(nameof(mode), (int)value);
                }
            }
        }

        /// <summary>
        /// 当前网络延迟，毫秒
        /// </summary>
        [JsonIgnore]
        public float delay
        {
            get => _delay;
            set
            {
                if (_delay != value)
                {
                    _delay = value;
                    NotifyUIUpdate(nameof(delay), value);
                }
            }
        }

        /// <summary>
        /// 当前网络类型的图标
        /// </summary>
        [JsonIgnore]
        public Sprite Icon
        {
            get => AssetManager.Instance.GetNetworkIcon(mode);
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(mode), (int)mode);
            NotifyUIUpdate(nameof(delay), delay);
            NotifyUIUpdate(nameof(Icon), Icon);
        }

        #endregion

        #region 重写父类UpdateModel()函数


        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if (nameof(mode).Equals(propertyName)) { mode = (NetworkServiceMode)propertyValue; }
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
        }

        public override void UpdateModel(string propertyName, float propertyValue)
        {
            if (nameof(delay).Equals(propertyName)) { delay = propertyValue; }
        }

        public override void UpdateModel(string propertyName, bool propertyValue)
        {
        }
        #endregion

        public URLInfo GetHttpURL(GameEvent gameEvent)
        {
            if (!Urls.ContainsKey(gameEvent)) { return default; }
            return Urls[gameEvent];
        }

    }
} 
