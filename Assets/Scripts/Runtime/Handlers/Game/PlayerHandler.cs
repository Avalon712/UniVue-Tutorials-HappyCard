using HappyCard.Entities;
using HappyCard.Managers;
using HayypCard.Entities;
using HayypCard.Enums;
using HayypCard.Utils;
using System.Collections.Generic;
using TMPro;
using UniVue;
using UniVue.Evt;
using UniVue.Runtime.View.Views;
using UniVue.Tween;
using UniVue.Tween.Tweens;

namespace HayypCard.Handlers
{
    /// <summary>
    /// 玩家的控制器，显示玩家的出牌、状态
    /// </summary>
    public sealed class PlayerHandler : EventRegister
    {
        private GamingInfo _gamingInfo;
        private string _timerView;
        private string _showOutCardsView;
        private TweenTimer _timer;

        public PlayerHandler(GamingInfo gamingInfo,string viewName) 
        {
            _gamingInfo = gamingInfo;

            using(var it = Vue.Router.GetView(viewName).GetNestedViews().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    if (it.Current is ClampListView) 
                        _showOutCardsView = it.Current.name;
                    else
                        _timerView = it.Current.name;
                }
            }
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 回合同步
        /// </summary>
        public void BoutSync(BoutRecord bout,string senderID)
        {
            if(senderID == _gamingInfo.PlayerID)
            {
                //1. 记录回合
                _gamingInfo.AddBoutRecord(bout);

                //2. 暂停定时器
                StopTimer();

                //3. 如果当前位出牌则显示出牌
                if (_gamingInfo.State == PlayerGameState.OutCard)
                    ShowOutCards(bout.OutCards);

                //4. 如果当前状态为非出牌状态则情况显示出牌区域
                else
                    CloseShowOutCards();

                //5. 如果当前玩家是包、叫地主则直接重新开始定时器（因为当前又该此玩家进行做选择）
                //TODO：抢地主、反包
                if(bout.State == PlayerGameState.Bao || bout.State == PlayerGameState.CallLandlord)
                {
                    StartTimer();
                }
            }
        }

        public void ShowNext(string receiverID)
        {
            //轮到当前玩家
            if (receiverID == _gamingInfo.PlayerID)
                //1.开始定时
                StartTimer();
            else
                //关闭定时器
                StopTimer();
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 本地同步发牌事件
        /// </summary>
        public void DealCards()
        {
            //判断当前玩家是否是第一个开始做选择的人
            if (PokerHelper.IsFirstMakeChoice(_gamingInfo.OriginalCards, GameDataManager.Instance.RoomInfo.Gameplay))
            {
                StartTimer();
            }
        }

        //------------------------------------------------------------------------------------------

        private void StartTimer()
        {
            TMP_Text timerTxt = Vue.Router.GetView(_timerView).viewObject.GetComponentInChildren<TMP_Text>();
            int timer = GameDataManager.Instance.GameSetting.Timer;

            _timer = TweenBehavior.Timer(() => { timerTxt.text = (--timer).ToString(); })
                .Interval(1).ExecuteNum(timer); //每个1秒执行一次，执行timer次

            _timer.Call(() =>
            {
                LogHelper.Info($"玩家{_gamingInfo.PlayerName}可能已经掉线!");
                //TODO
                //1. 发送心跳包
                //2. 自动做出选择
                //3. 关闭定时器
                Vue.Router.Close(_timerView);
            });
        }

        private void StopTimer()
        {
            _timer?.Kill();
            _timer = null;
            Vue.Router.Close(_timerView);
        }

        private void ShowOutCards(List<PokerCard> outCards)
        {
            outCards.Sort((p1, p2) => p1 - p2);
            
            Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
            ClampListView view = Vue.Router.GetView<ClampListView>(_showOutCardsView);
            for (int i = 0; i < outCards.Count; i++)
                view.AddData(pokers[outCards[i]]);

            Vue.Router.Open(_showOutCardsView);
        }

        private void CloseShowOutCards()
        {
            Vue.Router.Close(_showOutCardsView);
            Vue.Router.GetView<ClampListView>(_showOutCardsView).Clear();
        }
    }
}
