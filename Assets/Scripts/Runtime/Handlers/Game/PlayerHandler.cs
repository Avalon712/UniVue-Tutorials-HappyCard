using HappyCard.Entities;
using HappyCard.Enums;
using HayypCard.Network.Entities;
using System;
using UniVue.Evt;

namespace HayypCard.Handlers
{
    /// <summary>
    /// 玩家的控制器，显示玩家的出牌、状态
    /// </summary>
    public sealed class PlayerHandler : EventRegister
    {
        private GamingInfo _gamingInfo;

        public PlayerHandler(GamingInfo gamingInfo) 
        {
            _gamingInfo = gamingInfo;

            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.DealCards, ReceiveDealCards);
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到发牌同步事件
        /// </summary>
        private void ReceiveDealCards(SyncInfo syncInfo)
        {

        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 本地同步
        /// </summary>
        public void DealCards()
        {
            
        }
    }
}
