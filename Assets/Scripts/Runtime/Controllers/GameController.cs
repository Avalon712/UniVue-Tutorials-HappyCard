using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.Controllers
{
    /// <summary>
    /// 这个类负责控制游戏的开始、结束等同步玩家游戏事件
    /// </summary>
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class GameController : EventRegister
    {        
        /// <summary>
        /// 当前以及准备好了的人数
        /// </summary>
        private int _preparedCounter;

        /// <summary>
        /// 当前玩家选中的扑克
        /// </summary>
        public List<PokerCard> Selected { get; private set; }

        public GameController()
        {
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.Prepare, ReceivePlayerPrepare);

            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);
        }

        //------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到玩家准备信息
        /// </summary>
        private void ReceivePlayerPrepare(SyncInfo info)
        {
            _preparedCounter -= 1;

            bool isOwner = GameDataManager.Instance.RoomInfo.OwnerName == GameDataManager.Instance.Player.Name;
            if (_preparedCounter == 0 && isOwner)
            {
                DealCards();
            }
        }

        //------------------------------------------------------------------------------------------------------

        public void Prepare()
        {
            //LogHelper.Info("\"准备开始游戏-Prepare\"事件触发");
            
            _preparedCounter -= 1;

            //发送准备同步信息
            SyncInfo syncInfo = new(GameEvent.Prepare, null, null);
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //如果当前房间中所有的玩家都已经准备好了就开始发牌
            bool isOwner = GameDataManager.Instance.RoomInfo.OwnerName == GameDataManager.Instance.Player.Name;
            if (_preparedCounter == 0 && isOwner)
            {
                DealCards();
            }
        }

        //------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 发牌，只有房主才有权限发牌
        /// </summary>
        private void DealCards()
        {
            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            List<GamingInfo> gamingInfos = roomInfo.PlayerGamingInfos;
            Dictionary<string, object> data = new Dictionary<string, object>(5);
            GameSetting setting = GameDataManager.Instance.GameSetting;

            if (roomInfo.Gameplay == Gameplay.FightLandord)
            {
                PokerCard[] remaining;
                List<PokerCard>[] pokerCards = setting.ShuffleMode == ShuffleMode.Random ?
                                PokerHelper.Shuffle(out remaining) : PokerHelper.NoShuffle(out remaining);

                data.Add(nameof(roomInfo.RemainingCards), remaining);

                for (int i = 0; i < gamingInfos.Count; i++)
                {
                    gamingInfos[i].OriginalCards = pokerCards[i];
                    data.Add(gamingInfos[i].PlayerID, pokerCards[i]);
                }

                roomInfo.RemainingCards = remaining;
            }
            else
            {
                List<PokerCard>[] pokerCards = setting.ShuffleMode == ShuffleMode.Random ?
                    PokerHelper.Shuffle() : PokerHelper.NoShuffle();

                for (int i = 0; i < gamingInfos.Count; i++)
                {
                    gamingInfos[i].OriginalCards = pokerCards[i];
                    data.Add(gamingInfos[i].PlayerID, pokerCards[i]);
                }
            }

            //网络同步
            string message = JsonConvert.SerializeObject(data);
            SyncInfo syncInfo = new SyncInfo(GameEvent.DealCards, null, message);
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //本地同步
            //Vue.Event.GetRegister<PlayerSelfHandler>().DealCards();
            //using (var it = Vue.Event.GetRegisters<PlayerHandler>().GetEnumerator())
            //{
            //    while (it.MoveNext()) { it.Current.DealCards(); }
            //}
        }
    }

}
