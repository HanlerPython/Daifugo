using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test01.Model;

namespace test01.Controller
{
    public class GameManager
    {
        private readonly List<Player> _players;
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        private readonly Deck _deck;
        private Player _currentPlayer;

        //定義一個事件，當玩家手牌發生變化時觸發
        public event EventHandler OnPlayerHandChanged;

        public GameManager()
        {
            _players = new List<Player>();
            _deck = new Deck();
            _currentPlayer = null;
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
            foreach (Player player in _players)
            {
                player.AddCards(_deck.Draw(13)); //just for test
            }
            //如果有人訂閱這個事件，就觸發它
            OnPlayerHandChanged?.Invoke(null, EventArgs.Empty);
        }
        public IReadOnlyList<Card> GetCurrentPlayerHand()
        {
            return _currentPlayer.Hand;
        }
        public void TryPlayCard(IEnumerable<Card> cardsToPlay)
        {
            // 1. 邏輯驗證 (省略)
            // 2. 更新資料
            _currentPlayer.RemoveCards(cardsToPlay);
            OnPlayerHandChanged?.Invoke(null, EventArgs.Empty);
        }

    }
}
