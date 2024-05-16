using UniVue.ViewModel.Attr;

namespace HayypCard.Enums
{
    public enum PlayerState
    {
        [EnumAlias("在线")]
        Online,

        [EnumAlias("离线")]
        Offline,

        [EnumAlias("在房间中")]
        InRoom,

        [EnumAlias("游戏中")]
        Gaming,
    }
}
