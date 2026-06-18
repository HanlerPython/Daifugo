using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;

namespace test01.Controller.States.GameStates
{
    public class RoundInitState : IGameState
    {
        private Player _targetAi; //等待和玩家換牌的人機
        private int _requiredCount; //與人機換牌的張數

        public void Enter(GameManager gm)
        {
            gm.CurrentPlay = null;
            gm.CurrentHands = HandsEvaluator.Hands.Null;
            gm.NotifyDeskChanged();
            gm.IsReversed = false;
            gm.CurrentPlayerIdx = -1;
            gm.LastPlayedPlayerIdx = -1;
            gm.FinishedPlayersCount = 0;
            gm.PassCount = 0;

            //給起始玩家和其下家最多牌
            gm.Deck.Create();
            gm.Deck.Shuffle();
            Player daifugo = null;
            Player fugo = null;
            Player hinmin = null;
            Player daihinmin = null;
            foreach(Player p in gm.Players)
            {
                p.AddCards(gm.Deck.Draw(13));
                switch (p.Rank)
                {
                    case 1:
                        daifugo = p;
                        break;
                    case 2:
                        fugo = p;
                        break;
                    case 3:
                        hinmin = p;
                        break;
                    case 4:
                        daihinmin = p;
                        break;
                }
            }
            if(daifugo == null)
            {
                //第一局遊戲由主玩家和下家抽最多
                gm.Players[0].AddCards(gm.Deck.Draw(1));
                gm.Players[1].AddCards(gm.Deck.Draw(1));
                //跳過換牌步驟
                RoundStart(gm);
            }
            else
            {
                //後續開局由大富豪和其下家抽最多
                daifugo.AddCards(gm.Deck.Draw(1));
                gm.Players[(daifugo.Id + 1) % 4].AddCards(gm.Deck.Draw(1));

                //如果玩家是貧民或大貧民，直接換牌並開始，否則等待選牌操作
                if (gm.Players[0] == daihinmin || gm.Players[0] == hinmin)
                {
                    ExchangeCards(daifugo, daihinmin, daifugo.GetLowestCards(2));
                    ExchangeCards(fugo, hinmin, fugo.GetLowestCards(1));
                    RoundStart(gm);
                }
                else
                {
                    gm.CurrentPlayerIdx = 0; //讓玩家能正常打牌
                    if (gm.Players[0] == daifugo)
                    {
                        _targetAi = daihinmin;
                        _requiredCount = 2;
                        //先幫另外兩個AI換
                        ExchangeCards(fugo, hinmin, fugo.GetLowestCards(1));
                    }
                    else
                    {
                        _targetAi = hinmin;
                        _requiredCount = 1;
                        ExchangeCards(daifugo, daihinmin, fugo.GetLowestCards(1));
                    }
                    //顯示提示文字
                    gm.NotifyCardExchanging();
                }
            }
            gm.NotifyPlayerHandChanged();
        }

        //初始化階段只處理換牌操作
        public bool PlayCard(GameManager gm, IEnumerable<Card> cards)
        {
            if (cards.Count() != _requiredCount)
            {
                MessageBox.Show($"請精確選擇 {_requiredCount} 張牌進行交換！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ExchangeCards(gm.Players[0], _targetAi, cards);
            gm.NotifyPlayerHandChanged();
            gm.CurrentPlayerIdx = -1;
            RoundStart(gm);

            return true;
        }
        public bool Pass(GameManager gm) => false;
        public bool SubmitSpecialAction(GameManager gm, IEnumerable<Card> cards) => false;
        private void RoundStart(GameManager gm)
        {
            //從手上有方塊三的人開始
            for (int i = 0; i < 4 && gm.CurrentPlayerIdx == -1; i++)
            {
                foreach (Card card in gm.Players[i].Hand)
                {
                    if (card.SuitType == Card.Suit.DIAMONDS && card.RankType == Card.Rank.THREE)
                    {
                        gm.CurrentPlayerIdx = i;
                        break;
                    }
                }
            }

            gm.ChangeState(new PlayerTurnState());
        }
        private void ExchangeCards(Player highRankPlayer, Player lowRankPlayer, IEnumerable<Card> givenCards)
        {
            //取得下位者最大的牌
            var takenCards = lowRankPlayer.GetHighestNonJokers(givenCards.Count());

            //雙方移除對應卡牌
            highRankPlayer.RemoveCards(givenCards);
            lowRankPlayer.RemoveCards(takenCards);

            //雙方加入交換的卡牌
            highRankPlayer.AddCards(takenCards);
            lowRankPlayer.AddCards(givenCards);
        }
    }
}
