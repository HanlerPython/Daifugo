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
                await Task.Delay(500); //模擬思考時間
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
            var playHands = gm.HandsEvaluator.Evaluate(sortedPlay, gm.CurrentPlay, gm.CurrentHands, gm.IsReversed,  gm.IsSuitLocked);
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
                    playHands = Hands.Flush; //默認為同花順
            }

            gm.PassCount = 0;
            int index = gm.CurrentPlayerIdx;
            var CurrentPlayer = gm.Players[index];
            gm.LastPlayedPlayerIdx = index;
            gm.DiscardPile.AddRange(sortedPlay);
            CurrentPlayer.RemoveCards(sortedPlay); //會檢查手牌張數觸發脫出
            gm.CurrentHands = playHands;
            gm.LastPlay = gm.CurrentPlay;
            gm.CurrentPlay = sortedPlay;
            gm.NotifyDeskChanged();

            //若這輪還沒結束
            if(!gm.CheckFinished())
                gm.ChangeState(new SpecialActionState());

            return true;
        }
        public bool Pass(GameManager gm)
        {
            gm.PassCount++;
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

            return true;
        }
        private void NextTurn(GameManager gm)
        {
            gm.IsTemporaryReversed = false;
            gm.IsSuitLocked = false;
            gm.PassCount = 0;
            gm.LastPlayedPlayerIdx = -1;
            gm.LastPlay = null;
            gm.CurrentPlay = null;
            gm.CurrentHands = Hands.Null;
            gm.NotifyDeskChanged();
        }
    }
}