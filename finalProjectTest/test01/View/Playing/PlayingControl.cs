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
using test01.Utils;

namespace test01.View.Playing
{
    public partial class PlayingControl : UserControl
    {
        public HandView Hand => this._hand;
        public DeskView Desk => this._deskView;
        public AiHandView TopAiHand => this._topAiHand;
        public AiHandView LeftAiHand => this._leftAiHand;
        public AiHandView RightAiHand => this._rightAiHand;
        private GameManager _gameManager;

        public event EventHandler OnPlayerPass;
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
            this._leftAiHand.PlayerIndex = 1;  
            this._leftAiHand.Initialize(gameManager);
            this._topAiHand.PlayerIndex = 2;
            this._topAiHand.Initialize(gameManager);
            this._rightAiHand.PlayerIndex = 3; 
            this._rightAiHand.Initialize(gameManager);

            _passBtn.BackgroundImage = ResourceManager.GetButtonImage(0);
            _passBtn.BackgroundImageLayout = ImageLayout.Stretch;
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
            OnPlayerPass?.Invoke(this, EventArgs.Empty);
        }
        private void PassBtn_MouseHover(object sender, EventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(1);
        }
        private void PassBtn_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(0);
        }
        private void PassBtn_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(2);
        }
        private void PassBtn_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(1);
        }
        private void HandleRevolutionStarted(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(215, 50, 50);
        }
        private void HandleRevolutionEnded(object sender, EventArgs e)
        {
            this.BackColor = Color.FromArgb(42, 168, 67);
        }
    }
}
