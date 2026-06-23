using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Controller.States.AppStates;
using test01.Utils;
using test01.View.MainMenu;

namespace test01.View.Playing
{
    public partial class MainMenuControl : UserControl
    {
        public int SelectedDifficulty { get; private set; } // 0=普通, 1=宇宙無敵巨激超難, 預設普通
        private readonly CreditsControl _credits; //製作人員名單

        public event EventHandler OnStartGame;

        public MainMenuControl()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill; //讓UserControl填滿父容器
            this.BackColor = Color.FromArgb(102, 24, 24); //設定背景色

            //設定按鈕背景圖片
            _btnStart.BackgroundImage = ResourceManager.GetButtonImage(0);
            _btnStart.BackgroundImageLayout = ImageLayout.Stretch;  // 圖片填滿按鈕
            _btnDifficulty.BackgroundImage = ResourceManager.GetButtonImage(0);
            _btnDifficulty.BackgroundImageLayout = ImageLayout.Stretch;
            _btnExit.BackgroundImage = ResourceManager.GetButtonImage(0);
            _btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            _btnCredits.BackgroundImage = ResourceManager.GetButtonImage(0);
            _btnCredits.BackgroundImageLayout = ImageLayout.Stretch;

            //設定標題圖片
            _titlePic.Image = ResourceManager.GetTitleImage();

            _credits = new()
            {
                Visible = false
            };
            this.Controls.Add(_credits);
            _credits.Location = new Point((this.Width - _credits.Width) / 2, (this.Height - _credits.Height) / 2 + 100);
            _credits.Anchor = AnchorStyles.None;
            _credits.BringToFront();

            SelectedDifficulty = 0;
        }

        private void Btn_Start_Click(object sender, EventArgs e)
        {
            OnStartGame?.Invoke(this, EventArgs.Empty);
        }
        private void Btn_Difficulty_Click(object sender, EventArgs e)
        {
            SelectedDifficulty = (SelectedDifficulty + 1) % 2;
            _btnDifficulty.Text = SelectedDifficulty == 0 ? "簡單" : "宇宙無敵巨激超難";
        }
        private void Btn_Credits_Click(object sender, EventArgs e)
        {
            _credits.Visible = !_credits.Visible;
        }
        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Button_MouseHover(object sender, EventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(1);
        }
        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(0);
        }
        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(2);
        }
        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Control btn)
                btn.BackgroundImage = ResourceManager.GetButtonImage(1);
        }
    }
}
