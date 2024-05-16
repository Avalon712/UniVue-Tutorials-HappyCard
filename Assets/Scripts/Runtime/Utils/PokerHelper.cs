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
        /// 检查是否符合出牌规则
        /// </summary>
        /// <param name="currentOutCards">当前玩家出的牌</param>
        /// <param name="lastOutCards">上一个玩家出的牌，如果上家没有出牌则为null</param>
        /// <returns>当前玩家出牌是否符合规则</returns>
        public static bool CheckOutCardsRule(Gameplay gameplay,List<PokerCard> currentOutCards, List<PokerCard> lastOutCards)
        {
            return false;
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
    }
}
