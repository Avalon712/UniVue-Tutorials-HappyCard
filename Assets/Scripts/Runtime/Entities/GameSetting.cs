using HayypCard.Enums;
using UniVue.Model;

namespace HappyCard.Entities
{
    /*
    2024/05/09 09:05:02 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class GameSetting : BaseModel
    {
        private Gameplay _gameplay;
        private ShuffleMode _shufflemode;
        private bool _allowuserecorder;
        private bool _allowuseprop;
        private GameLevel _level;
        private float _bgmvolume = 1;
        private float _sfxvolume = 1;
        private int _timer = 30;

        /// <summary>
        /// 游戏玩法
        /// </summary>
        public Gameplay Gameplay
        {
            get => _gameplay;
            set
            {
                if(_gameplay != value)
                {
                    _gameplay = value;
                    NotifyUIUpdate(nameof(Gameplay), (int)value);
                }
            }
        }

        /// <summary>
        /// 洗牌模式
        /// </summary>
        public ShuffleMode ShuffleMode
        {
            get => _shufflemode;
            set
            {
                if(_shufflemode != value)
                {
                    _shufflemode = value;
                    NotifyUIUpdate(nameof(ShuffleMode), (int)value);
                }
            }
        }

        /// <summary>
        /// 是否允许使用记牌器
        /// </summary>
        public bool AllowUseRecorder
        {
            get => _allowuserecorder;
            set
            {
                if(_allowuserecorder != value)
                {
                    _allowuserecorder = value;
                    NotifyUIUpdate(nameof(AllowUseRecorder), value);
                }
            }
        }

        /// <summary>
        /// 是否允许使用道具
        /// </summary>
        public bool AllowUseProp
        {
            get => _allowuseprop;
            set
            {
                if(_allowuseprop != value)
                {
                    _allowuseprop = value;
                    NotifyUIUpdate(nameof(AllowUseProp), value);
                }
            }
        }

        /// <summary>
        /// 游戏难度
        /// </summary>
        public GameLevel Level
        {
            get => _level;
            set
            {
                if(_level != value)
                {
                    _level = value;
                    NotifyUIUpdate(nameof(Level), (int)value);
                }
            }
        }

        /// <summary>
        /// 背景声音大小，[0,1]
        /// </summary>
        public float BGMVolume
        {
            get => _bgmvolume;
            set
            {
                if(_bgmvolume != value)
                {
                    _bgmvolume = value;
                    NotifyUIUpdate(nameof(BGMVolume), value);
                }
            }
        }

        /// <summary>
        /// 特效声音大小，[0,1]
        /// </summary>
        public float SFXVolume
        {
            get => _sfxvolume;
            set
            {
                if(_sfxvolume != value)
                {
                    _sfxvolume = value;
                    NotifyUIUpdate(nameof(SFXVolume), value);
                }
            }
        }

        /// <summary>
        /// 每次允许玩家思考的时间，[30,120]
        /// </summary>
        public int Timer
        {
            get => _timer;
            set
            {
                if(_timer != value)
                {
                    _timer = value;
                    NotifyUIUpdate(nameof(Timer), value);
                }
            }
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(Gameplay), (int)Gameplay);
            NotifyUIUpdate(nameof(ShuffleMode), (int)ShuffleMode);
            NotifyUIUpdate(nameof(AllowUseRecorder), AllowUseRecorder);
            NotifyUIUpdate(nameof(AllowUseProp), AllowUseProp);
            NotifyUIUpdate(nameof(Level), (int)Level);
            NotifyUIUpdate(nameof(BGMVolume), BGMVolume);
            NotifyUIUpdate(nameof(SFXVolume), SFXVolume);
            NotifyUIUpdate(nameof(Timer), Timer);
        }

        #endregion

        #region 重写父类UpdateModel()函数


        public override void UpdateModel(string propertyName, int propertyValue)
        {
            if(nameof(Gameplay).Equals(propertyName)){ Gameplay = (Gameplay) propertyValue; }
            else if(nameof(ShuffleMode).Equals(propertyName)){ ShuffleMode = (ShuffleMode) propertyValue; }
            else if(nameof(Level).Equals(propertyName)){ Level = (GameLevel) propertyValue; }
            else if (nameof(Timer).Equals(propertyName)) { Timer = propertyValue; }
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
        }

        public override void UpdateModel(string propertyName, float propertyValue)
        {
            if(nameof(BGMVolume).Equals(propertyName)){ BGMVolume = propertyValue; }
            else if(nameof(SFXVolume).Equals(propertyName)){ SFXVolume = propertyValue; }
        }

        public override void UpdateModel(string propertyName, bool propertyValue)
        {
            if(nameof(AllowUseRecorder).Equals(propertyName)){ AllowUseRecorder = propertyValue; }
            else if(nameof(AllowUseProp).Equals(propertyName)){ AllowUseProp = propertyValue; }
        }
        #endregion
    }
}
