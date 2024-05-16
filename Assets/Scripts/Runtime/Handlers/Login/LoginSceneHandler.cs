using HappyCard.Entities;
using HappyCard.Entities.Configs;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network.Entities;
using HayypCard.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HappyCard.Handlers
{
    [EventCallAutowire(true, nameof(GameScene.Login))]
    public sealed class LoginSceneHandler : EventRegister
    {
        #region 登录事件

        [EventCall(nameof(GameEvent.Login))]
        public void HandLoginEvt(User user, bool remember)
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
                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open(httpInfo.exception);
            };

            NetworkManager.Instance.SendHttpRequest(httpInfo, GameEvent.Login);
        }
        #endregion

        //----------------------------------------------------------------------------------------------

        #region 注册事件

        [EventCall(nameof(GameEvent.Signup))]
        public void HandSignupEvt(User user)
        {
            LogHelper.Info($"\"用户注册-Signup\"事件触发：email={user.email}, name={user.name}, password={user.password}");

            user.registerDate = DateTime.Now;
            user.lastLoginDate = DateTime.Now;

            HttpInfo<User, User> httpInfo = new(GameEvent.Signup);
            httpInfo.requestData = user;

            httpInfo.onSuccessed = () =>
            {
                GameDataManager.Instance.User = httpInfo.responseData;

                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("注册成功！可以进行登录!");
            };

            httpInfo.onFailed = () =>
            {
                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open($"异常: {httpInfo.exception}，服务器返回信息：{httpInfo.raw}");
            };

            NetworkManager.Instance.SendHttpRequest(httpInfo, GameEvent.Signup);
        }
        #endregion

        //----------------------------------------------------------------------------------------------

        #region 账号申诉事件
        [EventCall(nameof(GameEvent.Appeal))]
        public void HandAppealEvt(string email)
        {
            LogHelper.Info($"用户账号申诉事件触发：email={email}");
        }
        #endregion

        //----------------------------------------------------------------------------------------------

        #region 自动登录事件
        public void AutoLogin()
        {
            User user = GameDataManager.Instance.User ;

            if (user != null && !string.IsNullOrEmpty(user.password) && !string.IsNullOrEmpty(user.name))
            {
                //为登录输入框自动赋值
                Dictionary<string, object> args = new Dictionary<string, object>()
                    {
                        { nameof(user.name),user.name },
                        { nameof(user.password),user.password }
                    };
                Vue.Event.SetEventArgs(nameof(GameEvent.Login), nameof(GameUI.LoginView), args);
            }
        }
        #endregion

        //---------------------------------------------------------------------------------------

    }
}

