using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public class RoundInitState : IGameState
    {
        public void Enter(GameManager gm)
        {
            gm.CurrentPlay = null;
            gm.CurrentHands = HandsEvaluator.Hands.Null;
            gm.IsReversed = false;
            //決定起始玩家 (如持有梅花3或大富豪階級)
            gm.CurrentPlayerIdx = 0;

            //給起始玩家和其下家最多牌
            gm.Deck.Create();
            gm.Deck.Shuffle();
            foreach (Player player in gm.Players)
            {
                player.AddCards(gm.Deck.Draw(13));
            }
            gm.Players[gm.CurrentPlayerIdx].AddCards(gm.Deck.Draw(1));
            gm.Players[gm.GetNextPlayerIdx()].AddCards(gm.Deck.Draw(1));
            gm.NotifyPlayerHandChanged();

            //進入玩家回合狀態
            gm.ChangeState(new PlayerTurnState());
        }

        //初始化階段不執行任何操作
        public bool PlayCard(GameManager gm, IEnumerable<Card> cards) => false;
        public bool Pass(GameManager gm) => false;
        public bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards) => false;
    }
}
