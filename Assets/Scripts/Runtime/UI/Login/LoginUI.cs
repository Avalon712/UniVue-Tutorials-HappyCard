using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network.Entities;
using HayypCard.Utils;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true,nameof(GameScene.Login))]
    public sealed class LoginUI : EventRegister
    {

        #region 玩家登录事件

        [EventCall(nameof(GameEvent.Login))]
        private void Login(User user, bool remember)
        {
            LogHelper.Info($"\"用户登录-Login\"事件触发：name={user.name}, password={user.password}, rememberMe={remember}");

            HttpInfo<User, Player> httpInfo = new(GameEvent.Login);
            httpInfo.requestData = user;

            httpInfo.onSuccessed = () =>
            {
                Player player = httpInfo.responseData;

                user.id = player.ID;
                user.lastLoginDate = DateTime.Now;
                //更新玩家的游戏天数
                player.days = user.lastLoginDate.Subtract(user.registerDate).Days;

                //只有勾选记住我后才写入存档
                if (remember) { GameDataManager.Instance.User = user; }

                GameDataManager.Instance.Player = httpInfo.responseData;

                SceneManager.LoadScene(nameof(GameScene.Main), LoadSceneMode.Single);
            };

            httpInfo.onFailed = () =>
            {
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open(httpInfo.exception);
            };

            NetworkManager.Instance.SendHttpRequest(httpInfo, GameEvent.Login);
        }

        #endregion

        #region 自动登录
        public void AutoLogin()
        {
            User user = GameDataManager.Instance.User;

            if (user != null && !string.IsNullOrEmpty(user.password) && !string.IsNullOrEmpty(user.name))
            {
                //为登录输入框自动赋值
                Dictionary<string, object> args = new Dictionary<string, object>()
                    {
                        { nameof(user.name),user.name },
                        { nameof(user.password),user.password }
                    };
                Vue.Event.SetEventArgs(nameof(GameEvent.Login), nameof(HappyCard.Enums.GameUI.LoginView), args);
            }
        }
        #endregion
    }
}
