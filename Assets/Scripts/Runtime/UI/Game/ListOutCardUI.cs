using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Controllers;
using HayypCard.Entities;
using HayypCard.Enums;
using System.Collections.Generic;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.Runtime.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class ListOutCardUI : EventRegister
    {

        [EventCall(nameof(GameEvent.OutCard))]
        private void OutCard()
        {
            List<PokerCard> selected = Vue.Event.GetRegister<GameController>().Selected;

            if (selected != null && selected.Count > 0)
            {
                Dictionary<PokerCard, CardInfo> pokers = GameDataManager.Instance.Pokers;
                ClampListView listOutCardView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListOutCardView));
                listOutCardView.Clear();
                
                for (int i = 0; i < selected.Count; i++)
                {
                    listOutCardView.AddData(pokers[selected[i]]);
                }

                //排一下序
                listOutCardView.Sort((c1, c2) => (c1 as CardInfo).PokerCard - (c2 as CardInfo).PokerCard);

                Vue.Router.Open(listOutCardView.name);
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.Pass))]
        private void Pass()
        {
            ClampListView listOutCardView = Vue.Router.GetView<ClampListView>(nameof(GameUI.ListOutCardView));
            listOutCardView.Clear();
        }
    }
}
