using HayypCard.Enums;
using System.Collections.Generic;

namespace HayypCard.Utils
{
    public static class PokerHelper
    {
        /// <summary>
        /// 完全可以进行随机化的牌，随着对局的增多，随机程度越高，这里面不包含两张Jaker，
        /// 如果插入了这两张Jaker，洗完牌后记得移除，以免影响后面其它的玩法
        /// </summary>
        private static readonly List<PokerCard> CARDS;

        static PokerHelper()
        {
            CARDS = new List<PokerCard>(54) //54防止添加两种Jaker后扩容
            {
                PokerCard.Spade_3,PokerCard.Heart_3,PokerCard.Club_3,PokerCard.Diamond_3,
                PokerCard.Spade_4,PokerCard.Heart_4,PokerCard.Club_4,PokerCard.Diamond_4,
                PokerCard.Spade_5,PokerCard.Heart_5,PokerCard.Club_5,PokerCard.Diamond_5,
                PokerCard.Spade_6,PokerCard.Heart_6,PokerCard.Club_6,PokerCard.Diamond_6,
                PokerCard.Spade_7,PokerCard.Heart_7,PokerCard.Club_7,PokerCard.Diamond_7,
                PokerCard.Spade_8,PokerCard.Heart_8,PokerCard.Club_8,PokerCard.Diamond_8,
                PokerCard.Spade_9,PokerCard.Heart_9,PokerCard.Club_9,PokerCard.Diamond_9,
                PokerCard.Spade_10,PokerCard.Heart_10,PokerCard.Club_10,PokerCard.Diamond_10,
                PokerCard.Spade_J,PokerCard.Heart_J,PokerCard.Club_J,PokerCard.Diamond_J,
                PokerCard.Spade_Q,PokerCard.Heart_Q,PokerCard.Club_Q,PokerCard.Diamond_Q,
                PokerCard.Spade_K,PokerCard.Heart_K,PokerCard.Club_K,PokerCard.Diamond_K,
                PokerCard.Spade_A,PokerCard.Heart_A,PokerCard.Club_A,PokerCard.Diamond_A,
                PokerCard.Spade_2,PokerCard.Heart_2,PokerCard.Club_2,PokerCard.Diamond_2
            };
        }

        /// <summary>
        /// 板子牌洗牌 ----- 随机洗牌
        /// </summary>
        /// <returns>List<PokerCard[]></returns>
        public static List<PokerCard>[] RandomShuffleForBanZiPao()
        {
            //[0,51]为板子炮的扑克牌范围
            //每人13张牌
            List<PokerCard>[] results = new List<PokerCard>[4]
            { new List<PokerCard>(13), new List<PokerCard>(13), new List<PokerCard>(13), new List<PokerCard>(13) };

            //洗牌
            CARDS.Shuffle();

            for (int i = 0; i < CARDS.Count; i++)
            {
                results[i / 13].Add(CARDS[i]);
            }

            return results;
        }

        /// <summary>
        /// 斗地主随机洗牌
        /// </summary>
        /// <returns></returns>
        public static List<PokerCard>[] RandomShuffleForFightLandlord(out PokerCard[] remaining)
        {
            //每人最多20张牌
            List<PokerCard>[] results = new List<PokerCard>[3] 
            { new List<PokerCard>(20), new List<PokerCard>(20), new List<PokerCard>(20) };
            
            //加入两张大小王
            CARDS.Add(PokerCard.Black_Joker);
            CARDS.Add(PokerCard.Red_Joker);

            CARDS.Shuffle(); //洗牌

            remaining = new PokerCard[3] { CARDS[0], CARDS[1], CARDS[2] };

            for (int i = 3; i < CARDS.Count; i++) { results[(i-3) / 17].Add(CARDS[i]); }

            //移除两张大小王
            CARDS.Remove(PokerCard.Red_Joker);
            CARDS.Remove(PokerCard.Black_Joker);

            return results;
        }

        /// <summary>
        /// 炸金花随机洗牌
        /// </summary>
        /// <param name="people">参与人数</param>
        /// <returns></returns>
        public static List<PokerCard>[] RandomShuffleForZhaJinHua(int people)
        {
            //每人13张牌
            List<PokerCard>[] results = new List<PokerCard>[people];
            for (int i = 0; i < people; i++){ results[i]=new List<PokerCard>(3); }

            CARDS.Shuffle(); //洗牌

            for (int i = 0; i < results.Length; i++)
            {
                int tmp = i * 3;
                results[i].Add(CARDS[tmp]);
                results[i].Add(CARDS[tmp+1]);
                results[i].Add(CARDS[tmp+2]);
            }

            return results;
        }

        /// <summary>
        /// 检查是否符合出牌规则  ----- 板子炮
        /// </summary>
        /// <param name="currentOutCards">当前玩家出的牌</param>
        /// <param name="lastOutCards">上一个玩家出的牌，如果上家没有出牌则为null</param>
        /// <param name="lastOutCardsType">上一家出牌的类型</param>
        /// <returns>当前玩家出牌是否符合规则</returns>
        public static bool CheckOutCardsRuleForBanZiPao(List<PokerCard> currentOutCards, List<PokerCard> lastOutCards, BanZiPaoPokerType lastOutCardsType)
        {
            BanZiPaoPokerType currentType = GetPokerTypeForBanZiPao(currentOutCards);

            if(currentType == BanZiPaoPokerType.None) { return false; }

            int currentTypeCode = (int)currentType;
            int lastTypeCode = (int)lastOutCardsType;

            if(currentTypeCode == lastTypeCode)
            {
                //只有在是顺子的情况下要求两者出的牌的数量必须一致
                if(lastOutCardsType == BanZiPaoPokerType.ShunZi && lastOutCards.Count != currentOutCards.Count)
                    return false;

                int sum = 0;
                for (int i = 0; i < currentOutCards.Count; i++)
                    sum = sum + ((3 + ((int)currentOutCards[0]) / 4) - (3 + ((int)lastOutCards[0]) / 4));
                return sum > 0;
            }

            //大于4后的牌都是炸弹牌型
            return currentTypeCode > lastTypeCode && currentTypeCode > 4;
        }


        /// <summary>
        /// 检查是否符合出牌规则 ---- 斗地主
        /// </summary>
        /// <param name="currentOutCards">当前玩家出的牌</param>
        /// <param name="lastOutCards">上一个玩家出的牌，如果上家没有出牌则为null</param>
        /// <param name="lastOutCardsType">上一家出牌的类型</param>
        /// <returns>当前玩家出牌是否符合规则</returns>
        public static bool CheckOutCardsRuleForLandlord(List<PokerCard> currentOutCards, List<PokerCard> lastOutCards, LandlordPokerType lastOutCardsType)
        {
            LandlordPokerType currentType = GetPokerTypeForLandlord(currentOutCards);

            if (currentType == LandlordPokerType.None) { return false; }

            int currentTypeCode = (int)currentType;
            int lastTypeCode = (int)lastOutCardsType;

            if (currentTypeCode == lastTypeCode)
            {
                //TODO
            }

            //大于16后的牌都是炸弹牌型
            return currentTypeCode > lastTypeCode && currentTypeCode > 16;
        }

        /// <summary>
        /// 炸金花比较大小
        /// </summary>
        /// <param name="current">当前玩家的手牌</param>
        /// <param name="last">上一个玩家的手牌</param>
        /// <returns>大于0：current>last；小于0：current<last</returns>
        public static int Compare(List<PokerCard> current, List<PokerCard> last)
        {
            return 0;
        }


        /// <summary>
        /// 获得当前出牌的类型  ---- 板子炮
        /// </summary>
        public static BanZiPaoPokerType GetPokerTypeForBanZiPao(List<PokerCard> cards) 
        {
            if(cards.Count == 1) { return BanZiPaoPokerType.Single; }
            else if (IsDouble(cards)) { return BanZiPaoPokerType.Double; }
            else if (IsShunZi(cards, 3)) { return BanZiPaoPokerType.ShunZi; }
            else if (IsThreeBomb(cards)) { return BanZiPaoPokerType.ThreeBomb; }
            else if (IsBanZiPao(cards)) { return BanZiPaoPokerType.BanZiPao; }
            else if (IsBomb(cards)) { return BanZiPaoPokerType.Bomb; }
            else if (IsLoong(cards)) { return BanZiPaoPokerType.Loong; }
            else if (IsSmallLoongBomb(cards)) { return BanZiPaoPokerType.SmallLoongBomb; }
            else if (IsBigLoongBomb(cards)) { return BanZiPaoPokerType.BigSmallLoongBomb; }
            return BanZiPaoPokerType.None;
        }

        /// <summary>
        /// 获取当前出牌的类型 ---- 斗地主
        /// </summary>
        private static LandlordPokerType GetPokerTypeForLandlord(List<PokerCard> cards)
        {
            if(cards.Count == 1) { return LandlordPokerType.Single; }
            else if (IsDouble(cards)) { return LandlordPokerType.Double; }
            else if (IsShunZi(cards, 5)) { return LandlordPokerType.ShunZi; }
            else if (IsThreeWithNone(cards)) { return LandlordPokerType.ThreeWithNone; }
            else if (IsThreeWithOne(cards)) { return LandlordPokerType.ThreeWithOne; }
            else if (IsThreeWithDouble(cards)) { return LandlordPokerType.FourWithDouble; }
            else if (IsBomb(cards)) { return LandlordPokerType.Bomb; }
            else if (IsKingBomb(cards)) { return LandlordPokerType.KingBomb; }
            else if (IsFourWithTwo(cards)) { return LandlordPokerType.FourWithTwo; }
            else if (IsFourWithDouble(cards)) { return LandlordPokerType.FourWithDouble; }
            else if (IsFourWithDuplicateDouble(cards)) { return LandlordPokerType.FourWithDuplicateDouble; }
            else if (IsLianDui(cards)) { return LandlordPokerType.LiandDui; }

            else if (IsAeroplaneWithNone(cards)) { return LandlordPokerType.AeroplaneWithNone; }
            else if (IsAeroplaneWithThree(cards)) { return LandlordPokerType.AeroplaneWithThree; }
            else if (IsAeroplaneWithFour(cards)) { return LandlordPokerType.AeroplaneWithFour; }
            else if (IsAeroplaneWithFive(cards)) { return LandlordPokerType.AeroplaneWithFive; }
            else if (IsAeroplaneWithThreeDouble(cards)) { return LandlordPokerType.AeroplaneWithThreeDouble; }
            else if (IsAeroplaneWithFourDouble(cards)) { return LandlordPokerType.AeroplaneWithFourDouble; }

            return LandlordPokerType.None;
        }

        /// <summary>
        /// 是否是连对
        /// </summary>
        public static bool IsLianDui(List<PokerCard> cards)
        {
            return IsBanZiPao(cards);
        }

        /// <summary>
        /// 是否为单出三张牌 --- 斗地主
        /// </summary>
        public static bool IsThreeWithNone(List<PokerCard> cards)
        {
            return IsThreeBomb(cards);
        }

        /// <summary>
        /// 三带一
        /// </summary>
        public static bool IsThreeWithOne(List<PokerCard> cards)
        {
            if (cards.Count == 4)
            {
                int max = -1, min = 100, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                return min * 3 + max == sum || min * 3 + max == sum;
            }
            return false;
        }

        /// <summary>
        /// 三代一对
        /// </summary>
        public static bool IsThreeWithDouble(List<PokerCard> cards)
        {
            if(cards.Count == 5)
            {
                int max = -1, min = 100, sum=0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if(code > max) { max = code; }
                    if(code < min) { min = code; }
                }
                return min * 3 + max * 2 == sum || min * 2 + max * 3 == sum;
            }
            return false;
        }

        /// <summary>
        /// 四代一对
        /// </summary>
        public static bool IsFourWithDouble(List<PokerCard> cards)
        {
            if (cards.Count == 6)
            {
                int max = -1, min = 100, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                return min * 4 + max * 2 == sum || min * 2 + max * 4 == sum;
            }
            return false;
        }

        /// <summary>
        /// 四代二
        /// </summary>
        public static bool IsFourWithTwo(List<PokerCard> cards)
        {
            if (cards.Count == 6)
            {
                int r = 0, count = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    int tmp = 1 << code;
                    count += (r & tmp) > 0 ? 1 : 0;
                    r |= tmp;
                }
                return count == 3;
            }
            return false;
        }


        /// <summary>
        /// 飞机什么都不带
        /// </summary>
        public static bool IsAeroplaneWithNone(List<PokerCard> cards)
        {
            int num = cards.Count / 3;
            if (cards.Count % 3 == 0 && num > 2)
            {
                int max = 0, sum=0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    max = max < code ? code : max;
                }
                return 3 * num * (max - (num - 1) / 2) == sum;
            }
            return false;
        }

        /// <summary>
        /// 飞机带三张单牌
        /// </summary>
        public static bool IsAeroplaneWithThree(List<PokerCard> cards)
        {
            if (cards.Count == 12)
            {
                int r = 0, sum = 0, temp = 0, max = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    int tmp = 1 << code;
                    if((r & tmp) > 0 && code > max)
                        max = code ;
                    else
                        temp += code;
                    r |= tmp;
                }
                return 6 * max - 6 + temp == sum;
            }
            return false;
        }

        /// <summary>
        /// 飞机带四张单牌
        /// </summary>
        public static bool IsAeroplaneWithFour(List<PokerCard> cards)
        {
            if (cards.Count == 16)
            {
                int r = 0, sum = 0, temp = 0, max = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    int tmp = 1 << code;
                    if ((r & tmp) > 0 && code > max)
                        max = code;
                    else
                        temp += code;
                    r |= tmp;
                }
                return 8 * max - 12 + temp == sum;
            }
            return false;
        }

        /// <summary>
        /// 飞机带五张单牌
        /// </summary>
        public static bool IsAeroplaneWithFive(List<PokerCard> cards)
        {
            if (cards.Count == 20)
            {
                int r = 0, sum = 0, temp = 0, max = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    int tmp = 1 << code;
                    if ((r & tmp) > 0 && code > max)
                        max = code;
                    else
                        temp += code;
                    r |= tmp;
                }
                return 10 * max - 20 + temp == sum;
            }
            return false;
        }

        /// <summary>
        /// 飞机带三对
        /// </summary>
        public static bool IsAeroplaneWithThreeDouble(List<PokerCard> cards)
        {
            if (cards.Count == 15)
            {
                int r = 0, count = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    int tmp = 1 << code;
                    count += (r & tmp) > 0 ? 1 : 0;
                    r |= tmp;
                }
                return count == 9;
            }
            return false;
        }

        /// <summary>
        /// 飞机带四对
        /// </summary>
        public static bool IsAeroplaneWithFourDouble(List<PokerCard> cards)
        {
            if (cards.Count == 20)
            {
                int r = 0, count = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    int tmp = 1 << code;
                    count += (r & tmp) > 0 ? 1 : 0;
                    r |= tmp;
                }
                return count == 12;
            }
            return false;
        }

        /// <summary>
        /// 四代两对
        /// </summary>
        public static bool IsFourWithDuplicateDouble(List<PokerCard> cards)
        {
            if (cards.Count == 10)
            {
                int max = -1, min = 100, sum = 0,center=0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                    if(code != max && code != min) { center = code; }
                }
                return min * 4 + max * 2 + center * 2 == sum || 
                       min * 2 + max * 4 + center * 2 == sum || 
                       min * 2 + max * 2 + center * 4 == sum;
            } 
            return false;
        }

        /// <summary>
        /// 是否是王炸
        /// </summary>
        public static bool IsKingBomb(List<PokerCard> cards)
        {
            return cards.Count==2 && cards.Contains(PokerCard.Black_Joker) && cards.Contains(PokerCard.Red_Joker);
        }


        /// <summary>
        /// 是否是顺子
        /// </summary>
        /// <param name="atLeast">至少需要几张牌</param>
        public static bool IsShunZi(List<PokerCard> cards,int atLeast)
        {
            if (atLeast > cards.Count) return false;
            
            int min = 100, max = -1, sum = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                int code = 3 + ((int)cards[i]) / 4;
                if (code >= 15) { return false; } //顺子中不允许带2
                sum += code;
                if (code > max) { max = code; }
                if (code < min) { min = code; }
            }
            //使用等差数列判断
            return (min + max) * cards.Count / 2 == sum;
        }

        /// <summary>
        /// 是否是对子
        /// </summary>
        public static bool IsDouble(List<PokerCard> cards)
        {
            return cards.Count == 2 && (3 + ((int)cards[0]) / 4) - (3 + ((int)cards[1]) / 4) == 0;
        }

        /// <summary>
        /// 是否是板子炮
        /// </summary>
        public static bool IsBanZiPao(List<PokerCard> cards)
        {
            int count = cards.Count;
            if(count >= 6 && count % 2 == 0)
            {
                int min = 100, max = -1, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    if (code >= 15) { return false; } //板子炮中不允许带2
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                return (min + max) * cards.Count / 2 == sum;
            }
            return false;
        }

        /// <summary>
        /// 是否是三炸
        /// </summary>
        public static bool IsThreeBomb(List<PokerCard> cards)
        {
            if(cards.Count == 3)
            {
                int sum=0, first=0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    first = code;
                    sum += code;
                }
                return sum / first == 3;
            }
            return false;
        }

        /// <summary>
        /// 是否是炸弹
        /// </summary>
        public static bool IsBomb(List<PokerCard> cards)
        {
            if (cards.Count == 4)
            {
                int sum = 0, first = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    first = code;
                    sum += code;
                }
                return sum / first == 4;
            }
            return false;
        }

        /// <summary>
        /// 是否是一条龙
        /// </summary>
        public static bool IsLoong(List<PokerCard> cards)
        {
            if (cards.Count != 13) return false;

            int min = 100, max = -1, sum = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                int code = 3 + ((int)cards[i]) / 4;
                sum += code;
                if (code > max) { max = code; }
                if (code < min) { min = code; }
            }
            //使用等差数列判断
            return (min + max) * cards.Count / 2 == sum;
        }

        /// <summary>
        /// 是否是小滚龙
        /// </summary>
        public static bool IsSmallLoongBomb(List<PokerCard> cards)
        {
            int count = cards.Count;
            if (count >= 9 && count % 3 == 0)
            {
                int min = 100, max = -1, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    if (code >= 15) { return false; } //板子炮中不允许带2
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                return (min + max) * cards.Count / 2 == sum;
            }
            return false;
        }

        /// <summary>
        /// 是否是大滚龙
        /// </summary>
        public static bool IsBigLoongBomb(List<PokerCard> cards)
        {
            if (cards.Count == 12)
            {
                int min = 100, max = -1, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    int code = 3 + ((int)cards[i]) / 4;
                    if (code >= 15) { return false; } //板子炮中不允许带2
                    sum += code;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                return (min + max) * cards.Count / 2 == sum;
            }
            return false;
        }

    }
}
