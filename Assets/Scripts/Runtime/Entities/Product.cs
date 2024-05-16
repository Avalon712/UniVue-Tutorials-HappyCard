using UniVue.Model;
using UnityEngine;
using HayypCard.Enums;
using System;

namespace HappyCard.Entities
{
    /*
    2024/05/06 17:58:47 Build By UniVue ScriptEditor
    UniVue 作者: Avalon712
    Github地址: https://github.com/Avalon712/UniVue
    */

    /// <summary>
    /// 静态数据
    /// </summary>
    public sealed class Product : ScriptableModel
    {
        [Header("唯一编号")]
        [SerializeField] private string id;

        [Header("商品分类")]
        [SerializeField] private ProductType productType;

        [Header("商品名称")]
        [SerializeField] private string productName;

        [Header("商品上架时间")]
        [SerializeField] private string listDate;

        [Header("购买此商品要消耗的货币类型")]
        [SerializeField] private CurrencyType currencyType;

        [Header("商品标签")]
        [SerializeField] private ProductLabel productLabel;

        [Header("原价")]
        [SerializeField] private int price;

        [Header("折扣")]
        [Range(1,10)]
        [SerializeField] private float discount;

        [Header("售卖数量")]
        [SerializeField] private int sellNum;

        [Header("额外赠送")]
        [Range(1,200)]
        [SerializeField] private int bonus;

        [Header("商品图标")]
        [SerializeField] private Sprite icon;

        [Header("商品标签的图标")]
        [SerializeField] private Sprite labelIcon;

        [Header("货币图标")]
        [SerializeField] private Sprite currencyIcon;

        /// <summary>
        /// 唯一编号
        /// </summary>
        public string ID => id;

        /// <summary>
        /// 商品分类
        /// </summary>
        public ProductType ProductType => productType;

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName => productName;

        /// <summary>
        /// 购买此商品要消耗的货币类型
        /// </summary>
        public CurrencyType CurrencyType => currencyType;

        /// <summary>
        /// 商品标签
        /// </summary>
        public ProductLabel ProductLabel => productLabel;

        /// <summary>
        /// 售卖数量
        /// </summary>
        public int SellNum => sellNum;

        /// <summary>
        /// 额外赠送数量的比例,5表示5%
        /// </summary>
        public int Bonus => bonus;

        /// <summary>
        /// 商品图标
        /// </summary>
        public Sprite Icon => icon;

        /// <summary>
        /// 商品标签的图标
        /// </summary>
        public Sprite LabelIcon => labelIcon;

        /// <summary>
        /// 购买此商品要消耗的货币类型
        /// </summary>
        public Sprite CurrencyIcon => currencyIcon;

        /// <summary>
        /// 折扣，7标识打7折
        /// </summary>
        public float Discount => discount;

        /// <summary>
        /// 没有打折前的价格
        /// </summary>
        public int Price => price;

        /// <summary>
        /// 打折后的价格
        /// </summary>
        public int RealPrice =>(int)( Price * (Discount / 10));


        #region 重写父类NotifyAll()函数

        public override void NotifyAll()
        {
            NotifyUIUpdate(nameof(ID), ID);
            NotifyUIUpdate(nameof(ProductName), ProductName);
            NotifyUIUpdate(nameof(Bonus), Bonus);
            NotifyUIUpdate(nameof(Icon), Icon);
            NotifyUIUpdate(nameof(LabelIcon), LabelIcon);
            NotifyUIUpdate(nameof(CurrencyIcon), CurrencyIcon);
            NotifyUIUpdate(nameof(Discount), Discount);
            NotifyUIUpdate(nameof(Price), Price);
            NotifyUIUpdate(nameof(RealPrice), RealPrice);
            NotifyUIUpdate(nameof(SellNum), SellNum);
        }

        #endregion
    }
}
