using System;
using UniVue.ViewModel.Attr;

namespace HappyCard.Enums
{
    /// <summary>
    /// 游戏成就
    /// </summary>
    [Flags]
    public enum GameTag
    {
        [EnumAlias("暂无成就")]
        None = 0,

        [EnumAlias("冒险主义者")]
        Adventurist = 1,

        [EnumAlias("幸运之神")]
        GodOfLuck = 2,

        [EnumAlias("精算师")]
        Acturay = 4,

        [EnumAlias("苦干家")]
        HardWorker = 8,

        [EnumAlias("大富翁")]
        Monopoly =16,
    }
}
