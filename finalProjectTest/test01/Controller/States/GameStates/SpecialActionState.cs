using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;
using static test01.Model.HandsEvaluator;

namespace test01.Controller.States.GameStates
{
    public class SpecialActionState : IGameState
    {
        private bool _isS3OnJoker;
        private int _skipCount;
        private int _giveCount;
        private bool _isEightCut;
        private int _discardCount;

        public void Enter(GameManager gm)
        {
            if (gm.CurrentPlay.Count() == 1 && gm.LastPlay != null)
            {
                Card currentCard = gm.CurrentPlay.First();
                Card lastCard = gm.LastPlay.First();
                //黑桃三單吃鬼牌
                if (lastCard.SuitType == Card.Suit.JOKER
                    && currentCard.SuitType == Card.Suit.SPADES
                    && currentCard.RankType == Card.Rank.THREE)
                {
                    _isS3OnJoker = true;
                    EndTrick(gm);
                    return;
                }

                //鎖花色
                if (lastCard.SuitType != Card.Suit.JOKER && currentCard.SuitType != Card.Suit.JOKER
                    && lastCard.SuitType == currentCard.SuitType)
                    gm.IsSuitLocked = true;
            }
            _isS3OnJoker = false;

            //革命
            if (gm.CurrentPlay.Count >= 4)
            {
                gm.Reversed = !gm.Reversed;
                if (gm.Reversed)
                    gm.NotifyRevolutionStarted();
                else
                    gm.NotifyRevolutionEnded();
            }

            //特殊牌效
            _skipCount = 0;
            _giveCount = 0;
            _isEightCut = false;
            _discardCount = 0;
            //同點時鬼牌可參與牌效
            if (gm.CurrentHands == Hands.SameRank)
            {
                //取得這組牌的真實點數(排除鬼牌)
                var realCard = gm.CurrentPlay.FirstOrDefault(c => c.SuitType != Card.Suit.JOKER);

                if (realCard != null)
                {
                    int totalCount = gm.CurrentPlay.Count;
                    switch (realCard.RankType)
                    {
                        case Card.Rank.FIVE:
                            _skipCount = totalCount;
                            break;
                        case Card.Rank.SEVEN:
                            _giveCount = totalCount;
                            break;
                        case Card.Rank.EIGHT:
                            _isEightCut = true;
                            break;
                        case Card.Rank.TEN:
                            _discardCount = totalCount;
                            break;
                        case Card.Rank.JACK:
                            gm.IsTemporaryReversed = true;
                            break;
                    }
                }
            }
            //同花順時鬼牌排除牌效
            else if (gm.CurrentHands == Hands.Flush)
            {
                foreach (Card card in gm.CurrentPlay)
                {
                    switch (card.RankType)
                    {
                        case Card.Rank.FIVE:
                            _skipCount++;
                            break;
                        case Card.Rank.SEVEN:
                            _giveCount++;
                            break;
                        case Card.Rank.EIGHT:
                            _isEightCut = true;
                            break;
                        case Card.Rank.TEN:
                            _discardCount++;
                            break;
                        case Card.Rank.JACK:
                            gm.IsTemporaryReversed = true;
                            break;
                    }
                }
            }

            Player currentPlayer = gm.Players[gm.CurrentPlayerIdx];
            //跳過丟牌階段
            if (!currentPlayer.Hand.Any() || (_giveCount == 0 && _discardCount == 0))
            {
                EndTrick(gm);
            }
            else if(currentPlayer.Id != 0) //人機
            {
                GameSnapshot gs = new GameSnapshot(gm);
                if (_giveCount > 0)
                {
                    var cardsToGiveAway = gm.Ai.DecideDiscard(gs, currentPlayer.Hand, _giveCount);
                    PlayCard(gm, cardsToGiveAway);
                }
                if (_discardCount > 0)
                {
                    var cardsToDiscard = gm.Ai.DecideDiscard(gs, currentPlayer.Hand, _discardCount);
                    PlayCard(gm, cardsToDiscard);
                }
            }
            else
            {
                //提示進入丟牌階段
                gm.NotifyCardExchanging();
            }
        }

        public bool PlayCard(GameManager gm, IEnumerable<Card> cards)
        {
            if(_giveCount > 0)
            {
                if (cards.Count() > _giveCount)
                {
                    MessageBox.Show($"請至少選擇 {_giveCount} 張牌給下家！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    Player currentPlayer = gm.Players[gm.CurrentPlayerIdx];
                    Player nextPlayer = gm.Players[gm.GetNextPlayerIdx()];
                    currentPlayer.RemoveCards(cards);
                    nextPlayer.AddCards(cards);
                    if (currentPlayer.Id == 0 || nextPlayer.Id == 0)
                        gm.NotifyPlayerHandChanged();
                    _giveCount = 0;

                    //沒牌的話直接跳過
                    if (!currentPlayer.Hand.Any())
                        _discardCount = 0;
                }
            }
            else if(_discardCount > 0)
            {
                if (cards.Count() > _discardCount)
                {
                    MessageBox.Show($"請至少選擇 {_discardCount} 張牌丟棄！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else
                {
                    Player currentPlayer = gm.Players[gm.CurrentPlayerIdx];
                    currentPlayer.RemoveCards(cards);
                    if (currentPlayer.Id == 0)
                        gm.NotifyPlayerHandChanged();
                    _discardCount = 0;
                }
            }

            //檢查是否結束
            if (gm.CheckFinished())
                return true;

            //已結束丟牌操作
            if(_giveCount == 0 && _discardCount == 0)
            {
                EndTrick(gm);
            }

            return true;
        }
        public bool Pass(GameManager gm) => false;
        private void EndTrick(GameManager gm)
        {
            int activePlayersCount = gm.Players.Count(p => p.StateType != Player.State.Finished && p.Id != gm.CurrentPlayerIdx);

            //直接獲得牌權
            if (_isS3OnJoker || _isEightCut || _skipCount >= activePlayersCount)
            {
                //若打出玩家已脫出，則直接給下一個
                if (gm.Players[gm.CurrentPlayerIdx].StateType == Player.State.Finished)
                    gm.CurrentPlayerIdx = gm.GetNextPlayerIdx();
                else
                    gm.CurrentPlayerIdx = gm.LastPlayedPlayerIdx;
                gm.IsTemporaryReversed = false;
                gm.IsSuitLocked = false;
                gm.PassCount = 0;
                gm.LastPlayedPlayerIdx = -1;
                gm.LastPlay = null;
                gm.CurrentPlay = null;
                gm.CurrentHands = Hands.Null;
                gm.NotifyDeskChanged();
            }
            else
            {
                //跳過多人，若不須跳過則只會執行一次
                for (int i = 0; i <= _skipCount; i++)
                {
                    gm.CurrentPlayerIdx = gm.GetNextPlayerIdx();
                }
            }

            gm.ChangeState(new PlayerTurnState());
        }
    }
}
