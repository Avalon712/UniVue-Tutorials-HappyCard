using HappyCard.Entities;
using System.Collections.Generic;

namespace HayypCard.Entities
{
    /// <summary>
    /// 玩家本地存档数据
    /// </summary>
    public sealed class ArchiveData
    {
        public User User { get; set; }

        public Player Player { get; set; }

        public List<BagItem> Bag { get; set; }

        public List<BattleInfo> BattleRecord { get; set; }

        public GameSetting GameSetting { get; set; }
    }
}
