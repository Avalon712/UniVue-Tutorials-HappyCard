using UnityEngine;
using UniVue.Model;

namespace HayypCard.Entities
{
    /// <summary>
    /// 头像信息
    /// </summary>
    public sealed class HeadIcon : BaseModel
    {
        public string IconName { get; private set; }

        public Sprite Icon { get; private set; }

        public HeadIcon(Sprite icon)
        {
            Icon = icon;
            IconName = icon.name;
        }

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(IconName), IconName);
            NotifyUIUpdate(nameof(Icon), Icon);
        }
    }
}
