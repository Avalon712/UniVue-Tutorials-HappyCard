namespace HappyCard.Enums
{
    public enum GameEvent
    {
        /// <summary>
        /// 玩家登录
        /// </summary>
        Login,

        /// <summary>
        /// 玩家注册
        /// </summary>
        Signup,

        /// <summary>
        /// 账号申诉
        /// </summary>
        Appeal,

        /// <summary>
        /// 购买体力值
        /// </summary>
        BuyHP,

        /// <summary>
        /// 购买金币
        /// </summary>
        BuyCoin,

        /// <summary>
        /// 购买钻石
        /// </summary>
        BuyDiamond,

        /// <summary>
        /// 购买道具
        /// </summary>
        BuyProp,

        /// <summary>
        /// 玩家使用道具
        /// </summary>
        UseProp,

        /// <summary>
        /// 显示道具信息
        /// </summary>
        ShowPropInfo,

        /// <summary>
        /// 保存存档
        /// </summary>
        SaveArchive,

        /// <summary>
        /// 显示玩家当前自己的信息
        /// </summary>
        ShowSelfInfo,

        /// <summary>
        /// 创建房间
        /// </summary>
        CreateRoom,

        /// <summary>
        /// 申请加入房间
        /// </summary>
        ApplyForJoinRoom,

        /// <summary>
        /// 玩家进入房间
        /// </summary>
        EnterRoom,

        /// <summary>
        /// 玩家退出房间
        /// </summary>
        QuitRoom,

        /// <summary>
        /// 房主销毁房间
        /// </summary>
        DestroyRoom,

        /// <summary>
        /// 接受玩家加入房间的申请
        /// </summary>
        AcceptApplyForJoinRoom,

        /// <summary>
        /// 拒绝玩家加入房间的申请
        /// </summary>
        RefuseApplyForJoinRoom,

        /// <summary>
        /// 检查游戏更新
        /// </summary>
        CheckUpdate,

        /// <summary>
        /// 开始游戏
        /// </summary>
        StartGame,

        /// <summary>
        /// 将玩家踢出房间
        /// </summary>
        KickOutRoom,

        /// <summary>
        /// 转让房主
        /// </summary>
        TransferRoomOwner,

        /// <summary>
        /// 聊天事件
        /// </summary>
        Chat,

        /// <summary>
        /// 邀请玩家加入房间
        /// </summary>
        Invite,

        /// <summary>
        /// 玩家接收邀请
        /// </summary>
        AcceptInvite,

        /// <summary>
        /// 玩家拒绝接收邀请
        /// </summary>
        RefuseInvite,

        /// <summary>
        /// 显示玩家的信息事件
        /// </summary>
        ShowPlayerInfo,

        /// <summary>
        /// 更改玩家的名字事件
        /// </summary>
        ModifyName,

        /// <summary>
        /// 更改玩家头像事件
        /// </summary>
        ModifyHeadIcon,

        /// <summary>
        /// 准备发牌阶段
        /// </summary>
        Prepare,

        /// <summary>
        /// 暂停游戏
        /// </summary>
        PauseGame,

        /// <summary>
        /// 修改房间中的游戏设置事件
        /// </summary>
        ModifyGameSetting,

        /// <summary>
        /// 游戏对局结束
        /// </summary>
        GameOver,

        /// <summary>
        /// 发牌事件
        /// </summary>
        DealCards,

        /// <summary>
        /// 加地主
        /// </summary>
        CallLandlord,

        /// <summary>
        /// 不叫地主
        /// </summary>
        NoCallLandlord,

        /// <summary>
        /// 包牌事件
        /// </summary>
        Bao,

        /// <summary>
        /// 玩家不包牌
        /// </summary>
        NoBao,

        /// <summary>
        /// 选中牌事件
        /// </summary>
        Selected,

        /// <summary>
        /// 出牌事件 
        /// </summary>
        OutCard,

        /// <summary>
        /// 回合同步事件
        /// </summary>
        BoutSync,

        /// <summary>
        /// 不要
        /// </summary>
        Pass,

        /// <summary>
        /// 重选
        /// </summary>
        Reset,
    }
}
