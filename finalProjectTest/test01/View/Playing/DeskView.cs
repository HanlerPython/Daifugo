using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using test01.Controller;
using test01.Model;
using test01.Utils;

namespace test01.View.Playing
{
    public partial class DeskView : UserControl
    {
        private readonly Label _infoLabel; // 顯示：「玩家 [張三] 打出：」
        private readonly List<PictureBox> _displayedCards; // 顯示打出的卡牌圖片
        private GameManager _gameManager;

        public DeskView()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.Transparent; // 讓底色透過去，看得到遊戲背景
            _displayedCards = new List<PictureBox>();

            //設置位置
            _infoLabel = new Label
            {
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Microsoft JhengHei", 12, FontStyle.Bold),
                Location = new Point(20, 20) // 或透過 OnLayout 置中
            };
            this.Controls.Add(_infoLabel);
        }

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnDeskChanged += HandleDeskChanged;
            _gameManager.OnCardExchanging += HandleCardExchanging;
        }
        public void HandleDeskChanged(object sender, EventArgs e)
        {
            string playerName = _gameManager.Players[_gameManager.CurrentPlayerIdx].Name;
            IEnumerable<Card> playedCards = _gameManager.CurrentPlay;

            //清理舊卡牌圖片
            foreach (var pb in _displayedCards)
            {
                this.Controls.Remove(pb);
                pb.Dispose();
            }
            _displayedCards.Clear();

            if (playedCards == null || !playedCards.Any())
            {
                _infoLabel.Text = $"當前玩家: {playerName}\r\n目前牌桌上沒有牌";
                return;
            }

            _infoLabel.Text = $"當前玩家: {playerName}";

            // 動態生成並排列打出的卡牌（位置計算邏輯可封裝在 OnLayout 或在此直接計算）
            int y = 69;
            int spacing = 30;//重疊在一起
            int cardWidth = 60;
            int totalWidth = ((_displayedCards.Count - 1) * spacing) + cardWidth;
            int startX = (this.ClientSize.Width - totalWidth) / 2;
            

            foreach (var card in playedCards)
            {
                PictureBox cardImg = new PictureBox
                {
                    Image = ResourceManager.GetCardFaceImage(card.SuitType, card.RankType),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Location = new Point(startX, y)
                };
                // 設定圖片大小（通常比手牌稍微小一點點作為視覺區隔）
                cardImg.Width = 60;
                cardImg.Height = 80;

                _displayedCards.Add(cardImg);
                this.Controls.Add(cardImg);
                cardImg.BringToFront();

                startX += spacing;
            }
        }
        public void HandleCardExchanging(object sender, EventArgs e)
        {
            _infoLabel.Text = "請進行卡牌交換";
        }
    }
}