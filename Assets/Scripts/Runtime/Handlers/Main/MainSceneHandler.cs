using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class MainSceneHandler : EventRegister
    {

        public MainSceneHandler()
        {
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;
            handle.AddResponseHandle(GameEvent.Invite, ReceiveInvite)
                  .AddResponseHandle(GameEvent.AcceptApplyForJoinRoom, ReceiveAcceptApplyForJoinRoom)
                  .AddResponseHandle(GameEvent.RefuseApplyForJoinRoom, ReceiveRefuseApplyForJoinRoom);

            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);
        }


        //----------------------------------------------------------------------------------------------------------

        //邀请玩家只能在广域网下进行
        #region 接收到玩家邀请加入房间事件

        private void ReceiveInvite(SyncInfo syncInfo)
        {
            //解析数据
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(syncInfo.Message);
            
            //创建一个临时对象
            Player player = new Player()
            {
                Name = data[nameof(player.Name)],
                HeadIconName = data[nameof(player.HeadIconName)]
            };
            RoomInfo roomInfo = new RoomInfo()
            {
                ID = data[nameof(roomInfo.ID)]
            };

            //绑定数据
            Vue.Router.GetView(nameof(GameUI.FriendInvitNotifyView)).BindModel(player, false, null, true)
                .BindModel(roomInfo, false, null, true);

            //打开
            Vue.Router.Open(nameof(GameUI.FriendInvitNotifyView));

        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 显示玩家自己的信息事件

        [EventCall(nameof(GameEvent.ShowSelfInfo))]
        private void ShowSelfInfo()
        {
            LogHelper.Info("\"显示玩家自己信息-ShowSelfInfo\"事件触发!");

            //由于PlayerInfoView这个视图下的BaseInfoView以及BattleInfoView都是通用视图，
            //有可能上一次显示的是好友的信息，因此需要重新进行绑定
            Vue.Router.GetView(nameof(GameUI.BaseInfoView)).RebindModel(GameDataManager.Instance.Player);
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).RebindData(GameDataManager.Instance.BattleRecord);
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 更改玩家的头像
        [EventCall(nameof(GameEvent.ModifyHeadIcon))]
        private void ModifyHeadIcon(Sprite icon)
        {
            LogHelper.Info($"\"更改玩家的头像-ModifyHeadIcon\"事件触发，新的头像的名称{icon.name}");
            GameDataManager.Instance.Player.HeadIcon = icon;
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 更改玩家的名字
        [EventCall(nameof(GameEvent.ModifyName))]
        private void ModifyName(string newName)
        {
            LogHelper.Info($"\"更改玩家的名字-ModifyName\"事件触发，玩家新的名称{newName}");
            GameDataManager.Instance.Player.Name = newName;
        }
        #endregion
        
        //----------------------------------------------------------------------------------------------------------

        #region 创建房间事件
        [EventCall(nameof(GameEvent.CreateRoom))]
        private void CreateRoom()
        {
            LogHelper.Info("\"创建房间-CreateRoom\"事件触发!");

            if (Vue.Event.GetRegister<PropHandler>().UseProp(PropType.RoomCard))
            {
                //创建房间信息
                Player player = GameDataManager.Instance.Player;
                string roomId = IPEndPointHelper.EndPointToString(NetworkManager.Instance.GetHostEndPoint());
                Gameplay gameplay = GameDataManager.Instance.GameSetting.Gameplay;
                int shouldPeopleNum = (int)gameplay + 3;
                RoomInfo roomInfo = new RoomInfo(roomId, shouldPeopleNum, player.Name, gameplay);

                roomInfo.Players.Add(player);

                GameDataManager.Instance.RoomInfo = roomInfo;

                //添加对局信息
                roomInfo.PlayerGamingInfos.Add(new GamingInfo(player));

                //切换到Room场景
                SceneManager.LoadScene(nameof(GameScene.Room));
            }
            else
            {
                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("创建房间失败！你还没有房卡呢，快去买一张吧！");
            }
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 玩家申请加入房间事件

        [EventCall(nameof(GameEvent.ApplyForJoinRoom))]
        private void ApplyForJoinRoom(string roomId)
        {
            LogHelper.Info($"\"加入房间-JoinRoom\"事件触发: 房间ID={roomId}!");

            Player player = GameDataManager.Instance.Player;
            Dictionary<string, string> data = new()
            {
                {nameof(player.ID) , player.ID },
                {nameof(player.Name),player.Name },
                {nameof(player.HeadIconName), player.HeadIconName},
            };
            string message = JsonConvert.SerializeObject(data);
            SyncInfo syncInfo = new SyncInfo(GameEvent.ApplyForJoinRoom, null, message);

            //同步消息响应处理
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(2);
            handle.AddResponseHandle(GameEvent.AcceptApplyForJoinRoom, ReceiveAcceptApplyForJoinRoom)
                .AddResponseHandle(GameEvent.RefuseApplyForJoinRoom, ReceiveRefuseApplyForJoinRoom);

            if(NetworkManager.Instance.GetGameNetworkServiceMode() == NetworkServiceMode.WAN)
            {
                syncInfo.Message = roomId;
                NetworkManager.Instance.SendSyncInfo(syncInfo,ref handle);
            }
            else
            {
                //局域网下的roomId就是房主的IP地址+端口
                EndPoint endPoint = IPEndPointHelper.StringToEndPoint(roomId);
                if(endPoint == null)
                {
                    Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("输入的房间号有误，请重新进行输入!");
                }
                else
                {
                    NetworkManager.Instance.SendSyncInfo(syncInfo, endPoint, ref handle);
                }
            }

        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 拒绝玩家加入房间事件
        private void ReceiveRefuseApplyForJoinRoom(SyncInfo syncInfo)
        {
            LogHelper.Info($"\"拒绝玩家加入房间-RefuseApplyForJoinRoom\"事件触发,响应消息: {syncInfo.Message}");

            Vue.Router.GetView<TipView>("玩家暂时没有响应您的请求~");
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------

        #region 接受玩家加入房间的申请事件
        private void ReceiveAcceptApplyForJoinRoom(SyncInfo syncInfo)
        {
            LogHelper.Info($"\"接受玩家加入房间-AcceptApplyForJoinRoom\"事件触发");

            RoomInfo roomInfo = JsonConvert.DeserializeObject<RoomInfo>(syncInfo.Message);
            GameDataManager.Instance.RoomInfo = roomInfo;
            Player player = GameDataManager.Instance.Player;
            roomInfo.Players.Add(player);
            roomInfo.PlayerGamingInfos.Add(new GamingInfo(player)) ;

            //发送一个进入房间事件
            string message = JsonConvert.SerializeObject(player);
            SyncInfo syncResponse = new SyncInfo(GameEvent.EnterRoom, null, message);
            NetworkManager.Instance.SendSyncInfo(syncResponse);

            //切换到Room场景
            SceneManager.LoadScene(nameof(GameScene.Room));
        }
        #endregion

        //-----------------------------------------------------------------------------------------------------------

        #region 显示玩家信息事件
        [EventCall(nameof(GameEvent.ShowPlayerInfo))]
        private void ShowPlayerInfo(string id)
        {
            LogHelper.Info($"\"显示玩家信息-ShowPlayerInfo\"事件触发,显示玩家信息的玩家的ID={id}");
        }
        #endregion

        //-----------------------------------------------------------------------------------------------------------

        #region 玩家接受邀请事件
        [EventCall(nameof(GameEvent.AcceptInvite))]
        private void AcceptInvite(string roomId)
        {
            LogHelper.Info($"\"玩家接受邀请-AcceptInvite\"事件触发,房间ID={roomId}");
        }
        #endregion

        //-----------------------------------------------------------------------------------------------------------

        #region 玩家拒绝接受邀请事件
        [EventCall(nameof(GameEvent.RefuseInvite))]
        private void RefuseInvite()
        {
            LogHelper.Info("\"玩家拒绝接受邀请-RefuseInvite\"事件触发");
        }
        #endregion
    }
}
