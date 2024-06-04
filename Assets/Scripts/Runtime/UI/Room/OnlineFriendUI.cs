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
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Room))]
    public sealed class OnlineFriendUI : EventRegister
    {
        #region 邀请玩家加入房间事件

        [EventCall(nameof(GameEvent.Invite))]
        private void Invite(string id)
        {
            LogHelper.Info($"\"邀请玩家加入房间-Invite\"事件触发, 好友ID={id}");

            //判断当前房间是否已经满员
            if (GameDataManager.Instance.RoomInfo.Full)
            {
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.FastTipView)).Open("当前房间已经满员啦，无法再邀请更多的好友!");
            }
            else
            {
                //数据
                Player player = GameDataManager.Instance.Player;
                Dictionary<string, string> data = new()
                {
                    { nameof(player.Name),player.Name},
                    {"ID",GameDataManager.Instance.RoomInfo.ID },
                    {nameof(player.HeadIconName),GameDataManager.Instance.Player.HeadIconName }
                };
                string message = JsonConvert.SerializeObject(data);

                SyncInfo syncInfo = new SyncInfo(GameEvent.Invite, id, message);

                NetworkManager.Instance.SendSyncInfo(syncInfo);
            }
        }
        #endregion
    }
}
