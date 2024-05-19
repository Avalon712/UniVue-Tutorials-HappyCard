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
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Evt.Evts;
using UniVue.Runtime.View.Views;
using UniVue.Tween;
using UniVue.Tween.Tweens;
using UniVue.Utils;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    /// <summary>
    /// 当前玩家自己
    /// 注：在游戏对局中发送的所有同步事件均为 GameEvent.BoutSync
    /// </summary>
    public sealed class PlayerSelfHandler : EventRegister
    {
        private GamingInfo _gamingInfo;
        /// <summary>
        /// 玩家选中的牌
        /// </summary>
        private List<ValueTuple<PokerCard,Image>> _selected;
        /// <summary>
        /// 当前玩家出的牌 ===> _selected.Item1集合
        /// </summary>
        private List<PokerCard> _outCards;
        /// <summary>
        /// 上一个玩家出的牌
        /// </summary>
        private List<PokerCard> _lastPlayerOutCards;
        /// <summary>
        /// 上一家出牌的类型码
        /// </summary>
        private int _lastOutCardsTypeCode;
        /// <summary>
        /// 定时器
        /// </summary>
        private TweenTimer _timer;
        /// <summary>
        /// 显示倒计时
        /// </summary>
        private TMP_Text _timerTxt;

        /// <summary>
        /// 用此值来依次标记当前应该打开哪个操作视图
        /// </summary>
        private int _flag;

        public PlayerSelfHandler(GamingInfo gamingInfo) 
        {
            _gamingInfo = gamingInfo;
            _selected = new(13);
            _outCards = new(13);

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
        private void ReceiveBoutSync(SyncInfo syncInfo)
        {
            BoutRecord bout = JsonConvert.DeserializeObject<BoutRecord>(syncInfo.Message);

            //记录上一家的出牌状态
            if(bout.State == PlayerGameState.OutCard)
            {
                _lastOutCardsTypeCode = bout.PokerTypeCode;
                _lastPlayerOutCards = bout.OutCards;
            }

            //如果轮到当前玩家做选择
            if(syncInfo.ReceiverID == _gamingInfo.PlayerID)
                OpenCurrentOpView(); //打开当前玩家的选项视图

            //如果上家是出牌则要记录上家出的牌
            if (bout.State == PlayerGameState.OutCard)
            {
                _lastPlayerOutCards = bout.OutCards;
                _lastOutCardsTypeCode = bout.PokerTypeCode;
            }

            //本地同步
            LocalSync(bout, syncInfo.SenderID);
        }

        //--------------------------------------------------------------------------------------------

        /// <summary>
        /// 发牌事件
        /// </summary>
        public void DealCards()
        {
            //修改状态
            _gamingInfo.State = PlayerGameState.OutCard;

            RoomInfo roomInfo = GameDataManager.Instance.RoomInfo;
            Gameplay gameplay = roomInfo.Gameplay;

            //将玩家的手牌信息数据绑定到视图
            Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
            List<PokerCard> cards = _gamingInfo.OriginalCards;
            cards.Sort((p1, p2) => p1 - p2); //对牌进行排序
            ClampListView listCardView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));
            for (int i = cards.Count - 1; i > -1; i--)
            {
                listCardView.AddData(pokers[cards[i]]);
            }

            //根据当前游戏模式以及是否该当前玩家第一个进行做选择显示指定操作视图
            if (PokerHelper.IsFirstMakeChoice(_gamingInfo.OriginalCards, gameplay))
            {
                Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("当前您是第一个开始进行做选择的人");
                OpenCurrentOpView();
            }
        }

        //---------------------------------------------------------------------------------------------
        //                                          UI事件                                           //
        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Prepare))]
        private void Prepare()
        {
            //生成回合记录数据
            BoutRecord bout = new() { State = PlayerGameState.Prepared };
            _gamingInfo.AddBoutRecord(bout);

            //关闭准备视图
            Vue.Router.Close(nameof(GameUI.PrepareOperationView));

            //发送同步
            SyncInfo syncInfo = new(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);
        }

        [EventCall(nameof(GameEvent.CallLandlord))]
        private void CallLandlord()
        {
            LogHelper.Info("\"叫地主-CallLandlord\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.CallLandlord };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.FightLandlordOptionalView));

            //发送同步
            SyncInfo syncInfo = new SyncInfo(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);
            
            //直接开始出牌阶段
            Vue.Router.Open(nameof(GameUI.OutCardOperationView));

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);
            
            //停止计时器
            StopTimer();
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
            SyncInfo syncInfo = new SyncInfo(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);

            //停止计时器
            StopTimer();
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
            SyncInfo syncInfo = new SyncInfo(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //直接开始出牌阶段
            Vue.Router.Open(nameof(GameUI.OutCardOperationView));

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);

            //停止计时器
            StopTimer();
        }

        //---------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.NoBao))]
        private void NoBao()
        {
            LogHelper.Info("\"不包牌-NoBao\"事件触发");

            BoutRecord bout = new() { State = PlayerGameState.NoBao };
            _gamingInfo.AddBoutRecord(bout);

            //关闭操作视图
            Vue.Router.Close(nameof(GameUI.BanZiPaoOptionalView));

            //发送同步
            SyncInfo syncInfo = new SyncInfo(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);

            //停止计时器
            StopTimer();
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
                _outCards.Remove(cardInfo.PokerCard);
                Tween(img, false); //动画效果
            }
            else
            {
                cardInfo.Selected = true;  //设置为已选中的状态
                _selected.Add((cardInfo.PokerCard, img)); //加入已经选中的集合中
                _outCards.Add(cardInfo.PokerCard);
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
                int typeCode; //当前出牌的类型码
                if (!CheckOutOutCards(out typeCode))
                {
                    Vue.Router.GetView<TipView>(nameof(GameUI.FastTipView)).Open("当前出牌不符合规则!");
                    return;
                }

                //2. 重置上一家的出牌状态
                _lastPlayerOutCards = null;
                _lastOutCardsTypeCode = 0;

                //3. 显示出的牌以及将出的牌从手牌区移除
                Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
                ClampListView showOutCardsView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ShowOutCardsView));
                ClampListView listCardsView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListCardView));

                for (int i = 0; i < _selected.Count; i++)
                {
                    CardInfo cardInfo = pokers[_selected[i].Item1];
                    showOutCardsView.AddData(cardInfo); //加入显示玩家出牌视图中
                    listCardsView.RemoveData(cardInfo); //从手牌区移除
                    Tween(_selected[i].Item2, false); //恢复原来的状态
                }

                //排一下序
                showOutCardsView.Sort((c1, c2) => (c1 as CardInfo).PokerCard - (c2 as CardInfo).PokerCard);
                listCardsView.Sort((c1, c2) => (c1 as CardInfo).PokerCard - (c2 as CardInfo).PokerCard);

                Vue.Router.Open(nameof(GameUI.ShowOutCardsView)); //打开显示出牌的视图

                //3. 记录回合状态
                BoutRecord bout = new() { State = PlayerGameState.OutCard, PokerTypeCode = typeCode, OutCards = _outCards };
                _gamingInfo.AddBoutRecord(bout);

                //4. 发送同步事件
                SyncInfo syncInfo = new(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
                NetworkManager.Instance.SendSyncInfo(syncInfo);
               
                //本地同步-显示下一个玩家的选择
                ShowNext(syncInfo.ReceiverID);

                _selected.Clear();

                //停止计时器
                StopTimer();
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

            //同步状态
            SyncInfo syncInfo = new(GameEvent.BoutSync, GetNextPlayerID(), JsonConvert.SerializeObject(bout));
            NetworkManager.Instance.SendSyncInfo(syncInfo);

            //重置所有牌的状态
            Reset();

            //关闭出牌选项视图
            Vue.Router.Close(nameof(GameUI.OutCardOperationView));
            //清空显示出牌视图的牌
            Vue.Router.GetView<ClampListView>(nameof(GameUI.ShowOutCardsView)).Clear();

            //本地同步-显示下一个玩家的选择
            ShowNext(syncInfo.ReceiverID);

            //停止计时器
            StopTimer();
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
                _outCards.Clear();
            }
        }

        //---------------------------------------------封装的工具方法------------------------------------------------

        /// <summary>
        /// 获取当前玩家的下一个玩家的ID
        /// </summary>
        private string GetNextPlayerID()
        {
            //获取下一个玩家做选择的ID
            List<GamingInfo> gamingInfos = GameDataManager.Instance.RoomInfo.PlayerGamingInfos;
            string selfId = GameDataManager.Instance.Player.ID;
            int selfIndex = gamingInfos.FindIndex(g => g.PlayerID == selfId);

            int count = gamingInfos.Count;
            for (int i = 1; i < count; i++)
            {
                GamingInfo next = gamingInfos[(selfIndex + i) % count];
                //找到从当前玩家开始,找到下一个第一个没有玩牌的玩家
                if(next.State != PlayerGameState.Finished)
                    return next.PlayerID;
            }
            return null;
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
        private bool CheckOutOutCards(out int typeCode)
        {
            typeCode = 0;

            Gameplay gameplay = GameDataManager.Instance.RoomInfo.Gameplay;

            //1. 如果上一家没有人出牌则当前玩家可以随意进行出牌（只要符合当前的游戏规则）
            if(_lastOutCardsTypeCode == 0)
            {
                if (gameplay == Gameplay.BanZiPao)
                    typeCode = (int)PokerHelper.GetBanZiPaoPokerType(_outCards);
                else if (gameplay == Gameplay.FightLandord)
                    typeCode = (int)PokerHelper.GetLandlordPokerType(_outCards);
            }

            //2. 如果上一家有人出牌则需要比较当前玩家所出的牌是否大于上家
            else
            {
                if (gameplay == Gameplay.BanZiPao)
                {
                    BanZiPaoPokerType lastType = (BanZiPaoPokerType)_lastOutCardsTypeCode;
                    BanZiPaoPokerType currentType;
                    if (PokerHelper.FastCheck(_outCards, _lastPlayerOutCards, lastType, out currentType))
                    {
                        typeCode = (int)currentType;
                    }
                }
                else if (gameplay == Gameplay.FightLandord)
                {
                    LandlordPokerType lastType = (LandlordPokerType)_lastOutCardsTypeCode;
                    LandlordPokerType currentType;
                    if(PokerHelper.FastCheck(_outCards,_lastPlayerOutCards,lastType,out currentType))
                    {
                        typeCode = (int)currentType;
                    }
                }
            }

            return typeCode != 0;
        }

        /// <summary>
        /// 本地同步
        /// </summary>
        private void LocalSync(BoutRecord bout, string senderID)
        {
            //同步到本地的其它玩家
            using (var it = Vue.Event.GetRegisters<PlayerHandler>().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    it.Current.BoutSync(bout, senderID);
                }
            }
        }

        /// <summary>
        /// 显示本地下的一个玩家做选择
        /// </summary>
        private void ShowNext(string receiverID)
        {
            //同步到本地的其它玩家
            using (var it = Vue.Event.GetRegisters<PlayerHandler>().GetEnumerator())
            {
                while (it.MoveNext())
                {
                    it.Current.ShowNext(receiverID);
                }
            }
        }


        /// <summary>
        /// 开始计时器
        /// </summary>
        private void StartTimer()
        {
            int timer = GameDataManager.Instance.GameSetting.Timer;
            _timer = TweenBehavior.Timer(() => _timerTxt.text = (--timer).ToString())
                .Interval(1).ExecuteNum(timer);

            //定时器结束时自动做选择不要
            _timer.Call(Pass);
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        private void StopTimer()
        {
            _timer?.Kill();
            _timer = null;
        }

        /// <summary>
        /// 打开当前选项视图
        /// </summary>
        private void OpenCurrentOpView()
        {
            Gameplay gameplay = GameDataManager.Instance.RoomInfo.Gameplay;
            
            IView view = null;

            if (gameplay == Gameplay.FightLandord)
            {
                if(_flag == 0)
                    view = Vue.Router.GetView(nameof(GameUI.FightLandlordOptionalView));
                else
                    view = Vue.Router.GetView(nameof(GameUI.OutCardOperationView));
            }
            //判断当前玩家是否有牌黑桃7，有则先做选择
            else if (gameplay == Gameplay.BanZiPao)
            {
                if(_flag == 0)
                    view = Vue.Router.GetView(nameof(GameUI.BanZiPaoOptionalView));
                else
                    view = Vue.Router.GetView(nameof(GameUI.OutCardOperationView));
            }
            else
            {
                //TODO
            }

            _flag++;
            //设置当前显示倒计时的组件
            _timerTxt = ComponentFindUtil.DepthFind<TMP_Text>(view.viewObject, "Timer_Txt");
            //开始计时
            StartTimer(); 
        }
    }
}
