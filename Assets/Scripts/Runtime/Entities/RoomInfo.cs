using HayypCard.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using UniVue.Model;

namespace HappyCard.Entities
{
    /*
    2024/05/11 15:29:04 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class RoomInfo : BaseModel
    {
        private string _id;
        private int _shouldpeoplenum;
        private string _ownername;
        private List<Player> _players;
        private Gameplay _gameplay;

        public RoomInfo() { }

        public RoomInfo(string id,int shouldPeopleNum,string ownerName, Gameplay gameplay)
        {
            _id = id;
            _shouldpeoplenum = shouldPeopleNum;
            _players = new List<Player>(shouldPeopleNum);
            _ownername = ownerName;
            PlayerGamingInfos = new(); //初始化
            _gameplay = gameplay;
        }

        /// <summary>
        /// 房间的游戏玩法
        /// </summary>
        public Gameplay Gameplay {
            get => _gameplay;
            set
            {
                if (_gameplay != value)
                {
                    _gameplay = value;
                    NotifyUIUpdate(nameof(Gameplay), (int)value);
                }
            }
        }

        /// <summary>
        /// 房间中的人数
        /// </summary>
        public List<Player> Players => _players;

        /// <summary>
        /// 房间中每个玩家的游戏对局信息
        /// </summary>
        public List<GamingInfo> PlayerGamingInfos { get; set; }

        /// <summary>
        /// 斗地主剩余的那三张牌
        /// </summary>
        public PokerCard[] RemainingCards { get; set; }

        /// <summary>
        /// 房间ID，为房主的本机IP+端口
        /// </summary>
        public string ID
        {
            get => _id;
            set
            {
                if(_id != value)
                {
                    _id = value;
                    NotifyUIUpdate(nameof(ID), value);
                }
            }
        }

        /// <summary>
        /// 当前房间是否已经满员
        /// </summary>
        [JsonIgnore]
        public bool Full => ShouldPeopleNum == CurrentPeopleNum;

        /// <summary>
        /// 应该有的房间人数
        /// </summary>
        public int ShouldPeopleNum
        {
            get => _shouldpeoplenum;
            set
            {
                if(_shouldpeoplenum != value)
                {
                    _shouldpeoplenum = value;
                    NotifyUIUpdate(nameof(ShouldPeopleNum), value);
                }
            }
        }

        /// <summary>
        /// 当前房间中的人数
        /// </summary>
        public int CurrentPeopleNum
        {
            get => Players.Count;
        }

        /// <summary>
        /// 房主的名称
        /// </summary>
        public string OwnerName
        {
            get => _ownername;
            set
            {
                if(_ownername != value)
                {
                    _ownername = value;
                    NotifyUIUpdate(nameof(OwnerName), value);
                }
            }
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(ID), ID);
            NotifyUIUpdate(nameof(ShouldPeopleNum), ShouldPeopleNum);
            NotifyUIUpdate(nameof(CurrentPeopleNum), CurrentPeopleNum);
            NotifyUIUpdate(nameof(OwnerName), OwnerName);
        }

        #endregion

        #region 重写父类UpdateModel()函数


        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if(nameof(ShouldPeopleNum).Equals(propertyName)){ ShouldPeopleNum = propertyValue; }
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
            if(nameof(ID).Equals(propertyName)){ ID = propertyValue; }
            else if(nameof(OwnerName).Equals(propertyName)){ OwnerName = propertyValue; }
        }

        public override void UpdateModel(string propertyName, float propertyValue)
        {
        }

        public override void UpdateModel(string propertyName, bool propertyValue)
        {
        }
        #endregion
    }
}
