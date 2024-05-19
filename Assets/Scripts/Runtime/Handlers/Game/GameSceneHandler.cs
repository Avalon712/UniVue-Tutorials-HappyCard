using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Entities;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    [EventCallAutowire(true,nameof(GameScene.Game))]
    public sealed class GameSceneHandler : EventRegister
    {
        /// <summary>
        /// 当前以及准备好了的人数
        /// </summary>
        private int _prepareCounter;

        public GameSceneHandler()
        {
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.Prepare, ReceivePlayerPrepare);
            
            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);

            InitScene();
        }

        /// <summary>
        /// 初始化场景中的一些基本信息
        /// </summary>
        private void InitScene()
        {
            //1. 安排每个玩家的位置顺序以及绑定数据
            List<Player> players = GameDataManager.Instance.RoomInfo.Players;
            string selfID = GameDataManager.Instance.Player.ID;
            int index = players.FindIndex(p => p.ID == selfID); //获取当前玩家的顺序
            ArrangePos(index, players);

            //2. 初始化准备计数器
            _prepareCounter = players.Count;

  
        }

        /// <summary>
        /// 安排每个玩家的位置
        /// </summary>
        /// <param name="selfIndex">当前玩家自己的索引顺序</param>
        /// <param name="players">房间中的所有玩家</param>
        private void ArrangePos(int selfIndex,List<Player> players)
        {
            //视图绑定玩家的顺序
            string[] views = new string[] 
            {
                nameof(GameUI.LeftPlayerView1),
                nameof(GameUI.RightPlayerView1),
                nameof(GameUI.RightPlayerView2),
                nameof(GameUI.LeftPlayerView2)
            };

            int nextPlayerIndex = (selfIndex + 1) % players.Count;
            int viewIndex = 0;

            while (nextPlayerIndex != selfIndex)
            {
                IView view = Vue.Router.GetView(views[viewIndex++]);
                view.viewObject.SetActive(true);

                //绑定数据
                Player p = players[nextPlayerIndex];
                GamingInfo gamingInfo = GameDataManager.Instance.RoomInfo.PlayerGamingInfos.Find(g => g.PlayerID == p.ID);
                view.BindModel(gamingInfo, false);

                //注册处理器
                new PlayerHandler(gamingInfo, view.name);

                //获取下一个玩家的位置索引
                nextPlayerIndex = (nextPlayerIndex+1) % players.Count;
            }
        }

        //-------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// 接收到玩家准备信息
        /// </summary>
        private void ReceivePlayerPrepare(SyncInfo info)
        {
            _prepareCounter -= 1;
            
            bool isOwner = GameDataManager.Instance.RoomInfo.OwnerName == GameDataManager.Instance.Player.Name;
            if (_prepareCounter == 0 && isOwner)
            {
                StartDealCards();
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.ShowPlayerInfo))]
        private void ShowPlayerInfo(string id)
        {
            LogHelper.Info($"\"显示玩家信息-ShowPlayerInfo\"事件触发,显示玩家信息的玩家的ID={id}");
        }

        //-------------------------------------------------------------------------------------------------
        
        [EventCall(nameof(GameEvent.ShowSelfInfo))]
        private void ShowSelfInfo()
        {
            LogHelper.Info($"\"显示玩家自己的信息-ShowSelfInfo\"事件触发");
            
            //由于PlayerInfoView这个视图下的BaseInfoView以及BattleInfoView都是通用视图，
            //有可能上一次显示的是好友的信息，因此需要重新进行绑定
            Vue.Router.GetView(nameof(GameUI.BaseInfoView)).RebindModel(GameDataManager.Instance.Player);
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).RebindData(GameDataManager.Instance.BattleRecord);
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.PauseGame))]
        private void PauseGame()
        {
            LogHelper.Info($"\"暂停游戏-PauseGame\"事件触发");
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.ModifyGameSetting))]
        private void ModifyGameSetting()
        {
            LogHelper.Info($"\"修改房间中的游戏设置-ModifyGameSetting\"事件触发");
        }

        //-------------------------------------------------------------------------------------------------
        
        [EventCall(nameof(GameEvent.Chat))]
        private void Chat(string message)
        {
            LogHelper.Info($"\"聊天-Chat\"事件触发, 发送的聊天内容={message}");

            //创建聊天信息
            ChatInfo chatInfo = new ChatInfo() { Speaker = GameDataManager.Instance.Player.Name, Message = message };
            //创建同步消息
            SyncInfo syncInfo = new SyncInfo(GameEvent.Chat, null,JsonConvert.SerializeObject(chatInfo));

            //显示聊天消息
            Vue.Router.GetView<ListView>(nameof(GameUI.ChatView)).AddData(chatInfo);
            //发送同步消息
            NetworkManager.Instance.SendSyncInfo(syncInfo);
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Prepare))]
        private void Prepare()
        {
            LogHelper.Info("\"准备开始游戏-Prepare\"事件触发");
            _prepareCounter -= 1;

            //发送准备同步信息
            SyncInfo syncInfo = new(GameEvent.Prepare, null, null);
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //如果当前房间中所有的玩家都已经准备好了就开始发牌
            bool isOwner = GameDataManager.Instance.RoomInfo.OwnerName == GameDataManager.Instance.Player.Name;
            if (_prepareCounter==0 && isOwner)
            {
                StartDealCards();
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.GameOver))]
        private void GameOver()
        {
            //重置计数器
            _prepareCounter = GameDataManager.Instance.RoomInfo.Players.Count;
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 开始发牌，只有房主才有权限发牌
        /// </summary>
        private void StartDealCards()
        {
            List<GamingInfo> gamingInfos = GameDataManager.Instance.RoomInfo.PlayerGamingInfos;

            Dictionary<string, object> data = new Dictionary<string, object>(5);

            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            if (roomInfo.Gameplay == Gameplay.FightLandord)
            {
                PokerCard[] remaining;
                List<PokerCard>[] pokerCards = PokerHelper.RandomShuffleForFightLandlord(out remaining);

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
                List<PokerCard>[] pokerCards = PokerHelper.RandomShuffleForBanZiPao();

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
            Vue.Event.GetRegister<PlayerSelfHandler>().DealCards();
            using(var it = Vue.Event.GetRegisters<PlayerHandler>().GetEnumerator())
            {
                while (it.MoveNext()) {it.Current.DealCards();}
            }
        }
    }
}
