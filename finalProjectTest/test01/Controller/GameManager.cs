using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;

namespace test01.Controller
{
    public class GameManager
    {
        private readonly List<Player> _players;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        private readonly Deck _deck;
        private List<Card> _currentPlay;
        public IReadOnlyList<Card> CurretnPlay => _currentPlay?.AsReadOnly();
        private HandsEvaluator.Hands _currentHands;
        private bool _isReversed; //是否為大革命狀態或J反
        private Player _currentPlayer;
        private readonly HandsEvaluator _handsEvaluator;

        //當玩家手牌張數發生變化時觸發
        public event EventHandler OnPlayerHandChanged;

        public GameManager()
        {
            _players = new List<Player>();
            _deck = new Deck();
            _currentPlay = null;
            _currentHands = HandsEvaluator.Hands.Null;
            _isReversed = false;
            _currentPlayer = null;
            _handsEvaluator = new HandsEvaluator();
        }
        public void Initialize()
        {
            _players.Add(new Player(0, "Yaju Senpai"));
            _players.Add(new Player(1, "Chisa"));
            _players.Add(new Player(2, "Yachiyo"));
            _players.Add(new Player(3, "Doro"));
            _currentPlayer = _players[0];
            _deck.Create();
            _deck.Shuffle();
            _currentPlay = null;
            _currentHands = HandsEvaluator.Hands.Null;
            _isReversed = false;
            foreach (Player player in _players)
            {
                player.AddCards(_deck.Draw(13)); //just for test
            }

            //廣播手牌改變事件
            OnPlayerHandChanged?.Invoke(null, EventArgs.Empty);
        }
        public IReadOnlyList<Card> GetCurrentPlayerHand()
        {
            return _currentPlayer.Hand;
        }
        public IEnumerable<Card> UpdateRecommendations(IEnumerable<Card> selectedCards)
        {
            return _handsEvaluator.Recommand(_currentPlayer.Hand, selectedCards, CurretnPlay, _currentHands, false);
        }
        public void TryPlayCard(IEnumerable<Card> cardsToPlay)
        {
            List<Card> sortedPlay = cardsToPlay
                .OrderBy(c => c.RankType)
                .ThenBy(c => c.SuitType)
                .ToList();
            //先檢查是否合法
            var playHands = _handsEvaluator.Evaluate(sortedPlay, CurretnPlay, _isReversed, _currentHands);
            if (playHands == HandsEvaluator.Hands.Illegal)
            {
                MessageBox.Show("牌型不合法!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentHands = playHands;
            _currentPlayer.RemoveCards(sortedPlay);
            _currentPlay = sortedPlay;
            OnPlayerHandChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
