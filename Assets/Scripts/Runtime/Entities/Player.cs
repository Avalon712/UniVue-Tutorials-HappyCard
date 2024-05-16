using UniVue.Model;
using UnityEngine;
using HappyCard.Enums;
using Newtonsoft.Json;
using HappyCard.Managers;
using HayypCard.Enums;
using System.Collections.Generic;

namespace HappyCard.Entities
{
    /*
    2024/05/01 09:53:39 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class Player : BaseModel
    {
        private string _id;
        private string _name;
        private int _level;
        private int _exp;
        private int _hp;
        private string _headIconName;//玩家的头像的图标的名称
        private int _coin;
        private int _diamond;
        private int _likes;
        private int _days;
        private GameTag _tags;
        private PlayerState _state;

        private List<Sprite> _usedPropIcons; //玩家当前使用了的道具的图标
        private Sprite _headIcon;

        /// <summary>
        /// 获得一个默认的玩家
        /// </summary>
        public static Player Default
        {
            get
            {
                Player player = new Player();
                player.UsedProps = new(); //不能为null
                player._id = (Random.value * 1_000_000).ToString();
                player._name = "Avalon712";
                player._level = 1;
                player._hp = 10;
                player._headIconName = "default";
                player._coin = 1000;
                player._diamond = 10;
                player._days = 1;
                player._tags = GameTag.None;
                return player;
            }
        }

        /// <summary>
        /// 玩家唯一标识
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
        /// 玩家的状态
        /// </summary>
        [JsonIgnore]
        public PlayerState State
        {
            get => _state;
            set
            {
                if(_state != value)
                {
                    _state = value;
                    NotifyUIUpdate(nameof(State), (int)value);
                }
            }
        }

        /// <summary>
        /// 玩家名称
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if(_name != value)
                {
                    _name = value;
                    NotifyUIUpdate(nameof(Name), value);
                }
            }
        }

        /// <summary>
        /// 玩家等级
        /// </summary>
        public int Level
        {
            get => _level;
            set
            {
                if(_level != value)
                {
                    _level = value;
                    NotifyUIUpdate(nameof(Level), value);
                }
            }
        }

        /// <summary>
        /// 玩家当前等级的经验值
        /// </summary>
        public int Exp
        {
            get => _exp;
            set
            {
                if(_exp != value)
                {
                    _exp = value;
                    NotifyUIUpdate(nameof(Exp), value);
                }
            }
        }

        /// <summary>
        /// 玩家体力值每次开始一局游戏将会减少一点体力值
        /// </summary>
        public int HP
        {
            get => _hp;
            set
            {
                if(_hp != value)
                {
                    _hp = value;
                    NotifyUIUpdate(nameof(HP), value);
                }
            }
        }

        /// <summary>
        /// 玩家的头像的图标的名称
        /// </summary>
        public string HeadIconName { get => _headIconName; set => _headIconName = value; }

        /// <summary>
        /// 玩家头像
        /// </summary>
        [JsonIgnore]
        public Sprite HeadIcon
        {
            get
            {
                if(_headIcon == null)
                {
                    _headIcon = GameDataManager.Instance.HeadIcons.Find(h => h.IconName == HeadIconName)?.Icon;
                    if(_headIcon == null)
                    {
                        //没找到则显示一张默认头像
                        _headIcon = GameDataManager.Instance.HeadIcons.Find(h => h.IconName == "default")?.Icon;
                    }
                }
                return _headIcon;
            }
            set
            {
                if(_headIconName != value.name && value!=null)
                {
                    _headIconName = value.name;
                    _headIcon = value;
                    NotifyUIUpdate(nameof(HeadIcon), value);
                }
            }
        }

        /// <summary>
        /// 玩家当前金币数量
        /// </summary>
        public int Coin
        {
            get => _coin;
            set
            {
                if(_coin != value)
                {
                    _coin = value;
                    NotifyUIUpdate(nameof(Coin), value);
                }
            }
        }

        /// <summary>
        /// 当前玩家使用了的道具
        /// </summary>
        public List<PropType> UsedProps { get; set; }

        /// <summary>
        /// 当前玩家使用了的道具的图标
        /// </summary>
        [JsonIgnore]
        public List<Sprite> UsedPropIcons
        {
            get
            {
                if (_usedPropIcons == null)
                {
                    _usedPropIcons = new List<Sprite>(UsedProps.Count);
                    for (int i = 0; i < UsedProps.Count; i++)
                    {
                        _usedPropIcons.Add(GameDataManager.Instance.PropInfos[UsedProps[i]].Icon);
                    }
                }
                return _usedPropIcons;
            }
        }

        /// <summary>
        /// 显示当前玩家是否使用了道具信息，如果没有使用则显示指定的提示内容
        /// </summary>
        [JsonIgnore]
        public string IsUsingPropTip => UsedProps.Count == 0 ? "未使用任何道具" : null;

        /// <summary>
        /// 玩家拥有的钻石数量
        /// </summary>
        public int Diamond
        {
            get => _diamond;
            set
            {
                if(_diamond != value)
                {
                    _diamond = value;
                    NotifyUIUpdate(nameof(Diamond), value);
                }
            }
        }

        /// <summary>
        /// 玩家的获赞数
        /// </summary>
        public int likes
        { 
            get => _likes;
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    NotifyUIUpdate(nameof(likes), value);
                }
            }
        }

        /// <summary>
        /// 玩家的游戏天数
        /// </summary>
        public int days
        {
            get
            {
                return _days;
            }
            set
            {
                if (_days != value)
                {
                    _days = value;
                    NotifyUIUpdate(nameof(days), value);
                }
            }
        }

        /// <summary>
        /// 玩家成就
        /// </summary>
        public GameTag tags
        {
            get => _tags;
            set
            {
                if(_tags != value)
                {
                    _tags = value;
                    NotifyUIUpdate(nameof(tags), (int)value);
                }
            }
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(IsUsingPropTip), IsUsingPropTip);
            NotifyUIUpdate(nameof(UsedProps), UsedProps);
            NotifyUIUpdate(nameof(UsedPropIcons), UsedPropIcons);
            NotifyUIUpdate(nameof(ID), ID);
            NotifyUIUpdate(nameof(State), (int)State);
            NotifyUIUpdate(nameof(Name), Name);
            NotifyUIUpdate(nameof(Level), Level);
            NotifyUIUpdate(nameof(Exp), Exp);
            NotifyUIUpdate(nameof(HP), HP);
            NotifyUIUpdate(nameof(HeadIcon), HeadIcon);
            NotifyUIUpdate(nameof(Coin), Coin);
            NotifyUIUpdate(nameof(Diamond), Diamond);
            NotifyUIUpdate(nameof(likes), likes);
            NotifyUIUpdate(nameof(days), days);
            NotifyUIUpdate(nameof(tags), (int)tags);
        }

        #endregion


        #region 重写父类UpdateModel()函数

        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if(nameof(Level).Equals(propertyName)){ Level = propertyValue; }
            else if (nameof(State).Equals(propertyName)) { State = (PlayerState)propertyValue; }
            else if(nameof(Exp).Equals(propertyName)){ Exp = propertyValue; }
            else if(nameof(HP).Equals(propertyName)){ HP = propertyValue; }
            else if(nameof(Coin).Equals(propertyName)){ Coin = propertyValue; }
            else if(nameof(Diamond).Equals(propertyName)){ Diamond = propertyValue; }
            else if (nameof(likes).Equals(propertyName)) { likes = propertyValue; }
            else if (nameof(days).Equals(propertyName)) { days = propertyValue; }
            else if (nameof(tags).Equals(propertyName)) { tags = (GameTag)propertyValue; }
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
            if (nameof(ID).Equals(propertyName)) { ID = propertyValue; }
            else if (nameof(Name).Equals(propertyName)){ Name = propertyValue; }
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
