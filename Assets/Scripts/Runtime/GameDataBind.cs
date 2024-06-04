using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Utils;
using HayypCard.Entities;
using HayypCard.Enums;
using HayypCard.UI;
using System.Collections.Generic;
using UniVue;
using UniVue.Runtime.View.Views;
using UniVue.View.Views;

namespace HayypCard
{
    public sealed class GameDataBind : Singleton<GameDataBind>
    {
        public void PrepareGameDataAfterSceneLoaded(GameScene gameScene)
        {
            switch (gameScene)
            {
                case GameScene.Login:
                    PrepareLoginScene();
                    break;

                case GameScene.Main:
                    PrepareMainScene();
                    break;

                case GameScene.Room:
                    PrepareRoomScene();
                    break;

                case GameScene.Game:
                    PrepareGameScene();
                    break;
            }
        }

        public void DisposeGameDataAfterSceneUnloaded(GameScene gameScene)
        {
            switch (gameScene)
            {
                case GameScene.Login:
                    DisposeLoginScene();
                    break;

                case GameScene.Main:
                    DisposeMainScene();
                    break;

                case GameScene.Room:
                    DisposeRoomScene();
                    break;

                case GameScene.Game:
                    DisposeGameScene();
                    break;
            }
        }

        //----------------------------------------登录场景----------------------------------------------
        private void PrepareLoginScene()
        {
            //1. 关闭LoadView，打开登录视图
            Vue.Router.Skip(nameof(GameUI.LoadView), nameof(GameUI.LoginView));

            //2. 检查当前网络状态
            if(NetworkManager.Instance.GetGameNetworkServiceMode() == NetworkServiceMode.LAN)
            {
                string message = "当前游戏无法连接上服务器,只能在局域网下进行游戏，可能部分游戏功能无法支持，但是仍可以进行登录!";
                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open(message);
            }

            //3. 自动登录检查
            Vue.Event.GetRegister<LoginUI>().AutoLogin();
        }

        private void DisposeLoginScene()
        {
            //None
        }

        //-----------------------------------------大厅场景---------------------------------------------
        private void PrepareMainScene()
        {
            //关闭LoadView
            Vue.Router.Close(nameof(GameUI.LoadView));

            //1 将所有头像数据绑定HeadIconView
            Vue.Router.GetView<GridView>(nameof(GameUI.HeadIconView)).BindData(GameDataManager.Instance.HeadIcons);

            //2 绑定商品数据到商店视图
            Vue.Router.GetView<ListView>(nameof(GameUI.CoinShopView)).BindData(GameDataManager.Instance.CoinProducts);
            Vue.Router.GetView<ListView>(nameof(GameUI.DiamondShopView)).BindData(GameDataManager.Instance.DiamondProducts);
            Vue.Router.GetView<ListView>(nameof(GameUI.HPShopView)).BindData(GameDataManager.Instance.HPProducts);
            Vue.Router.GetView<GridView>(nameof(GameUI.PropShopView)).BindData(GameDataManager.Instance.PropProducts);

            //3 生成PropInfoView的UIBundle ---> 方便后面的RebindModel()函数的调用
            List<BagItem> bag = GameDataManager.Instance.Bag;
            bool temped = bag.Count == 0;
            BagItem temp = temped ? GameDataManager.Instance.AddBagItem(PropType.Income14) : bag[0];
            Vue.Router.GetView(nameof(GameUI.PropInfoView)).BindModel(temp);
            if (temped) { bag.Remove(temp); } //临时对象删除

            //4 绑定玩家的背包信息到背包视图
            Vue.Router.GetView<GridView>(nameof(GameUI.BagView)).BindData(GameDataManager.Instance.Bag);

            //5 绑定玩家的信息到主视图
            GameDataManager.Instance.Player.Bind(nameof(GameUI.MainView));

            //6 绑定玩家自己的信息到PlayerInfoView下的BaseInfoView（这个视图也可以用于显示好友的信息）
            GameDataManager.Instance.Player.Bind(nameof(GameUI.BaseInfoView));

            //7 读取玩家的对局记录数据，同时绑定到BattleRecordView上（这个视图也可以用于显示好友的对局记录信息）
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).BindData(GameDataManager.Instance.BattleRecord);

            //8 绑定游戏设置GameSetting数据到设置视图、创建房间视图、游戏玩法视图
            GameDataManager.Instance.GameSetting.Bind(nameof(GameUI.SettingView));
            GameDataManager.Instance.GameSetting.Bind(nameof(GameUI.CreateRoomView));
            GameDataManager.Instance.GameSetting.Bind(nameof(GameUI.GameplayView));

            //9 绑定任务数据到TaskView视图
            Vue.Router.GetView<ListView>(nameof(GameUI.TaskView)).BindData(GameDataManager.Instance.TaskInfos);
        }

        private void DisposeMainScene()
        {
            //卸载商店信息
            GameDataManager.Instance.UnloadAllProducts();
            //卸载任务信息
            GameDataManager.Instance.UnloadAllTaskInfos();
        }

        //-----------------------------------------房间场景---------------------------------------------
        private void PrepareRoomScene()
        {
            //关闭LoadView
            Vue.Router.Close(nameof(GameUI.LoadView));

            //1.将房间信息绑定是房间视图
            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            roomInfo.Bind(nameof(GameUI.RoomView));

            //2. 将房间中的玩家绑定到PlayersView视图
            Vue.Router.GetView<ListView>(nameof(GameUI.PlayersView)).BindData(roomInfo.Players);

            //3. 绑定玩家自己的信息到PlayerInfoView下的BaseInfoView（这个视图也可以用于显示好友的信息）
            GameDataManager.Instance.Player.Bind(nameof(GameUI.BaseInfoView));

            //4. 读取玩家的对局记录数据，同时绑定到BattleRecordView上（这个视图也可以用于显示好友的对局记录信息）
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).BindData(GameDataManager.Instance.BattleRecord);

            //5.绑定聊天信息到ChatView上
            Vue.Router.GetView<ListView>(nameof(GameUI.ChatView)).BindData(new List<ChatInfo>());
        }

        private void DisposeRoomScene()
        {
            //None
        }

        //-----------------------------------------游戏场景---------------------------------------------
        private void PrepareGameScene()
        {
            //关闭LoadView
            Vue.Router.Close(nameof(GameUI.LoadView));

            //1. 生成PropInfoView的UIBundle ---> 方便后面的RebindModel()函数的调用
            List<BagItem> bag = GameDataManager.Instance.Bag;
            bool temped = bag.Count == 0;
            BagItem temp = temped ? GameDataManager.Instance.AddBagItem(PropType.Income14) : bag[0];
            Vue.Router.GetView(nameof(GameUI.PropInfoView)).BindModel(temp);
            if (temped) { bag.Remove(temp); } //临时对象删除

            //2. 绑定玩家的背包信息到背包视图
            Vue.Router.GetView<GridView>(nameof(GameUI.BagView)).BindData(GameDataManager.Instance.Bag);

            //3. 绑定玩家信息到PlayerSelfView
            GameDataManager.Instance.Player.Bind(nameof(GameUI.PlayerSelfView));

            //4 绑定玩家自己的信息到PlayerInfoView下的BaseInfoView（这个视图也可以用于显示好友的信息）
            GameDataManager.Instance.Player.Bind(nameof(GameUI.BaseInfoView));

            //5 读取玩家的对局记录数据，同时绑定到BattleRecordView上（这个视图也可以用于显示好友的对局记录信息）
            Vue.Router.GetView<ListView>(nameof(GameUI.BattleRecordView)).BindData(GameDataManager.Instance.BattleRecord);

            //6. 绑定游戏设置数据当GameSettingView
            GameDataManager.Instance.GameSetting.Bind(nameof(GameUI.GameSettingView));

            //7. 绑定聊天信息到ChatView上
            Vue.Router.GetView<ListView>(nameof(GameUI.ChatView)).BindData(new List<ChatInfo>());

            //8. 绑定扑克信息到ListCardView和ShowOutCardsView上
            Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView)).BindData(new List<CardInfo>());
            Vue.Router.GetView<ClampListView>(nameof(GameUI.ListOutCardView)).BindData(new List<CardInfo>());
        }

        private void DisposeGameScene()
        {
            //卸载所有的扑克牌图标
            GameDataManager.Instance.UnloadAllPokerIcons();
        }
    }
}
