using UniVue.Model;
using UnityEngine;
using HayypCard.Enums;
using System.Collections.Generic;
using Newtonsoft.Json;
using HappyCard.Managers;

namespace HappyCard.Entities
{
    /*
    2024/05/14 09:48:16 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class GamingInfo : BaseModel
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        private PlayerGameState _currentState;
        private string _remainingCardsInfo; //剩余牌的提示信息

        public GamingInfo()
        {
        }

        public GamingInfo(Player player) 
        {
            BoutRecord = new();
            PlayerID = player.ID;
            PlayerName = player.Name;
            PlayerHeadIconName = player.HeadIconName;
        }

        /// <summary>
        /// 玩家的ID
        /// </summary>
        public string PlayerID { get; set; }

        /// <summary>
        /// 玩家的名称
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 玩家的头像的名称
        /// </summary>
        public string PlayerHeadIconName { get; set; }

        /// <summary>
        /// 记录每回合玩家出的牌，key=回合序号 value=当前回合玩家的状态信息
        /// 注意：如果要添加回合不要直接操作此对象
        /// </summary>
        public List<BoutRecord> BoutRecord { get; set; }

        /// <summary>
        /// 玩家原始的手牌(不要操作这个牌，这个牌作为记录进行对局回放)
        /// </summary>
        public List<PokerCard> OriginalCards { get; set; }

        //----------------------------------------------------------------------------------------------

        /// <summary>
        /// 玩家的头像
        /// </summary>
        [JsonIgnore]
        public Sprite PlayerHeadIcon
        {
            get
            {
                Sprite sprite = GameDataManager.Instance.HeadIcons.Find(img => img.IconName == PlayerHeadIconName)?.Icon;
                if(sprite == null)
                { 
                    PlayerHeadIconName = "default"; //显示一张默认图片
                    sprite = GameDataManager.Instance.HeadIcons.Find(img => img.IconName == PlayerHeadIconName)?.Icon;
                }
                return sprite;
            }
        }

        /// <summary>
        /// 玩家剩余的牌的数量提示，如：剩余12，在未使用道具的情况下只有当剩余3张牌时才显示
        /// </summary>
        [JsonIgnore]
        public string RemainingInfo
        {
            get => _remainingCardsInfo;
            set 
            {
                if(value != _remainingCardsInfo)
                {
                    NotifyUIUpdate(nameof(RemainingInfo), value);
                }
            }
        }

        /// <summary>
        /// 玩家当前回合中的状态，如：已玩牌、不要、叫地主、跟注等提示信息
        /// </summary>
        [JsonIgnore]
        public PlayerGameState State
        {
            get => _currentState;
            set
            {
                if(_currentState != value)
                {
                    _currentState = value;
                    NotifyUIUpdate(nameof(State), (int)State);
                }
            }
        }

        /// <summary>
        /// 添加回合记录
        /// </summary>
        public void AddBoutRecord(BoutRecord record)
        {
            BoutRecord.Add(record);
            State = record.State;
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(PlayerID), PlayerID);
            NotifyUIUpdate(nameof(PlayerName), PlayerName);
            NotifyUIUpdate(nameof(RemainingInfo), RemainingInfo);
            NotifyUIUpdate(nameof(State), (int)State);
            NotifyUIUpdate(nameof(PlayerHeadIcon), PlayerHeadIcon);
        }

        #endregion
    }

    /// <summary>
    /// 回合记录
    /// </summary>
    public sealed class BoutRecord
    {
        /// <summary>
        /// 定时器结束的时间点
        /// </summary>
        public int TimerEndPoint { get; set; }

        /// <summary>
        /// 本回合玩家的状态
        /// </summary>
        public PlayerGameState State { get; set; }

        /// <summary>
        /// 本回合玩家出的牌
        /// </summary>
        public List<PokerCard> OutCards { get; set; }

        /// <summary>
        /// 当前回合出牌的类型的类型码
        /// </summary>
        public int PokerTypeCode { get; set; }

        /// <summary>
        /// 炸金花中玩家跟注的金币数量
        /// </summary>
        public int FollowCoin { get; set; }

        /// <summary>
        /// 炸金花中玩家下注的金币数量
        /// </summary>
        public int BetCoin { get; set; }
    }
}
