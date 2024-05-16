using UniVue.ViewModel.Attr;

namespace HappyCard.Enums
{
    /// <summary>
    /// 当前游戏的网络服务模式
    /// </summary>
    public enum NetworkServiceMode
    {
        /// <summary>
        /// 广域网
        /// </summary>
        [EnumAlias("广域网")]
        WAN,

        /// <summary>
        /// 局域网
        /// </summary>
        [EnumAlias("局域网")]
        LAN,
    }
}
