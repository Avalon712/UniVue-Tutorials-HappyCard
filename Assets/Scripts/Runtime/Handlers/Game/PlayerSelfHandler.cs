using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Entities;
using HayypCard.Enums;
using HayypCard.Network.Entities;
using HayypCard.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Evt.Evts;
using UniVue.Runtime.View.Views;
using UniVue.Tween;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    /// <summary>
    /// 当前玩家自己
    /// </summary>
    public sealed class PlayerSelfHandler : EventRegister
    {
        private GamingInfo _gamingInfo;
        /// <summary>
        /// 玩家选中的牌
        /// </summary>
        private List<ValueTuple<PokerCard,Image>> _selected;
        /// <summary>
        /// 上一个玩家出的牌
        /// </summary>
        private List<PokerCard> _lastPlayerOutCards; 

        public PlayerSelfHandler(GamingInfo gamingInfo) 
        {
            _gamingInfo = gamingInfo;
            _selected = new(13);

            //绑定数据
            _gamingInfo.Bind(nameof(GameUI.CardView));
            _gamingInfo.Bind(nameof(GameUI.ShowOutCardsView));

            //注册网络同步回调
            SyncInfoResponseHandle handle = new SyncInfoResponseHandle(1);
            handle.JustOneCall = false;

            handle.AddResponseHandle(GameEvent.BoutSync, ReceiveBoutSync);
        }


        //--------------------------------------------------------------------------------------------

        /// <summary>
        /// 接收到回合同步事件
        /// </summary>
        private void ReceiveBoutSync(SyncInfo info)
        {
            BoutRecord bout = JsonConvert.DeserializeObject<BoutRecord>(info.Message);
            //同步到本地的其它玩家
            using(var it = Vue.Event.GetRegisters<PlayerHandler>().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    it.Current.BoutSync(bout);
                }
            }
        }


        //--------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Prepare))]
        private void Prepare()
        {
            //生成回合记录数据
            BoutRecord bout = new(){ State = PlayerGameState.Prepared };
            _gamingInfo.AddBoutRecord(bout);

            //关闭准备视图
            Vue.Router.Close(nameof(GameUI.PrepareOperationView));
        }

        //--------------------------------------------------------------------------------------------

        public void DealCards()
        {
            //修改状态
            _gamingInfo.State = PlayerGameState.None;

            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            Gameplay gameplay = roomInfo.Gameplay;

            //将玩家的手牌信息数据绑定到视图
            Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
            List<PokerCard> cards = _gamingInfo.OriginalCards;
            cards.Sort((p1, p2) => p1 - p2); //对牌进行排序
            ClampListView listCardView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));
            for (int i = cards.Count-1; i > -1; i--) {
                listCardView.AddData(pokers[cards[i]]);
            }

            //测试
            Vue.Router.Open(nameof(GameUI.OutCardOperationView));

            //根据当前游戏模式以及是否该当前玩家第一个进行做选择显示指定操作视图
            //判断当前玩家是否有牌红桃3，有则先做选择
            //if (gameplay == Gameplay.FightLandord && _gamingInfo.OriginalCards.Contains(Enums.PokerCard.Diamond_3))
            //{
            //    Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("你当前拥有红桃3可以先进行叫地主");
            //    Vue.Router.Open(nameof(GameUI.FightLandlordOptionalView));
            //}
            ////判断当前玩家是否有牌黑桃7，有则先做选择
            //else if (gameplay == Gameplay.BanZiPao && _gamingInfo.OriginalCards.Contains(Enums.PokerCard.Spade_7))
            //{
            //    Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("你当前拥有黑桃7可以先进行选择");
            //    Vue.Router.Open(nameof(GameUI.BanZiPaoOptionalView));
            //}
            //else
            //{
            //    //TODO
            //}
        }

        //---------------------------------------------------------------------------------------------
        //                                          UI事件                                           //
        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.CallLandlord))]
        private void CallLandlord()
        {
            LogHelper.Info("\"叫地主-CallLandlord\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.CallLandlord };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.FightLandlordOptionalView));

            //发送同步
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.CallLandlord, null, null));
            
            //直接开始出牌阶段
            Vue.Router.Open(nameof(GameUI.OutCardOperationView));
        }

        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.NoCallLandlord))]
        private void NoCallLandlord()
        {
            LogHelper.Info("\"不叫地主-NoCallLandlord\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.NoCallLandlord };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.FightLandlordOptionalView));

            //发送同步
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.NoCallLandlord, null, GetNextPlayerID()));
        }

        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Bao))]
        private void Bao()
        {
            LogHelper.Info("\"包牌-Bao\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.Bao };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.BanZiPaoOptionalView));

            //发送同步
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.Bao, null, null));

            //直接开始出牌阶段
            Vue.Router.Open(nameof(GameUI.OutCardOperationView));
        }

        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.NoBao))]
        private void NoBao()
        {
            LogHelper.Info("\"包牌-Bao\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.NoBao };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.BanZiPaoOptionalView));

            //发送同步
            NetworkManager.Instance.SendSyncInfo(new SyncInfo(GameEvent.NoBao, null, GetNextPlayerID()));

        }

        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Selected))]
        private void Selected(PokerCard pokerCard, UIEvent trigger)
        {
            LogHelper.Info($"\"选中-Selected\"事件触发,选中的扑克牌{pokerCard}");
            CardInfo cardInfo = GameDataManager.Instance.Pokers[pokerCard];

            //获取触发事件的UI组件，来做动画效果
            Image img = trigger.GetEventUI<Button>().transform.parent.GetComponent<Image>();

            if (cardInfo.Selected)
            {
                cardInfo.Selected = false;
                _selected.Remove((cardInfo.PokerCard, img));
                Tween(img, false); //动画效果
            }
            else
            {
                cardInfo.Selected = true;  //设置为已选中的状态
                _selected.Add((cardInfo.PokerCard, img)); //加入已经选中的集合中
                Tween(img, true);
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.OutCard))]
        private void OutCard()
        {
            LogHelper.Info("\"出牌-OutCard\"事件触发");

            if (_selected.Count > 0)
            {
                //1. 检查是否符合出牌规则
                if (!CheckOutOutCards())
                    Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("当前出牌不符合规则!");

                //2. 显示出的牌以及将出的牌从手牌区移除
                Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
                ClampListView showOutCardsView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ShowOutCardsView));
                ClampListView listCardsView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));

                for (int i = 0; i < _selected.Count; i++)
                {
                    CardInfo cardInfo = pokers[_selected[i].Item1];
                    showOutCardsView.AddData(cardInfo); //加入显示玩家出牌视图中
                    listCardsView.RemoveData(cardInfo); //从手牌区移除
                    Tween(_selected[i].Item2, false);
                }

                Vue.Router.Open(nameof(GameUI.ShowOutCardsView)); //打开显示出牌的视图

                //3. 记录回合状态
                BoutRecord bout = new() { State = PlayerGameState.None };
                _gamingInfo.AddBoutRecord(bout);

                //4. 发送同步事件

                _selected.Clear();
            }
            else
            {
                Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("你尚未选中任何牌!");
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Pass))]
        private void Pass()
        {
            LogHelper.Info("\"不要-Pass\"事件触发");

            //记录回合状态
            BoutRecord bout = new() { State = PlayerGameState.Pass };
            _gamingInfo.AddBoutRecord(bout);

            //重置所有牌的状态
            Reset();
        }

        //--------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Reset))]
        private void Reset()
        {
            LogHelper.Info("\"重选-Reset\"事件触发");

            //重置所有牌的状态
            if (_selected.Count > 0)
            {
                Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
                for (int i = 0; i < _selected.Count; i++)
                {
                    CardInfo cardInfo = pokers[_selected[i].Item1];
                    cardInfo.Selected = false;
                    Tween(_selected[i].Item2, false);
                }
                _selected.Clear();
            }
        }

        //---------------------------------------------封装的工具方法------------------------------------------------

        /// <summary>
        /// 获取当前玩家的下一个玩家的ID
        /// </summary>
        private string GetNextPlayerID()
        {
            //获取下一个玩家做选择的ID
            List<Player> players = GameDataManager.Instance.RoomInfo.Players;
            string selfId = GameDataManager.Instance.Player.ID;
            int selfIndex = players.FindIndex(p => p.ID == selfId);
            return players[(selfIndex + 1) % players.Count].ID;
        }

        private void Tween(Image img,bool up)
        {
            Vector3 p = img.transform.localPosition;
            p.y = up ?  p.y + 40 : p.y - 40;
            TweenBehavior.DoLocalMove(img.transform, 0.1f, p); //动画效果
        }

        /// <summary>
        /// 检查玩家出牌是否符合规则
        /// </summary>
        private bool CheckOutOutCards()
        {
            //1. 如果上一家没有人出牌则当前玩家可以随意进行出牌（只要符合当前的游戏规则）

            //2. 如果上一家有人出牌则需要比较当前玩家所出的牌是否大于上家

            return false;
        }
    }
}
