using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class MainUI : EventRegister
    {

        #region 创建房间事件

        [EventCall(nameof(GameEvent.CreateRoom))]
        private void CreateRoom()
        {
            LogHelper.Info("\"创建房间-CreateRoom\"事件触发!");

            if (Vue.Event.GetRegister<BagUI>().UseProp(PropType.RoomCard))
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
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("创建房间失败！你还没有房卡呢，快去买一张吧！");
            }
        }

        #endregion

        #region 显示当前玩家信息事件

        [EventCall(nameof(GameEvent.ShowSelfInfo))]
        private void ShowSelfInfo()
        {
            LogHelper.Info("\"显示玩家自己信息-ShowSelfInfo\"事件触发!");

            //由于PlayerInfoView这个视图下的BaseInfoView以及BattleInfoView都是通用视图，
            //有可能上一次显示的是好友的信息，因此需要重新进行绑定
            Vue.Router.GetView(nameof(HappyCard.Enums.GameUI.BaseInfoView)).RebindModel(GameDataManager.Instance.Player);
            Vue.Router.GetView<ListView>(nameof(HappyCard.Enums.GameUI.BattleRecordView)).RebindData(GameDataManager.Instance.BattleRecord);
        }

        #endregion

        #region 保存存档事件

        [EventCall(nameof(GameEvent.SaveArchive))]
        private void SaveArchive()
        {
            LogHelper.Info($"\"保存存档-SaveArchive\"事件触发!");

            //异步任务
            Task.Run(() =>
            {
                //1 本地保存数据
                string archiveData = JsonConvert.SerializeObject(GameDataManager.Instance.ArchiveData);
                File.WriteAllText(GameDataManager.Instance.localArchiveFilePath, archiveData);

                //2 将所有存档数据同步到服务器
                LogHelper.Info("存档事件尚未完成功能: 将所有存档数据同步到服务器");
            });

            Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("数据保存成功!");
        }

        #endregion
    }
}
