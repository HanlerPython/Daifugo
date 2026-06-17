using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.Controller;
using static test01.Model.HandsEvaluator;

namespace test01.Model
{
    public class GameSnapshot
    {
        public Hands CurrentHandsType { get; } // 場上牌型 (如同花順、三條)
        public int CurrentWeight { get; }      // 當前場上權重
        public int CurrentCardCount { get; }   // 當前場上張數
        public bool IsReversed { get; }        // 是否處於革命/反轉狀態

        // 新增：公開建構子，讓你可以在測試區 Mock，也讓隊友未來可以生成快照
        public GameSnapshot(GameManager gm)
        {
            CurrentHandsType = gm.CurrentHands;
            if (gm.CurrentPlay != null)
                CurrentCardCount = gm.CurrentPlay.Count();
            else
                CurrentCardCount = 0;
            IsReversed = gm.IsReversed;
            int weight = gm.HandsEvaluator.GetHandsWeight(CurrentHandsType, gm.CurrentPlay, IsReversed);
            CurrentWeight = weight;
        }

        public List<IEnumerable<Card>> GetValidPlays(IEnumerable<Card> hand)
        {
            var validPlays = new List<IEnumerable<Card>>();
            var playerHand = hand.ToList();

            // 1. 永遠保留 Pass 的權利
            validPlays.Add(new List<Card>());

            // 修正：直接讀取自身的屬性，不再需要 context
            int currentCount = this.CurrentCardCount;
            int currentWeight = this.CurrentWeight;
            bool isReversed = this.IsReversed;
            Hands currentHandsType = this.CurrentHandsType;

            // 2. 自由出牌狀態 (場上無牌，或是自己贏得上一輪)
            if (currentCount == 0)
            {
                // 包含所有單張、所有可能的對子、三條、鐵支、同花順
                validPlays.AddRange(GenerateAllSingleCards(playerHand));
                validPlays.AddRange(GenerateAllSameRanks(playerHand));
                validPlays.AddRange(GenerateAllFlushes(playerHand));
                return validPlays;
            }

            // 3. 跟牌狀態 (必須符合場上牌型與張數，且權重更大)
            if (currentHandsType == Hands.SameRank)
            {
                validPlays.AddRange(GetValidSameRanks(playerHand, currentCount, currentWeight, isReversed));
            }
            else if (currentHandsType == Hands.Flush)
            {
                validPlays.AddRange(GetValidFlushes(playerHand, currentCount, currentWeight, isReversed));
            }

            return validPlays;
        }

        private List<IEnumerable<Card>> GenerateAllSingleCards(List<Card> hand)
        {
            return hand.Select(c => new List<Card> { c }).ToList<IEnumerable<Card>>();
        }

        private List<IEnumerable<Card>> GenerateAllSameRanks(List<Card> hand)
        {
            var results = new List<IEnumerable<Card>>();

            // 1. 分離 Joker 與一般牌
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER).OrderByDescending(c => c.Weight).ToList();
            var regularCards = hand.Where(c => c.SuitType != Card.Suit.JOKER).ToList();

            // 2. 純 Joker 對子 (如果手牌剛好有兩張大/小王)
            if (jokers.Count >= 2)
            {
                results.Add(jokers.Take(2).ToList());
            }

            // 3. 一般牌 + Joker 補位窮舉
            var groups = regularCards.GroupBy(c => c.Weight);
            foreach (var group in groups)
            {
                int actualCount = group.Count();
                int maxPossibleCount = actualCount + jokers.Count;

                // 窮舉可能的長度：2(對子), 3(三條), 4(鐵支)
                for (int targetLength = 2; targetLength <= 4; targetLength++)
                {
                    // 如果這組數字的「實體牌 + 手上的 Joker 數量」足夠湊出目標長度
                    if (maxPossibleCount >= targetLength && actualCount > 0)
                    {
                        // 計算需要徵召幾張 Joker
                        int neededJokers = targetLength - actualCount;
                        neededJokers = Math.Max(0, neededJokers); // 如果實體牌已經夠了，就不需要 Joker

                        var play = group.Take(targetLength - neededJokers)
                                        .Concat(jokers.Take(neededJokers))
                                        .ToList();
                        results.Add(play);
                    }
                }
            }

            return results;
        }

        private List<IEnumerable<Card>> GenerateAllFlushes(List<Card> hand)
        {
            var results = new List<IEnumerable<Card>>();
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER).ToList();
            int jokerCount = jokers.Count;

            var suits = hand.Where(c => c.SuitType != Card.Suit.JOKER).GroupBy(c => c.SuitType);

            foreach (var suitGroup in suits)
            {
                var cards = suitGroup.OrderBy(c => c.Weight).ToList();
                int n = cards.Count;

                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                    {
                        int actualCards = j - i + 1;
                        int rankSpread = cards[j].Weight - cards[i].Weight + 1;
                        int internalGaps = rankSpread - actualCards;

                        if (internalGaps > jokerCount) continue;

                        int totalPotentialLength = actualCards + jokerCount;
                        if (totalPotentialLength >= 3)
                        {
                            int neededJokers = internalGaps;
                            int extraJokers = Math.Min(jokerCount - neededJokers, 13 - rankSpread);

                            var play = cards.Skip(i).Take(actualCards)
                                            .Concat(jokers.Take(neededJokers + extraJokers))
                                            .ToList();

                            if (play.Count >= 3)
                            {
                                results.Add(play);
                            }
                        }
                    }
                }
            }
            return results;
        }

        private List<IEnumerable<Card>> GetValidSameRanks(List<Card> hand, int requiredCount, int currentWeight, bool isReversed)
        {
            var results = new List<IEnumerable<Card>>();

            // 1. 預先分離鬼牌與一般牌
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER)
                             .OrderByDescending(c => c.Weight) // 確保大王排前面
                             .ToList();
            var regularCards = hand.Where(c => c.SuitType != Card.Suit.JOKER).ToList();

            // 2. 🌟 修正：處理「純鬼牌」打法 🌟
            if (jokers.Count >= requiredCount)
            {
                // 如果場上是一般牌 (Weight < 13)，Joker 永遠可以直接壓制 (無視革命)
                bool isCurrentNormalCard = currentWeight < 13;

                // 如果場上已經是 Joker，純比大小 (例如大王 14 > 小王 13)，且不受革命反轉影響
                bool isJokerBeatingJoker = jokers.First().Weight > currentWeight;

                if (isCurrentNormalCard || isJokerBeatingJoker)
                {
                    results.Add(jokers.Take(requiredCount).ToList());
                }
            }

            // 3. 處理「一般牌 + 鬼牌補位」打法 (這裡的邏輯原本就是對的，因為它是用一般牌的 weight 去比較)
            var groupedByRank = regularCards.GroupBy(c => c.Weight);
            foreach (var group in groupedByRank)
            {
                int weight = group.Key;
                bool isValidWeight = isReversed ? (weight < currentWeight) : (weight > currentWeight);

                if (isValidWeight)
                {
                    int availableCards = group.Count();
                    int missingCount = requiredCount - availableCards;
                    int neededJokers = Math.Max(0, missingCount);

                    if (neededJokers <= jokers.Count)
                    {
                        var play = group.Take(requiredCount - neededJokers)
                                        .Concat(jokers.Take(neededJokers)) // 這裡會從大王開始拿
                                        .ToList();
                        results.Add(play);
                    }
                }
            }

            return results;
        }

        private List<IEnumerable<Card>> GetValidFlushes(List<Card> hand, int requiredCount, int currentWeight, bool isReversed)
        {
            var results = new List<IEnumerable<Card>>();
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER).ToList();
            int jokerCount = jokers.Count;

            var suits = hand.Where(c => c.SuitType != Card.Suit.JOKER).GroupBy(c => c.SuitType);

            foreach (var suitGroup in suits)
            {
                var cards = suitGroup.OrderBy(c => c.Weight).ToList();
                int n = cards.Count;

                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                    {
                        int actualCards = j - i + 1;
                        int rankSpread = cards[j].Weight - cards[i].Weight + 1;
                        int internalGaps = rankSpread - actualCards;

                        if (internalGaps > jokerCount) continue;

                        if (actualCards + jokerCount >= requiredCount && rankSpread <= requiredCount)
                        {
                            int maxPossibleWeight = cards[i].Weight + requiredCount - 1;
                            bool isValidWeight = isReversed ? (cards[i].Weight < currentWeight) : (maxPossibleWeight > currentWeight);

                            if (isValidWeight)
                            {
                                int neededJokersToReachCount = requiredCount - actualCards;
                                var play = cards.Skip(i).Take(actualCards)
                                                .Concat(jokers.Take(neededJokersToReachCount))
                                                .ToList();
                                results.Add(play);
                            }
                        }
                    }
                }
            }
            return results;
        }
    }
}