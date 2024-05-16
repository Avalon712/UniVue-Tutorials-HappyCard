using UnityEngine;
using HayypCard.Enums;

namespace HappyCard.Entities
{
    /*
    2024/05/06 17:50:45 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    /// <summary>
    /// 静态数据
    /// </summary>
    public sealed class PropInfo : ScriptableObject
    {
        [Header("道具名称")]
        [SerializeField] private string propName;

        [Header("道具类型")]
        [SerializeField] private PropType propType;

        [Header("描述")]
        [SerializeField] private string description;

        [Header("道具图标")]
        [SerializeField] private Sprite icon;

        [Header("是否属于增益效果的道具")]
        [SerializeField] private bool _gain;

        /// <summary>
        /// 道具类型
        /// </summary>
        public PropType PropType { get => propType; set => propType = value; }

        /// <summary>
        /// 是否是增益效果的道具
        /// </summary>
        public bool Gain => _gain;

        /// <summary>
        /// 道具名称
        /// </summary>
        public string PropName => propName;

        /// <summary>
        /// 道具图标
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// 描述内容，使用富文本
        /// <para> &lt;color=#7FCFDE&gt;作用描述：&lt;/color&gt;</para>
        ///<para>&lt;color=#4D96BA&gt;在游戏中使用此道具后获得好牌的概率将会提高30%\n\n&lt;/color&gt;</para>
        /// <para>&lt;color=#7FCFDE&gt;获得方式：&lt;/color&gt;</para>
        /// <para>&lt;color=#D4C49B&gt;商店购买、每日赠送、每日任务&lt;/color&gt;</para>
        /// </summary>
        public string Description => description;
    }
}
