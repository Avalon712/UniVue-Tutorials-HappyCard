using HappyCard.Enums;
using HayypCard.Utils;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true,nameof(GameScene.Room))]
    public sealed class Room_PlayerInfoUI : EventRegister
    {
        #region 将玩家踢出房间

        [EventCall(nameof(GameEvent.KickOutRoom))]
        private void KickOutRoom(string id)
        {
            LogHelper.Info($"\"玩家被踢出房间-KickOutRoom\"事件触发, 被踢出房间的玩家的ID={id}");
        }

        #endregion

        #region 转让房主

        [EventCall(nameof(GameEvent.TransferRoomOwner))]
        private void TransferRoomOwner(string id)
        {
            LogHelper.Info($"\"转让房主-TransferRoomOwner\"事件触发, 新的房主的ID={id}");
        }
        #endregion
    }
}
