using UniVue.Model;
using UnityEngine;
using HayypCard.Enums;
using System;

namespace HayypCard.Entities
{
    /*
    2024/06/02 18:11:40 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class TaskInfo : ScriptableModel
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Sprite _iconFrame;
        [SerializeField] private TaskLevel[] _levels;

        [Serializable]
        public class TaskLevel
        {
            public CurrencyType currencyType;
            public Sprite rewardIcon;
            public string description;
            public int rewardNum;
        }

        public class TaskProcess
        {
            public string id { get; set; }
            public float process { get; set; }
            public int level { get; set; }
        }

        /// <summary>
        /// 记录当前任务的进度信息
        /// </summary>
        public TaskProcess Task { get; set; } = new TaskProcess() {level = 0, process = 0 };

        /// <summary>
        /// 任务进度,[0,1]
        /// </summary>
        public float Process
        {
            get => Task.process;
            set
            {
                if(Task.process != value)
                {
                    Task.process = value;
                    NotifyUIUpdate(nameof(Process), value);
                }
            }
        }

        /// <summary>
        /// 任务内容描述
        /// </summary>
        public string Description
        {
            get => _levels[Task.level].description;
        }

        /// <summary>
        /// 任务图标
        /// </summary>
        public Sprite Icon
        {
            get => _icon;
            set
            {
                if(_icon != value)
                {
                    _icon = value;
                    NotifyUIUpdate(nameof(Icon), value);
                }
            }
        }

        /// <summary>
        /// 任务图标的背景图标
        /// </summary>
        public Sprite IconFrame
        {
            get => _iconFrame;
            set
            {
                if(_iconFrame != value)
                {
                    _iconFrame = value;
                    NotifyUIUpdate(nameof(IconFrame), value);
                }
            }
        }

        /// <summary>
        /// 任务等级
        /// </summary>
        public int Level
        {
            get => Task.level;
            set
            {
                if(Task.level != value)
                {
                    Task.level = value;
                    NotifyUIUpdate(nameof(Level), value);
                }
            }
        }

        /// <summary>
        /// 任务奖励的图标
        /// </summary>
        public Sprite RewardIcon
        {
            get => _levels[Task.level].rewardIcon;
        }

        /// <summary>
        /// 奖励数量
        /// </summary>
        public int RewardNum
        {
            get => _levels[Task.level].rewardNum;
        }

        /// <summary>
        /// 任务唯一编号
        /// </summary>
        public string ID
        {
            get => _id;
            set
            {
                if(_id != value)
                {
                    _id = value;
                    NotifyUIUpdate(nameof(ID), value);
                }
            }
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(Process), Process);
            NotifyUIUpdate(nameof(Description), Description);
            NotifyUIUpdate(nameof(Icon), Icon);
            NotifyUIUpdate(nameof(IconFrame), IconFrame);
            NotifyUIUpdate(nameof(Level), Level);
            NotifyUIUpdate(nameof(RewardIcon), RewardIcon);
            NotifyUIUpdate(nameof(RewardNum), RewardNum);
            NotifyUIUpdate(nameof(ID), ID);
        }

        #endregion

        #region 重写父类UpdateModel()函数


        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if(nameof(Level).Equals(propertyName)){ Level = propertyValue; }
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
            if(nameof(ID).Equals(propertyName)){ ID = propertyValue; }
        }

        public override void UpdateModel(string propertyName, float propertyValue)
        {
            if(nameof(Process).Equals(propertyName)){ Process = propertyValue; }
        }

        public override void UpdateModel(string propertyName, bool propertyValue)
        {
        }
        #endregion
    }
}
