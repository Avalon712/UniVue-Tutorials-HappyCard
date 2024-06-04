using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Tween;
using UniVue.Tween.Tweens;
using UniVue.Utils;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class MatchUI : EventRegister
    {
        private TweenTask _matchTweenAni; //显示匹配动画


        [EventCall(nameof(GameEvent.StartMatch))]
        private void StartMatch()
        {
            LogHelper.Info("\"开始匹配事件-StartMatch\"事件触发");
            //封装数据
            Dictionary<string, string> data = new()
            {
                { "ID",GameDataManager.Instance.Player.ID},
                {"Settings",JsonConvert.SerializeObject(GameDataManager.Instance.GameSetting) }
            };

            //显示匹配动画
            TMP_Text aniTxt = GameObjectFindUtil.BreadthFind("~AnimationTxt", Vue.Router.GetView(nameof(HappyCard.Enums.GameUI.MatchingView)).viewObject).GetComponent<TMP_Text>();
            _matchTweenAni = TweenBehavior.Typewriter(aniTxt, "......", 6f).Loop(1000);

            //发送开始匹配事件
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.Calls.Add((GameEvent.SuccessMatch, SuccessInMatching));
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.StartMatch, null, JsonConvert.SerializeObject(data)), ref handle);
        }



        [EventCall(nameof(GameEvent.CancelMatch))]
        private void CancelMatch()
        {
            LogHelper.Info("\"取消匹配事件-CancelMatch\"事件触发");
            //发送取消匹配事件
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.CancelMatch, null, GameDataManager.Instance.Player.ID));
            _matchTweenAni.Kill();
            _matchTweenAni = null;
        }



        /// <summary>
        /// 匹配成功事件回调函数
        /// </summary>
        private void SuccessInMatching(SyncInfo syncInfo)
        {
            //获取到房间对象
            GameDataManager.Instance.RoomInfo = JsonConvert.DeserializeObject<RoomInfo>(syncInfo.Message);
            //进入游戏场景
            SceneManager.LoadScene(nameof(GameScene.Game));
            _matchTweenAni = null;
        }

    }
}
