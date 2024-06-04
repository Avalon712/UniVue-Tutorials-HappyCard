using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Utils;
using System.Collections.Generic;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{
    [EventCallAutowire(true, nameof(GameScene.Game))]
    public sealed class Game_MainUI : EventRegister
    {
        public Game_MainUI()
        {
            InitUIElements();
        }

        private void InitUIElements()
        {
            //安排每个玩家的位置顺序以及绑定数据
            List<Player> players = GameDataManager.Instance.RoomInfo.Players;
            string selfID = GameDataManager.Instance.Player.ID;
            int index = players.FindIndex(p => p.ID == selfID); //获取当前玩家的顺序
            ArrangePos(index, players);
        }

        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 安排每个玩家的位置
        /// </summary>
        /// <param name="selfIndex">当前玩家自己的索引顺序</param>
        /// <param name="players">房间中的所有玩家</param>
        private void ArrangePos(int selfIndex, List<Player> players)
        {
            //视图绑定玩家的顺序
            string[] views = new string[]
            {
                nameof(GameUI.LeftPlayerView1),
                nameof(GameUI.RightPlayerView1),
                nameof(GameUI.RightPlayerView2),
                nameof(GameUI.LeftPlayerView2)
            };

            int nextPlayerIndex = (selfIndex + 1) % players.Count;
            int viewIndex = 0;

            while (nextPlayerIndex != selfIndex)
            {
                IView view = Vue.Router.GetView(views[viewIndex++]);
                view.viewObject.SetActive(true);

                //绑定数据
                Player p = players[nextPlayerIndex];
                GamingInfo gamingInfo = GameDataManager.Instance.RoomInfo.PlayerGamingInfos.Find(g => g.PlayerID == p.ID);
                view.BindModel(gamingInfo, false);

                //创建其它玩家在本地的UI表现
                new PlayerUI(view.name);

                //获取下一个玩家的位置索引
                nextPlayerIndex = (nextPlayerIndex + 1) % players.Count;
            }
        }

        //-------------------------------------------------------------------------------------------------

        [EventCall(nameof(GameEvent.ShowPlayerInfo))]
        private void ShowPlayerInfo(string id)
        {
            LogHelper.Info($"\"显示玩家信息-ShowPlayerInfo\"事件触发,显示玩家信息的玩家的ID={id}");
        }

        //-------------------------------------------------------------------------------------------------
    }
}
