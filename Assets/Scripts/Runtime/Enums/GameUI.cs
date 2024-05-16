
namespace HappyCard.Enums
{
    /// <summary>
    /// 所有的游戏UI视图
    /// </summary>
    public enum GameUI
    {
        /// <summary>
        /// 登录视图
        /// </summary>
        LoginView,
        
        /// <summary>
        /// 注册用户视图
        /// </summary>
        SignupView,

        /// <summary>
        /// 账号申诉视图
        /// </summary>
        AppealView,
       
        /// <summary>
        /// 需要玩家作出确认动作的视图
        /// </summary>
        EnsureTipView,

        /// <summary>
        /// 游戏加载视图
        /// </summary>
        LoadView,

        /// <summary>
        /// 消息提示
        /// </summary>
        TipView,

        /// <summary>
        /// 商店视图
        /// </summary>
        ShopView,

        /// <summary>
        /// 体力商店（ShopView的嵌套视图）--- ListView
        /// </summary>
        HPShopView,

        /// <summary>
        /// 钻石商店（ShopView的嵌套视图）--- ListView
        /// </summary>
        DiamondShopView,

        /// <summary>
        /// 金币商店（ShopView的嵌套视图）--- ListView
        /// </summary>
        CoinShopView,

        /// <summary>
        /// 道具商店（ShopView的嵌套视图）--- GridView
        /// </summary>
        PropShopView,

        /// <summary>
        /// 玩家的背包视图
        /// </summary>
        BagView,

        /// <summary>
        /// 玩家申请加入房间通知视图
        /// </summary>
        PlayerJoinRoomNotifyView,

        /// <summary>
        /// 显示道具信息 （BagView的嵌套视图）
        /// </summary>
        PropInfoView,

        /// <summary>
        /// Game场景或Main场景中的主视图
        /// </summary>
        MainView,

        /// <summary>
        /// 显示玩家信息的视图
        /// </summary>
        PlayerInfoView,

        /// <summary>
        /// 显示玩家基础信息的视图 （PlayerInfoView的嵌套视图）
        /// </summary>
        BaseInfoView,

        /// <summary>
        /// 玩家对局记录视图（PlayerInfoView的嵌套视图）
        /// </summary>
        BattleRecordView,

        /// <summary>
        /// Main场景中的游戏设置视图
        /// </summary>
        SettingView,

        /// <summary>
        /// 创建房间视图
        /// </summary>
        CreateRoomView,

        /// <summary>
        /// Room场景下的RoomView
        /// </summary>
        RoomView,

        /// <summary>
        /// Room场景中显示房间中的玩家的视图
        /// </summary>
        PlayersView,

        /// <summary>
        /// Room场景中根据玩家的不同身份显示不同的操作视图
        /// </summary>
        OperationView,

        /// <summary>
        /// 聊天视图
        /// </summary>
        ChatView,

        /// <summary>
        /// 邀请玩家加入房间视图
        /// </summary>
        FriendInvitNotifyView,

        /// <summary>
        /// 短暂的快速显示提示消息的视图
        /// </summary>
        FastTipView,

        /// <summary>
        /// 显示游戏中自带的所有头像图标的视图
        /// </summary>
        HeadIconView,

        /// <summary>
        /// Game场景中的游戏设置视图
        /// </summary>
        GameSettingView,

        /// <summary>
        /// Game场景中用于显示当前玩家自己基础信息的视图
        /// </summary>
        PlayerSelfView,

        /// <summary>
        /// Game场景中左边的第一个玩家的视图 
        /// </summary>
        LeftPlayerView1,

        /// <summary>
        /// Game场景中左边的第二个玩家的视图 
        /// </summary>
        LeftPlayerView2,

        /// <summary>
        /// Game场景中右边的第一个玩家的视图 
        /// </summary>
        RightPlayerView1,

        /// <summary>
        /// Game场景中右边的第二个玩家的视图 
        /// </summary>
        RightPlayerView2,

        /// <summary>
        /// 玩家准备视图，Game场景中玩家点击准备后就开始准备发牌  -----（CardView的嵌套视图）
        /// </summary>
        PrepareOperationView,

        /// <summary>
        /// 出牌选项视图  ----（CardView的嵌套视图）
        /// </summary>
        OutCardOperationView,

        /// <summary>
        /// 板子炮玩法下，开始出牌前玩家进行的流程：包、不包 ----（CardView的嵌套视图）
        /// </summary>
        BanZiPaoOptionalView,

        /// <summary>
        /// 斗地主玩法下，开始出牌前玩家进行的流程：叫地主、不叫 -----（CardView的嵌套视图）
        /// </summary>
        FightLandlordOptionalView,

        /// <summary>
        /// 炸金花玩法下，玩家的选项 ------- （CardView的嵌套视图）
        /// </summary>
        ZhanJinHuaOptionalView,

        /// <summary>
        /// 显示当前玩家出的牌的视图 ------- （CardView的嵌套视图）
        /// </summary>
        ShowOutCardsView,

        /// <summary>
        /// 显示玩家手牌的视图 ------- （CardView的嵌套视图）
        /// </summary>
        ListCardView,

        /// <summary>
        /// 显示玩家的出牌、手牌、操作选项等的总视图
        /// </summary>
        CardView,
    }
}
