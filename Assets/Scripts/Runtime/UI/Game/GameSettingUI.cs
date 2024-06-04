using HappyCard.Enums;
using HayypCard.Utils;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class GameSettingUI : EventRegister
    {

        [EventCall(nameof(GameEvent.ModifyGameSetting))]
        private void ModifyGameSetting()
        {
            LogHelper.Info($"\"修改房间中的游戏设置-ModifyGameSetting\"事件触发");
        }

    }
}
