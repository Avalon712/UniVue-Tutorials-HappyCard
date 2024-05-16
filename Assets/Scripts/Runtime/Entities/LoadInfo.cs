using UniVue.Model;

namespace HappyCard.Entities
{
    /*
    2024/05/05 12:28:01 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    public sealed class LoadInfo : BaseModel
    {
        private float _process;
        private string _message;

        /// <summary>
        /// 处理进度
        /// </summary>
        public float process
        {
            get => _process;
            set
            {
                if(_process != value)
                {
                    _process = value;
                    NotifyUIUpdate(nameof(process), value);
                }
            }
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        public string message
        {
            get => _message;
            set
            {
                if(_message != value)
                {
                    _message = value;
                    NotifyUIUpdate(nameof(message), value);
                }
            }
        }

        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(process), process);
            NotifyUIUpdate(nameof(message), message);
        }

        #endregion

        #region 重写父类UpdateModel()函数


        public override void UpdateModel(string propertyName, int propertyValue)
        {
        }

        public override void UpdateModel(string propertyName, string propertyValue)
        {
            if(nameof(message).Equals(propertyName)){ message = propertyValue; }
        }

        public override void UpdateModel(string propertyName, float propertyValue)
        {
            if(nameof(process).Equals(propertyName)){ process = propertyValue; }
        }

        public override void UpdateModel(string propertyName, bool propertyValue)
        {
        }
        #endregion
    }
}
