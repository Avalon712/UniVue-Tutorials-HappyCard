using UniVue.ViewModel.Attr;

namespace HayypCard.Enums
{
    /// <summary>
    /// 商品的标签
    /// </summary>
    public enum ProductLabel
    {
        None,

        [EnumAlias("火热")]
        HOT,

        [EnumAlias("推荐")]
        Recommend,

        [EnumAlias("新品")]
        NEW,
    }
}
