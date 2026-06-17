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
        public Deck Deck { get; private set; }
        public List<Card> CurrentPlay { get; set; }
        public HandsEvaluator.Hands CurrentHands { get; set; }
        public bool IsReversed { get; set; } //是否為大革命狀態或J反
        public int CurrentPlayerIdx { get; set; }
        public HandsEvaluator HandsEvaluator { get; }
        public GreedyAIStrategy Ai { get; }


        //當玩家手牌張數發生變化時觸發
        public event EventHandler OnPlayerHandChanged;
        //使外部改變牌桌狀態
        public event EventHandler OnDeskChanged;

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
            return HandsEvaluator.Recommand(Players[CurrentPlayerIdx].Hand, selectedCards, CurrentPlay, CurrentHands, false);
        }
        public void NotifyPlayerHandChanged()
        {
            OnPlayerHandChanged?.Invoke(this, EventArgs.Empty);
        }
        public void NotifyDeskChanged()
        {
            OnDeskChanged?.Invoke(this, EventArgs.Empty);
        }

        //由個別state實作
        public bool TryPlayCard(IEnumerable<Card> cards) => _currentState.PlayCard(this, cards);
        public bool TryPass() => _currentState.Pass(this);
        public bool TrySubmitSpecialAction(IEnumerable<Card> cards) => _currentState.SubmitSpecialAction(this, cards);
    }
}
