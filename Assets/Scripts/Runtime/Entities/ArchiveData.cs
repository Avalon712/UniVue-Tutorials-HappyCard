using HappyCard.Entities;
using System.Collections.Generic;
using static HayypCard.Entities.TaskInfo;

namespace HayypCard.Entities
{
    /// <summary>
    /// 玩家本地存档数据
    /// </summary>
    public sealed class ArchiveData
    {
        /// <summary>
        /// 玩家登录等信息
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// 玩家的数据
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// 玩家的背包数据
        /// </summary>
        public List<BagItem> Bag { get; set; }

        /// <summary>
        /// 玩家对局数据
        /// </summary>
        public List<BattleInfo> BattleRecord { get; set; }

        /// <summary>
        /// 游戏设置数据
        /// </summary>
        public GameSetting GameSetting { get; set; }

        /// <summary>
        /// 玩家的任务数据
        /// </summary>
        public List<TaskProcess> TaskProcesses { get; set; }

    }
}
