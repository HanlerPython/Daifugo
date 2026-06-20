using System;
using System.Collections.Generic;
using System.Linq;
using test01.Controller;
using static test01.Model.HandsEvaluator;

namespace test01.Model.Interfaces
{
    public interface IPlayerStrategy
    {
        IEnumerable<Card> DecidePlay(GameSnapshot context, IEnumerable<Card> hand);
        IEnumerable<Card> DecideDiscard(GameSnapshot context, IEnumerable<Card> hand, int count);
    }

    public class GreedyAIStrategy : IPlayerStrategy
    {
        // 🌟 難度特徵開關
        public enum Difficulty { Easy, Hard }
        private readonly Difficulty _difficulty;

        // 建構子 (預設為困難模式)
        public GreedyAIStrategy(Difficulty difficulty = Difficulty.Hard)
        {
            _difficulty = difficulty;
        }

        public IEnumerable<Card> DecidePlay(GameSnapshot context, IEnumerable<Card> hand)
        {
            List<IEnumerable<Card>> validPlays = context.GetValidPlays(hand);

            if (validPlays == null || !validPlays.Any())
            {
                return new List<Card>();
            }

            IEnumerable<Card> bestPlay = null;
            int bestScore = int.MinValue;

            foreach (var play in validPlays)
            {
                IEnumerable<Card> remainingHand = hand.Except(play);
                int currentScore = EvaluateState(context, play, remainingHand);

                if (currentScore > bestScore)
                {
                    bestScore = currentScore;
                    bestPlay = play;
                }
            }

            return bestPlay ?? new List<Card>();
        }

        public int EvaluateState(GameSnapshot context, IEnumerable<Card> play, IEnumerable<Card> remainingHand)
        {
            // 1. PASS 的處理 (提早 Return)
            if (play == null || !play.Any())
            {
                if (context.CurrentCardCount == 0) return int.MinValue;
                return CalculateHandValue(remainingHand, context.IsReversed, new List<Card>());
            }

            bool futureIsReversed = context.IsReversed;
            if (play.Count() >= 4) futureIsReversed = !context.IsReversed;

            // ==========================================================
            // 終局生死判斷
            // ==========================================================
            if (!remainingHand.Any())
            {
                if (IsFoulCard(play.First(), futureIsReversed)) return -2000000;
                return 1000000;
            }

            // 慢性自殺防禦
            if (remainingHand.All(c => IsFoulCard(c, futureIsReversed)))
            {
                return -1000000;
            }

            List<Card> knownCards = new List<Card>();
            if (_difficulty == Difficulty.Hard)
            {
                knownCards = context.DiscardPile.Concat(remainingHand).ToList();
            }

            // 基礎手牌價值計算
            int score = CalculateHandValue(remainingHand, futureIsReversed, knownCards);
            score += (play.Count() * 400) + 500;

            // ==========================================================
            // 聽牌覺醒
            // ==========================================================
            bool canFinishNextTurn = IsReadyToWin(remainingHand);
            if (canFinishNextTurn)
            {
                score += 50000;
            }

            // ==========================================================
            // 丟垃圾防呆
            // ==========================================================
            if (context.CurrentCardCount == 0 && play.Any())
            {
                int dPower = GetDynamicPower(play.First(), context.IsReversed, knownCards);

                if (dPower >= 11) score -= 3000;
                score -= (dPower * 50);
            }

            // ==========================================================
            // 主動觸發鎖花色獎勵 (惡意刺殺)
            // ==========================================================
            if (_difficulty == Difficulty.Hard && play.Any() && context.CurrentCardCount > 0 && !context.IsSuitLocked)
            {
                var playSuits = play.Select(c => c.SuitType).ToList();
                var reqSuits = context.CurrentPlaySuits;

                if (!playSuits.Contains(Card.Suit.JOKER) && !reqSuits.Contains(Card.Suit.JOKER) && playSuits.Count == reqSuits.Count)
                {
                    bool isExactMatch = true;
                    var tempReq = reqSuits.ToList();
                    foreach (var suit in playSuits)
                    {
                        if (tempReq.Contains(suit)) tempReq.Remove(suit);
                        else { isExactMatch = false; break; }
                    }

                    if (isExactMatch)
                    {
                        score += 1500;
                    }
                }
            }

            var playedRanks = play.Select(c => c.RankType).ToList();
            if (playedRanks.Contains(Card.Rank.EIGHT)) score += 500;
            if (playedRanks.Contains(Card.Rank.SEVEN) || playedRanks.Contains(Card.Rank.TEN)) score += (play.Count() * 150);

            // ==========================================================
            // 防禦模式
            // ==========================================================
            if (_difficulty == Difficulty.Hard && context.OpponentHandCounts.Any(count => count <= 2))
            {
                if (context.CurrentCardCount == 0 && play.Count() == 1)
                {
                    if (GetStandardPower(play.First(), futureIsReversed) < 10) score -= 20000;
                }

                if (context.CurrentCardCount > 0 && play.Count() > 0)
                {
                    score += GetStandardPower(play.First(), futureIsReversed) * 300;
                }

                if (play.Count() >= 2) score += 1000;
            }

            // ==========================================================
            // Joker 控制與懲罰
            // ==========================================================
            int usedJokers = play.Count(c => c.SuitType == Card.Suit.JOKER);
            if (usedJokers > 0 && !canFinishNextTurn)
            {
                score -= (usedJokers * 1000);
            }

            return score;
        }

        private int CalculateHandValue(IEnumerable<Card> hand, bool isReversed, List<Card> knownCards)
        {
            int totalValue = 0;
            foreach (var card in hand)
            {
                totalValue += GetHeuristicScore(card, isReversed, knownCards);
            }

            var groups = hand.Where(c => c.SuitType != Card.Suit.JOKER).GroupBy(c => c.RankType);
            foreach (var group in groups)
            {
                int count = group.Count();
                if (count == 2) totalValue += 100;
                if (count == 3) totalValue += 300;
                if (count >= 4) totalValue += 1000;
            }

            return totalValue;
        }

        private int GetStandardPower(Card card, bool isReversed)
        {
            if (card.SuitType == Card.Suit.JOKER) return card.RankType == Card.Rank.RED ? 14 : 13;

            int baseWeight = card.RankType switch
            {
                Card.Rank.THREE => 0,
                Card.Rank.FOUR => 1,
                Card.Rank.FIVE => 2,
                Card.Rank.SIX => 3,
                Card.Rank.SEVEN => 4,
                Card.Rank.EIGHT => 8,
                Card.Rank.NINE => 6,
                Card.Rank.TEN => 7,
                Card.Rank.JACK => 8,
                Card.Rank.QUEEN => 9,
                Card.Rank.KING => 10,
                Card.Rank.ACE => 11,
                Card.Rank.TWO => 12,
                _ => 0
            };

            return isReversed ? (12 - baseWeight) : baseWeight;
        }

        private int GetDynamicPower(Card card, bool isReversed, List<Card> knownCards)
        {
            if (card.SuitType == Card.Suit.JOKER) return card.RankType == Card.Rank.RED ? 14 : 13;

            int myPower = GetStandardPower(card, isReversed);
            int dynamicPower = myPower;

            if (knownCards != null && knownCards.Any())
            {
                int totalStrongerCardsInDeck = ((12 - myPower) * 4) + 2;

                int seenStrongerCards = knownCards.Count(c => GetStandardPower(c, isReversed) > myPower);
                int strongerCardsLeft = totalStrongerCardsInDeck - seenStrongerCards;

                if (strongerCardsLeft <= 0) dynamicPower = 12;
                else if (strongerCardsLeft <= 2) dynamicPower = Math.Max(myPower, 11);
            }

            return dynamicPower;
        }

        private int GetHeuristicScore(Card card, bool isReversed, List<Card> knownCards)
        {
            int dynamicPower = GetDynamicPower(card, isReversed, knownCards);
            int score = (int)Math.Pow(dynamicPower, 2);

            if (dynamicPower == 11) score += 400;
            if (dynamicPower == 12) score += 800;
            if (dynamicPower >= 13) score += 1500;

            switch (card.RankType)
            {
                case Card.Rank.EIGHT: score += 300; break;
                case Card.Rank.SEVEN:
                case Card.Rank.TEN: score += 200; break;
                case Card.Rank.FIVE:
                case Card.Rank.JACK: score += 100; break;
                case Card.Rank.THREE:
                    if (card.SuitType == Card.Suit.SPADES) score += 400;
                    break;
            }

            return score;
        }

        public IEnumerable<Card> DecideDiscard(GameSnapshot context, IEnumerable<Card> hand, int count)
        {
            if (count <= 0 || hand == null || !hand.Any()) return new List<Card>();

            List<Card> knownCards = _difficulty == Difficulty.Hard ? context.DiscardPile.Concat(hand).ToList() : new List<Card>();
            return hand.OrderBy(card => GetHeuristicScore(card, context.IsReversed, knownCards)).Take(count).ToList();
        }

        // ==========================================================
        // 輔助方法區塊
        // ==========================================================
        private bool IsFoulCard(Card card, bool isReversed)
        {
            if (card.SuitType == Card.Suit.JOKER) return true;
            if (!isReversed && card.RankType == Card.Rank.TWO) return true;
            if (isReversed && card.RankType == Card.Rank.THREE) return true;
            return false;
        }

        private bool IsReadyToWin(IEnumerable<Card> hand)
        {
            if (hand == null || !hand.Any()) return true;
            if (hand.Count() == 1) return true;
            return hand.GroupBy(c => c.RankType).Count() == 1;
        }
    }
}