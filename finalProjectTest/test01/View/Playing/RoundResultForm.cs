using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using test01.Utils;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test01.View.Playing
{
    public partial class RoundResultForm : Form
    {
        private Button _btnClose;
        private TableLayoutPanel _tableLayout;
        private Label _lblTitle;

        public RoundResultForm(string titleText, string[] results, int[] playerIds)
        {
            InitializeComponent();
            Initialize(titleText, results, playerIds); // 傳遞下去
        }

        private void Initialize(string titleText, string[] results, int[] playerIds)
        {
            this.SuspendLayout();
            this.DoubleBuffered = true;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;

            this.BackgroundImage = Utils.ResourceManager.GetCreditsBackImage();
            this.BackgroundImageLayout = ImageLayout.Stretch;

            this.Size = new Size(650, 700);

            //關閉按鈕
            _btnClose = new Button
            {
                Text = "X",
                Size = new Size(30, 30)
            };
            _btnClose.Location = new Point(this.Width - _btnClose.Width - 5, 5);
            _btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnClose.BackColor = Color.Firebrick;
            _btnClose.ForeColor = Color.White;
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.Font = new Font("Arial", 12F, FontStyle.Bold);
            _btnClose.TextAlign = ContentAlignment.MiddleCenter;
            _btnClose.Cursor = Cursors.Hand;

            //關閉視窗
            _btnClose.Click += (s, e) => { this.DialogResult = DialogResult.OK; this.Close(); };

            //排版容器
            _tableLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = Color.Transparent
            };

            _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));
            _tableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));

            for (int i = 0; i < results.Length; i++)
            {
                _tableLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            //頂部標題
            _lblTitle = new Label
            {
                Text = titleText,
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 20F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 175,
                TextAlign = ContentAlignment.MiddleCenter
            };

            //每一名的結果標籤與圖片
            for (int i = 0; i < results.Length; i++)
            {
                // 取得對應圖片 (i 為名次，playerIds[i] 為該名次玩家的 ID)
                Image frameImg = ResourceManager.GetPlayerFrameImage(i);
                Image avatarImg = ResourceManager.GetPlayerAvatarImage(playerIds[i]);

                PictureBox picRank = new()
                {
                    Height = 100,
                    Width = 80,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    BackColor = Color.Transparent,
                    Margin = new Padding(0),
                    // 使用合成函式生成最終圖片
                    Image = CombineAvatarAndFrame(frameImg, avatarImg)
                };

                Label lblResult = new()
                {
                    Text = results[i],
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Font = new Font("微軟正黑體", 25F, FontStyle.Bold),
                    Dock = DockStyle.Top,
                    Height = 100,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Margin = new Padding(0)
                };

                _tableLayout.Controls.Add(picRank, 0, i);
                _tableLayout.Controls.Add(lblResult, 1, i);
            }

            this.Controls.Add(_tableLayout);
            this.Controls.Add(_lblTitle);
            this.Controls.Add(_btnClose);

            //確保圖層順序正確
            _lblTitle.SendToBack();
            _tableLayout.BringToFront();
            _btnClose.BringToFront();

            this.ResumeLayout(false);
        }
        private Image CombineAvatarAndFrame(Image frame, Image avatar)
        {
            if (frame == null || avatar == null) return frame ?? avatar;

            // 以邊框尺寸為基準建立畫布
            Bitmap bmp = new(frame.Width, frame.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 啟用高品質平滑渲染
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // 1. 繪製底層邊框
                g.DrawImage(frame, 0, 0, frame.Width, frame.Height);

                // 2. 計算頭像的縮放比例與偏移量 (此處設定頭像佔邊框的 75%)
                float scale = 0.75f;
                int avatarWidth = (int)(frame.Width * scale);
                int avatarHeight = (int)(frame.Height * scale);

                // 計算置中座標
                int offsetX = (frame.Width - avatarWidth) / 2;
                int offsetY = (frame.Height - avatarHeight) / 2;

                // 3. 繪製上層頭像
                g.DrawImage(avatar, offsetX, offsetY, avatarWidth, avatarHeight);
            }
            return bmp;
        }
    }
}
