using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Controller;

namespace test01.Model
{
    public class HandsEvaluator
    {
        public enum Hands
        {
            SameRank,
            Flush,
            Both,
            Illegal,
            Null
        }

        //回傳不需要被改為灰階的牌
        public IEnumerable<Card> Recommand(IEnumerable<Card> hand, IEnumerable<Card> selectedCards, IEnumerable<Card> currentPlay, Hands currentHands, bool isReversed, bool isSuitLocked)
        {
            List<Card> matchingCards = new List<Card>();
            List<Card> playerHand = hand.ToList();
            int handJokerCount = 0; //手中的joker數量
            for(int i = playerHand.Count() - 1; i >= 0; i--)
            {
                if (playerHand[i].SuitType != Card.Suit.JOKER)
                    break;

                matchingCards.Add(playerHand[i]);
                handJokerCount++;
                playerHand.RemoveAt(i);
            }
            //該玩家為新出牌者，推薦所有可行同點或同花順
            if (currentPlay == null)
            { 
                //還沒選牌的初始狀態
                if(selectedCards.Count() == 0)
                {
                    //回傳空指標代表將全部牌都能打
                    return null;
                }

                int selJokerCnt = GetJokerCount(selectedCards); //選中牌中joker的數量
                int nonJokerCardsCnt = selectedCards.Count() - selJokerCnt;
                if (nonJokerCardsCnt == 0)
                {
                    //只選鬼牌時，所有牌都能與之成型
                    return null;
                }
                else if (nonJokerCardsCnt == 1)
                {
                    //找同點
                    matchingCards.AddRange(FindSameRank(playerHand, selectedCards));

                    //找同花順
                    matchingCards.AddRange(FindFlush(playerHand, selectedCards, handJokerCount));
                }
                else //選超過一張非joker牌
                {
                    Hands selHands = HandsType(selectedCards);
                    if(selHands == Hands.SameRank)
                    {
                        matchingCards.AddRange(FindSameRank(playerHand, selectedCards));
                    }
                    else //同花順或組到一半的同花順
                    {
                        matchingCards.AddRange(FindFlush(playerHand, selectedCards, handJokerCount));
                    }
                }
            }
            else //場上已有牌型
            {
                //手上張數不夠
                int currentCount = currentPlay.Count();
                if (hand.Count() < currentCount)
                {
                    matchingCards.Clear();
                    return matchingCards;
                }

                int currentWeight = GetHandsWeight(currentHands, currentPlay, isReversed);
                //還沒選牌的初始狀態
                if (selectedCards.Count() == 0)
                {
                    if (currentHands == Hands.SameRank)
                    {
                        //如果只有一張大王，必輸
                        if(currentPlay.First().RankType == Card.Rank.RED)
                        {
                            matchingCards.Clear();
                            return matchingCards;
                        }

                        var validPlays = playerHand
                            //比較權重，並考慮是否反轉
                            .Where(c => c.Weight * (isReversed ? -1 : 1) > currentWeight)
                            //若鎖花色則只留同花色
                            .Where(c => !isSuitLocked || currentCount != 1 || c.SuitType == Card.Suit.JOKER || c.SuitType == currentPlay.First().SuitType)
                            //以權重分組
                            .GroupBy(c => c.Weight)
                            //該組合張數加上鬼牌數量應大於場上牌型數量
                            .Where(g => g.Count() + handJokerCount >= currentCount)
                            //所有組合各自轉回List
                            .Select(g => g.ToList())
                            //變成List< list<> >
                            .ToList();

                        foreach (List<Card> play in validPlays) {
                            matchingCards.AddRange(play);
                        }
                    }
                    else//場上是同花順
                    {
                        //所有非鬼牌花色
                        var allSuits = new[] { Card.Suit.SPADES, Card.Suit.HEARTS, Card.Suit.DIAMONDS, Card.Suit.CLUBS };

                        //同花順中的最小與最大權重邊界
                        int ABSOLUTE_MIN_WEIGHT = 0;
                        int ABSOLUTE_MAX_WEIGHT = 12;

                        foreach (var suit in allSuits)
                        {
                            //取得該花色的牌
                            var sameSuitHand = playerHand.Where(c => c.SuitType == suit).ToList();

                            //如果加上鬼牌張數都不夠，直接略過
                            if (sameSuitHand.Count + handJokerCount < currentCount)
                                continue;

                            //測試每一個可能的順子起點
                            for (int startWeight = ABSOLUTE_MIN_WEIGHT; startWeight <= ABSOLUTE_MAX_WEIGHT - currentCount + 1; startWeight++)
                            {
                                int endWeight = startWeight + currentCount - 1;

                                //計算這個順子的最終點數是否大於場上
                                int evaluatedWeight = isReversed ? (endWeight * -1) : startWeight;
                                if (evaluatedWeight <= currentWeight)
                                    continue;

                                //計算玩家手牌在這個權重區間內有幾張牌
                                var cardsInWindow = sameSuitHand.Where(c => c.Weight >= startWeight && c.Weight <= endWeight).ToList();
                                int neededJokers = currentCount - cardsInWindow.Count;

                                //如果手上的鬼牌足夠填補這個區間的空缺，則這些實體牌都可以被推薦
                                if (neededJokers <= handJokerCount)
                                {
                                    matchingCards.AddRange(cardsInWindow);
                                }
                            }
                        }
                    }

                    matchingCards = matchingCards.Distinct().ToList();
                    //通常發生於只有鬼牌可打時
                    if (matchingCards.Count() < currentCount)
                        matchingCards.Clear();
                }
                //已選牌
                else
                {
                    if (selectedCards.Count() >= currentCount) //選擇張數已達上限
                        return selectedCards; //只推薦原本已選的牌

                    //算出已選的實體牌數量
                    int selJokerCnt = GetJokerCount(selectedCards);
                    int nonJokerCardsCnt = selectedCards.Count() - selJokerCnt;

                    //如果只有選鬼牌，誰都能搭
                    if (nonJokerCardsCnt == 0)
                        return null;

                    //如果場上是同點
                    if (currentHands == Hands.SameRank)
                    {
                        matchingCards.AddRange(FindSameRank(playerHand, selectedCards, currentCount, currentWeight, isReversed, handJokerCount));
                    }
                    //如果場上是同花
                    else if (currentHands == Hands.Flush)
                    {
                        matchingCards.AddRange(FindFlush(playerHand, selectedCards, currentCount, currentWeight, isReversed, handJokerCount));
                    }
                }
            }

            return matchingCards;
        }
        public Hands Evaluate(IEnumerable<Card> cardsToPlay, IEnumerable<Card> currentPlay, Hands currentHands, bool isReversed, bool isSuitLocked)
        {
            Hands playHands = HandsType(cardsToPlay);
            //該玩家為新出牌者
            if (currentPlay == null)
                return playHands; //直接回傳

            //場面已有牌
            //張數不同
            if (cardsToPlay.Count() != currentPlay.Count())
                return Hands.Illegal;

            //先將模稜兩可的情況改成場面牌型
            if (playHands == Hands.Both)
                playHands = currentHands;
            //牌型不同
            if (playHands != currentHands)
                return Hands.Illegal;

            //黑桃三單吃鬼牌
            if (currentPlay.Count() == 1)
            {
                var currentCard = currentPlay.First();
                var playCard = cardsToPlay.First();
                if (currentCard.SuitType == Card.Suit.JOKER &&
                    playCard.SuitType == Card.Suit.SPADES && playCard.RankType == Card.Rank.THREE)
                {
                    return Hands.SameRank; //視為合法打出
                }
            }

            /*
            //單張鎖花色
            if (isSuitLocked && currentPlay.Count() == 1)
            {
                var playCard = cardsToPlay.First();
                var requiredSuit = currentPlay.First().SuitType;

                //鬼牌可以無視鎖定打出，否則花色必須一致
                if (playCard.SuitType != Card.Suit.JOKER && playCard.SuitType != requiredSuit)
                    return Hands.Illegal;
            }
            */

            //點數大小沒贏
            if (GetHandsWeight(currentHands, currentPlay, isReversed) >= GetHandsWeight(playHands, cardsToPlay, isReversed))
                return Hands.Illegal;

            return playHands;
        }
        private Hands HandsType(IEnumerable<Card> hands)
        {
            bool isFlush = false;
            //檢查是否為同點
            HashSet<Card.Suit> suits = new HashSet<Card.Suit>();
            int jokerCount = 0;
            int rank = hands.First().Weight;
            foreach(Card card in hands)
            {
                if (card.SuitType == Card.Suit.JOKER)
                {
                    jokerCount++;
                    continue;
                }
                //有不同點的牌或是出現同花色的牌
                if (rank != card.Weight || suits.Contains(card.SuitType))
                {
                    isFlush = true;
                }

                suits.Add(card.SuitType);
            }
            if (!isFlush)
            {
                //如果是兩張鬼牌加任一一張牌，可解釋為同花順或同點
                if (jokerCount == 2 && hands.Count() == 3)
                    return Hands.Both;
                
                return Hands.SameRank;
            }

            //檢查為同花還是非法牌型
            //同花至少要三張
            if (hands.Count() < 3)
                return Hands.Illegal;

            rank--;
            foreach (Card card in hands)
            {
                //超過一種花色
                if (suits.Count() > 1)
                    return Hands.Illegal;

                if (card.SuitType == Card.Suit.JOKER)
                    continue;

                if(card.Weight - rank != 1)
                {
                    if(jokerCount > 0)
                    {
                        //用一張joker來替代
                        jokerCount--;
                    }
                    else //沒有鬼牌但點數不連續
                    {
                        return Hands.Illegal;
                    }
                }

                rank = card.Weight;
                suits.Add(card.SuitType);
            }

            return Hands.Flush;
        }
        private IEnumerable<Card> FindSameRank(IEnumerable<Card> hand, IEnumerable<Card> selectedCards)
        {
            List<Card> matchingCards = new List<Card>();
            int selectedRank = selectedCards.First().Weight;
            foreach (Card card in hand)
            {
                if (card.Weight == selectedRank)
                {
                    matchingCards.Add(card);
                }
            }

            return matchingCards;
        }
        //考量場面牌型去尋找
        private IEnumerable<Card> FindSameRank(IEnumerable<Card> hand, IEnumerable<Card> selectedCards, int currentCount, int currentWeight, bool isReversed, int handJokerCount)
        {
            List<Card> matchingCards = new List<Card>();

            //排除鬼牌取得權重
            var realSelected = selectedCards.Where(c => c.SuitType != Card.Suit.JOKER).ToList();
            if (!realSelected.Any()) return matchingCards;

            int selectedRankWeight = realSelected.First().Weight;

            //權重如果不夠大，直接回傳空
            int evaluatedWeight = selectedRankWeight * (isReversed ? -1 : 1);
            if (evaluatedWeight <= currentWeight)
                return matchingCards;

            //找出手中同點數的牌
            var cardsOfRank = hand.Where(c => c.Weight == selectedRankWeight).ToList();

            //如果所有牌的數量還是湊不到場上要求的張數，回傳空
            if (cardsOfRank.Count + handJokerCount < currentCount)
                return matchingCards;

            //條件皆吻合，將這些同點數的牌全部推薦出來
            matchingCards.AddRange(cardsOfRank);

            return matchingCards;
        }
        private IEnumerable<Card> FindFlush(IEnumerable<Card> hand, IEnumerable<Card> selectedCards, int handJokerCount)
        {
            List<Card> matchingCards = new List<Card>();
            if (selectedCards == null || !selectedCards.Any()) return matchingCards;

            //取得目標花色與選取範圍的邊界 (最小與最大點數)
            Card.Suit targetSuit = selectedCards.First().SuitType;
            var sortedSelected = selectedCards
                .Where(c => c.SuitType == targetSuit)
                .OrderBy(c => c.Weight)
                .ToList();
            int minSelWeight = sortedSelected.First().Weight;
            int maxSelWeight = sortedSelected.Last().Weight;

            //準備同花色的手牌(確保由小到大排序)
            var sameSuitHand = hand
                .Where(c => c.SuitType == targetSuit)
                .OrderBy(c => c.Weight)
                .ToList();

            //計算需要消耗多少張joker補空隙
            int internalRangeSpan = maxSelWeight - minSelWeight + 1;

            //計算該範圍內，實際有幾張牌
            int internalCardCount = sameSuitHand
                .Where(c => c.Weight >= minSelWeight && c.Weight <= maxSelWeight)
                .Select(c => c.Weight)
                .Distinct()
                .Count();

            int neededJokersForInternal = internalRangeSpan - internalCardCount;

            //如果Joker不夠補
            if (neededJokersForInternal > handJokerCount)
            {
                return matchingCards;
            }

            //內部範圍的牌無條件加入
            matchingCards.AddRange(sameSuitHand.Where(c => c.Weight >= minSelWeight && c.Weight <= maxSelWeight));

            //剩下可用於向外擴張的Joker數量
            int remainingJokers = handJokerCount - neededJokersForInternal;

            //向左擴張
            int currentJokers = remainingJokers;
            int currentLeftWeight = minSelWeight;
            int leftIdx = sameSuitHand.FindLastIndex(c => c.Weight < minSelWeight);

            while (leftIdx >= 0)
            {
                int diff = currentLeftWeight - sameSuitHand[leftIdx].Weight;
                if (diff != 1)
                {
                    if (currentJokers >= diff - 1)
                        currentJokers -= (diff - 1);
                    else
                        break; //Joker不夠補，停止向左擴張
                }
                matchingCards.Add(sameSuitHand[leftIdx]);
                currentLeftWeight = sameSuitHand[leftIdx].Weight;
                leftIdx--;
            }

            //向右擴張
            currentJokers = remainingJokers;
            int currentRightWeight = maxSelWeight;
            int rightIdx = sameSuitHand.FindIndex(c => c.Weight > maxSelWeight);

            if (rightIdx != -1)
            {
                while (rightIdx < sameSuitHand.Count)
                {
                    int diff = sameSuitHand[rightIdx].Weight - currentRightWeight;
                    if (diff != 1)
                    {
                        if (currentJokers >= diff - 1)
                            currentJokers -= (diff - 1);
                        else
                            break; //Joker不夠補
                    }
                    matchingCards.Add(sameSuitHand[rightIdx]);
                    currentRightWeight = sameSuitHand[rightIdx].Weight;
                    rightIdx++;
                }
            }

            //同花順至少要三張
            if (matchingCards.Count < 3)
                matchingCards.Clear();

            //去除重複加入的元素並重新排序後回傳
            return matchingCards.Distinct().OrderBy(c => c.Weight).ToList();
        }
        //受限於場上張數與權重的同花順搜尋
        private IEnumerable<Card> FindFlush(IEnumerable<Card> hand, IEnumerable<Card> selectedCards, int currentCount, int currentWeight, bool isReversed, int handJokerCount)
        {
            List<Card> matchingCards = new List<Card>();

            var realSelected = selectedCards.Where(c => c.SuitType != Card.Suit.JOKER).ToList();
            if (!realSelected.Any()) return matchingCards;

            Card.Suit targetSuit = realSelected.First().SuitType;
            var sortedSelected = realSelected.OrderBy(c => c.Weight).ToList();
            int minSelWeight = sortedSelected.First().Weight;
            int maxSelWeight = sortedSelected.Last().Weight;

            //如果目前選取的牌，頭尾跨度已經超過了場上要求的張數，絕對不合法
            if (maxSelWeight - minSelWeight + 1 > currentCount)
                return matchingCards;

            var sameSuitHand = hand.Where(c => c.SuitType == targetSuit).ToList();

            //同花順可能開始的權重範圍
            int minPossibleStartWeight = maxSelWeight - currentCount + 1;
            int maxPossibleStartWeight = minSelWeight;

            //同花順中的最小與最大權重邊界
            int ABSOLUTE_MIN_WEIGHT = 0;
            int ABSOLUTE_MAX_WEIGHT = 12;

            //測試每一個可能的順子起點
            for (int startWeight = minPossibleStartWeight; startWeight <= maxPossibleStartWeight; startWeight++)
            {
                int endWeight = startWeight + currentCount - 1;

                //如果這個組合超出實體牌的上下限，略過
                if (startWeight < ABSOLUTE_MIN_WEIGHT || endWeight > ABSOLUTE_MAX_WEIGHT)
                    continue;

                //計算這個順子的最終點數是否大於場上
                int evaluatedWeight = isReversed ? (endWeight * -1) : startWeight;
                if (evaluatedWeight <= currentWeight)
                    continue;

                //計算範圍區間有幾張牌
                var cardsInWindow = sameSuitHand.Where(c => c.Weight >= startWeight && c.Weight <= endWeight).ToList();
                int neededJokers = currentCount - cardsInWindow.Count;

                //如果手上的鬼牌足夠填補這個區間的空缺就合法
                if (neededJokers <= handJokerCount)
                {
                    matchingCards.AddRange(cardsInWindow);
                }
            }

            //將所有合法組合的牌去重複並排序後回傳
            return matchingCards.Distinct().OrderBy(c => c.Weight).ToList();
        }
        private int GetJokerCount(IEnumerable<Card> cards)
        {
            int cnt = 0;
            for(int i = cards.Count() - 1; i >= 0; i--)
            {
                if (cards.ElementAt(i).SuitType != Card.Suit.JOKER)
                    break;

                cnt++;
            }

            return cnt;
        }
        public int GetHandsWeight(Hands hands, IEnumerable<Card> cards, bool isReversed)
        {
            if (hands == Hands.SameRank)
            {
                int weight = cards.ElementAt(0).Weight;
                //反轉時將非小丑牌變號
                if (isReversed  && cards.ElementAt(0).SuitType != Card.Suit.JOKER)
                    weight *= -1;

                return weight;
            }
            else if(hands == Hands.Flush)
            {
                //將joker牌拿掉
                int jokerCount = 0;
                List<Card> nonJokerCards = cards.ToList();
                for(int i = nonJokerCards.Count() - 1; i >= 0; i--)
                {
                    if (nonJokerCards[i].SuitType != Card.Suit.JOKER)
                        break;

                    jokerCount++;
                    nonJokerCards.RemoveAt(i);
                }

                //填補非連號的空隙
                //空隙數為頭尾距離減去元素數量
                int leftSideWeight = nonJokerCards.First().Weight;
                int rightSideWeight = nonJokerCards.Last().Weight;
                int gap = (rightSideWeight - leftSideWeight + 1) - nonJokerCards.Count();
                
                //鬼牌先拿去填補空隙
                jokerCount -= gap;

                if (jokerCount <= 0) //沒有鬼牌能用了
                    return isReversed ? rightSideWeight * -1 : leftSideWeight;

                //剩下鬼牌還能拿來往大的地方填
                if (!isReversed)
                    jokerCount -= Card.Rank.TWO - nonJokerCards.Last().RankType;
                else
                    jokerCount -= nonJokerCards.First().RankType - Card.Rank.THREE;

                if (jokerCount <= 0) //沒有鬼牌能用了
                    return isReversed ? rightSideWeight * -1 : leftSideWeight;

                //剩下鬼牌只能再往小的填
                if (!isReversed)
                    leftSideWeight -= jokerCount;
                else
                    rightSideWeight += jokerCount;

                return isReversed ? rightSideWeight * -1 : leftSideWeight;
            }

            return 0;
        }
    }
}