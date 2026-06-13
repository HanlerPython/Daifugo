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
        public int Id { get; }
        public string Name { get; }
        private readonly List<Card> _hand;
        public IReadOnlyList<Card> Hand => _hand.AsReadOnly(); //對外僅供檢視的手牌

        public Player(int id, string name)
        {
            this.Id = id;
            this.Name = name;
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
            SortHand();

            // 集中處理狀態更新
            CheckWinCondition();
        }
        private void CheckWinCondition()
        {
            if (_hand.Count == 0)
            {
                // 標記該玩家已脫出...
            }
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
