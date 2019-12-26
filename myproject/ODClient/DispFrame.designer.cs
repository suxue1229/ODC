namespace ODClient
{
    partial class DispFrame
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.DispLayout = new System.Windows.Forms.TableLayoutPanel();
            this.DispPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DispTitle = new System.Windows.Forms.Panel();
            this.TitleLayout = new System.Windows.Forms.TableLayoutPanel();
            this.DispShow = new System.Windows.Forms.Button();
            this.DispLabel = new System.Windows.Forms.Label();
            this.DispBanner = new System.Windows.Forms.PictureBox();
            this.DispLayout.SuspendLayout();
            this.DispTitle.SuspendLayout();
            this.TitleLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DispBanner)).BeginInit();
            this.SuspendLayout();
            // 
            // DispLayout
            // 
            this.DispLayout.AutoSize = true;
            this.DispLayout.ColumnCount = 1;
            this.DispLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DispLayout.Controls.Add(this.DispPanel, 0, 1);
            this.DispLayout.Controls.Add(this.DispTitle, 0, 0);
            this.DispLayout.Dock = System.Windows.Forms.DockStyle.Top;
            this.DispLayout.Location = new System.Drawing.Point(0, 0);
            this.DispLayout.Margin = new System.Windows.Forms.Padding(0);
            this.DispLayout.Name = "DispLayout";
            this.DispLayout.RowCount = 2;
            this.DispLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.DispLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.DispLayout.Size = new System.Drawing.Size(480, 22);
            this.DispLayout.TabIndex = 1;
            this.DispLayout.SizeChanged += new System.EventHandler(this.DispLayout_SizeChanged);
            // 
            // DispPanel
            // 
            this.DispPanel.AutoSize = true;
            this.DispPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.DispPanel.Location = new System.Drawing.Point(0, 22);
            this.DispPanel.Margin = new System.Windows.Forms.Padding(0);
            this.DispPanel.Name = "DispPanel";
            this.DispPanel.Size = new System.Drawing.Size(480, 0);
            this.DispPanel.TabIndex = 3;
            // 
            // DispTitle
            // 
            this.DispTitle.Controls.Add(this.TitleLayout);
            this.DispTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispTitle.Location = new System.Drawing.Point(0, 0);
            this.DispTitle.Margin = new System.Windows.Forms.Padding(0);
            this.DispTitle.Name = "DispTitle";
            this.DispTitle.Size = new System.Drawing.Size(480, 22);
            this.DispTitle.TabIndex = 0;
            // 
            // TitleLayout
            // 
            this.TitleLayout.ColumnCount = 3;
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TitleLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TitleLayout.Controls.Add(this.DispShow, 0, 0);
            this.TitleLayout.Controls.Add(this.DispLabel, 1, 0);
            this.TitleLayout.Controls.Add(this.DispBanner, 2, 0);
            this.TitleLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TitleLayout.Location = new System.Drawing.Point(0, 0);
            this.TitleLayout.Margin = new System.Windows.Forms.Padding(0);
            this.TitleLayout.Name = "TitleLayout";
            this.TitleLayout.RowCount = 1;
            this.TitleLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TitleLayout.Size = new System.Drawing.Size(480, 22);
            this.TitleLayout.TabIndex = 0;
            // 
            // DispShow
            // 
            this.DispShow.Dock = System.Windows.Forms.DockStyle.Left;
            this.DispShow.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control;
            this.DispShow.FlatAppearance.BorderSize = 0;
            this.DispShow.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.Control;
            this.DispShow.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Control;
            this.DispShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.DispShow.Location = new System.Drawing.Point(0, 0);
            this.DispShow.Margin = new System.Windows.Forms.Padding(0);
            this.DispShow.Name = "DispShow";
            this.DispShow.Size = new System.Drawing.Size(22, 22);
            this.DispShow.TabIndex = 0;
            this.DispShow.UseVisualStyleBackColor = true;
            this.DispShow.Paint += new System.Windows.Forms.PaintEventHandler(this.DispShow_Paint);
            this.DispShow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DispShow_MouseClick);
            this.DispShow.MouseEnter += new System.EventHandler(this.DispShow_MouseEnter);
            this.DispShow.MouseLeave += new System.EventHandler(this.DispShow_MouseLeave);
            // 
            // DispLabel
            // 
            this.DispLabel.AutoSize = true;
            this.DispLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DispLabel.Location = new System.Drawing.Point(22, 0);
            this.DispLabel.Margin = new System.Windows.Forms.Padding(0);
            this.DispLabel.Name = "DispLabel";
            this.DispLabel.Size = new System.Drawing.Size(91, 22);
            this.DispLabel.TabIndex = 1;
            this.DispLabel.Text = "DispFrame(0)";
            this.DispLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DispLabel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DispShow_MouseClick);
            // 
            // DispBanner
            // 
            this.DispBanner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispBanner.Location = new System.Drawing.Point(113, 0);
            this.DispBanner.Margin = new System.Windows.Forms.Padding(0);
            this.DispBanner.Name = "DispBanner";
            this.DispBanner.Size = new System.Drawing.Size(367, 22);
            this.DispBanner.TabIndex = 2;
            this.DispBanner.TabStop = false;
            this.DispBanner.Paint += new System.Windows.Forms.PaintEventHandler(this.DispBanner_Paint);
            this.DispBanner.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DispShow_MouseClick);
            // 
            // DispFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.DispLayout);
            this.Name = "DispFrame";
            this.Size = new System.Drawing.Size(480, 100);
            this.Load += new System.EventHandler(this.DispFrame_Load);
            this.SizeChanged += new System.EventHandler(this.DispFrame_SizeChanged);
            this.DispLayout.ResumeLayout(false);
            this.DispLayout.PerformLayout();
            this.DispTitle.ResumeLayout(false);
            this.TitleLayout.ResumeLayout(false);
            this.TitleLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DispBanner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel DispLayout;
        private System.Windows.Forms.Button DispShow;
        private System.Windows.Forms.Label DispLabel;
        private System.Windows.Forms.PictureBox DispBanner;
        private System.Windows.Forms.FlowLayoutPanel DispPanel;
        private System.Windows.Forms.Panel DispTitle;
        private System.Windows.Forms.TableLayoutPanel TitleLayout;
    }
}
