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
            this._deskView = new test01.View.Playing.DeskView();
            this._hand = new test01.View.Playing.HandView();
            this._playerZonePanel.SuspendLayout();
            this._deskPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _passBtn
            // 
            this._passBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._passBtn.Location = new System.Drawing.Point(575, 3);
            this._passBtn.Name = "_passBtn";
            this._passBtn.Size = new System.Drawing.Size(94, 39);
            this._passBtn.TabIndex = 0;
            this._passBtn.Text = "Pass";
            this._passBtn.UseVisualStyleBackColor = true;
            // 
            // _playerZonePanel
            // 
            this._playerZonePanel.Controls.Add(this._hand);
            this._playerZonePanel.Controls.Add(this._passBtn);
            this._playerZonePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._playerZonePanel.Location = new System.Drawing.Point(0, 461);
            this._playerZonePanel.Name = "_playerZonePanel";
            this._playerZonePanel.Size = new System.Drawing.Size(800, 139);
            this._playerZonePanel.TabIndex = 1;
            // 
            // _deskPanel
            // 
            this._deskPanel.Controls.Add(this._deskView);
            this._deskPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._deskPanel.Location = new System.Drawing.Point(0, 0);
            this._deskPanel.Name = "_deskPanel";
            this._deskPanel.Size = new System.Drawing.Size(800, 461);
            this._deskPanel.TabIndex = 3;
            // 
            // _deskView
            // 
            this._deskView.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this._deskView.BackColor = System.Drawing.Color.Transparent;
            this._deskView.Location = new System.Drawing.Point(163, 160);
            this._deskView.Name = "_deskView";
            this._deskView.Size = new System.Drawing.Size(475, 295);
            this._deskView.TabIndex = 2;
            // 
            // _hand
            // 
            this._hand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._hand.Location = new System.Drawing.Point(0, 46);
            this._hand.Name = "_hand";
            this._hand.Size = new System.Drawing.Size(800, 93);
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
    }
}
