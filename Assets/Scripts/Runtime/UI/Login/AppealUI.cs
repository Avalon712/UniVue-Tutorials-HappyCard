using HappyCard.Enums;
using HayypCard.Utils;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Login))]
    public sealed class AppealUI : EventRegister
    {
        #region 账号申诉事件

        [EventCall(nameof(GameEvent.Appeal))]
        private void Appeal(string email)
        {
            LogHelper.Info($"用户账号申诉事件触发：email={email}");
        }

        #endregion
    }
}
