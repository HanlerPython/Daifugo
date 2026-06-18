using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test01.View.UserControls
{
    public partial class PlayingControl : UserControl
    {
        public HandView Hand => this._hand;
        public event Action OnPassRequested;
        public PlayingControl()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(40, 44, 52);

            BindBackgroundClick(this);
            this._passBtn.Click += PassBtn_Click;
        }

        private void BindBackgroundClick(Control parent)
        {
            //點擊非按鈕或是卡牌都會取消選取
            if (!(parent is Button) && !(parent is CardView))
            {
                parent.Click += Background_Click;
            }

            //繼續往下找子元件
            foreach (Control child in parent.Controls)
            {
                BindBackgroundClick(child);
            }
        }
        private void Background_Click(object sender, EventArgs e)
        {
            Hand.Unselect();
        }
        private void PassBtn_Click(object sender, EventArgs e)
        {
            //對外廣播pass觸發事件
            OnPassRequested?.Invoke();
        }
    }
}
