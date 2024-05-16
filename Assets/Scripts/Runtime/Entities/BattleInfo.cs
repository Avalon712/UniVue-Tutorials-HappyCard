using HayypCard.Enums;
using UniVue.Model;

namespace HappyCard.Entities
{
    /*
    2024/05/08 21:21:27 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    /// <summary>
    /// 静态数据
    /// </summary>
    public sealed class BattleInfo : BaseModel
    {
        private string _date;
        private Gameplay _gameplay;
        private GameMode _gamemode;
        private string _roomid;
        private string _players;
        private string _result;

        /// <summary>
        /// 对局日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 游戏玩法：斗地主、板子炮、炸金花
        /// </summary>
        public Gameplay Gameplay { get; set; }

        /// <summary>
        /// 游戏模式：随机匹配、开房间
        /// </summary>
        public GameMode Gamemode { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// 游戏对局中的好友名字信息，以字符'、'分开
        /// </summary>
        public string Players { get; set; }

        /// <summary>
        /// 对局结果，以字符↑表示赢，字符↓表示输，同时使用富文本
        /// <para>张三&lt;color=yellow&gt;↑100&lt;/color&gt;、李四&lt;color=green&gt;↓999&lt;/color&gt;、小明&lt;color=green&gt;↓800&lt;/color&gt;</para>
        /// </summary>
        public string Result { get; set; }


        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(Date), Date);
            NotifyUIUpdate(nameof(Gameplay), (int)Gameplay);
            NotifyUIUpdate(nameof(Gamemode), (int)Gamemode);
            NotifyUIUpdate(nameof(RoomId), RoomId);
            NotifyUIUpdate(nameof(Players), Players);
            NotifyUIUpdate(nameof(Result), Result);
        }

        #endregion

    }
}
