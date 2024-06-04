using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network.Entities;
using HayypCard.Utils;
using System;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Login))]
    public sealed class RegisterUI : EventRegister
    {
        #region 玩家注册事件

        [EventCall(nameof(GameEvent.Signup))]
        private void Register(User user)
        {
            LogHelper.Info($"\"用户注册-Signup\"事件触发：email={user.email}, name={user.name}, password={user.password}");

            user.registerDate = DateTime.Now;
            user.lastLoginDate = DateTime.Now;

            HttpInfo<User, User> httpInfo = new(GameEvent.Signup);
            httpInfo.requestData = user;

            httpInfo.onSuccessed = () =>
            {
                GameDataManager.Instance.User = httpInfo.responseData;

                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("注册成功！可以进行登录!");
            };

            httpInfo.onFailed = () =>
            {
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open($"异常: {httpInfo.exception}，服务器返回信息：{httpInfo.raw}");
            };

            NetworkManager.Instance.SendHttpRequest(httpInfo, GameEvent.Signup);
        }

        #endregion
    }
}
