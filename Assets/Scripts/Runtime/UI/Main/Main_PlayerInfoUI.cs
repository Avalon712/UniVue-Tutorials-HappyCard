using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Utils;
using UnityEngine;
using UniVue.Evt;
using UniVue.Evt.Attr;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class Main_PlayerInfoUI : EventRegister
    {
        #region 更改当前玩家的头像

        [EventCall(nameof(GameEvent.ModifyHeadIcon))]
        private void ModifyHeadIcon(Sprite icon)
        {
            LogHelper.Info($"\"更改玩家的头像-ModifyHeadIcon\"事件触发，新的头像的名称{icon.name}");
            GameDataManager.Instance.Player.HeadIcon = icon;
        }

        #endregion

        #region 更改当前玩家的名字

        [EventCall(nameof(GameEvent.ModifyName))]
        private void ModifyName(string newName)
        {
            LogHelper.Info($"\"更改玩家的名字-ModifyName\"事件触发，玩家新的名称{newName}");
            GameDataManager.Instance.Player.Name = newName;
        }

        #endregion
    }
}
