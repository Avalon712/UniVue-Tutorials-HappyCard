using UniVue.ViewModel.Attr;

namespace HayypCard.Enums
{
    /// <summary>
    /// 游戏难度
    /// </summary>
    public enum GameLevel
    {
        [EnumAlias("简单")]
        Easy,
        [EnumAlias("中等")]
        Medium,
        [EnumAlias("困难")]
        Hard,
    }
}
