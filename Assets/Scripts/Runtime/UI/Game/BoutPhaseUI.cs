using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using System;
using TMPro;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Tween;
using UniVue.Utils;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class BoutPhaseUI : EventRegister
    {
        private string _openedPhaseUIViewName;

        /// <summary>
        /// 根据指定的回合阶段显示指定选项
        /// </summary>
        public void OpenPhaseUI(BoutPhase phase,Action onTimerEnd = null)
        {
            bool startTimer = false;
            switch (phase)
            {
                case BoutPhase.Prepare:
                    _openedPhaseUIViewName = nameof(GameUI.PreparePhaseView);
                    break;
                case BoutPhase.DealCard:
                    //None
                    break;
                case BoutPhase.JiaoDiZhu:
                    _openedPhaseUIViewName = nameof(GameUI.JiaoDiZhuPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.QiangDiZhu:
                    _openedPhaseUIViewName = nameof(GameUI.QiangJiaoDiZhuPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.Bao:
                    _openedPhaseUIViewName = nameof(GameUI.BaoPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.FanBao:
                    _openedPhaseUIViewName = nameof(GameUI.FanBaoPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.JiaoDuiYou:
                    _openedPhaseUIViewName = nameof(GameUI.JiaoDuiYouPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.OutCard:
                    _openedPhaseUIViewName = nameof(GameUI.OutCardPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.ZhaJinHuaOutCard:
                    _openedPhaseUIViewName = nameof(GameUI.ZhaJinHuaOutCardPhaseView);
                    startTimer = true;
                    break;
                case BoutPhase.GameOver:
                    //None
                    break;
            }

            Vue.Router.Open(_openedPhaseUIViewName);
            if (startTimer) 
            {
                IView timerView = Vue.Router.GetView(nameof(GameUI.PhaseTimerView));
                Vue.Router.Open(timerView.name);

                //开始定时
                TMP_Text timerTxt = GameObjectFindUtil.BreadthFind("Timer_Txt", timerView.viewObject).GetComponent<TMP_Text>();
                int timer = GameDataManager.Instance.GameSetting.Timer;
                timerTxt.text = timer.ToString();
                onTimerEnd += CloseLastOpendPhaseUI;
                TweenBehavior.Timer(() =>
                {
                    timerTxt.text = timer.ToString();
                    --timer;
                }).Interval(1f).ExecuteNum(timer).Call(onTimerEnd);
            }
        }

        /// <summary>
        /// 关闭上次打开的选项UI
        /// </summary>
        private void CloseLastOpendPhaseUI()
        {
            if (_openedPhaseUIViewName != null)
                Vue.Router.Close(_openedPhaseUIViewName);
            _openedPhaseUIViewName = null;
            //关闭定时器视图
            Vue.Router.Close(nameof(GameUI.PhaseTimerView));
        }
    }



    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class PreparePhaseUI : EventRegister
    {

        [EventCall(nameof(GameEvent.Prepare))]
        private void Prepare()
        {
            //关闭准备视图
            Vue.Router.Close(nameof(GameUI.PreparePhaseView));
        }

    }
}
