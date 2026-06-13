using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Model;

namespace test01.View
{
    public class CardView : PictureBox
    {
        public Card Card { get; }
        public bool IsSelected { get; private set; }
        private bool _isHovering;
        public event EventHandler CardPlayed;

        public CardView(Card card)
        {
            this.IsSelected = false;
            this._isHovering = false;
            this.Card = card;
            SizeMode = PictureBoxSizeMode.AutoSize;

            this.MouseEnter += CardView_MouseEnter;
            this.MouseLeave += CardView_MouseLeave;
            this.Click += CardView_Click;
            this.DoubleClick += CardView_Click; //避免點擊過快時無法觸發一般Click

            UpdateImage();
        }
        public void UpdateImage()
        {
            this.Image = ResourceManager.getCardFaceImage(Card.SuitType, Card.RankType);
        }
        public void Unselect()
        {
            if (IsSelected)
            {
                IsSelected = false;
                this.Top = 10;
                this.Invalidate(); //觸發重繪
            }
        }
        private void CardView_MouseEnter(object sender, EventArgs e)
        {
            _isHovering = true;
            this.Invalidate();
        }
        private void CardView_MouseLeave(object sender, EventArgs e)
        {
            _isHovering = false;
            this.Invalidate();
        }
        private void CardView_Click(object sender, EventArgs e)
        {
            //點擊已被選中的牌
            if (IsSelected)
            {
                //觸發自訂事件，讓HandView知道這張牌被打出了
                CardPlayed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                IsSelected = true;
                this.Top = 0; //選中時向上移動
                this.Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe); //畫出原本的撲克牌圖片
            if (IsSelected)
            {
                //畫綠色粗邊框
                using (Pen pen = new Pen(Color.Green, 5))
                {
                    //內縮避免邊框被裁剪
                    pe.Graphics.DrawRectangle(pen, 2, 2, this.Width - 5, this.Height - 5);
                }
            }
            else if (_isHovering)
            {
                //畫黃色細邊框
                using (Pen pen = new Pen(Color.Yellow, 3))
                {
                    pe.Graphics.DrawRectangle(pen, 1, 1, this.Width - 3, this.Height - 3);
                }
            }
        }
    }
}
