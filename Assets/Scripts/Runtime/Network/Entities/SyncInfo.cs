using HappyCard.Enums;
using HappyCard.Managers;

namespace HayypCard.Network.Entities
{
    public sealed class SyncInfo 
    {
        /// <summary>
        /// 游戏事件
        /// </summary>
        public GameEvent GameEvent { get; set; }

        /// <summary>
        /// 发送此同步消息的玩家的ID
        /// </summary>
        public string SenderID { get;set; }

        /// <summary>
        /// 接受此同步消息的玩家的ID，如果为null则进行房间广播
        /// </summary>
        public string ReceiverID { get; set; }

        /// <summary>
        /// 同步消息内容
        /// </summary>
        public string Message { get; set; }

        public SyncInfo() { }

        public SyncInfo(GameEvent gameEvent,string receiverID,string message) 
        {
            GameEvent = gameEvent;
            ReceiverID = receiverID;
            SenderID = GameDataManager.Instance.Player.ID;
        }
    }
}
