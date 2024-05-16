using HayypCard.Enums;
using System;

namespace HayypCard.Utils
{
    public sealed class PropTypeHelper
    {
        private PropTypeHelper() { }

        /// <summary>
        /// 道具类型的英文名称映射为枚举
        /// </summary>
        /// <param name="typeString"></param>
        /// <remarks>不使用反射、以及减少字符串垃圾</remarks>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static PropType EnStringToEnum(string typeString)
        {
            switch (typeString)
            {
                case nameof(PropType.RoomCard):
                    return PropType.RoomCard;

                case nameof(PropType.DoubleCard):
                    return PropType.DoubleCard;

                case nameof(PropType.Income14):
                    return PropType.Income14;

                case nameof(PropType.Income30):
                    return PropType.Income30;

                case nameof(PropType.HpRecover):
                    return PropType.HpRecover;

                case nameof(PropType.Luck):
                    return PropType.Luck;

                case nameof(PropType.MaxRandomDiamondGift):
                    return PropType.MaxRandomDiamondGift;

                case nameof(PropType.RandomCoinGift):
                    return PropType.RandomCoinGift;

                case nameof(PropType.RandomDiamondGift):
                    return PropType.RandomDiamondGift;

                case nameof(PropType.RandomHpGift):
                    return PropType.RandomHpGift;

                case nameof(PropType.RandomLuckGift):
                    return PropType.RandomLuckGift;

                case nameof(PropType.RandomPropGift):
                    return PropType.RandomPropGift;

                case nameof(PropType.CardRecorder):
                    return PropType.CardRecorder;

                case nameof(PropType.RegretCard):
                    return PropType.RegretCard;

                case nameof(PropType.TipCard):
                    return PropType.TipCard;
            }

            throw new NotSupportedException($"无法将字符串{typeString}转为PropType枚举类型");
        }

        /// <summary>
        /// 道具类型的中文名称映射为枚举
        /// </summary>
        /// <param name="typeString"></param>
        /// <remarks>不使用反射、以及减少字符串垃圾</remarks>
        /// <returns></returns>
        public static PropType CNStringToEnum(string typeString)
        {
            switch (typeString)
            {
                case "记牌器": return PropType.CardRecorder;
                case "加倍卡": return PropType.DoubleCard;
                case "体力恢复卡": return PropType.HpRecover;
                case "14天收益增加卡":return PropType.Income14;
                case "30天收益增加卡":return PropType.Income30;
                case "幸运四叶草":return PropType.Luck;
                case "超大随机钻石礼物":return PropType.MaxRandomDiamondGift;
                case "随机金币礼物":return PropType.RandomCoinGift;
                case "随机钻石礼物":return PropType.RandomDiamondGift;
                case "随机体力礼物":return PropType.RandomHpGift;
                case "随机幸运四叶草礼物":return PropType.RandomLuckGift;
                case "随机道具礼物":return PropType.RandomPropGift;
                case "悔牌卡":return PropType.RegretCard;
                case "房卡":return PropType.RoomCard;
                case "出牌提示卡":return PropType.TipCard;
            }

            throw new NotSupportedException($"无法将字符串{typeString}转为PropType枚举类型");
        }
    }
}
