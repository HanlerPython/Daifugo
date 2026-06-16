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
        public HandView Hand { get; private set; } //手牌顯示元件

        public PlayingControl()
        {
            this.Dock = DockStyle.Fill;

            //初始化HandView
            Hand = new HandView();
            this.Controls.Add(Hand);
            Hand.Location = new Point(0, 400);

            this.Click += Background_Click;
        }

        //點擊空白處時取消選取所有元件
        private void Background_Click(object sender, EventArgs e)
        {
            Hand.Unselect();
        }
    }
}
