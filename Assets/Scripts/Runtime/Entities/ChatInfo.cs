using HappyCard.Managers;
using Newtonsoft.Json;
using UniVue.Model;

namespace HayypCard.Entities
{
    public sealed class ChatInfo : BaseModel
    {
        /// <summary>
        /// 说话人的名字
        /// </summary>
        public string Speaker { get; set; }

        /// <summary>
        /// 说话内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 聊天的完整消息 ，富文本
        /// 4091DD
        /// </summary>
        [JsonIgnore]
        public string Aspect => GameDataManager.Instance.Player.Name==Speaker ? $"<color=#4091DD>我: </color>{Message}" : $"<color=#BF8D48>{Speaker}: </color>{Message}";

        public ChatInfo() { }

        public ChatInfo(string speaker, string message)
        {
            Speaker = speaker;
            Message = message;
        } 

        public override void NotifyAll()
        { 
        //    NotifyUIUpdate(nameof(Speaker), Speaker);
        //    NotifyUIUpdate(nameof(Message), Message);
            NotifyUIUpdate(nameof(Aspect), Aspect);
        }

    }
}
