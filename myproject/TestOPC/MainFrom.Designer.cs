namespace TestOPC
{
    partial class MainFrom
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblState = new System.Windows.Forms.Label();
            this.tsslServerState = new System.Windows.Forms.ToolStripStatusLabel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.txtTagValue = new System.Windows.Forms.TextBox();
            this.txtQualities = new System.Windows.Forms.TextBox();
            this.txtTimeStamps = new System.Windows.Forms.TextBox();
            this.txtGroupIsActive = new System.Windows.Forms.TextBox();
            this.txtRemoteServerIP = new System.Windows.Forms.TextBox();
            this.txtGroupDeadband = new System.Windows.Forms.TextBox();
            this.txtUpdateRate = new System.Windows.Forms.TextBox();
            this.txtIsActive = new System.Windows.Forms.TextBox();
            this.txtIsSubscribed = new System.Windows.Forms.TextBox();
            this.tsslServerStartTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslversion = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnSetGroupPro = new System.Windows.Forms.Button();
            this.txtWriteTagValue = new System.Windows.Forms.TextBox();
            this.cmbServerName = new System.Windows.Forms.ComboBox();
            this.btnConnServer = new System.Windows.Forms.Button();
            this.StatBar = new System.Windows.Forms.StatusStrip();
            this.label_txtRemoteServerIP = new System.Windows.Forms.Label();
            this.label_cmbServerName = new System.Windows.Forms.Label();
            this.label_txtGroupIsActive = new System.Windows.Forms.Label();
            this.label_txtGroupDeadband = new System.Windows.Forms.Label();
            this.label_IsActive = new System.Windows.Forms.Label();
            this.label_IsSubscribed = new System.Windows.Forms.Label();
            this.label_UpdateRate = new System.Windows.Forms.Label();
            this.label_Tag = new System.Windows.Forms.Label();
            this.label_Qualities = new System.Windows.Forms.Label();
            this.label_TimeStamps = new System.Windows.Forms.Label();
            this.btnWrite = new System.Windows.Forms.Button();
            this.groupBox_OPCServer = new System.Windows.Forms.GroupBox();
            this.groupBox_set = new System.Windows.Forms.GroupBox();
            this.groupBox_currentsize = new System.Windows.Forms.GroupBox();
            this.groupBox_write = new System.Windows.Forms.GroupBox();
            this.StatBar.SuspendLayout();
            this.groupBox_OPCServer.SuspendLayout();
            this.groupBox_set.SuspendLayout();
            this.groupBox_currentsize.SuspendLayout();
            this.groupBox_write.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblState
            // 
            this.lblState.AutoSize = true;
            this.lblState.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblState.ForeColor = System.Drawing.Color.MediumPurple;
            this.lblState.Location = new System.Drawing.Point(542, 265);
            this.lblState.Name = "lblState";
            this.lblState.Size = new System.Drawing.Size(0, 17);
            this.lblState.TabIndex = 0;
            // 
            // tsslServerState
            // 
            this.tsslServerState.BackColor = System.Drawing.SystemColors.Control;
            this.tsslServerState.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslServerState.Name = "tsslServerState";
            this.tsslServerState.Size = new System.Drawing.Size(122, 24);
            this.tsslServerState.Text = "tsslServerState";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(278, 19);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(255, 424);
            this.listBox1.TabIndex = 2;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // txtTagValue
            // 
            this.txtTagValue.Enabled = false;
            this.txtTagValue.Location = new System.Drawing.Point(76, 19);
            this.txtTagValue.Name = "txtTagValue";
            this.txtTagValue.Size = new System.Drawing.Size(162, 25);
            this.txtTagValue.TabIndex = 3;
            // 
            // txtQualities
            // 
            this.txtQualities.Enabled = false;
            this.txtQualities.Location = new System.Drawing.Point(76, 60);
            this.txtQualities.Name = "txtQualities";
            this.txtQualities.Size = new System.Drawing.Size(162, 25);
            this.txtQualities.TabIndex = 4;
            // 
            // txtTimeStamps
            // 
            this.txtTimeStamps.Enabled = false;
            this.txtTimeStamps.Location = new System.Drawing.Point(76, 99);
            this.txtTimeStamps.Name = "txtTimeStamps";
            this.txtTimeStamps.Size = new System.Drawing.Size(162, 25);
            this.txtTimeStamps.TabIndex = 5;
            // 
            // txtGroupIsActive
            // 
            this.txtGroupIsActive.Location = new System.Drawing.Point(191, 26);
            this.txtGroupIsActive.Name = "txtGroupIsActive";
            this.txtGroupIsActive.Size = new System.Drawing.Size(49, 25);
            this.txtGroupIsActive.TabIndex = 6;
            this.txtGroupIsActive.Text = "true";
            // 
            // txtRemoteServerIP
            // 
            this.txtRemoteServerIP.Location = new System.Drawing.Point(73, 24);
            this.txtRemoteServerIP.Name = "txtRemoteServerIP";
            this.txtRemoteServerIP.Size = new System.Drawing.Size(162, 25);
            this.txtRemoteServerIP.TabIndex = 7;
            this.txtRemoteServerIP.Text = "127.0.0.1";
            // 
            // txtGroupDeadband
            // 
            this.txtGroupDeadband.Location = new System.Drawing.Point(191, 64);
            this.txtGroupDeadband.Name = "txtGroupDeadband";
            this.txtGroupDeadband.Size = new System.Drawing.Size(49, 25);
            this.txtGroupDeadband.TabIndex = 8;
            this.txtGroupDeadband.Text = "0";
            // 
            // txtUpdateRate
            // 
            this.txtUpdateRate.Location = new System.Drawing.Point(191, 192);
            this.txtUpdateRate.Name = "txtUpdateRate";
            this.txtUpdateRate.Size = new System.Drawing.Size(49, 25);
            this.txtUpdateRate.TabIndex = 9;
            this.txtUpdateRate.Text = "250";
            // 
            // txtIsActive
            // 
            this.txtIsActive.Location = new System.Drawing.Point(191, 104);
            this.txtIsActive.Name = "txtIsActive";
            this.txtIsActive.Size = new System.Drawing.Size(49, 25);
            this.txtIsActive.TabIndex = 10;
            this.txtIsActive.Text = "true";
            // 
            // txtIsSubscribed
            // 
            this.txtIsSubscribed.Location = new System.Drawing.Point(191, 146);
            this.txtIsSubscribed.Name = "txtIsSubscribed";
            this.txtIsSubscribed.Size = new System.Drawing.Size(49, 25);
            this.txtIsSubscribed.TabIndex = 11;
            this.txtIsSubscribed.Text = "true";
            // 
            // tsslServerStartTime
            // 
            this.tsslServerStartTime.BackColor = System.Drawing.SystemColors.Control;
            this.tsslServerStartTime.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslServerStartTime.Name = "tsslServerStartTime";
            this.tsslServerStartTime.Size = new System.Drawing.Size(155, 24);
            this.tsslServerStartTime.Text = "tsslServerStartTime";
            // 
            // tsslversion
            // 
            this.tsslversion.BackColor = System.Drawing.SystemColors.Control;
            this.tsslversion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tsslversion.Name = "tsslversion";
            this.tsslversion.Size = new System.Drawing.Size(90, 24);
            this.tsslversion.Text = "tsslversion";
            // 
            // btnSetGroupPro
            // 
            this.btnSetGroupPro.Location = new System.Drawing.Point(155, 233);
            this.btnSetGroupPro.Name = "btnSetGroupPro";
            this.btnSetGroupPro.Size = new System.Drawing.Size(85, 30);
            this.btnSetGroupPro.TabIndex = 15;
            this.btnSetGroupPro.Text = "设置";
            this.btnSetGroupPro.UseVisualStyleBackColor = true;
            this.btnSetGroupPro.Click += new System.EventHandler(this.btnSetGroupPro_Click);
            // 
            // txtWriteTagValue
            // 
            this.txtWriteTagValue.Location = new System.Drawing.Point(10, 27);
            this.txtWriteTagValue.Name = "txtWriteTagValue";
            this.txtWriteTagValue.Size = new System.Drawing.Size(138, 25);
            this.txtWriteTagValue.TabIndex = 16;
            // 
            // cmbServerName
            // 
            this.cmbServerName.Location = new System.Drawing.Point(73, 64);
            this.cmbServerName.Name = "cmbServerName";
            this.cmbServerName.Size = new System.Drawing.Size(162, 23);
            this.cmbServerName.TabIndex = 17;
            this.cmbServerName.Text = "KEPware.KEPServerEx.V4";
            // 
            // btnConnServer
            // 
            this.btnConnServer.Location = new System.Drawing.Point(150, 103);
            this.btnConnServer.Name = "btnConnServer";
            this.btnConnServer.Size = new System.Drawing.Size(85, 30);
            this.btnConnServer.TabIndex = 18;
            this.btnConnServer.Text = "连接";
            this.btnConnServer.UseVisualStyleBackColor = true;
            this.btnConnServer.Click += new System.EventHandler(this.btnConnServer_Click);
            // 
            // StatBar
            // 
            this.StatBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslServerState,
            this.tsslServerStartTime,
            this.tsslversion});
            this.StatBar.Location = new System.Drawing.Point(0, 455);
            this.StatBar.Name = "StatBar";
            this.StatBar.ShowItemToolTips = true;
            this.StatBar.Size = new System.Drawing.Size(807, 29);
            this.StatBar.TabIndex = 1;
            // 
            // label_txtRemoteServerIP
            // 
            this.label_txtRemoteServerIP.AutoSize = true;
            this.label_txtRemoteServerIP.Location = new System.Drawing.Point(36, 27);
            this.label_txtRemoteServerIP.Name = "label_txtRemoteServerIP";
            this.label_txtRemoteServerIP.Size = new System.Drawing.Size(31, 15);
            this.label_txtRemoteServerIP.TabIndex = 19;
            this.label_txtRemoteServerIP.Text = "IP:";
            // 
            // label_cmbServerName
            // 
            this.label_cmbServerName.AutoSize = true;
            this.label_cmbServerName.Location = new System.Drawing.Point(8, 67);
            this.label_cmbServerName.Name = "label_cmbServerName";
            this.label_cmbServerName.Size = new System.Drawing.Size(67, 15);
            this.label_cmbServerName.TabIndex = 20;
            this.label_cmbServerName.Text = "服务器：";
            // 
            // label_txtGroupIsActive
            // 
            this.label_txtGroupIsActive.Location = new System.Drawing.Point(10, 29);
            this.label_txtGroupIsActive.Name = "label_txtGroupIsActive";
            this.label_txtGroupIsActive.Size = new System.Drawing.Size(175, 15);
            this.label_txtGroupIsActive.TabIndex = 21;
            this.label_txtGroupIsActive.Text = "DefaultGroupIsActive:";
            // 
            // label_txtGroupDeadband
            // 
            this.label_txtGroupDeadband.AutoSize = true;
            this.label_txtGroupDeadband.Location = new System.Drawing.Point(9, 67);
            this.label_txtGroupDeadband.Name = "label_txtGroupDeadband";
            this.label_txtGroupDeadband.Size = new System.Drawing.Size(175, 15);
            this.label_txtGroupDeadband.TabIndex = 22;
            this.label_txtGroupDeadband.Text = "DefaultGroupDeadband:";
            // 
            // label_IsActive
            // 
            this.label_IsActive.AutoSize = true;
            this.label_IsActive.Location = new System.Drawing.Point(105, 107);
            this.label_IsActive.Name = "label_IsActive";
            this.label_IsActive.Size = new System.Drawing.Size(79, 15);
            this.label_IsActive.TabIndex = 23;
            this.label_IsActive.Text = "IsActive:";
            // 
            // label_IsSubscribed
            // 
            this.label_IsSubscribed.AutoSize = true;
            this.label_IsSubscribed.Location = new System.Drawing.Point(73, 149);
            this.label_IsSubscribed.Name = "label_IsSubscribed";
            this.label_IsSubscribed.Size = new System.Drawing.Size(111, 15);
            this.label_IsSubscribed.TabIndex = 24;
            this.label_IsSubscribed.Text = "IsSubscribed:";
            // 
            // label_UpdateRate
            // 
            this.label_UpdateRate.AutoSize = true;
            this.label_UpdateRate.Location = new System.Drawing.Point(89, 195);
            this.label_UpdateRate.Name = "label_UpdateRate";
            this.label_UpdateRate.Size = new System.Drawing.Size(95, 15);
            this.label_UpdateRate.TabIndex = 25;
            this.label_UpdateRate.Text = "UpdateRate:";
            // 
            // label_Tag
            // 
            this.label_Tag.AutoSize = true;
            this.label_Tag.Location = new System.Drawing.Point(8, 22);
            this.label_Tag.Name = "label_Tag";
            this.label_Tag.Size = new System.Drawing.Size(62, 15);
            this.label_Tag.TabIndex = 26;
            this.label_Tag.Text = "Tag值: ";
            // 
            // label_Qualities
            // 
            this.label_Qualities.AutoSize = true;
            this.label_Qualities.Location = new System.Drawing.Point(18, 63);
            this.label_Qualities.Name = "label_Qualities";
            this.label_Qualities.Size = new System.Drawing.Size(52, 15);
            this.label_Qualities.TabIndex = 27;
            this.label_Qualities.Text = "品质：";
            // 
            // label_TimeStamps
            // 
            this.label_TimeStamps.AutoSize = true;
            this.label_TimeStamps.Location = new System.Drawing.Point(3, 102);
            this.label_TimeStamps.Name = "label_TimeStamps";
            this.label_TimeStamps.Size = new System.Drawing.Size(67, 15);
            this.label_TimeStamps.TabIndex = 28;
            this.label_TimeStamps.Text = "时间戳：";
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(154, 27);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(85, 23);
            this.btnWrite.TabIndex = 29;
            this.btnWrite.Text = "写入";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnWrite_Click);
            // 
            // groupBox_OPCServer
            // 
            this.groupBox_OPCServer.Controls.Add(this.btnConnServer);
            this.groupBox_OPCServer.Controls.Add(this.txtRemoteServerIP);
            this.groupBox_OPCServer.Controls.Add(this.label_txtRemoteServerIP);
            this.groupBox_OPCServer.Controls.Add(this.cmbServerName);
            this.groupBox_OPCServer.Controls.Add(this.label_cmbServerName);
            this.groupBox_OPCServer.Location = new System.Drawing.Point(12, 12);
            this.groupBox_OPCServer.Name = "groupBox_OPCServer";
            this.groupBox_OPCServer.Size = new System.Drawing.Size(256, 140);
            this.groupBox_OPCServer.TabIndex = 30;
            this.groupBox_OPCServer.TabStop = false;
            this.groupBox_OPCServer.Text = "连接OPC服务器";
            // 
            // groupBox_set
            // 
            this.groupBox_set.Controls.Add(this.txtIsSubscribed);
            this.groupBox_set.Controls.Add(this.txtGroupIsActive);
            this.groupBox_set.Controls.Add(this.txtGroupDeadband);
            this.groupBox_set.Controls.Add(this.txtUpdateRate);
            this.groupBox_set.Controls.Add(this.txtIsActive);
            this.groupBox_set.Controls.Add(this.btnSetGroupPro);
            this.groupBox_set.Controls.Add(this.label_UpdateRate);
            this.groupBox_set.Controls.Add(this.label_txtGroupIsActive);
            this.groupBox_set.Controls.Add(this.label_IsSubscribed);
            this.groupBox_set.Controls.Add(this.label_txtGroupDeadband);
            this.groupBox_set.Controls.Add(this.label_IsActive);
            this.groupBox_set.Location = new System.Drawing.Point(12, 170);
            this.groupBox_set.Name = "groupBox_set";
            this.groupBox_set.Size = new System.Drawing.Size(256, 273);
            this.groupBox_set.TabIndex = 31;
            this.groupBox_set.TabStop = false;
            this.groupBox_set.Text = "属性设置";
            // 
            // groupBox_currentsize
            // 
            this.groupBox_currentsize.Controls.Add(this.txtQualities);
            this.groupBox_currentsize.Controls.Add(this.txtTagValue);
            this.groupBox_currentsize.Controls.Add(this.txtTimeStamps);
            this.groupBox_currentsize.Controls.Add(this.label_Tag);
            this.groupBox_currentsize.Controls.Add(this.label_TimeStamps);
            this.groupBox_currentsize.Controls.Add(this.label_Qualities);
            this.groupBox_currentsize.Location = new System.Drawing.Point(545, 21);
            this.groupBox_currentsize.Name = "groupBox_currentsize";
            this.groupBox_currentsize.Size = new System.Drawing.Size(254, 139);
            this.groupBox_currentsize.TabIndex = 32;
            this.groupBox_currentsize.TabStop = false;
            this.groupBox_currentsize.Text = "当前值";
            // 
            // groupBox_write
            // 
            this.groupBox_write.Controls.Add(this.txtWriteTagValue);
            this.groupBox_write.Controls.Add(this.btnWrite);
            this.groupBox_write.Location = new System.Drawing.Point(545, 171);
            this.groupBox_write.Name = "groupBox_write";
            this.groupBox_write.Size = new System.Drawing.Size(254, 66);
            this.groupBox_write.TabIndex = 33;
            this.groupBox_write.TabStop = false;
            this.groupBox_write.Text = "写入值";
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 484);
            this.Controls.Add(this.groupBox_write);
            this.Controls.Add(this.groupBox_currentsize);
            this.Controls.Add(this.groupBox_set);
            this.Controls.Add(this.groupBox_OPCServer);
            this.Controls.Add(this.StatBar);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblState);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "碧水源OPC客户端";
            this.StatBar.ResumeLayout(false);
            this.StatBar.PerformLayout();
            this.groupBox_OPCServer.ResumeLayout(false);
            this.groupBox_OPCServer.PerformLayout();
            this.groupBox_set.ResumeLayout(false);
            this.groupBox_set.PerformLayout();
            this.groupBox_currentsize.ResumeLayout(false);
            this.groupBox_currentsize.PerformLayout();
            this.groupBox_write.ResumeLayout(false);
            this.groupBox_write.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip StatBar;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.ToolStripStatusLabel tsslServerState;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox txtTagValue;
        private System.Windows.Forms.TextBox txtQualities;
        private System.Windows.Forms.TextBox txtTimeStamps;
        private System.Windows.Forms.TextBox txtGroupIsActive;
        private System.Windows.Forms.TextBox txtRemoteServerIP;
        private System.Windows.Forms.TextBox txtGroupDeadband;
        private System.Windows.Forms.TextBox txtUpdateRate;
        private System.Windows.Forms.TextBox txtIsActive;
        private System.Windows.Forms.TextBox txtIsSubscribed;
        private System.Windows.Forms.ToolStripStatusLabel tsslServerStartTime;
        private System.Windows.Forms.ToolStripStatusLabel tsslversion;
        private System.Windows.Forms.Button btnSetGroupPro;
        private System.Windows.Forms.TextBox txtWriteTagValue;
        private System.Windows.Forms.ComboBox cmbServerName;
        private System.Windows.Forms.Button btnConnServer;
        private System.Windows.Forms.Label label_txtRemoteServerIP;
        private System.Windows.Forms.Label label_cmbServerName;
        private System.Windows.Forms.Label label_txtGroupIsActive;
        private System.Windows.Forms.Label label_txtGroupDeadband;
        private System.Windows.Forms.Label label_IsActive;
        private System.Windows.Forms.Label label_IsSubscribed;
        private System.Windows.Forms.Label label_UpdateRate;
        private System.Windows.Forms.Label label_Tag;
        private System.Windows.Forms.Label label_Qualities;
        private System.Windows.Forms.Label label_TimeStamps;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.GroupBox groupBox_OPCServer;
        private System.Windows.Forms.GroupBox groupBox_set;
        private System.Windows.Forms.GroupBox groupBox_currentsize;
        private System.Windows.Forms.GroupBox groupBox_write;
    }
}

