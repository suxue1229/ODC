namespace ODClient
{
    partial class ODClient
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ODClient));
            this.MainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.StatBar = new System.Windows.Forms.StatusStrip();
            this.StatOPC = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatNET = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainLayout
            // 
            this.MainLayout.AutoSize = true;
            this.MainLayout.ColumnCount = 1;
            this.MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.MainLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainLayout.Location = new System.Drawing.Point(0, 0);
            this.MainLayout.Name = "MainLayout";
            this.MainLayout.RowCount = 1;
            this.MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.MainLayout.Size = new System.Drawing.Size(784, 20);
            this.MainLayout.TabIndex = 0;
            // 
            // StatBar
            // 
            this.StatBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatOPC,
            this.StatNET});
            this.StatBar.Location = new System.Drawing.Point(0, 416);
            this.StatBar.Name = "StatBar";
            this.StatBar.ShowItemToolTips = true;
            this.StatBar.Size = new System.Drawing.Size(784, 26);
            this.StatBar.TabIndex = 1;
            // 
            // StatOPC
            // 
            this.StatOPC.BackColor = System.Drawing.SystemColors.Control;
            this.StatOPC.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatOPC.Name = "StatOPC";
            this.StatOPC.Size = new System.Drawing.Size(117, 21);
            this.StatOPC.Text = "OPC Server Status";
            // 
            // StatNET
            // 
            this.StatNET.BackColor = System.Drawing.SystemColors.Control;
            this.StatNET.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.StatNET.Name = "StatNET";
            this.StatNET.Size = new System.Drawing.Size(101, 21);
            this.StatNET.Text = "Network Status";
            // 
            // ODClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(784, 442);
            this.Controls.Add(this.StatBar);
            this.Controls.Add(this.MainLayout);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ODClient";
            this.Text = "污水信息化平台客户端";
            this.StatBar.ResumeLayout(false);
            this.StatBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel MainLayout;
        private System.Windows.Forms.StatusStrip StatBar;
        private System.Windows.Forms.ToolStripStatusLabel StatOPC;
        private System.Windows.Forms.ToolStripStatusLabel StatNET;
    }
}

