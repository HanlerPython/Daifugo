namespace test01.View.UserControls
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
            this.PlayerZonePanel = new System.Windows.Forms.Panel();
            this._hand = new test01.View.HandView();
            this.PlayerZonePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _passBtn
            // 
            this._passBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._passBtn.Location = new System.Drawing.Point(537, 56);
            this._passBtn.Name = "_passBtn";
            this._passBtn.Size = new System.Drawing.Size(94, 39);
            this._passBtn.TabIndex = 0;
            this._passBtn.Text = "Pass";
            this._passBtn.UseVisualStyleBackColor = true;
            // 
            // PlayerZonePanel
            // 
            this.PlayerZonePanel.Controls.Add(this._hand);
            this.PlayerZonePanel.Controls.Add(this._passBtn);
            this.PlayerZonePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.PlayerZonePanel.Location = new System.Drawing.Point(0, 486);
            this.PlayerZonePanel.Name = "PlayerZonePanel";
            this.PlayerZonePanel.Size = new System.Drawing.Size(800, 194);
            this.PlayerZonePanel.TabIndex = 1;
            // 
            // _hand
            // 
            this._hand.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._hand.Location = new System.Drawing.Point(0, 101);
            this._hand.Name = "_hand";
            this._hand.Size = new System.Drawing.Size(800, 93);
            this._hand.TabIndex = 2;
            // 
            // PlayingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PlayerZonePanel);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PlayingControl";
            this.Size = new System.Drawing.Size(800, 680);
            this.PlayerZonePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _passBtn;
        private System.Windows.Forms.Panel PlayerZonePanel;
        private HandView _hand;
    }
}
