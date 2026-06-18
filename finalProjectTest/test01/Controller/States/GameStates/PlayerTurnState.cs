using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;
using static test01.Model.HandsEvaluator;

namespace test01.Controller.States.GameStates
{
    public class PlayerTurnState : IGameState
    {
        public async void Enter(GameManager gm)
        {
            gm.NotifyDeskChanged();
            if (gm.CurrentPlayerIdx != 0) //人機
            {
                await Task.Delay(1000); //模擬思考時間
                var currentAi = gm.Players[gm.CurrentPlayerIdx];
                GameSnapshot gs = new GameSnapshot(gm);
                var cardsToPlay = gm.Ai.DecidePlay(gs, currentAi.Hand);
                if (cardsToPlay.Any())
                    PlayCard(gm, cardsToPlay);
                else
                    Pass(gm);
            }
            else //輪到主玩家
                gm.NotifyPlayerTurnStarted();
        }

        public bool PlayCard(GameManager gm, IEnumerable<Card> cards)
        {

            List<Card> sortedPlay = cards
                .OrderBy(c => c.RankType)
                .ThenBy(c => c.SuitType)
                .ToList();
            var playHands = gm.HandsEvaluator.Evaluate(sortedPlay, gm.CurrentPlay, gm.IsReversed, gm.CurrentHands);
            if (playHands == Hands.Illegal)
                return false;
            else if(playHands == Hands.Both)
            {
                if(gm.CurrentPlayerIdx == 0) //主玩家
                {
                    var ressult = MessageBox.Show("這是同花順嗎?", "選擇牌型", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (ressult == DialogResult.Yes)
                        playHands = Hands.Flush;
                    else
                        playHands = Hands.SameRank;
                }
                else //人機
                    playHands = Hands.Flush; //暫時默認為同花順
            }

            gm.PassCount = 0;
            int index = gm.CurrentPlayerIdx;
            var CurrentPlayer = gm.Players[index];
            gm.LastPlayedPlayerIdx = index;
            gm.CurrentHands = playHands;
            CurrentPlayer.RemoveCards(sortedPlay); //會檢查手牌張數觸發脫出
            gm.CurrentPlay = sortedPlay;
            gm.NotifyDeskChanged();

            if (CurrentPlayer.StateType == Player.State.Finished)
            {
                gm.FinishedPlayersCount++;
                CurrentPlayer.Rank = gm.FinishedPlayersCount;
                if (gm.FinishedPlayersCount == 3)
                {
                    gm.Players[gm.GetNextPlayerIdx()].Rank = 4;
                    gm.ChangeState(new RoundEndState());
                    return true;
                }
            }

            bool isDiscardEffect = false; // 替換為實際卡牌判斷
            if (isDiscardEffect)
            {
                // 中斷一般輪轉，切換至等待捨棄狀態
                //gm.ChangeState(new WaitingDiscardState(/* 傳遞需捨棄的張數 */));
            }
            else //一般出牌，輪轉下一位
            {
                NextPlayer(gm);
            }

            return true;
        }
        public bool Pass(GameManager gm)
        {
            gm.PassCount++;
            NextPlayer(gm);
            return true;
        }

        public bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards) => false;
        private void NextPlayer(GameManager gm)
        {
            int activePlayersCount = gm.Players.Count(p => p.StateType != Player.State.Finished);
            int index = gm.GetNextPlayerIdx();
            if (index != -1)
            {
                gm.CurrentPlayerIdx = index;
                //如果又回到最後出牌的人，進到下個回合，清空牌桌
                if (index == gm.LastPlayedPlayerIdx)
                    NextTurn(gm);
                //如果上一個脫出的人出牌後其他活躍玩家都Pass
                else if (activePlayersCount == gm.PassCount)
                    NextTurn(gm);
                gm.ChangeState(new PlayerTurnState());
            }
            else
            {
                //基本不可能發生(所有人都是脫出狀態)
            }
        }
        private void NextTurn(GameManager gm)
        {
            gm.PassCount = 0;
            gm.LastPlayedPlayerIdx = -1;
            gm.CurrentPlay = null;
            gm.CurrentHands = Hands.Null;
            gm.NotifyDeskChanged();
        }
    }
}