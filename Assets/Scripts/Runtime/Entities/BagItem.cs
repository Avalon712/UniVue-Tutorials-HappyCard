using UniVue.Model;
using UnityEngine;
using HayypCard.Enums;
using Newtonsoft.Json;
using System;
using HappyCard.Managers;

namespace HappyCard.Entities
{
    /*
    2024/05/08 11:04:00 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class BagItem : BaseModel
    {
        private int _num;
        private PropType _propType; //根据这个来加载PropInfo

        /// <summary>
        /// 开始使用的日期
        /// </summary>
        public DateTime StartUseDate { get; set; } 

        /// <summary>
        /// 当前道具是否正在使用中
        /// </summary>
        public bool Using { get; set; }

        /// <summary>
        /// 道具名称
        /// </summary>
        [JsonIgnore]
        public string PropName => GameDataManager.Instance.PropInfos[_propType].PropName;

        /// <summary>
        /// 道具类型
        /// </summary>
        public PropType PropType 
        { 
            get => _propType;
            set => _propType = value;
        }

        /// <summary>
        /// 道具图标
        /// </summary>
        [JsonIgnore]
        public Sprite Icon => GameDataManager.Instance.PropInfos[_propType].Icon;

        /// <summary>
        /// 描述内容
        /// </summary>
        [JsonIgnore]
        public string Description => GameDataManager.Instance.PropInfos[_propType].Description;


        /// <summary>
        /// 玩家拥有的数量
        /// </summary>
        public int Num
        {
            get => _num;
            set
            {
                if(_num != value)
                {
                    _num = value;
                    NotifyUIUpdate(nameof(Num), value);
                }
            }
        }

        public BagItem() { }


        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(PropName), PropName);
            NotifyUIUpdate(nameof(PropType), (int)PropType);
            NotifyUIUpdate(nameof(Icon), Icon);
            NotifyUIUpdate(nameof(Description), Description);
            NotifyUIUpdate(nameof(Num), Num);
        }

        #endregion

        #region 重写父类UpdateModel()函数

        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if(nameof(Num).Equals(propertyName)){ Num = propertyValue; }
        }

        #endregion
    }
}
