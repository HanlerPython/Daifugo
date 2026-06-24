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
    public partial class AiHandView : UserControl
    {
        private readonly List<CardView> _cardViews;
        private GameManager _gameManager;
        private int _playerIndex;
        private readonly Label _nameLabel;

        [Browsable(true)]
        [Category("Game Logic")]
        [Description("設定此區塊對應的玩家索引 (1, 2, 3...)")]
        public int PlayerIndex
        {
            get => _playerIndex;
            set
            {
                _playerIndex = value;
                UpdatePlayerName();
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsVertical { get; set; } = false;

        public AiHandView() : this(0)
        {
        }

        public AiHandView(int playerIndex)
        {
            _playerIndex = playerIndex;
            _cardViews = new List<CardView>();

            //開啟雙緩衝，在記憶體畫好再貼上，解決閃爍問題
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            _nameLabel = new Label
            {
                AutoSize = false,
                Height = 30,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_nameLabel);
        }

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnHandChanged += HandleHandChanged;

            UpdatePlayerName();
        }

        private void UpdatePlayerName()
        {
            if (_gameManager?.Players != null && _playerIndex >= 0 && _playerIndex < _gameManager.Players.Count)
            {
                _nameLabel.Text = _gameManager.Players[_playerIndex].Name;
            }
            else
            {
                _nameLabel.Text = $"AI 玩家 {_playerIndex}";
            }
        }

        private void HandleHandChanged(object sender, EventArgs e)
        {
            UpdatePlayerName();
            var currentHand = _gameManager.Players[_playerIndex].Hand;
            Draw(currentHand);
        }

        public void Draw(IEnumerable<Card> hand)
        {
            this.SuspendLayout();

            this.Controls.Clear();
            this.Controls.Add(_nameLabel);
            _cardViews.Clear();

            if (!hand.Any())
            {
                this.ResumeLayout(true);
                return;
            }

            foreach (var card in hand)
            {
                CardView cardView = new(card, true);

                if (IsVertical && cardView.Image != null)
                {
                    Image rotatedImage = (Image)cardView.Image.Clone();
                    rotatedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    cardView.Image = rotatedImage;

                    (cardView.Height, cardView.Width) = (cardView.Width, cardView.Height);
                }

                _cardViews.Add(cardView);
                this.Controls.Add(cardView);
            }

            this.ResumeLayout(true);
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            _nameLabel.Width = this.ClientSize.Width;

            if (!_cardViews.Any())
            {
                //沒牌的時候，讓名字單獨在畫面上垂直置中
                _nameLabel.Top = (this.ClientSize.Height - _nameLabel.Height) / 2;
                _nameLabel.Left = 0;
                return;
            }

            int labelHeight = _nameLabel.Height;

            if (IsVertical)
            {
                int cardHeight = _cardViews[0].Height;
                int spacing = 15;
                int gap = 10; 

                int maxAvailableHeight = this.ClientSize.Height - labelHeight - gap;

                if (_cardViews.Count > 1)
                {
                    int calculatedSpacing = (maxAvailableHeight - cardHeight) / (_cardViews.Count - 1);
                    if (calculatedSpacing < spacing)
                    {
                        spacing = calculatedSpacing;
                    }
                }

                //計算純卡牌排列出來的總高度
                int totalCardsHeight = ((_cardViews.Count - 1) * spacing) + cardHeight;
                //整體大區塊的總高度（名字 + 間距 + 卡牌）
                int totalBlockHeight = labelHeight + gap + totalCardsHeight;

                //計算這個大區塊在整條 Dock 空間裡的置中起點 Y
                int blockStartY = (this.ClientSize.Height - totalBlockHeight) / 2;

                //名字在大區塊的最頂端
                _nameLabel.Top = blockStartY;
                _nameLabel.Left = 0;

                //卡牌接在名字與間距的下方
                int startY = blockStartY + labelHeight + gap;

                for (int i = 0; i < _cardViews.Count; i++)
                {
                    _cardViews[i].Top = startY + (i * spacing);
                    _cardViews[i].Left = (this.ClientSize.Width - _cardViews[i].Width) / 2;
                }
            }
            else
            {
                _nameLabel.Top = 5;
                _nameLabel.Left = 0;

                int cardWidth = _cardViews[0].Width;
                int spacing = 15;

                if (_cardViews.Count > 1)
                {
                    int maxAvailableWidth = this.ClientSize.Width;
                    int calculatedSpacing = (maxAvailableWidth - cardWidth) / (_cardViews.Count - 1);
                    if (calculatedSpacing < spacing)
                    {
                        spacing = calculatedSpacing;
                    }
                }

                int totalWidth = ((_cardViews.Count - 1) * spacing) + cardWidth;
                int startX = (this.ClientSize.Width - totalWidth) / 2;

                int maxAvailableHeight = this.ClientSize.Height - labelHeight - 5;
                int startY = labelHeight + 5 + (maxAvailableHeight - _cardViews[0].Height) / 2;

                for (int i = 0; i < _cardViews.Count; i++)
                {
                    _cardViews[i].Left = startX + (i * spacing);
                    _cardViews[i].Top = startY;
                }
            }
        }
    }
}