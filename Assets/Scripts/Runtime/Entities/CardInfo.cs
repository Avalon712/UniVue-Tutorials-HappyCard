using HayypCard.Enums;
using Newtonsoft.Json;
using UnityEngine;
using UniVue.Model;

namespace HayypCard.Entities
{
    public sealed class CardInfo : BaseModel
    {
        /// <summary>
        /// 当前是否被选中
        /// </summary>
        [JsonIgnore]
        public bool Selected { get; set; }

        public PokerCard PokerCard { get; set; }

        /// <summary>
        /// 扑克图标
        /// </summary>
        [JsonIgnore]
        public Sprite Icon { get; set; }

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(PokerCard), (int)PokerCard);
            NotifyUIUpdate(nameof(Icon), Icon);
        }
    }
}
