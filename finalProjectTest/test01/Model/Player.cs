using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace test01.Model
{
    public class Player
    {
        public enum State
        {
            Active,
            Finished //脫出
        }

        public int Id { get; }
        public string Name { get; }
        public State StateType { get; set; }
        public int Rank { get; set; }
        private readonly List<Card> _hand;
        public IReadOnlyList<Card> Hand => _hand.AsReadOnly(); //對外僅供檢視的手牌

        public Player(int id, string name)
        {
            this.Id = id;
            this.Name = name;
            this.StateType = State.Active;
            this.Rank = 0;
            this._hand = new List<Card>();
        }
        public void AddCards(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                _hand.Add(card);
            }
            SortHand();
        }
        public void RemoveCards(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                _hand.Remove(card);
            }

            //脫出檢查
            if (_hand.Count == 0)
                StateType = State.Finished;
        }
        public void ClearHand() {
            _hand.Clear();
        }
        //方便換牌功能
        public IEnumerable<Card> GetHighestNonJokers(int count)
        {
            // 排除鬼牌，依權重與花色降序取前N張
            return Hand.Where(c => c.SuitType != Card.Suit.JOKER)
                       .OrderByDescending(c => c.Weight)
                       .ThenByDescending(c => c.SuitType)
                       .Take(count)
                       .ToList();
        }

        public IEnumerable<Card> GetLowestCards(int count)
        {
            //依權重與花色升序取前N張
            return Hand.OrderBy(c => c.Weight)
                       .ThenBy(c => c.SuitType)
                       .Take(count)
                       .ToList();
        }
        private void SortHand()
        {
            _hand.Sort((a, b) =>
            {
                int rankCompare = a.RankType.CompareTo(b.RankType);
                if (rankCompare != 0) return rankCompare;

                return a.SuitType.CompareTo(b.SuitType);
            });
        }
    }
}
