
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
        /// 显示当前玩家手牌的视图 
        /// </summary>
        ListCardView,

        /// <summary>
        /// 根据当前回合阶段显示指定选项的视图
        /// </summary>
        BoutPhaseView,

        /// <summary>
        /// 准备阶段视图 ------- BoutPhaseView的嵌套视图
        /// </summary>
        PreparePhaseView,

        /// <summary>
        /// 板子炮选择包、不包阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        BaoPhaseView,

        /// <summary>
        /// 板子炮反包、不反包阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        FanBaoPhaseView,

        /// <summary>
        /// 叫地主阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        JiaoDiZhuPhaseView,

        /// <summary>
        /// 板子炮叫队友阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        JiaoDuiYouPhaseView,

        /// <summary>
        /// 出牌阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        OutCardPhaseView,

        /// <summary>
        /// 显示阶段做选择的定时器------- BoutPhaseView的嵌套视图
        /// </summary>
        PhaseTimerView,

        /// <summary>
        /// 抢地主阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        QiangJiaoDiZhuPhaseView,

        /// <summary>
        /// 炸金花阶段------- BoutPhaseView的嵌套视图
        /// </summary>
        ZhaJinHuaOutCardPhaseView,

        /// <summary>
        /// 显示当前玩家出的牌的视图
        /// </summary>
        ListOutCardView,

        /// <summary>
        /// 左边第一个玩家显示出牌的视图 -- LeftPlayerView1的嵌套视图
        /// </summary>
        LeftShowOutCardsView1,

        /// <summary>
        /// 左边第一个玩家的定时器视图 -- LeftPlayerView1的嵌套视图
        /// </summary>
        LeftTimerView1,

        /// <summary>
        ///  左边第二个玩家显示出牌的视图-- LeftPlayerView2的嵌套视图
        /// </summary>
        LeftShowOutCardsView2,

        /// <summary>
        /// 左边第二个玩家的定时器视图-- LeftPlayerView2的嵌套视图
        /// </summary>
        LeftTimerView2,

        /// <summary>
        /// 右边第一个玩家显示出牌的视图-- RightPlayerView1的嵌套视图
        /// </summary>
        RightShowOutCardsView1,

        /// <summary>
        /// 右边第一个玩家的定时器视图-- RightPlayerView1的嵌套视图
        /// </summary>
        RightTimerView1,

        /// <summary>
        ///  右边第二个玩家显示出牌的视图-- RightPlayerView2的嵌套视图
        /// </summary>
        RightShowOutCardsView2,

        /// <summary>
        /// 右边第二个玩家的定时器视图-- RightPlayerView2的嵌套视图
        /// </summary>
        RightTimerView2,

        /// <summary>
        /// 游戏匹配玩法设置视图 -- Main场景
        /// </summary>
        GameplayView,

        /// <summary>
        /// 正在匹配中的视图 -- Main场景
        /// </summary>
        MatchingView,

        /// <summary>
        /// 任务列表
        /// </summary>
        TaskView,
    }
}
