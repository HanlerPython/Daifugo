using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public class PlayerTurnState : IGameState
    {
        public void Enter(GameManager gm)
        {
            if(gm.CurrentPlayerIdx != 0)//人機
            {
                var currentAi = gm.Players[gm.CurrentPlayerIdx];
                GameSnapshot gs = new GameSnapshot(gm);
                var cardsToPlay = gm.Ai.DecidePlay(gs, currentAi.Hand);
                if(cardsToPlay.Any())
                    PlayCard(gm, cardsToPlay);
                else
                    Pass(gm);
            }
        }

        public bool PlayCard(GameManager gm, IEnumerable<Card> cards)
        {
            List<Card> sortedPlay = cards
                .OrderBy(c => c.RankType)
                .ThenBy(c => c.SuitType)
                .ToList();
            var playHands = gm.HandsEvaluator.Evaluate(sortedPlay, gm.CurrentPlay, gm.IsReversed, gm.CurrentHands);
            if (playHands == HandsEvaluator.Hands.Illegal)
                return false;

            gm.CurrentHands = playHands;
            gm.Players[gm.CurrentPlayerIdx].RemoveCards(sortedPlay);
            gm.CurrentPlay = sortedPlay;
            gm.NotifyDeskChanged();

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
            // 紀錄 Pass，並輪轉下一位
            NextPlayer(gm);
            return true;
        }

        public bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards) => false;
        private void NextPlayer(GameManager gm)
        {
            int index = gm.GetNextPlayerIdx();
            if (index != -1)
            {
                gm.CurrentPlayerIdx = index;
                if(index == 0)
                    gm.NotifyPlayerHandChanged();
                gm.ChangeState(new PlayerTurnState());
            }
            else
            {
                //end
            }
        }
    }
}