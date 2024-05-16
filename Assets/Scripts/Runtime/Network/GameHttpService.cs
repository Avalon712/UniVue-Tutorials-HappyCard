using HappyCard.Network.Entities;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HappyCard.Network
{
    /// <summary>
    /// 负责向服务器发送Http请求
    /// </summary>
   // [DisallowMultipleComponent]
    public sealed class GameHttpService : MonoBehaviour
    {

        public void AsyncSendRequest<R1,R2>(HttpInfo<R1,R2> httpInfo) where R1:class where R2:class
        {
            if(httpInfo.method == HttpMethod.GET)
            {
                StartCoroutine(AsyncSendGetRequest(httpInfo));
            }
            else if(httpInfo.method == HttpMethod.POST)
            {
                StartCoroutine(AsyncSendPostRequest(httpInfo));
            }
        }

        //-----------------------------------------------------------------------------------------

        public IEnumerator AsyncSendGetRequest<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            UnityWebRequest www = UnityWebRequest.Get(httpInfo.url);
            var asyncOperation = www.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                httpInfo.onProcess?.Invoke(asyncOperation.progress);
                yield return null;
            }

            httpInfo.success = www.result == UnityWebRequest.Result.Success;
            httpInfo.raw = www.downloadHandler.text;

            OnCompleted(www,httpInfo);
        }

        public IEnumerator AsyncSendPostRequest<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(httpInfo.Encode());
            UnityWebRequest www = new UnityWebRequest(httpInfo.url, "POST");
            www.uploadHandler = new UploadHandlerRaw(data);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var asyncOperation = www.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                httpInfo.onProcess?.Invoke(asyncOperation.progress);
                yield return null;
            }

            httpInfo.success = www.result == UnityWebRequest.Result.Success;
            httpInfo.raw = www.downloadHandler.text;

            OnCompleted(www,httpInfo);
        }

        //-----------------------------------------------------------------------------------------

        private void OnCompleted<R1, R2>(UnityWebRequest www, HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            httpInfo.success = www.result == UnityWebRequest.Result.Success;
            httpInfo.responsed = true;
            httpInfo.raw = www.downloadHandler.text;

            if (httpInfo.success)
            {
                httpInfo.responseData = httpInfo.Decode(httpInfo.raw);
                if (httpInfo.onSuccessed != null) { httpInfo.onSuccessed(); }
            }
            else
            {
                httpInfo.exception = www.error;
                if (httpInfo.onFailed != null) { httpInfo.onFailed(); }
            }

            //网络资源释放
            www.Dispose(); 
            httpInfo.Dispose(); 
        }
    }
}

