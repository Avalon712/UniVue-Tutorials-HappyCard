using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class JoinRoomUI : EventRegister
    {

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

            if (NetworkManager.Instance.GetGameNetworkServiceMode() == NetworkServiceMode.WAN)
            {
                syncInfo.Message = roomId;
                NetworkManager.Instance.SendSyncInfo(syncInfo, ref handle);
            }
            else
            {
                //局域网下的roomId就是房主的IP地址+端口
                EndPoint endPoint = IPEndPointHelper.StringToEndPoint(roomId);
                if (endPoint == null)
                {
                    Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("输入的房间号有误，请重新进行输入!");
                }
                else
                {
                    NetworkManager.Instance.SendSyncInfo(syncInfo, endPoint, ref handle);
                }
            }

        }
        #endregion

        #region 接收到"拒绝玩家加入房间"事件的回调
        private void ReceiveRefuseApplyForJoinRoom(SyncInfo syncInfo)
        {
            LogHelper.Info($"\"拒绝玩家加入房间-RefuseApplyForJoinRoom\"事件触发,响应消息: {syncInfo.Message}");

            Vue.Router.GetView<TipView>("玩家暂时没有响应您的请求~");
        }
        #endregion

        #region 接收到"接受玩家加入房间"的事件回调
        private void ReceiveAcceptApplyForJoinRoom(SyncInfo syncInfo)
        {
            LogHelper.Info($"\"接受玩家加入房间-AcceptApplyForJoinRoom\"事件触发");

            RoomInfo roomInfo = JsonConvert.DeserializeObject<RoomInfo>(syncInfo.Message);
            GameDataManager.Instance.RoomInfo = roomInfo;
            Player player = GameDataManager.Instance.Player;
            roomInfo.Players.Add(player);
            roomInfo.PlayerGamingInfos.Add(new GamingInfo(player));

            //发送一个进入房间事件
            string message = JsonConvert.SerializeObject(player);
            SyncInfo syncResponse = new SyncInfo(GameEvent.EnterRoom, null, message);
            NetworkManager.Instance.SendSyncInfo(syncResponse);

            //切换到Room场景
            SceneManager.LoadScene(nameof(GameScene.Room));
        }
        #endregion
    }
}
