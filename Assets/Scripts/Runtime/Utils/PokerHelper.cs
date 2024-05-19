using HayypCard.Enums;
using System;
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
        /// 判断当前玩家是否是第一个做选择的人
        /// </summary>
        public static bool IsFirstMakeChoice(List<PokerCard> cards,Gameplay gameplay)
        {
            //判断当前玩家是否有牌红桃3，有则先做选择
            if (gameplay == Gameplay.FightLandord)
            {
                return cards.Contains(PokerCard.Diamond_3);
            }
            //判断当前玩家是否有牌黑桃7，有则先做选择
            else if (gameplay == Gameplay.BanZiPao)
            {
                return cards.Contains(PokerCard.Spade_7);
            }
            else if(gameplay == Gameplay.ZhaJinHua)
            {
                //TODO
            }
            return false;
        }

        /// <summary>
        /// 检查是否符合出牌规则  ----- 板子炮
        /// </summary>
        /// <param name="current">当前玩家出的牌</param>
        /// <param name="last">上一个玩家出的牌，如果上家没有出牌则为null</param>
        /// <param name="lastType">上一家出牌的类型</param>
        /// <param name="currentType">当前出牌类型</param>
        /// <returns>当前玩家出牌是否符合规则</returns>
        public static bool Check(List<PokerCard> current, BanZiPaoPokerType currentType, List<PokerCard> last, BanZiPaoPokerType lastType)
        {
            //不符合规则的牌型
            if (currentType == BanZiPaoPokerType.None || lastType == BanZiPaoPokerType.None) { return false; }

            //同一类型比较，牌数必须一致
            if (currentType == lastType && current.Count == last.Count)
            {
                int sum = 0;
                for (int i = 0; i < current.Count; i++)
                    sum += (int)current[i] / 4 - (int)last[i] / 4;
                return sum > 0;
            }

            //不是同一类型则必须是炸弹牌型
            return currentType - lastType > 0 && (int)currentType >= 5;
        }

        /// <summary>
        /// 检查是否符合出牌规则  ----- 板子炮
        /// </summary>
        /// <param name="current">当前玩家出的牌</param>
        /// <param name="last">上一个玩家出的牌，如果上家没有出牌则为null</param>
        /// <param name="lastType">上一家出牌的类型</param>
        /// <param name="currentType">当前出牌类型</param>
        /// <returns>当前玩家出牌是否符合规则</returns>
        public static bool FastCheck(List<PokerCard> current, List<PokerCard> last, BanZiPaoPokerType lastType,out BanZiPaoPokerType currentType)
        {
            currentType = BanZiPaoPokerType.None;

            if (lastType == BanZiPaoPokerType.None) { throw new NotSupportedException("上家出牌类型不能为None类型!"); }

            if (current.Count == last.Count)
            {
                switch (lastType)
                {
                    case BanZiPaoPokerType.Single:
                        if (IsSingle(current)) { currentType = BanZiPaoPokerType.Single; }
                        break;
                    case BanZiPaoPokerType.Double:
                        if (IsDouble(current)) { currentType = BanZiPaoPokerType.Double; }
                        break;
                    case BanZiPaoPokerType.ShunZi:
                        if (IsShunZi(current, 3)) { currentType = BanZiPaoPokerType.ShunZi; }
                        break;
                    case BanZiPaoPokerType.ThreeBomb:
                        if (IsThreeBomb(current)) { currentType = BanZiPaoPokerType.ThreeBomb; }
                        break;
                    case BanZiPaoPokerType.BanZiPao:
                        if (IsBanZiPao(current)) { currentType = BanZiPaoPokerType.BanZiPao; }
                        break;
                    case BanZiPaoPokerType.Bomb:
                        if (IsBomb(current)) { currentType = BanZiPaoPokerType.Bomb; }
                        break;
                    case BanZiPaoPokerType.SmallLoongBomb:
                        if (IsSmallLoongBomb(current)) { currentType = BanZiPaoPokerType.SmallLoongBomb; }
                        break;
                    case BanZiPaoPokerType.BigLoongBomb:
                        if (IsBigLoongBomb(current)) { currentType = BanZiPaoPokerType.BigLoongBomb; }
                        break;
                }
            }
            else if(current.Count < last.Count)
            {
                if (IsThreeBomb(current)) { currentType = BanZiPaoPokerType.ThreeBomb; }
                else if (IsBanZiPao(current)) { currentType = BanZiPaoPokerType.BanZiPao; }
                else if (IsBomb(current)) { currentType = BanZiPaoPokerType.Bomb; }
                else if (IsSmallLoongBomb(current)) { currentType = BanZiPaoPokerType.SmallLoongBomb; }
                else if (IsBigLoongBomb(current)) { currentType = BanZiPaoPokerType.BigLoongBomb; }
             }

            return Check(current, currentType, last, lastType); ;
        }


        /// <summary>
        /// 判断当前玩家的出牌是否符合出牌规则 ----- 斗地主
        /// </summary>
        /// <param name="current">当前玩家出的牌</param>
        /// <param name="currentType">当前玩家出的牌的类型</param>
        /// <param name="last">上一个玩家出的牌</param>
        /// <param name="lastType">上一个玩家出的牌的类型</param>
        /// <remarks>这种方式需要提前指定当前玩家出的牌的类型以及上家出的牌的类型</remarks>
        /// <returns>true:符合出牌规则；false:不符合出牌规则</returns>
        public static bool Check(List<PokerCard> current, LandlordPokerType currentType, List<PokerCard> last, LandlordPokerType lastType)
        {
            //不符合规则的牌型
            if (currentType == LandlordPokerType.None || lastType == LandlordPokerType.None) { return false; }

            //同一类型比较，牌数必须一致
            if (currentType == lastType && current.Count == last.Count)
            {
                //对于单牌、顺子、连对的情况比较元素差的和的大小
                if (currentType == LandlordPokerType.Single || currentType == LandlordPokerType.ShunZi || currentType == LandlordPokerType.LiandDui)
                {
                    int sum = 0;
                    for (int i = 0; i < current.Count; i++)
                        sum += (int)current[i] / 4 - (int)last[i] / 4;
                    return sum > 0;
                }

                //对于其它情况都可以通过与操作比较
                return AddOnlyDuplicateGreater2(current) - AddOnlyDuplicateGreater2(last) > 0;
            }

            //不是同一类型则必须是炸弹牌型
            return currentType - lastType > 0 && (int)currentType >= 19;
        }

        //-----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 判断当前玩家的出牌是否符合出牌规则，无需先获取当前出牌的类型。----- 斗地主
        /// </summary>
        /// <param name="current">当前玩家出的牌</param>
        /// <param name="last">上一个玩家出的牌</param>
        /// <param name="lastType">上一个玩家出的牌的类型</param>
        /// <param name="currentType">当前玩家出牌类型</param>
        /// <remarks>此方法可以在无需提前知道当前玩家出的牌的类型</remarks>
        /// <returns>true:符合出牌规则；false:不符合出牌规则</returns>
        public static bool FastCheck(List<PokerCard> current, List<PokerCard> last, LandlordPokerType lastType, out LandlordPokerType currentType)
        {
            currentType = LandlordPokerType.None;
            if (lastType == LandlordPokerType.None) { throw new NotSupportedException("上家出牌类型不能为None类型!"); }

            if (IsKingBomb(current)) { currentType = LandlordPokerType.KingBomb; return true; }

            if (current.Count == last.Count)
            {
                switch (lastType)
                {
                    case LandlordPokerType.Single:
                        currentType = LandlordPokerType.Single;
                        break;
                    case LandlordPokerType.Double:
                        if (IsDouble(current)) { currentType = LandlordPokerType.Double; }
                        break;
                    case LandlordPokerType.ShunZi:
                        if (IsShunZi(current,5)) { currentType = LandlordPokerType.ShunZi; }
                        break;
                    case LandlordPokerType.LiandDui:
                        if (IsLiandDui(current)) { currentType = LandlordPokerType.LiandDui; }
                        break;
                    case LandlordPokerType.ThreeWithNone:
                        if (IsThreeWithNone(current)) { currentType = LandlordPokerType.ThreeWithNone; }
                        break;
                    case LandlordPokerType.ThreeWithOne:
                        if (IsThreeWithOne(current)) { currentType = LandlordPokerType.ThreeWithOne; }
                        break;
                    case LandlordPokerType.ThreeWihtDouble:
                        if (IsThreeWihtDouble(current)) { currentType = LandlordPokerType.ThreeWihtDouble; }
                        break;
                    case LandlordPokerType.FourWithTwo:
                        if (IsFourWithTwo(current)) { currentType = LandlordPokerType.FourWithTwo; }
                        break;
                    case LandlordPokerType.FourWithDouble:
                        if (IsFourWithDouble(current)) { currentType = LandlordPokerType.FourWithDouble; }
                        break;
                    case LandlordPokerType.FourWithTwoDouble:
                        if (IsFourWithTwoDouble(current)) { currentType = LandlordPokerType.FourWithTwoDouble; }
                        break;
                    case LandlordPokerType.AeroplaneWithNone:
                        if (IsAeroplaneWithNone(current)) { currentType = LandlordPokerType.AeroplaneWithNone; }
                        break;
                    case LandlordPokerType.AeroplaneWithTwo:
                        if (IsAeroplaneWithTwo(current)) { currentType = LandlordPokerType.AeroplaneWithTwo; }
                        break;
                    case LandlordPokerType.AeroplaneWithThree:
                        if (IsAeroplaneWithThree(current)) { currentType = LandlordPokerType.AeroplaneWithThree; }
                        break;
                    case LandlordPokerType.AeroplaneWithFour:
                        if (IsAeroplaneWithFour(current)) { currentType = LandlordPokerType.AeroplaneWithFour; }
                        break;
                    case LandlordPokerType.AeroplaneWithFive:
                        if (IsAeroplaneWithFive(current)) { currentType = LandlordPokerType.AeroplaneWithFive; }
                        break;
                    case LandlordPokerType.AeroplaneWithTwoDouble:
                        if (IsAeroplaneWithTwoDouble(current)) { currentType = LandlordPokerType.AeroplaneWithTwoDouble; }
                        break;
                    case LandlordPokerType.AeroplaneWithThreeDouble:
                        if (IsAeroplaneWithThreeDouble(current)) { currentType = LandlordPokerType.AeroplaneWithThreeDouble; }
                        break;
                    case LandlordPokerType.AeroplaneWithFourDouble:
                        if (IsAeroplaneWithFourDouble(current)) { currentType = LandlordPokerType.AeroplaneWithFourDouble; }
                        break;
                    case LandlordPokerType.Bomb:
                        if (IsBomb(current)) { currentType = LandlordPokerType.Bomb; }
                        break;
                }

                return Check(current, currentType, last, lastType);
            }
            else if (current.Count < last.Count && IsBomb(current))
            {
                currentType = LandlordPokerType.Bomb;
                return Check(current, currentType, last, lastType);
            }

            return false;
        }

        //-----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取斗地主的类型
        /// </summary>
        /// <param name="cards">要判断的牌的类型</param>
        /// <returns>牌的类型</returns>
        public static LandlordPokerType GetLandlordPokerType(List<PokerCard> cards)
        {
            //按牌的奇数还是偶数进行减少条件判断
            //按牌的数量减少条件判断

            int count = cards.Count;
            bool isDouble = count % 2 == 0;
            if (isDouble && count < 5)
            {
                if (IsDouble(cards)) { return LandlordPokerType.Double; }
                else if (IsThreeWithOne(cards)) { return LandlordPokerType.ThreeWithOne; }
                else if (IsBomb(cards)) { return LandlordPokerType.Bomb; }
                else if (IsKingBomb(cards)) { return LandlordPokerType.KingBomb; }
            }
            else if (!isDouble && count <= 5)
            {
                if (IsSingle(cards)) { return LandlordPokerType.Single; }
                else if (IsThreeWithNone(cards)) { return LandlordPokerType.ThreeWithNone; }
                else if (IsThreeWihtDouble(cards)) { return LandlordPokerType.ThreeWihtDouble; }
            }

            if (count >= 5)
            {
                if (IsShunZi(cards,5)) { return LandlordPokerType.ShunZi; }
                else if (IsLiandDui(cards)) { return LandlordPokerType.LiandDui; }
                else if (IsFourWithTwo(cards)) { return LandlordPokerType.FourWithTwo; }
                else if (IsFourWithDouble(cards)) { return LandlordPokerType.FourWithDouble; }
                else if (IsFourWithTwoDouble(cards)) { return LandlordPokerType.FourWithTwoDouble; }
                else if (IsAeroplaneWithNone(cards)) { return LandlordPokerType.AeroplaneWithNone; }
                else if (IsAeroplaneWithTwo(cards)) { return LandlordPokerType.AeroplaneWithTwo; }
            }

            if (count >= 10)
            {
                if (IsAeroplaneWithThree(cards)) { return LandlordPokerType.AeroplaneWithThree; }
                else if (IsAeroplaneWithFour(cards)) { return LandlordPokerType.AeroplaneWithFour; }
                else if (IsAeroplaneWithFive(cards)) { return LandlordPokerType.AeroplaneWithFive; }
                else if (IsAeroplaneWithTwoDouble(cards)) { return LandlordPokerType.AeroplaneWithTwoDouble; }
                else if (IsAeroplaneWithThreeDouble(cards)) { return LandlordPokerType.AeroplaneWithThreeDouble; }
                else if (IsAeroplaneWithFourDouble(cards)) { return LandlordPokerType.AeroplaneWithFourDouble; }
            }

            return LandlordPokerType.None;
        }

        //-----------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 获取板子炮的类型
        /// </summary>
        /// <param name="cards">要判断的牌的类型</param>
        /// <returns>牌的类型</returns>
        public static BanZiPaoPokerType GetBanZiPaoPokerType(List<PokerCard> cards)
        {
            if (IsSingle(cards)) { return BanZiPaoPokerType.Single; }
            if (IsDouble(cards)) { return BanZiPaoPokerType.Double; }
            if (IsShunZi(cards, 3)) { return BanZiPaoPokerType.ShunZi; }
            if (IsThreeBomb(cards)) { return BanZiPaoPokerType.ThreeBomb; }
            if (IsBanZiPao(cards)) { return BanZiPaoPokerType.BanZiPao; }
            if (IsBomb(cards)) { return BanZiPaoPokerType.Bomb; }
            if (IsLoong(cards)) { return BanZiPaoPokerType.Loong; }
            if (IsSmallLoongBomb(cards)) { return BanZiPaoPokerType.SmallLoongBomb; }
            if (IsBigLoongBomb(cards)) { return BanZiPaoPokerType.BigLoongBomb; }
            return BanZiPaoPokerType.None;
        }

        //-----------------------------------------------------------------------------------------------------------


        /// <summary>
        /// 板子炮中的一条龙
        /// </summary>
        private static bool IsLoong(List<PokerCard> cards)
        {
            if (cards.Count == 13)
            {
                int tmp = 0, temp, code, min = 99;
                for (int i = 0; i < cards.Count; i++)
                {
                    code = 3 + ((int)cards[i]) / 4;
                    if (code < min) { min = code; }
                    temp = 1 << code;
                    if ((tmp & temp) != 0) { return false; } 
                    tmp |= temp;
                }

                return (tmp >>= (min + cards.Count)) == 0;
            }
            return false;
        }


        /// <summary>
        /// 板子炮三炸
        /// </summary>
        private static bool IsThreeBomb(List<PokerCard> cards)
        {
            return IsThreeWithNone(cards);
        }

        /// <summary>
        /// 板子炮
        /// </summary>
        private static bool IsBanZiPao(List<PokerCard> cards)
        {
            return IsLiandDui(cards);
        }

        /// <summary>
        /// 小滚龙
        /// </summary>
        private static bool IsSmallLoongBomb(List<PokerCard> cards)
        {
            return IsAeroplaneWithNone(cards);
        }

        /// <summary>
        /// 大滚龙
        /// </summary>
        private static bool IsBigLoongBomb(List<PokerCard> cards)
        {
            return cards.Count==12 && IsAeroplaneWithNone(cards);
        }

        /// <summary>
        /// 单牌
        /// </summary>
        private static bool IsSingle(List<PokerCard> cards)
        {
            return cards.Count == 1;
        }

        /// <summary>
        /// 对子
        /// </summary>
        private static bool IsDouble(List<PokerCard> cards)
        {
            return cards.Count == 2 ? ((int)cards[0]) / 4 == ((int)cards[1]) / 4 : false;
        }

        /// <summary>
        /// 顺子
        /// </summary>
        private static bool IsShunZi(List<PokerCard> cards,int atLeast)
        {
            if (cards.Count >= atLeast)
            {
                int tmp = 0, temp, code, min = 99;
                for (int i = 0; i < cards.Count; i++)
                {
                    code = 3 + ((int)cards[i]) / 4;
                    if (code < min) { min = code; }
                    temp = 1 << code;
                    if ((tmp & temp) != 0 || code > 14) { return false; } //顺子不能包含王和2
                    tmp |= temp;
                }

                return (tmp >>= (min + cards.Count)) == 0;
            }
            return false;
        }

        /// <summary>
        /// 连对
        /// </summary>
        private static bool IsLiandDui(List<PokerCard> cards)
        {
            int count = cards.Count;
            if (count >= 6 && count % 2 == 0)
            {
                int code, min = 99, max = 0, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if (code > 14) { return false; } //不能包含王和2
                    if (code < min) { min = code; }
                    if (code > max) { max = code; }
                }

                return (min + max) * cards.Count / 2 == sum;
            }
            return false;
        }

        /// <summary>
        /// 斗地主单出三张牌
        /// </summary>
        private static bool IsThreeWithNone(List<PokerCard> cards)
        {
            return cards.Count == 3 && IsWith(cards, 3);
        }

        /// <summary>
        /// 三带一
        /// </summary>
        private static bool IsThreeWithOne(List<PokerCard> cards)
        {
            return cards.Count == 4 && IsWith(cards, 3);
        }

        /// <summary>
        /// 三带一对
        /// </summary>
        private static bool IsThreeWihtDouble(List<PokerCard> cards)
        {
            bool f = cards.Count == 5 && IsWith(cards, 4);
            if (f)
            {
                int code, min = 99, max = 0, sum = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    code = 3 + ((int)cards[i]) / 4;
                    sum += code;
                    if (code < min) { min = code; }
                    if (code > max) { max = code; }
                }

                f = min * 3 + max * 2 == sum || min * 2 + max * 3 == sum;
            }
            return f;
        }

        /// <summary>
        /// 四带二
        /// </summary>
        private static bool IsFourWithTwo(List<PokerCard> cards)
        {
            return cards.Count == 6 && IsWith(cards, 4);
        }

        /// <summary>
        /// 四带一对
        /// </summary>
        private static bool IsFourWithDouble(List<PokerCard> cards)
        {
            return cards.Count == 6 && IsWith(cards, 5);
        }

        /// <summary>
        /// 四带两对
        /// </summary>
        private static bool IsFourWithTwoDouble(List<PokerCard> cards)
        {
            bool f = cards.Count == 8 && IsWith(cards, 6);
            if (f) //由于四带两对和飞机带两张的条件一致，需要再进一步杨筛选
            {
                int tmp = 0, code, count = 0;
                for (int i = 0; i < cards.Count; i++)
                {
                    code = 3 + ((int)cards[i]) / 4;
                    tmp |= 1 << code;
                }
                while (tmp != 0)
                {
                    count += (tmp & 1);
                    tmp >>= 1;
                }
                f = count == 3;
            }
            return f;
        }

        /// <summary>
        /// 飞机什么都不带
        /// </summary>
        private static bool IsAeroplaneWithNone(List<PokerCard> cards)
        {
            return cards.Count % 3 == 0 && IsWith(cards, cards.Count);
        }

        /// <summary>
        /// 飞机带两张牌
        /// </summary>
        private static bool IsAeroplaneWithTwo(List<PokerCard> cards)
        {
            return cards.Count == 8 && IsAeroplaneWithWings(cards, 6);
        }

        /// <summary>
        /// 飞机带三张单牌
        /// </summary>
        private static bool IsAeroplaneWithThree(List<PokerCard> cards)
        {
            return cards.Count == 12 && IsAeroplaneWithWings(cards, 9);
        }

        /// <summary>
        /// 飞机带四张单牌
        /// </summary>
        private static bool IsAeroplaneWithFour(List<PokerCard> cards)
        {
            return cards.Count == 16 && IsAeroplaneWithWings(cards, 12);
        }

        /// <summary>
        /// 飞机带五张单牌
        /// </summary>
        private static bool IsAeroplaneWithFive(List<PokerCard> cards)
        {
            return cards.Count == 20 && IsAeroplaneWithWings(cards, 15);
        }

        /// <summary>
        /// 飞机带两对
        /// </summary>
        private static bool IsAeroplaneWithTwoDouble(List<PokerCard> cards)
        {
            return cards.Count == 10 && IsAeroplaneWithWings(cards, 8);
        }

        /// <summary>
        /// 飞机带三对
        /// </summary>
        private static bool IsAeroplaneWithThreeDouble(List<PokerCard> cards)
        {
            return cards.Count == 15 && IsAeroplaneWithWings(cards, 12);
        }

        /// <summary>
        /// 飞机带四对
        /// </summary>
        private static bool IsAeroplaneWithFourDouble(List<PokerCard> cards)
        {
            return cards.Count == 20 && IsAeroplaneWithWings(cards, 16);
        }

        /// <summary>
        /// 炸弹
        /// </summary>
        private static bool IsBomb(List<PokerCard> cards)
        {
            return cards.Count == 4 && IsWith(cards, 4);
        }

        /// <summary>
        /// 王炸
        /// </summary>
        private static bool IsKingBomb(List<PokerCard> cards)
        {
            return cards.Count == 2 && cards.Contains(PokerCard.Black_Joker) && cards.Contains(PokerCard.Red_Joker);
        }

        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// <para>判断是否为三带、四带或什么都不带</para>
        /// 4个相同的牌可以统计出的次数为4; 
        /// 3个相同的牌可以统计出的次数为3; 
        /// 2个相同的牌可以统计出的次数为1;
        /// 其它为0
        /// </summary>
        private static bool IsWith(List<PokerCard> cards, int count)
        {
            int r = 0, n = 0, p, k, tmp, code;
            for (int i = 0; i < cards.Count; i++)
            {
                code = 3 + ((int)cards[i]) / 4;
                tmp = 1 << code;
                p = (r & tmp) >> code; //判断第code位上是否为1
                k = (n & tmp) >> code;
                count -= p + k;
                if (p == 1) { n |= tmp; }
                if (k == 1) { n &= ~tmp; }
                r |= tmp;
            }
            return count == 0;
        }

        /// <summary>
        /// 判断飞机带翅膀
        /// </summary>
        private static bool IsAeroplaneWithWings(List<PokerCard> cards, int count)
        {
            int r = 0, n = 0, p, k, tmp, code, min = 100, max = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                code = 3 + ((int)cards[i]) / 4;
                tmp = 1 << code;
                p = (r & tmp) >> code; //判断第code位上是否为1
                k = (n & tmp) >> code;
                count -= p + k;
                if (p == 1) { n |= tmp; }
                if (k == 1)
                {
                    n &= ~tmp;
                    if (code > max) { max = code; }
                    if (code < min) { min = code; }
                }
                r |= tmp;
            }
            code = 0;
            for (int i = min; i <= max; i++)
                code += 1 << i;
            return count == 0 && (r & code) == code; //判断是否连续
        }

        /// <summary>
        /// 对扑克牌中相同牌的数量大于2的求和
        /// </summary>
        private static int AddOnlyDuplicateGreater2(List<PokerCard> cards)
        {
            // sum只记录相同牌的数量>=3的类型，n只记录相同牌的数量>=2的类型
            int r = 0, n = 0, tmp, code, sum = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                code = 3 + ((int)cards[i]) / 4;
                tmp = 1 << code;
                if ((r & tmp) == tmp)
                {
                    if ((n & tmp) == tmp)
                    {
                        sum |= tmp;
                    }
                    n |= tmp;
                }
                r |= tmp;

            }
            return sum;
        }

    }
}
