namespace test01.View.Playing
{
    partial class MainMenuControl
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this._btnStart = new System.Windows.Forms.Button();
            this._btnDifficulty = new System.Windows.Forms.Button();
            this._btnExit = new System.Windows.Forms.Button();
            this._btnCredits = new System.Windows.Forms.Button();
            this._titlePic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._titlePic)).BeginInit();
            this.SuspendLayout();
            // 
            // _btnStart
            // 
            this._btnStart.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._btnStart.FlatAppearance.BorderSize = 0;
            this._btnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._btnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnStart.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this._btnStart.ForeColor = System.Drawing.Color.White;
            this._btnStart.Location = new System.Drawing.Point(447, 351);
            this._btnStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(204, 57);
            this._btnStart.TabIndex = 0;
            this._btnStart.Text = "開始";
            this._btnStart.UseVisualStyleBackColor = false;
            this._btnStart.Click += new System.EventHandler(this.Btn_Start_Click);
            this._btnStart.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this._btnStart.MouseEnter += new System.EventHandler(this.Button_MouseHover);
            this._btnStart.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this._btnStart.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this._btnStart.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // _btnDifficulty
            // 
            this._btnDifficulty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._btnDifficulty.Cursor = System.Windows.Forms.Cursors.Default;
            this._btnDifficulty.FlatAppearance.BorderSize = 0;
            this._btnDifficulty.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._btnDifficulty.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnDifficulty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnDifficulty.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this._btnDifficulty.ForeColor = System.Drawing.Color.White;
            this._btnDifficulty.Location = new System.Drawing.Point(447, 448);
            this._btnDifficulty.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._btnDifficulty.Name = "_btnDifficulty";
            this._btnDifficulty.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._btnDifficulty.Size = new System.Drawing.Size(204, 57);
            this._btnDifficulty.TabIndex = 2;
            this._btnDifficulty.Text = "簡單";
            this._btnDifficulty.UseVisualStyleBackColor = false;
            this._btnDifficulty.Click += new System.EventHandler(this.Btn_Difficulty_Click);
            this._btnDifficulty.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this._btnDifficulty.MouseEnter += new System.EventHandler(this.Button_MouseHover);
            this._btnDifficulty.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this._btnDifficulty.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this._btnDifficulty.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // _btnExit
            // 
            this._btnExit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._btnExit.Cursor = System.Windows.Forms.Cursors.Default;
            this._btnExit.FlatAppearance.BorderSize = 0;
            this._btnExit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._btnExit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnExit.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this._btnExit.ForeColor = System.Drawing.Color.White;
            this._btnExit.Location = new System.Drawing.Point(447, 642);
            this._btnExit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._btnExit.Name = "_btnExit";
            this._btnExit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._btnExit.Size = new System.Drawing.Size(204, 57);
            this._btnExit.TabIndex = 3;
            this._btnExit.Text = "離開";
            this._btnExit.UseVisualStyleBackColor = false;
            this._btnExit.Click += new System.EventHandler(this.Btn_Exit_Click);
            this._btnExit.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this._btnExit.MouseEnter += new System.EventHandler(this.Button_MouseHover);
            this._btnExit.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this._btnExit.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this._btnExit.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // _btnCredits
            // 
            this._btnCredits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._btnCredits.Cursor = System.Windows.Forms.Cursors.Default;
            this._btnCredits.FlatAppearance.BorderSize = 0;
            this._btnCredits.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._btnCredits.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._btnCredits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._btnCredits.Font = new System.Drawing.Font("微軟正黑體", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this._btnCredits.ForeColor = System.Drawing.Color.White;
            this._btnCredits.Location = new System.Drawing.Point(447, 545);
            this._btnCredits.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this._btnCredits.Name = "_btnCredits";
            this._btnCredits.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._btnCredits.Size = new System.Drawing.Size(204, 57);
            this._btnCredits.TabIndex = 4;
            this._btnCredits.Text = "開發人員名單";
            this._btnCredits.UseVisualStyleBackColor = false;
            this._btnCredits.Click += new System.EventHandler(this.Btn_Credits_Click);
            this._btnCredits.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Button_MouseDown);
            this._btnCredits.MouseEnter += new System.EventHandler(this.Button_MouseHover);
            this._btnCredits.MouseLeave += new System.EventHandler(this.Button_MouseLeave);
            this._btnCredits.MouseHover += new System.EventHandler(this.Button_MouseHover);
            this._btnCredits.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Button_MouseUp);
            // 
            // _titlePic
            // 
            this._titlePic.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this._titlePic.Location = new System.Drawing.Point(177, 16);
            this._titlePic.Name = "_titlePic";
            this._titlePic.Size = new System.Drawing.Size(726, 304);
            this._titlePic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._titlePic.TabIndex = 5;
            this._titlePic.TabStop = false;
            // 
            // MainMenuControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._titlePic);
            this.Controls.Add(this._btnCredits);
            this.Controls.Add(this._btnExit);
            this.Controls.Add(this._btnDifficulty);
            this.Controls.Add(this._btnStart);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainMenuControl";
            this.Size = new System.Drawing.Size(1080, 720);
            ((System.ComponentModel.ISupportInitialize)(this._titlePic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.Button _btnDifficulty;
        private System.Windows.Forms.Button _btnExit;
        private System.Windows.Forms.Button _btnCredits;
        private System.Windows.Forms.PictureBox _titlePic;
    }
}
