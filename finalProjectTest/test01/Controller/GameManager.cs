using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Controller.States.GameStates;
using test01.Model;
using test01.Model.Interfaces;

namespace test01.Controller
{
    public class GameManager
    {
        private IGameState _currentState;
        private readonly List<Player> _players;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public List<Player> Winners { get; set; } = new List<Player>();
        public List<Player> Losers { get; set; } = new List<Player>();
        public Deck Deck { get; private set; }
        public List<Card> DiscardPile { get; set; } = new List<Card>();
        public List<Card> CurrentPlay { get; set; }
        public List<Card> LastPlay { get; set; }
        public HandsEvaluator.Hands CurrentHands { get; set; }
        public bool Reversed { get; set; } //是否為大革命狀態
        public bool IsTemporaryReversed { get; set; } //J反一回合後重新歸正
        public bool IsReversed => Reversed ^ IsTemporaryReversed; //當前反轉狀態
        public bool IsSuitLocked { get; set; } //是否鎖花色
        public int CurrentPlayerIdx { get; set; }
        public int LastPlayedPlayerIdx { get; set; }
        public int PassCount { get; set; } = 0;
        public HandsEvaluator HandsEvaluator { get; }
        public GreedyAIStrategy Ai { get; }


        //當玩家手牌張數發生變化時觸發
        public event EventHandler OnPlayerHandChanged;
        //使外部改變牌桌狀態
        public event EventHandler OnDeskChanged;
        //觸發輪到玩家時的前置作業
        public event EventHandler OnPlayerTurnStarted;
        //卡牌交換時改變場中文字
        public event EventHandler OnCardExchanging;

        public GameManager()
        {
            _players = new List<Player>();
            Deck = new Deck();
            HandsEvaluator = new HandsEvaluator();
            Ai = new GreedyAIStrategy();
        }
        public void Initialize()
        {
            _players.Add(new Player(0, "Yaju Senpai"));
            _players.Add(new Player(1, "Chisa"));
            _players.Add(new Player(2, "Yachiyo"));
            _players.Add(new Player(3, "Doro"));
            //進入輪次事前準備
            ChangeState(new RoundInitState());
        }
        public int GetNextPlayerIdx()
        {
            int index = CurrentPlayerIdx, initial = index;
            while (true) {
                index++;
                if (index > 3)
                    index = 0;
                //沒有還在遊戲中的玩家
                if (index == initial)
                    return -1;

                //跳過脫出狀態的玩家
                if (Players[index].StateType == Player.State.Finished)
                    continue;

                return index;
            }
        }
        public void ChangeState(IGameState newState)
        {
            _currentState = newState;
            _currentState.Enter(this);
        }
        public IEnumerable<Card> UpdateRecommendations(IEnumerable<Card> selectedCards)
        {
            //非遊玩階段(例如換牌階段)不會推薦牌型
            if (!(_currentState is PlayerTurnState))
            {
                return null;
            }

            return HandsEvaluator.Recommand(Players[CurrentPlayerIdx].Hand, selectedCards, CurrentPlay, CurrentHands, IsReversed, IsSuitLocked);
        }
        public bool CheckFinished()
        {
            Player currentPlayer = Players[CurrentPlayerIdx];
            if (currentPlayer.StateType == Player.State.Finished)
            {
                bool punish = false;
                if (CurrentHands == HandsEvaluator.Hands.SameRank)
                {
                    Card firstCard = CurrentPlay.First();
                    if(firstCard.SuitType == Card.Suit.JOKER
                        || (!Reversed && firstCard.RankType == Card.Rank.TWO)
                        || (Reversed && firstCard.RankType == Card.Rank.THREE))
                    {
                        punish = true;
                    }
                }
                if (punish)
                    Losers.Add(currentPlayer);
                else
                    Winners.Add(currentPlayer);

                if (Winners.Count + Losers.Count == 3)
                {
                    Losers.Add(Players[GetNextPlayerIdx()]);
                    ChangeState(new RoundEndState());
                    return true;
                }
                else
                    return false;
            }

            return false;
        }
        public void NotifyPlayerHandChanged()
        {
            OnPlayerHandChanged?.Invoke(this, EventArgs.Empty);
        }
        public void NotifyDeskChanged()
        {
            OnDeskChanged?.Invoke(this, EventArgs.Empty);
        }
        public void NotifyPlayerTurnStarted()
        {
            OnPlayerTurnStarted?.Invoke(this, EventArgs.Empty);
        }
        public void NotifyCardExchanging()
        {
            OnCardExchanging?.Invoke(this, EventArgs.Empty);
        }

        //由個別state實作
        public bool TryPlayCard(IEnumerable<Card> cards) => _currentState.PlayCard(this, cards);
        public bool TryPass() => _currentState.Pass(this);
    }
}
