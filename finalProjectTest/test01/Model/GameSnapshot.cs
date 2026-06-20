using System;
using System.Collections.Generic;
using System.Linq;
using test01.Controller;
using static test01.Model.HandsEvaluator;

namespace test01.Model
{
    public class GameSnapshot
    {
        public int CurrentPlayerIdx { get; }
        public Hands CurrentHandsType { get; }
        public int CurrentWeight { get; }
        public int CurrentCardCount { get; }
        public bool IsReversed { get; }
        public IReadOnlyList<Card> DiscardPile { get; }
        public List<int> OpponentHandCounts { get; }
        public bool IsSuitLocked { get; }
        public List<Card.Suit> CurrentPlaySuits { get; }

        public GameSnapshot(GameManager gm)
        {
            CurrentHandsType = gm.CurrentHands;
            CurrentCardCount = gm.CurrentPlay?.Count() ?? 0;
            IsReversed = gm.IsReversed;
            CurrentWeight = gm.HandsEvaluator.GetHandsWeight(CurrentHandsType, gm.CurrentPlay, IsReversed);

            DiscardPile = gm.DiscardPile?.ToList() ?? new List<Card>();

            OpponentHandCounts = gm.Players
                .Where(p => p.Id != gm.CurrentPlayerIdx && p.StateType != Player.State.Finished)
                .Select(p => p.Hand.Count)
                .ToList();
            CurrentPlayerIdx = gm.CurrentPlayerIdx;

            //擷取鎖花色情報
            IsSuitLocked = gm.IsSuitLocked;
            CurrentPlaySuits = gm.CurrentPlay?.Select(c => c.SuitType).ToList() ?? new List<Card.Suit>();
        }

        public List<IEnumerable<Card>> GetValidPlays(IEnumerable<Card> hand)
        {
            var validPlays = new List<IEnumerable<Card>>();
            var playerHand = hand.ToList();

            validPlays.Add(new List<Card>()); // 1. 永遠保留 Pass

            int currentCount = this.CurrentCardCount;
            int currentWeight = this.CurrentWeight;
            bool isReversed = this.IsReversed;
            Hands currentHandsType = this.CurrentHandsType;

            // 2. 自由出牌狀態
            if (currentCount == 0)
            {
                validPlays.AddRange(GenerateAllSingleCards(playerHand));
                validPlays.AddRange(GenerateAllSameRanks(playerHand));
                validPlays.AddRange(GenerateAllFlushes(playerHand));
                return validPlays;
            }

            // 3. 一般跟牌狀態
            if (currentHandsType == Hands.SameRank)
                validPlays.AddRange(GetValidSameRanks(playerHand, currentCount, currentWeight, isReversed));
            else if (currentHandsType == Hands.Flush)
                validPlays.AddRange(GetValidFlushes(playerHand, currentCount, currentWeight, isReversed));

            if (currentCount > 0)
            {
                var allBombs = GenerateAllSameRanks(playerHand).Where(p => p.Count() >= 4).ToList();
                var allFlushBombs = GenerateAllFlushes(playerHand).Where(p => p.Count() >= 4).ToList();
                var allAvailableBombs = allBombs.Concat(allFlushBombs).ToList();

                foreach (var bomb in allAvailableBombs)
                {
                    //只有當場上「本來就已經是炸彈 (張數>=4)」時，才能出張數更多或權重更大的炸彈！
                    if (currentCount >= 4)
                    {
                        if (bomb.Count() > currentCount)
                        {
                            validPlays.Add(bomb);
                        }
                        else if (bomb.Count() == currentCount)
                        {
                            int bombWeight = bomb.First(c => c.SuitType != Card.Suit.JOKER).Weight;
                            bool canBeat = isReversed ? (bombWeight < currentWeight) : (bombWeight > currentWeight);
                            if (canBeat) validPlays.Add(bomb);
                        }
                    }
                }
            }

            if (IsSuitLocked && currentCount > 0)
            {
                var lockedPlays = new List<IEnumerable<Card>>();
                var requiredSuits = CurrentPlaySuits.Where(s => s != Card.Suit.JOKER).ToList();

                foreach (var play in validPlays)
                {
                    // 特權 A: PASS 永遠合法
                    if (!play.Any())
                    {
                        lockedPlays.Add(play);
                        continue;
                    }

                    // 特權 B: 炸彈 (4張以上) 擁有破壞鎖定的特權，絕對合法
                    if (play.Count() >= 4)
                    {
                        lockedPlays.Add(play);
                        continue;
                    }

                    // 特權 C: Joker 是法外狂徒，打出的牌只要全是 Joker 就放行
                    if (play.All(c => c.SuitType == Card.Suit.JOKER))
                    {
                        lockedPlays.Add(play);
                        continue;
                    }

                    // 一般判定：檢查花色是否精準匹配
                    var providedSuits = play.Where(c => c.SuitType != Card.Suit.JOKER).Select(c => c.SuitType).ToList();
                    bool isMatch = true;

                    foreach (var reqSuit in requiredSuits)
                    {
                        if (providedSuits.Contains(reqSuit))
                        {
                            providedSuits.Remove(reqSuit); // 匹配成功，消耗掉一個 (支援未來如果加開雙重鎖定)
                        }
                        else
                        {
                            isMatch = false;
                            break;
                        }
                    }

                    if (isMatch)
                    {
                        lockedPlays.Add(play);
                    }
                }
                validPlays = lockedPlays;
            }
            //黑桃3單防
            if (currentCount == 1 && currentWeight >= 13)
            {
                var spade3 = playerHand.FirstOrDefault(c => c.RankType == Card.Rank.THREE && c.SuitType == Card.Suit.SPADES);

                // 防呆：確保黑桃3還沒被前面的邏輯加進去
                if (spade3 != null && !validPlays.Any(p => p.Count() == 1 && p.First() == spade3))
                {
                    validPlays.Add(new List<Card> { spade3 });
                }
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
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER).OrderByDescending(c => c.Weight).ToList();
            var regularCards = hand.Where(c => c.SuitType != Card.Suit.JOKER).ToList();

            if (jokers.Count >= 2) results.Add(jokers.Take(2).ToList());

            var groups = regularCards.GroupBy(c => c.Weight);
            foreach (var group in groups)
            {
                int actualCount = group.Count();
                int maxPossibleCount = actualCount + jokers.Count;

                for (int targetLength = 2; targetLength <= 4; targetLength++)
                {
                    if (maxPossibleCount < targetLength || actualCount == 0) continue;

                    int neededJokers = Math.Max(0, targetLength - actualCount);
                    var play = group.Take(targetLength - neededJokers).Concat(jokers.Take(neededJokers)).ToList();
                    results.Add(play);
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

                        // 🌟 攤平嵌套：提早 return 或 continue
                        if (internalGaps > jokerCount) continue;

                        int totalPotentialLength = actualCards + jokerCount;
                        if (totalPotentialLength < 3) continue;

                        int neededJokers = internalGaps;
                        int extraJokers = Math.Min(jokerCount - neededJokers, 13 - rankSpread);

                        var play = cards.Skip(i).Take(actualCards)
                                        .Concat(jokers.Take(neededJokers + extraJokers))
                                        .ToList();

                        if (play.Count >= 3) results.Add(play);
                    }
                }
            }
            return results;
        }

        private List<IEnumerable<Card>> GetValidSameRanks(List<Card> hand, int requiredCount, int currentWeight, bool isReversed)
        {
            var results = new List<IEnumerable<Card>>();
            var jokers = hand.Where(c => c.SuitType == Card.Suit.JOKER).OrderByDescending(c => c.Weight).ToList();
            var regularCards = hand.Where(c => c.SuitType != Card.Suit.JOKER).ToList();

            if (jokers.Count >= requiredCount)
            {
                bool isCurrentNormalCard = currentWeight < 13;
                bool isJokerBeatingJoker = jokers.First().Weight > currentWeight;

                if (isCurrentNormalCard || isJokerBeatingJoker)
                    results.Add(jokers.Take(requiredCount).ToList());
            }

            var groupedByRank = regularCards.GroupBy(c => c.Weight);
            foreach (var group in groupedByRank)
            {
                int weight = group.Key;
                bool isValidWeight;

                // 🌟 核心防禦：如果場上是鬼牌 (權重 >= 13)，一般牌絕對無法壓制！
                if (currentWeight >= 13)
                {
                    isValidWeight = false;
                }
                else
                {
                    isValidWeight = isReversed ? (weight < currentWeight) : (weight > currentWeight);
                }

                if (!isValidWeight) continue;

                int availableCards = group.Count();
                int neededJokers = Math.Max(0, requiredCount - availableCards);

                if (neededJokers <= jokers.Count)
                {
                    var play = group.Take(requiredCount - neededJokers).Concat(jokers.Take(neededJokers)).ToList();
                    results.Add(play);
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
                        if (actualCards + jokerCount < requiredCount || rankSpread > requiredCount) continue;

                        int maxPossibleWeight = cards[i].Weight + requiredCount - 1;
                        bool isValidWeight;

                        // 如果場上的同花順包含頂級鬼牌權重，嚴禁一般同花順用反轉邏輯壓制
                        if (currentWeight >= 13)
                        {
                            isValidWeight = false;
                        }
                        else
                        {
                            isValidWeight = isReversed ? (cards[i].Weight < currentWeight) : (maxPossibleWeight > currentWeight);
                        }

                        if (isValidWeight)
                        {
                            {
                                int neededJokersToReachCount = requiredCount - actualCards;
                                var play = cards.Skip(i).Take(actualCards).Concat(jokers.Take(neededJokersToReachCount)).ToList();
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