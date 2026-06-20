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

namespace test01.View.Playing
{
    public partial class PlayingControl : UserControl
    {
        public HandView Hand => this._hand;
        public DeskView Desk => this._deskView;
        private GameManager _gameManager;

        public event Action OnPassRequested;
        public PlayingControl()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(40, 44, 52);

            //蓋過桌面區域
            this._playerZonePanel.BringToFront();

            BindBackgroundClick(this);
            this._passBtn.Click += PassBtn_Click;
        }

        public void Initialize(GameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.OnRevolutionStarted += HandleRevolutionStarted;
            _gameManager.OnRevolutionEnded += HandleRevolutionEnded;
            this._hand.Initialize(gameManager);
            this._deskView.Initialize(gameManager);
        }
        private void BindBackgroundClick(Control parent)
        {
            //點擊非按鈕或是卡牌都會取消選取
            if (parent is not Button && parent is not CardView)
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
            _hand.Unselect();
        }
        private void PassBtn_Click(object sender, EventArgs e)
        {
            //非自己回合Pass不會觸發
            if (_gameManager.CurrentPlayerIdx != 0)
                return;

            //對外廣播pass觸發事件
            OnPassRequested?.Invoke();
        }
        private void HandleRevolutionStarted(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(215, 50, 50);
        }
        private void HandleRevolutionEnded(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(40, 44, 52);
        }
    }
}
