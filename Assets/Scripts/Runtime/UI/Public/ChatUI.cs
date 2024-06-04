using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Entities;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Room),nameof(GameScene.Game))]
    public sealed class ChatUI : EventRegister
    {
        public ChatUI()
        {
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.Chat, ReceiveChatInfo);

            NetworkManager.Instance.RegisterSyncInfoResponseHandle(ref handle);
        }

        /// <summary>
        /// 接收到聊天消息
        /// </summary>
        private void ReceiveChatInfo(SyncInfo syncInfo)
        {
            //显示聊天消息
            Vue.Router.GetView<ListView>(nameof(HappyCard.Enums.GameUI.ChatView)).AddData(JsonConvert.DeserializeObject<ChatInfo>(syncInfo.Message));
        }

        #region 发送聊天消息

        [EventCall(nameof(GameEvent.Chat))]
        private void Chat(string message)
        {
            LogHelper.Info($"\"聊天-Chat\"事件触发, 发送的聊天内容={message}");

            //创建聊天信息
            ChatInfo chatInfo = new ChatInfo() { Speaker = GameDataManager.Instance.Player.Name, Message = message };
            //创建同步消息
            SyncInfo syncInfo = new SyncInfo(GameEvent.Chat, null, JsonConvert.SerializeObject(chatInfo));

            //显示聊天消息
            Vue.Router.GetView<ListView>(nameof(GameUI.ChatView)).AddData(chatInfo);
            //发送同步消息
            NetworkManager.Instance.SendSyncInfo(syncInfo);
        }

        #endregion
    }
}
