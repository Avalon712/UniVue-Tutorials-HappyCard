using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Utils;
using System;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.Handlers
{
    [EventCallAutowire(true,nameof(GameScene.Main),nameof(GameScene.Room),nameof(GameScene.Game))]
    public sealed class PropHandler : EventRegister
    {

        [EventCall(nameof(GameEvent.UseProp))]
        private void UseProp(string type)
        {
            LogHelper.Info($"\"使用道具-UseProp\"事件触发：道具类型{type}");
            UseProp(PropTypeHelper.EnStringToEnum(type));
        }

        //--------------------------------------------------------------------------------------------


        [EventCall(nameof(GameEvent.ShowPropInfo))]
        private void ShowPropInfo(string type)
        {
            LogHelper.Info($"\"显示道具信息-ShowPropInfo\"事件触发：道具类型{type}");

            PropType propType = PropTypeHelper.EnStringToEnum(type);
            BagItem bagItem = GameDataManager.Instance.Bag.Find((item) => item.PropType == propType);
            //显示道具信息
            Vue.Router.GetView(nameof(GameUI.PropInfoView)).RebindModel(bagItem);
        }

        //--------------------------------------------------------------------------------------------

        /// <summary>
        /// 使用指定类型的道具
        /// </summary>
        /// <returns>是否使用成功</returns>
        public bool UseProp(PropType propType)
        {
            BagItem bagItem = GameDataManager.Instance.Bag.Find((item) => item.PropType == propType);

            //先判断是否已经使用过同类型的道具
            if (bagItem == null || bagItem.Using)
            {
                Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("你已经使用了一个相同类型的道具,不能再使用!");
                return false;
            }
            else
            {
                Player player = GameDataManager.Instance.Player;
                PropInfo propInfo = GameDataManager.Instance.PropInfos[propType];

                //如果当前属于增益效果的道具，则需要进行判断，最多只能使用5个增益类型的道具
                if (propInfo.Gain && player.UsedProps.Count==5)
                {
                    Vue.Router.GetView<TipView>(nameof(GameUI.TipView)).Open("最多只能使用5个增益效果类型的道具哦!");
                    return false;
                }

                //更新玩家的道具的信息
                bagItem.Using = true;
                bagItem.StartUseDate = DateTime.Now;
                bagItem.Num -= 1;

                if (bagItem.Num == 0)
                {
                    GameDataManager.Instance.Bag.Remove(bagItem);
                    //更新玩家的背包视图
                    Vue.Router.GetView<GridView>(nameof(GameUI.BagView)).RemoveData(bagItem);
                }

                //添加数据，只有增益型的道具才添加
                if (propInfo.Gain) 
                {
                    player.UsedProps.Add(propType);
                    //更新玩家的使用的道具的图标
                    player.UsedPropIcons.AddIf(GameDataManager.Instance.PropInfos[propType].Icon);
                    //通知进行更新UI
                    player.NotifyUIUpdate(nameof(player.UsedPropIcons), player.UsedPropIcons);
                    player.NotifyUIUpdate(nameof(player.IsUsingPropTip), player.IsUsingPropTip);
                }
            }

            return true;
        }

    }
}
