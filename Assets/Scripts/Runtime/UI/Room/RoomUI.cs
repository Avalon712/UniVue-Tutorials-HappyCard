﻿using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Tween;
using UniVue.Utils;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Room))]
    public sealed class RoomUI : EventRegister
    {
        public RoomUI()
        {
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(7);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.ApplyForJoinRoom, ReceivePlayerApplyFromJoinRoom)
                  .AddResponseHandle(GameEvent.EnterRoom, ReceivePlayerEnterRoom)
                  .AddResponseHandle(GameEvent.QuitRoom, ReceivePlayerQuitRoom)
                  .AddResponseHandle(GameEvent.KickOutRoom, ReceivePlayerKickOutRoom)
                  .AddResponseHandle(GameEvent.RefuseInvite, ReceiveRefuseInvite)
                  .AddResponseHandle(GameEvent.StartGame, ReceiveStartGame)
                  .AddResponseHandle(GameEvent.DestroyRoom, ReciveDestroyRoom);

            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);

            ShowOperationBtns(GameDataManager.Instance.Player.Name == GameDataManager.Instance.RoomInfo.OwnerName);
        }

        #region 根据当前玩家的身份显示底部状态栏中玩家可操作的按钮

        private void ShowOperationBtns(bool isRoomOwner)
        {
            GameObject viewObject = Vue.Router.GetView(nameof(HappyCard.Enums.GameUI.OperationView)).viewObject;
            string[] operations = { "Evt_QuitRoom_Btn", "Evt_StartGame_Btn", "Evt_DestroyRoom_Btn" };
            if (isRoomOwner)
            {
                using (var it = GameObjectFindUtil.BreadthFind(viewObject, operations).GetEnumerator())
                {
                    while (it.MoveNext())
                    {
                        it.Current.SetActive(it.Current.name != operations[0]);
                    }
                }
            }
            else
            {
                using (var it = GameObjectFindUtil.BreadthFind(viewObject, operations).GetEnumerator())
                {
                    while (it.MoveNext())
                    {
                        it.Current.SetActive(it.Current.name == operations[0]);
                    }
                }
            }
        }
        #endregion

        #region 网络消息同步事件处理
        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void ReceiveStartGame(SyncInfo syncInfo)
        {
            List<string> sequence = JsonConvert.DeserializeObject<List<string>>(syncInfo.Message);
            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            List<Player> players = roomInfo.Players;
            List<GamingInfo> gamingInfos = roomInfo.PlayerGamingInfos;

            //保证每个玩家那里的玩家的顺序都是一致的
            MakeTheSameSequence(players, sequence);
            MakeTheSameSequence(gamingInfos, sequence);

            //切换到Game场景
            SceneManager.LoadScene(nameof(GameScene.Game));
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到房间销毁事件
        /// </summary>
        private void ReciveDestroyRoom(SyncInfo info)
        {
            GameDataManager.Instance.RoomInfo = null;

            Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.FastTipView)).Open($"房主销毁了房间,3秒后将自动离开房间!");

            //添加定时任务,3秒后自动离开房间
            TweenBehavior.Timer(() => SceneManager.LoadScene(nameof(GameScene.Main))).Delay(3);
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 玩家拒绝加入房间
        /// </summary>
        private void ReceiveRefuseInvite(SyncInfo info)
        {
            Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.FastTipView)).Open("玩家暂时没有响应你的邀请~");
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到玩家申请加入房间的消息
        /// </summary>
        private void ReceivePlayerApplyFromJoinRoom(SyncInfo syncInfo)
        {
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(syncInfo.Message);
            Player temp = new Player() { ID = data[nameof(temp.ID)], Name = data[nameof(temp.Name)], HeadIconName = data[nameof(temp.HeadIconName)] };
            Vue.Router.GetView(nameof(HappyCard.Enums.GameUI.PlayerJoinRoomNotifyView)).BindModel(temp, false, null, true);
            Vue.Router.Open(nameof(HappyCard.Enums.GameUI.PlayerJoinRoomNotifyView));
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到玩家加入房间的消息
        /// </summary>
        private void ReceivePlayerEnterRoom(SyncInfo syncInfo)
        {
            Player enterPlayer = JsonConvert.DeserializeObject<Player>(syncInfo.Message);
            GameDataManager.Instance.RoomInfo.Players.Add(enterPlayer);
            GameDataManager.Instance.RoomInfo.PlayerGamingInfos.Add(new GamingInfo(enterPlayer));
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到玩家离开房间的消息
        /// </summary>
        private void ReceivePlayerQuitRoom(SyncInfo syncInfo)
        {
            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            Player quit = roomInfo.Players.Find((p) => p.ID == syncInfo.Message);
            roomInfo.Players.Remove(quit);
            roomInfo.PlayerGamingInfos.Remove(roomInfo.PlayerGamingInfos.Find(g => g.PlayerID == quit.ID));

            //刷新视图
            Vue.Router.GetView<ListView>(nameof(HappyCard.Enums.GameUI.PlayersView)).RemoveData(quit);
            //显示提示消息
            Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.FastTipView)).Open($"玩家{quit.Name}离开了房间");
        }

        //----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到玩家被房主踢出房间的消息
        /// </summary>
        private void ReceivePlayerKickOutRoom(SyncInfo syncInfo)
        {

        }

        #endregion

        #region UI事件处理 

        #region 开始游戏

        [EventCall(nameof(GameEvent.StartGame))]
        private void StartGame()
        { 
            //切换到Game场景
            SceneManager.LoadScene(nameof(GameScene.Game));
            
            //Gameplay gameplay = GameDataManager.Instance.GameSetting.Gameplay;

            //if ((!GameDataManager.Instance.RoomInfo.Full && gameplay != Gameplay.ZhaJinHua) || (gameplay == Gameplay.ZhaJinHua && GameDataManager.Instance.RoomInfo.CurrentPeopleNum < 2))
            //{
            //    Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("当前人数不够,再邀请几个好友一起玩吧!");
            //}
            //else
            //{
            //    //发送同步，将房主的好友顺序位置同步到所有玩家（确保每个人在牌桌上的顺序是一致的）
            //    RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            //    List<Player> players = roomInfo.Players;
            //    List<GamingInfo> gamingInfos = roomInfo.PlayerGamingInfos;

            //    List<string> sequence = new List<string>(players.Count);
            //    for (int i = 0; i < players.Count; i++) { sequence.Add(players[i].ID); }
            //    string message = JsonConvert.SerializeObject(sequence);

            //    //保证List<GamingInfo>的顺序也是与序列顺序一致的
            //    MakeTheSameSequence(gamingInfos, sequence);

            //    SyncInfo syncInfo = new SyncInfo() { GameEvent = GameEvent.StartGame, Message = message };
            //    NetworkManager.Instance.SendSyncInfo(syncInfo);

            //    //切换到Game场景
            //    SceneManager.LoadScene(nameof(GameScene.Game));
            //}
        }
        #endregion

        #region 玩家退出房间
        [EventCall(nameof(GameEvent.QuitRoom))]
        private void QuitRoom()
        {
            //LogHelper.Info($"\"玩家离开房间-QuitRoom\"事件触发");
            Vue.Router.GetView<EnsureTipView>(nameof(GameUI.EnsureTipView)).Open("提示", "确定要离开房间吗?", EnsureQuitRoom, CloseEnsureTipView);
        }
        #endregion

        #region 房主销毁房间
        [EventCall(nameof(GameEvent.DestroyRoom))]
        private void DestroyRoom()
        {
            //LogHelper.Info($"\"房主销毁房间-DestroyRoom\"事件触发");

            bool isOwner = GameDataManager.Instance.Player.Name == GameDataManager.Instance.RoomInfo.OwnerName;
            string message = isOwner ? "你当前是房主,离开房间将导致房间销毁，确认要离开房间吗?" : "确认要离开房间吗?";
            Vue.Router.GetView<EnsureTipView>(nameof(GameUI.EnsureTipView)).Open("提示", message, EnsureQuitRoom, CloseEnsureTipView);
        }
        #endregion

        #region 关闭EnsureTipView视图
        private void CloseEnsureTipView()
        {
            Vue.Router.Close(nameof(GameUI.EnsureTipView));
        }
        #endregion

        #endregion

        #region 确认退出房间
        private void EnsureQuitRoom()
        {
            bool isOwner = GameDataManager.Instance.Player.Name == GameDataManager.Instance.RoomInfo.OwnerName;
            GameEvent gameEvent = isOwner ? GameEvent.DestroyRoom : GameEvent.QuitRoom;
            SyncInfo syncInfo = new SyncInfo(gameEvent, null, null);
            NetworkManager.Instance.SendSyncInfo(syncInfo);
            SceneManager.LoadScene(nameof(GameScene.Main));
        }
        #endregion

        #region 使每个玩家的顺序在每个玩家的手机/PC上的顺序一致
        private void MakeTheSameSequence(List<Player> players, List<string> sequence)
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                string pID = sequence[i];
                for (int j = 0; j < players.Count; j++)
                {
                    if (players[j].ID == pID && i != j)
                    {
                        //位置互换
                        Player pj = players[j];
                        Player pi = players[i];
                        players[i] = pj;
                        players[j] = pi;
                    }
                }
            }
        }

        private void MakeTheSameSequence(List<GamingInfo> gamingInfos, List<string> sequence)
        {
            for (int i = 0; i < sequence.Count; i++)
            {
                string pID = sequence[i];
                for (int j = 0; j < gamingInfos.Count; j++)
                {
                    if (gamingInfos[j].PlayerID == pID && i != j)
                    {
                        //位置互换
                        GamingInfo gj = gamingInfos[j];
                        GamingInfo gi = gamingInfos[i];
                        gamingInfos[i] = gj;
                        gamingInfos[j] = gi;
                    }
                }
            }
        }

        #endregion
    }
}
