namespace ODClient
{
    partial class DispItem
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
            this.DispIcon = new System.Windows.Forms.PictureBox();
            this.DispQuality = new System.Windows.Forms.PictureBox();
            this.DispName = new System.Windows.Forms.Label();
            this.DispValue = new System.Windows.Forms.Label();
            this.DispUnit = new System.Windows.Forms.Label();
            this.DispLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DispIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DispQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // DispLayout
            // 
            this.DispLayout.ColumnCount = 4;
            this.DispLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DispLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.DispLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DispLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.DispLayout.Controls.Add(this.DispIcon, 0, 0);
            this.DispLayout.Controls.Add(this.DispQuality, 3, 0);
            this.DispLayout.Controls.Add(this.DispName, 1, 0);
            this.DispLayout.Controls.Add(this.DispValue, 1, 1);
            this.DispLayout.Controls.Add(this.DispUnit, 2, 1);
            this.DispLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispLayout.Location = new System.Drawing.Point(0, 0);
            this.DispLayout.Margin = new System.Windows.Forms.Padding(0);
            this.DispLayout.Name = "DispLayout";
            this.DispLayout.RowCount = 2;
            this.DispLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DispLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.DispLayout.Size = new System.Drawing.Size(200, 40);
            this.DispLayout.TabIndex = 0;
            // 
            // DispIcon
            // 
            this.DispIcon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispIcon.Image = global::ODClient.Properties.Resources.gear;
            this.DispIcon.Location = new System.Drawing.Point(0, 0);
            this.DispIcon.Margin = new System.Windows.Forms.Padding(0);
            this.DispIcon.Name = "DispIcon";
            this.DispLayout.SetRowSpan(this.DispIcon, 2);
            this.DispIcon.Size = new System.Drawing.Size(40, 40);
            this.DispIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.DispIcon.TabIndex = 0;
            this.DispIcon.TabStop = false;
            // 
            // DispQuality
            // 
            this.DispQuality.Dock = System.Windows.Forms.DockStyle.Left;
            this.DispQuality.Location = new System.Drawing.Point(190, 0);
            this.DispQuality.Margin = new System.Windows.Forms.Padding(0);
            this.DispQuality.Name = "DispQuality";
            this.DispLayout.SetRowSpan(this.DispQuality, 2);
            this.DispQuality.Size = new System.Drawing.Size(10, 40);
            this.DispQuality.TabIndex = 3;
            this.DispQuality.TabStop = false;
            this.DispQuality.Paint += new System.Windows.Forms.PaintEventHandler(this.DispQuality_Paint);
            // 
            // DispName
            // 
            this.DispName.AutoSize = true;
            this.DispLayout.SetColumnSpan(this.DispName, 2);
            this.DispName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispName.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DispName.Location = new System.Drawing.Point(43, 0);
            this.DispName.Name = "DispName";
            this.DispName.Size = new System.Drawing.Size(144, 20);
            this.DispName.TabIndex = 1;
            this.DispName.Text = "ItemName";
            this.DispName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DispValue
            // 
            this.DispValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispValue.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DispValue.Location = new System.Drawing.Point(43, 20);
            this.DispValue.Name = "DispValue";
            this.DispValue.Size = new System.Drawing.Size(91, 20);
            this.DispValue.TabIndex = 2;
            this.DispValue.Text = "ItemValue";
            this.DispValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DispUnit
            // 
            this.DispUnit.AutoSize = true;
            this.DispUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DispUnit.Location = new System.Drawing.Point(137, 20);
            this.DispUnit.Margin = new System.Windows.Forms.Padding(0);
            this.DispUnit.Name = "DispUnit";
            this.DispUnit.Size = new System.Drawing.Size(53, 20);
            this.DispUnit.TabIndex = 4;
            this.DispUnit.Text = "ItemUnit";
            this.DispUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DispItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.DispLayout);
            this.Name = "DispItem";
            this.Size = new System.Drawing.Size(200, 40);
            this.DispLayout.ResumeLayout(false);
            this.DispLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DispIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DispQuality)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel DispLayout;
        private System.Windows.Forms.PictureBox DispIcon;
        private System.Windows.Forms.Label DispName;
        private System.Windows.Forms.Label DispValue;
        private System.Windows.Forms.PictureBox DispQuality;
        private System.Windows.Forms.Label DispUnit;
    }
}
