using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test01.Model.Interfaces
{
    public interface IPlayerStrategy
    {
        IEnumerable<Card> DecidePlay(GameSnapshot context, IEnumerable<Card> hand);
        IEnumerable<Card> DecideDiscard(GameSnapshot context, IEnumerable<Card> hand, int count);
    }

    public class GreedyAIStrategy : IPlayerStrategy
    {
        public IEnumerable<Card> DecidePlay(GameSnapshot context, IEnumerable<Card> hand)
        {
            // 修正參數傳遞：假設隊友的合約或你的 Mock 方法叫 GenerateValidPlays
            List<IEnumerable<Card>> validPlays = context.GetValidPlays(hand);

            if (validPlays == null || !validPlays.Any())
            {
                return new List<Card>();
            }

            IEnumerable<Card> bestPlay = null;
            int bestScore = int.MinValue;

            foreach (var play in validPlays)
            {
                // 這裡依賴 Card 正確實作 Equals 與 GetHashCode
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
            // 1. 最高優先級：終結比賽
            if (!remainingHand.Any())
            {
                return 1000000;
            }

            // 2. Pass (不出牌) 的處理
            if (play == null || !play.Any())
            {
                // 嚴格防呆：如果是自由出牌 (Lead) 狀態，絕對不允許 Pass
                if (context.CurrentCardCount == 0) return int.MinValue;

                // Pass 的分數就是最純粹的剩餘手牌價值，不給任何同情分
                return CalculateHandValue(remainingHand, context.IsReversed);
            }

            int score = CalculateHandValue(remainingHand, context.IsReversed);
            score += (play.Count() * 150);

            // 🌟 核心升級：Joker 的終局必勝判斷 🌟
            int usedJokers = play.Count(c => c.SuitType == Card.Suit.JOKER);
            if (usedJokers > 0)
            {
                bool canFinishNextTurn = false;

                // 如果打出 Joker 後，手牌只剩下 1 張，
                // 既然 Joker 幾乎保證能搶下先手，那剩下的這 1 張下回合必定能無條件打出獲勝！
                if (remainingHand.Count() == 1)
                {
                    canFinishNextTurn = true;
                }
                // 如果剩下多張，但全部都是同一個數字（例如剩一對 6 或三條 8）
                // 拿到先手後同樣可以一次全部打出獲勝！
                else if (remainingHand.Any() && remainingHand.GroupBy(c => c.RankType).Count() == 1)
                {
                    canFinishNextTurn = true;
                }

                if (canFinishNextTurn)
                {
                    // AI 發現了必勝連招！免除 Joker 懲罰，並給予即將獲勝的極高分數
                    return 50000;
                }
                else
                {
                    // 如果這不是必勝連招，那就乖乖接受浪費 Joker 的高額扣分
                    score -= (usedJokers * 1000);
                }
            }

            return score;
        }

        // 將整手牌的評估邏輯獨立出來
        private int CalculateHandValue(IEnumerable<Card> hand, bool isReversed)
        {
            int totalValue = 0;

            // 實作重點 1: 剩餘手牌的單卡價值加總
            foreach (var card in hand)
            {
                totalValue += GetHeuristicScore(card, isReversed);
            }

            // 實作重點 2: 牌型加分 (鼓勵 AI 湊對子，不要隨便打單張拆牌)
            // 排除 Joker 後，計算相同數字的數量
            var groups = hand.Where(c => c.SuitType != Card.Suit.JOKER).GroupBy(c => c.RankType);
            foreach (var group in groups)
            {
                int count = group.Count();
                if (count == 2) totalValue += 100;  // 一對的額外價值
                if (count == 3) totalValue += 300;  // 三條的額外價值
                if (count == 4) totalValue += 1000; // 鐵支(革命種子)的極大價值
            }

            return totalValue;
        }

        // 將原本的 EvalConst 改寫為 C# 7.3 支援的傳統 switch，並結合革命反轉的數學邏輯
        private int GetHeuristicScore(Card card, bool isReversed)
        {
            int baseWeight;
            switch (card.RankType)
            {
                case Card.Rank.THREE: baseWeight = 0; break;
                case Card.Rank.FOUR: baseWeight = 1; break;
                case Card.Rank.FIVE: baseWeight = 2; break;
                case Card.Rank.SIX: baseWeight = 3; break;
                case Card.Rank.SEVEN: baseWeight = 4; break;
                case Card.Rank.EIGHT: baseWeight = 8; break; // 暫定為線性數值，後續由次方放大
                case Card.Rank.NINE: baseWeight = 6; break;
                case Card.Rank.TEN: baseWeight = 7; break;
                case Card.Rank.JACK: baseWeight = 8; break;
                case Card.Rank.QUEEN: baseWeight = 9; break;
                case Card.Rank.KING: baseWeight = 10; break;
                case Card.Rank.ACE: baseWeight = 11; break;
                case Card.Rank.TWO: baseWeight = 12; break;
                case Card.Rank.BLACK: return 5000; // 小王絕對特權
                case Card.Rank.RED: return 6000;   // 大王絕對特權
                default: baseWeight = 0; break;
            }

            // 處理革命狀態 (IsReversed)
            int effectivePower = isReversed ? (12 - baseWeight) : baseWeight;

            // 使用指數放大，讓 AI 知道保留 2 (144分) 遠比保留 3 (0分) 重要
            int score = (int)Math.Pow(effectivePower, 2);

            // 針對特殊戰術牌 (如 8 切) 的額外加分
            if (card.RankType == Card.Rank.EIGHT) score += 50;

            return score;
        }

        public IEnumerable<Card> DecideDiscard(GameSnapshot context, IEnumerable<Card> hand, int count)
        {
            // 捨棄邏輯：利用剛寫好的 GetHeuristicScore 找出價值最低的 N 張牌捨棄
            return hand.OrderBy(card => GetHeuristicScore(card, context.IsReversed)).Take(count);
        }
    }
}