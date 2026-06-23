using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using test01.Utils;

namespace test01.View.MainMenu
{
    public partial class CreditsControl : UserControl
    {
        private Button _btnClose;
        private TableLayoutPanel _tableLayout;
        private Label _lblTitle;

        public CreditsControl()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            this.SuspendLayout();
            this.DoubleBuffered = true; //避免圖片閃爍
            this.BackgroundImage = ResourceManager.GetCreditsBackImage();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Size = new Size(400, 380);
            this.BorderStyle = BorderStyle.FixedSingle;

            //關閉按鈕
            _btnClose = new Button
            {
                Text = "X",
                Size = new Size(30, 30)
            };
            _btnClose.Location = new Point(this.Width - _btnClose.Width - 5, 5); //右上角偏移 5px
            _btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right; //固定在右上
            _btnClose.BackColor = Color.Firebrick;
            _btnClose.ForeColor = Color.White;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.FlatAppearance.BorderSize = 0;//移除邊框
            _btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            _btnClose.TextAlign = ContentAlignment.MiddleCenter;
            _btnClose.Cursor = Cursors.Hand;

            //點擊隱藏此選單
            _btnClose.Click += (s, e) => this.Hide();

            //使用TableLayoutPanel達成垂直置中
            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 4; i++)
            {
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            //頂部標題設定
            _lblTitle = new Label
            {
                Text = "開發人員",
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 30F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 120,
                TextAlign = ContentAlignment.MiddleCenter
            };

            //4個開發者名稱
            string[] developers = { "何政輝", "王昱棋", "廖宏閔", "李翰俊" };
            for (int i = 0; i < 4; i++)
            {
                Label lblName = new()
                {
                    Text = developers[i],
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Font = new Font("微軟正黑體", 30F, FontStyle.Bold),
                    Dock = DockStyle.Top,
                    Height = 50,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                //加入至對應的列
                _tableLayout.Controls.Add(lblName, 0, i);
            }

            this.Controls.Add(_tableLayout);
            this.Controls.Add(_lblTitle);
            this.Controls.Add(_btnClose);

            //確保按鈕在最上層，不會被遮擋點擊判定
            _lblTitle.SendToBack();
            _tableLayout.BringToFront();
            _btnClose.BringToFront();

            this.ResumeLayout(false);
        }
    }
}
