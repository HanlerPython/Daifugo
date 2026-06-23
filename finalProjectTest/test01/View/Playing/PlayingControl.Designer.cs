namespace test01.View.Playing
{
    partial class PlayingControl
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
            this._passBtn = new System.Windows.Forms.Button();
            this._playerZonePanel = new System.Windows.Forms.Panel();
            this._deskPanel = new System.Windows.Forms.Panel();
            this._topAiHand = new test01.View.Playing.AiHandView();
            this._rightAiHand = new test01.View.Playing.AiHandView();
            this._leftAiHand = new test01.View.Playing.AiHandView();
            this._deskView = new test01.View.Playing.DeskView();
            this._hand = new test01.View.Playing.HandView();
            this._playerZonePanel.SuspendLayout();
            this._deskPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _passBtn
            // 
            this._passBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._passBtn.FlatAppearance.BorderSize = 0;
            this._passBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this._passBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this._passBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._passBtn.Font = new System.Drawing.Font("Arial", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._passBtn.ForeColor = System.Drawing.Color.White;
            this._passBtn.Location = new System.Drawing.Point(521, 3);
            this._passBtn.Name = "_passBtn";
            this._passBtn.Size = new System.Drawing.Size(127, 48);
            this._passBtn.TabIndex = 0;
            this._passBtn.Text = "Pass";
            this._passBtn.UseVisualStyleBackColor = true;
            this._passBtn.Click += new System.EventHandler(this.PassBtn_Click);
            this._passBtn.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PassBtn_MouseDown);
            this._passBtn.MouseEnter += new System.EventHandler(this.PassBtn_MouseHover);
            this._passBtn.MouseLeave += new System.EventHandler(this.PassBtn_MouseLeave);
            this._passBtn.MouseHover += new System.EventHandler(this.PassBtn_MouseHover);
            this._passBtn.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PassBtn_MouseUp);
            // 
            // _playerZonePanel
            // 
            this._playerZonePanel.Controls.Add(this._hand);
            this._playerZonePanel.Controls.Add(this._passBtn);
            this._playerZonePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._playerZonePanel.Location = new System.Drawing.Point(0, 379);
            this._playerZonePanel.Name = "_playerZonePanel";
            this._playerZonePanel.Size = new System.Drawing.Size(800, 221);
            this._playerZonePanel.TabIndex = 1;
            // 
            // _deskPanel
            // 
            this._deskPanel.Controls.Add(this._topAiHand);
            this._deskPanel.Controls.Add(this._rightAiHand);
            this._deskPanel.Controls.Add(this._leftAiHand);
            this._deskPanel.Controls.Add(this._deskView);
            this._deskPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._deskPanel.Location = new System.Drawing.Point(0, 0);
            this._deskPanel.Name = "_deskPanel";
            this._deskPanel.Size = new System.Drawing.Size(800, 379);
            this._deskPanel.TabIndex = 3;
            // 
            // _topAiHand
            // 
            this._topAiHand.Dock = System.Windows.Forms.DockStyle.Top;
            this._topAiHand.IsVertical = false;
            this._topAiHand.Location = new System.Drawing.Point(128, 0);
            this._topAiHand.Name = "_topAiHand";
            this._topAiHand.PlayerIndex = 0;
            this._topAiHand.Size = new System.Drawing.Size(555, 136);
            this._topAiHand.TabIndex = 7;
            // 
            // _rightAiHand
            // 
            this._rightAiHand.Dock = System.Windows.Forms.DockStyle.Right;
            this._rightAiHand.IsVertical = true;
            this._rightAiHand.Location = new System.Drawing.Point(683, 0);
            this._rightAiHand.Name = "_rightAiHand";
            this._rightAiHand.PlayerIndex = 0;
            this._rightAiHand.Size = new System.Drawing.Size(117, 379);
            this._rightAiHand.TabIndex = 6;
            // 
            // _leftAiHand
            // 
            this._leftAiHand.Dock = System.Windows.Forms.DockStyle.Left;
            this._leftAiHand.IsVertical = true;
            this._leftAiHand.Location = new System.Drawing.Point(0, 0);
            this._leftAiHand.Name = "_leftAiHand";
            this._leftAiHand.PlayerIndex = 0;
            this._leftAiHand.Size = new System.Drawing.Size(128, 379);
            this._leftAiHand.TabIndex = 5;
            // 
            // _deskView
            // 
            this._deskView.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._deskView.BackColor = System.Drawing.Color.Transparent;
            this._deskView.Location = new System.Drawing.Point(163, 121);
            this._deskView.Name = "_deskView";
            this._deskView.Size = new System.Drawing.Size(475, 252);
            this._deskView.TabIndex = 2;
            // 
            // _hand
            // 
            this._hand.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._hand.Cursor = System.Windows.Forms.Cursors.Default;
            this._hand.Location = new System.Drawing.Point(0, 72);
            this._hand.Name = "_hand";
            this._hand.Size = new System.Drawing.Size(800, 128);
            this._hand.TabIndex = 2;
            // 
            // PlayingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._deskPanel);
            this.Controls.Add(this._playerZonePanel);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PlayingControl";
            this.Size = new System.Drawing.Size(800, 600);
            this._playerZonePanel.ResumeLayout(false);
            this._deskPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _passBtn;
        private System.Windows.Forms.Panel _playerZonePanel;
        private HandView _hand;
        private DeskView _deskView;
        private System.Windows.Forms.Panel _deskPanel;
        private AiHandView _topAiHand;
        private AiHandView _rightAiHand;
        private AiHandView _leftAiHand;
    }
}
