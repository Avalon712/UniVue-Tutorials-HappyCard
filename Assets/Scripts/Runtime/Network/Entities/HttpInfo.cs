using HappyCard.Enums;
using HappyCard.Managers;
using Newtonsoft.Json;
using System;

namespace HappyCard.Network.Entities
{
    /// <summary>
    /// 封装Http接口已经处理好的数据信息
    /// </summary>
    /// <typeparam name="R1">请求发送的数据类型</typeparam>
    /// <typeparam name="R2">响应接受的数据类型</typeparam>
    public sealed class HttpInfo<R1, R2> : IDisposable where R1 : class where R2 : class
    {
        private bool _disposed;

        public string url { get; set; }

        public HttpMethod method { get; set; }

        /// <summary>
        /// 请求携带的数据
        /// </summary>
        public R1 requestData { get; set; }

        /// <summary>
        /// 解析好的数据
        /// 注：只有当responed=true时有数据
        /// </summary>
        public R2 responseData { get; set; }

        /// <summary>
        /// 指示当前请求是否已经处理完
        /// </summary>
        public bool responsed { get; internal set; }

        /// <summary>
        /// 请求处理是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 处理成功回调函数
        /// </summary>
        public Action onSuccessed { get; set; }

        /// <summary>
        /// 处理失败回调
        /// </summary>
        public Action onFailed { get; set; }

        /// <summary>
        /// 处理进度
        /// </summary>
        public Action<float> onProcess { get; set; }

        /// <summary>
        /// 处理失败时服务器返回的异常信息
        /// </summary>
        public string exception { get; set; }

        /// <summary>
        /// 没有经过解码的响应数据
        /// </summary>
        public string raw { get; set; }

        /// <summary>
        /// 无参构造方法适合需要自己设置请求参数的GET请求方式
        /// </summary>
        public HttpInfo() { }

        /// <summary>
        /// 适合POST方式以及不需要在url中设置请求参数的GET方式
        /// </summary>
        public HttpInfo(GameEvent gameEvent)
        {
            URLInfo url = NetworkManager.Instance.GetHttpURL(gameEvent);
            this.url = url.url;
            method = url.method;
        }

        public R2 Decode(string json)
        {
            return JsonConvert.DeserializeObject<R2>(json);
        }

        public string Encode()
        {
            return JsonConvert.SerializeObject(requestData);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                exception = null;
                requestData = default;
                responseData = default;
                onFailed = onSuccessed = null;
                onProcess = null;

                _disposed = true;
            }
        }
    }
}
