using HappyCard.Entities;
using HappyCard.Enums;
using HappyCard.Managers;
using HayypCard.Enums;
using HayypCard.Utils;
using UniVue;
using UniVue.Evt;
using UniVue.Evt.Attr;
using UniVue.View.Views;

namespace HayypCard.UI
{

    [EventCallAutowire(true, nameof(GameScene.Main))]
    public sealed class ShopUI : EventRegister
    {
        #region 购买体力商品

        [EventCall(nameof(GameEvent.BuyHP))]
        private void BuyHP(string id)
        {
            LogHelper.Info($"\"购买体力商品-BuyHP\"事件触发：商品编号{id}");
            Shop(id, ProductType.HP);
        }

        #endregion

        #region 购买金币商品

        [EventCall(nameof(GameEvent.BuyCoin))]
        private void BuyCoin(string id)
        {
            LogHelper.Info($"\"购买金币商品-BuyCoin\"事件触发：商品编号{id}");
            Shop(id, ProductType.Coin);
        }

        #endregion

        #region 购买钻石商品

        [EventCall(nameof(GameEvent.BuyDiamond))]
        private void BuyDiamond(string id)
        {
            LogHelper.Info($"\"购买钻石商品-BuyDiamond\"事件触发：商品编号{id}");
            Shop(id, ProductType.Diamond);
        }

        #endregion

        #region 购买刀具商品

        [EventCall(nameof(GameEvent.BuyProp))]
        private void BuyProp(string id)
        {
            LogHelper.Info($"\"购买道具商品-BuyProp\"事件触发：商品编号{id}");
            Shop(id, ProductType.Prop);
        }

        #endregion

        /// <summary>
        /// 购买指定类型、指定商品
        /// </summary>
        /// <param name="id">商品编号</param>
        /// <param name="productType">商品类型</param>
        public void Shop(string id, ProductType productType)
        {
            //1. 获取商品
            Product product = GameDataManager.Instance.GetProduct(id, productType);
            //2. 消费计算
            if (Consume(product))
            {
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("购买成功！");
            }
            else
            {
                Vue.Router.GetView<TipView>(nameof(HappyCard.Enums.GameUI.TipView)).Open("购买失败！您的购买条件尚不成熟！");
            }
        }

        /// <summary>
        /// 消费计算
        /// </summary>
        /// <returns>是否购买成功，true:成功</returns>
        private bool Consume(Product product)
        {
            Player player = GameDataManager.Instance.Player;

            int coins = player.Coin;
            int hps = player.HP;
            int diamonds = player.Diamond;

            //1. 先预计算再更新
            switch (product.CurrencyType)
            {
                case CurrencyType.Coin:
                    coins -= product.RealPrice;
                    if (coins < 0) { return false; } else { player.Coin = coins; }
                    break;
                case CurrencyType.Diamond:
                    diamonds -= product.RealPrice;
                    if (diamonds < 0) { return false; } else { player.Diamond = diamonds; }
                    break;
                case CurrencyType.HP:
                    hps -= product.RealPrice;
                    if (hps < 0) { return false; } else { player.HP = hps; }
                    break;
            }

            //2. 更新玩家的货币有关数据
            switch (product.ProductType)
            {
                case ProductType.Coin:
                    player.Coin += (int)(product.SellNum * (1 + product.Bonus / 100f));
                    break;

                case ProductType.Diamond:
                    player.Diamond += (int)(product.SellNum * (1 + product.Bonus / 100f));
                    break;

                case ProductType.HP:
                    player.HP += (int)(product.SellNum * (1 + product.Bonus / 100f));
                    break;

                case ProductType.Prop:
                    var item = GameDataManager.Instance.AddBagItem(PropTypeHelper.CNStringToEnum(product.ProductName));
                    if (item != null)
                    {
                        //刷新视图
                        Vue.Router.GetView<GridView>(nameof(HappyCard.Enums.GameUI.BagView)).AddData(item);
                    }
                    break;
            }

            return true;
        }
    }
}
