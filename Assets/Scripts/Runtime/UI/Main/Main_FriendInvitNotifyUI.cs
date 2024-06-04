using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class Main_FriendInvitNotifyUI : EventRegister
    {
        public Main_FriendInvitNotifyUI()
        {
            //注册接受网络同步事件
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;
            handle.AddResponseHandle(GameEvent.Invite, ReceiveInvite);

            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);
        }

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
            Vue.Router.GetView(nameof(HappyCard.Enums.GameUI.FriendInvitNotifyView)).BindModel(player, false, null, true)
                .BindModel(roomInfo, false, null, true);

            //打开
            Vue.Router.Open(nameof(HappyCard.Enums.GameUI.FriendInvitNotifyView));

        }
        #endregion

        #region 玩家接受邀请事件

        [EventCall(nameof(GameEvent.AcceptInvite))]
        private void AcceptInvite(string roomId)
        {
            LogHelper.Info($"\"玩家接受邀请-AcceptInvite\"事件触发,房间ID={roomId}");
        }

        #endregion

        #region 玩家拒绝接受邀请事件

        [EventCall(nameof(GameEvent.RefuseInvite))]
        private void RefuseInvite()
        {
            LogHelper.Info("\"玩家拒绝接受邀请-RefuseInvite\"事件触发");
        }

        #endregion
    }
}
