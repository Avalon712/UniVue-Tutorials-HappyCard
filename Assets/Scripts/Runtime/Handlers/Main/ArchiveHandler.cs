using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    [EventCallAutowire(true,nameof(GameScene.Main))]
    public sealed class ArchiveHandler : EventRegister
    {

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

            Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("数据保存成功!");
        }
    }
}
