using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HappyCard.Network.Entities;
using System;

namespace HappyCard.Network
{
    /// <summary>
    /// 离线模式，为单机游戏提高网络服务（模拟网络服务）
    /// </summary>
    public sealed class GameOfflineService 
    {
        public void SimulateHttpServer<R1, R2>(HttpInfo<R1, R2> httpInfo, GameEvent gameEvent) where R1 : class where R2 : class
        {
            try
            {
                httpInfo.responsed = true;
                switch (gameEvent)
                {
                    case GameEvent.Login:
                        httpInfo.success = Login(httpInfo);
                        break;
                    case GameEvent.Signup:
                        httpInfo.success = Signup(httpInfo);
                        break;
                    case GameEvent.CheckUpdate:
                        httpInfo.success = CheckUpdate(httpInfo);
                        break;
                    case GameEvent.Appeal:
                        httpInfo.success = Appeal(httpInfo);
                        break;
                }

                if (httpInfo.success)
                {
                    httpInfo.onSuccessed?.Invoke();
                }
                else
                {
                    httpInfo.onFailed?.Invoke();
                }
            }
            finally
            {
                httpInfo.Dispose();
            }
        }


        //------------------------------------------------------------------------------------------------

        private bool Appeal<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            httpInfo.exception = "当前无法连接远程服务器!";
            return false;
        }

        //------------------------------------------------------------------------------------------------

        private bool Login<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            Player player = GameDataManager.Instance.Player;

            if(player == null) { player = Player.Default; }
           
            httpInfo.responseData = player as R2;
            return true;
        }

        //------------------------------------------------------------------------------------------------

        private bool Signup<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            try
            {
                httpInfo.responseData = httpInfo.requestData as R2;
                return true;
            }
            catch (Exception e)
            {
                httpInfo.raw = e.Message;
                httpInfo.exception = e.Message;
                return false;
            }
        }

        //------------------------------------------------------------------------------------------------

        private bool CheckUpdate<R1, R2>(HttpInfo<R1, R2> httpInfo) where R1 : class where R2 : class
        {
            httpInfo.responseData = httpInfo.Decode(httpInfo.Encode());
            return true;
        }
    }
}
