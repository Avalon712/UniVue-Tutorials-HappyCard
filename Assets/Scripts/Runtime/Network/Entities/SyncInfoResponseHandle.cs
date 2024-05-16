using HappyCard.Enums;
using System;
using System.Collections.Generic;

namespace HayypCard.Network.Entities
{
    public struct SyncInfoResponseHandle 
    {
        /// <summary>
        /// 是否只执行一次回调就销毁，默认为true
        /// </summary>
        public bool JustOneCall { get; set; }

        /// <summary>
        /// 注册的事件触发时回调函数
        /// </summary>
        public List<ValueTuple<GameEvent,Action<SyncInfo>>> Calls { get; set; }

        /// <summary>
        /// 构造函数，参数是要注册的事件数
        /// </summary>
        /// <param name="count">要绑定的事件数</param>
        public SyncInfoResponseHandle(int count)
        {
            Calls = new List<(GameEvent, Action<SyncInfo>)>(count);
            JustOneCall = true;
        }

        public SyncInfoResponseHandle AddResponseHandle(GameEvent gameEvent,Action<SyncInfo> call)
        {
            Calls.Add((gameEvent, call));
            return this;
        }

        /// <summary>
        /// 是否销毁此同步响应
        /// </summary>
        public bool Handle(SyncInfo syncInfo)
        {
            bool flag = false;
            List<ValueTuple<GameEvent, Action<SyncInfo>>> calls = Calls;
            for (int i = 0; i < calls.Count; i++)
            {
                if (calls[i].Item1 == syncInfo.GameEvent)
                {
                    flag = true;
                    calls[i].Item2.Invoke(syncInfo);
                }
            }

            return JustOneCall && flag;
        }

    }
}
