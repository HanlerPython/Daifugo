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

        public HandView()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
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
            foreach (var ctrl in oldCards)
            {
                ctrl.Dispose();
            }

            int cardCount = hand.Count();
            if (cardCount == 0) return;

            List<CardView> cardViews = new List<CardView>();
            foreach (var card in hand)
            {
                cardViews.Add(new CardView(card));
            }

            int y = 10; //預留10px頂部浮動空間供選取時上浮
            int spacing = 45;
            int cardWidth = cardViews[0].Width;
            int totalWidth = ((cardCount - 1) * spacing) + cardWidth;
            int windowWidth = Form1.WindowWidth;
            int startX = (windowWidth - totalWidth) / 2;

            for (int i = 0; i < cardCount; i++)
            {
                CardView cardView = cardViews[i];
                int currentX = startX + (i * spacing);

                cardView.Location = new Point(currentX, y);
                cardView.CardPlayed += OnCardPlayed;

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
        private void OnCardPlayed(object sender, EventArgs e)
        {
            List<Card> cards = new List<Card>();

            foreach (Control control in this.Controls)
            {
                if (control is CardView cardView && cardView.IsSelected)
                {
                    cards.Add(cardView.Card);
                }
            }

            if (cards.Count > 0)
            {
                _gameManager.TryPlayCard(cards);
            }
        }
        private void HandleHandChanged(object sender, EventArgs e)
        {
            //確保主執行緒觸發時才作用
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleHandChanged(sender, e)));
                return;
            }

            var currentHand = _gameManager.GetCurrentPlayerHand();
            Draw(currentHand);
        }
    }
}
