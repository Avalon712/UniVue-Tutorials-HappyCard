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
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 回合同步
        /// </summary>
        public void BoutSync(BoutRecord bout)
        {

        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 本地同步发牌事件
        /// </summary>
        public void DealCards()
        {
            
        }
    }
}
