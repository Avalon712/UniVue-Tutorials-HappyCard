
using UniVue.ViewModel.Attr;

namespace HayypCard.Enums
{
    public enum PlayerGameState
    {
        /// <summary>
        /// 玩家当前回合出的是牌则没有状态
        /// </summary>
        [EnumAlias("")] //设置为空串这样当没有状态要显示时就隐藏UI
        None,

        /// <summary>
        /// 已经出完牌
        /// </summary>
        [EnumAlias("完牌")]
        Finished,

        /// <summary>
        /// 不要
        /// </summary>
        [EnumAlias("不要")]
        Pass,

        /// <summary>
        /// 跟注
        /// </summary>
        [EnumAlias("跟注")]
        Follow,

        /// <summary>
        /// 梭哈
        /// </summary>
        [EnumAlias("梭哈")]
        Soha,

        /// <summary>
        /// 弃牌
        /// </summary>
        [EnumAlias("弃牌")]
        Discard,

        /// <summary>
        /// 下注
        /// </summary>
        [EnumAlias("下注")]
        Bet,

        /// <summary>
        /// 包牌
        /// </summary>
        [EnumAlias("包牌")]
        Bao,

        /// <summary>
        /// 不包
        /// </summary>
        [EnumAlias("不包")]
        NoBao,

        /// <summary>
        /// 反包
        /// </summary>
        [EnumAlias("反包")]
        GrabBao,

        /// <summary>
        /// 玩家抢地主
        /// </summary>
        [EnumAlias("抢地主")]
        GrabLandlord,

        /// <summary>
        /// 叫地主
        /// </summary>
        [EnumAlias("叫地主")]
        CallLandlord,

        /// <summary>
        /// 玩家处于已准备状态
        /// </summary>
        [EnumAlias("已准备")]
        Prepared,

        /// <summary>
        /// 不叫地主
        /// </summary>
        [EnumAlias("不叫")]
        NoCallLandlord,
    }
}
