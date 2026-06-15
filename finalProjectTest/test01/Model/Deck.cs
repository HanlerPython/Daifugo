using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test01.Model
{
    public class Deck
    {
        private readonly List<Card> _cards;
        public IReadOnlyList<Card> Cards => _cards.AsReadOnly();
        private readonly Random _random;

        public Deck()
        {
            _cards = new List<Card>();
            _random = new Random();
        }
        public void Create()
        {
            _cards.Clear();

            //一般牌
            foreach (Card.Suit suit in Enum.GetValues(typeof(Card.Suit)))
            {
                if (suit == Card.Suit.JOKER)
                    continue;

                foreach (Card.Rank rank in Enum.GetValues(typeof(Card.Rank)))
                {
                    if (rank == Card.Rank.BLACK ||
                        rank == Card.Rank.RED)
                        continue;

                    _cards.Add(new Card(suit, rank));
                }
            }

            //大小王
            _cards.Add(new Card(Card.Suit.JOKER, Card.Rank.BLACK));
            _cards.Add(new Card(Card.Suit.JOKER, Card.Rank.RED));
        }
        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);

                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }
        public IEnumerable<Card> Draw(int n)
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < n && _cards.Count > 0; i++) //確保不會抽超過
            {
                int lastIndex = _cards.Count - 1;
                cards.Add(_cards[lastIndex]);
                _cards.RemoveAt(lastIndex);
            }

            return cards;
        }
    }
}
