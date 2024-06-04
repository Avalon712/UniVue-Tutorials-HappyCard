using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Controllers;
using HayypCard.Entities;
using HayypCard.Enums;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Evt.Evts;
using UniVue.Runtime.View.Views;
using UniVue.Tween;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class ListCardUI : EventRegister
    {
        private List<Image> _selected;

        /// <summary>
        /// 发牌事件
        /// </summary>
        public void DealCards(List<PokerCard> cards)
        {
            //将玩家的手牌信息数据绑定到视图
            Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;

            cards.Sort((p1, p2) => p1 - p2); //对牌进行排序
            ClampListView listCardView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));

            for (int i = cards.Count - 1; i > -1; i--)
            {
                listCardView.AddData(pokers[cards[i]]);
            }
        }

        //--------------------------------------------------------------------------------------------------


        [EventCall(nameof(GameEvent.OutCard))]
        private void OutCard()
        {
            if (_selected.Count > 0)
            {
                List<PokerCard> selected = Vue.Event.GetRegister<GameController>().Selected;
                Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
                ClampListView listCardsView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));

                for (int i = 0; i < selected.Count; i++)
                {
                    CardInfo cardInfo = pokers[selected[i]];
                    listCardsView.RemoveData(cardInfo); //从手牌区移除
                    Tween(_selected[i], false); //恢复原来的状态
                }

                //排一下序
                listCardsView.Sort((c1, c2) => (c1 as CardInfo).PokerCard - (c2 as CardInfo).PokerCard);

                _selected.Clear();
            }
        }


        //--------------------------------------------------------------------------------------------------


        [EventCall(nameof(GameEvent.Selected))]
        private void Selected(PokerCard pokerCard, UIEvent trigger)
        {
            CardInfo cardInfo = GameDataManager.Instance.Pokers[pokerCard];

            //获取触发事件的UI组件，来做动画效果
            Image img = trigger.GetEventUI<Button>().transform.parent.GetComponent<Image>();

            if (cardInfo.Selected)
            {
                cardInfo.Selected = false;
                _selected.Remove(img);
                Tween(img, false); //动画效果
            }
            else
            {
                cardInfo.Selected = true;  //设置为已选中的状态
                _selected.Add(img); //加入已经选中的集合中
                Tween(img, true);
            }
        }


        //--------------------------------------------------------------------------------------------------


        [EventCall(nameof(GameEvent.Pass))]
        private void Pass()
        {
            //重置所有牌的状态
            Reset();
        }

        //--------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Reset))]
        private void Reset()
        {
            //重置所有牌的状态
            if (_selected.Count > 0)
            {
                for (int i = 0; i < _selected.Count; i++)
                {
                    Tween(_selected[i], false);
                }
                _selected.Clear();
            }
        }

        //--------------------------------------------------------------------------------------------------

        private void Tween(Image img, bool up)
        {
            Vector3 p = img.transform.localPosition;
            p.y = up ? p.y + 40 : p.y - 40;
            TweenBehavior.DoLocalMove(img.transform, 0.1f, p); //动画效果
        }
    }
}
