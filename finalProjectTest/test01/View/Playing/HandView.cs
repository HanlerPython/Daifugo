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

namespace test01.View.Playing
{
    public partial class HandView : UserControl
    {
        private readonly List<CardView> _cardViews;
        private GameManager _gameManager;

        public HandView()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            _cardViews = new List<CardView>();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnHandChanged += HandleHandChanged;
            _gameManager.OnPlayerTurnStarted += HandlePlayerTurnStarted;

            
        }
        public void Draw(IEnumerable<Card> hand)
        {
            this.SuspendLayout();

            var oldCards = this.Controls.Cast<Control>().ToList();
            this.Controls.Clear();
            _cardViews.Clear();
            foreach (var ctrl in oldCards)
            {
                ctrl.Dispose();
            }

            if (!hand.Any())
            {
                this.ResumeLayout(true);
                return;
            }
                
            int y = 10; //預留選取上浮空間
            foreach (var card in hand)
            {
                CardView cardView = new(card, false)
                {
                    //x的位置透過OnLayout統一計算
                    Location = new Point(0, y)
                };
                cardView.OnCardPlayed += HandleCardPlayed;
                cardView.OnSelectionChanged += HandleSelectionChanged;

                _cardViews.Add(cardView);
                this.Controls.Add(cardView);
                cardView.BringToFront();
            }
            this.ResumeLayout(true);
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
            //非自己回合不推薦牌型
            if (_gameManager.CurrentPlayerIdx != 0)
                return;

            List<Card> selectedCards = new();
            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView && cardView.IsSelected)
                {
                    selectedCards.Add(cardView.Card);
                }
            }

            UpdateRecommendations(selectedCards);
        }
        private void HandleCardPlayed(object sender, EventArgs e)
        {
            //非自己回合無法出牌
            if(_gameManager.CurrentPlayerIdx != 0)
            {
                Unselect(); //提示出牌失敗
                return;
            }

            List<Card> cards = new();

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
                {
                    Unselect();
                    return;
                }
            }
        }
        private void HandleHandChanged(object sender, EventArgs e)
        {
            var currentHand = _gameManager.Players[0].Hand;
            Draw(currentHand);
        }
        private void HandlePlayerTurnStarted(object sender, EventArgs e)
        {
            Unselect();
            //主動觸發推薦手牌
            List<Card> cards = new();
            UpdateRecommendations(cards);
        }
        private void UpdateRecommendations(IEnumerable<Card> selectedCards)
        {
            List<Card> recommendedCards = (List<Card>)_gameManager.UpdateRecommendations(selectedCards);
            //所有牌都能打
            if (recommendedCards == null)
            {
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
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (_cardViews == null || !_cardViews.Any())
                return;

            int spacing = 30;
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
