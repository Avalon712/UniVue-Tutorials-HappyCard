using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Utils;
using UniVue.Evt.Attr;
using UniVue;
using UniVue.View.Views;
using UniVue.Evt;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class PlayerSelfUI : EventRegister
    {

        [EventCall(nameof(GameEvent.ShowSelfInfo))]
        private void ShowSelfInfo()
        {
            LogHelper.Info($"\"显示玩家自己的信息-ShowSelfInfo\"事件触发");

            //由于PlayerInfoView这个视图下的BaseInfoView以及BattleInfoView都是通用视图，
            //有可能上一次显示的是好友的信息，因此需要重新进行绑定
            Vue.Router.GetView(nameof(GameUI.BaseInfoView)).RebindModel(GameDataManager.Instance.Player);
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).RebindData(GameDataManager.Instance.BattleRecord);
        }


        [EventCall(nameof(GameEvent.PauseGame))]
        private void PauseGame()
        {
            LogHelper.Info($"\"暂停游戏-PauseGame\"事件触发");
        }


    }
}
