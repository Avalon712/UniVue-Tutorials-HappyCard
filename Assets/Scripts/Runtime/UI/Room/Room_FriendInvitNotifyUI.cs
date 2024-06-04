using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Room))]
    public sealed class Room_FriendInvitNotifyUI : EventRegister
    {
        //接受玩家加入房间的申请
        [EventCall(nameof(GameEvent.AcceptApplyForJoinRoom))]
        private void AcceptPlayerJoinRoom(string id)
        {
            LogHelper.Info($"\"接受玩家加入房间的申请-AcceptApplyForJoinRoom\"事件触发, 玩家的ID={id}");

            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            string message = JsonConvert.SerializeObject(roomInfo);
            SyncInfo syncInfo = new(GameEvent.AcceptApplyForJoinRoom, id, message);

            NetworkManager.Instance.SendSyncInfo(syncInfo);
        }


        //拒绝玩家加入房间的申请
        [EventCall(nameof(GameEvent.RefuseApplyForJoinRoom))]
        private void RefuseApplyForJoinRoom(string id)
        {
            LogHelper.Info($"\"拒绝玩家加入房间的申请-RefuseApplyForJoinRoom\"事件触发, 玩家的ID={id}");

            SyncInfo syncInfo = new(GameEvent.RefuseApplyForJoinRoom, id, null);

            NetworkManager.Instance.SendSyncInfo(syncInfo);
        }
    }
}
