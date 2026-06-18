using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Controller;
using test01.Model;

namespace test01.View
{
    public partial class HandView : UserControl
    {
        private GameManager _gameManager;
        private readonly List<CardView> _cardViews;

        public HandView()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            _cardViews = new List<CardView>();
        }
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnPlayerHandChanged += HandleHandChanged;
        }
        public void Draw(IEnumerable<Card> hand)
        {
            var oldCards = this.Controls.Cast<Control>().ToList();
            this.Controls.Clear();
            _cardViews.Clear();
            foreach (var ctrl in oldCards)
            {
                ctrl.Dispose();
            }

            if (!hand.Any())
                return;

            int y = 10; //預留選取上浮空間
            foreach (var card in hand)
            {
                CardView cardView = new CardView(card)
                {
                    //x的位置透過OnLayout統一計算
                    Location = new Point(0, y)
                };
                cardView.OnCardPlayed += HandleCardPlayed;
                cardView.OnSelectionChanged += HandleSelectionChanged;

                _cardViews.Add(cardView);
                this.Controls.Add(cardView);
            }
        }
        public void Unselect()
        {
            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView)
                {
                    cardView.Unselect();
                }
            }
        }
        private void HandleSelectionChanged(object sender, EventArgs e)
        {
            List<Card> selectedCards = new List<Card>();
            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView && cardView.IsSelected)
                {
                    selectedCards.Add(cardView.Card);
                }
            }

            List<Card> recommendedCards = (List<Card>)_gameManager.UpdateRecommendations(selectedCards);
            //所有牌都能打
            if (recommendedCards == null){
                foreach (Control control in this.Controls)
                {
                    if (control is CardView cardView)
                        cardView.Enabled = true;
                }

                return;
            }

            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView)
                {
                    //根據是否有推薦決定是否將其啟用
                    cardView.Enabled = recommendedCards.Contains(cardView.Card);
                }
            }
        }
        private void HandleCardPlayed(object sender, EventArgs e)
        {
            List<Card> cards = new List<Card>();

            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView && cardView.IsSelected)
                {
                    cards.Add(cardView.Card);
                }
            }

            if (cards.Any())
            {
                if (!_gameManager.TryPlayCard(cards))
                    MessageBox.Show("牌型不合法!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                    //更新當前玩家手牌
                    Invoke(new Action(() => HandleHandChanged(sender, e)));
            }
        }
        private void HandleHandChanged(object sender, EventArgs e)
        {
            var currentHand = _gameManager.Players[_gameManager.CurrentPlayerIdx].Hand;
            Draw(currentHand);
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_cardViews == null || !_cardViews.Any())
                return;

            int spacing = 45;
            int cardWidth = _cardViews[0].Width;
            int totalWidth = ((_cardViews.Count - 1) * spacing) + cardWidth;
            int startX = (this.ClientSize.Width - totalWidth) / 2;

            for (int i = 0; i < _cardViews.Count; i++)
            {
                _cardViews[i].Left = startX + (i * spacing);
            }
        }
    }
}
