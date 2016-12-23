namespace SpherePacking.MainWindow
{
    partial class MainWnd
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWnd));
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnTest = new System.Windows.Forms.Button();
            this.ttShowInfo = new System.Windows.Forms.ToolTip(this.components);
            this.openFiledDlg = new System.Windows.Forms.OpenFileDialog();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiImportBallsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSaveBallsInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSaveCurrentRenderer = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiGenerateImageSlices = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSaveSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiExitSystem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiProject = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiPreference = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSolveProblem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiStopSolveProblem = new System.Windows.Forms.ToolStripMenuItem();
            this.小工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiPlayback = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiSimulateSinter = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiVisualzieSlices = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiUserGuide = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tbTestInput = new System.Windows.Forms.TextBox();
            this.rwcVolumeDisp = new Kitware.VTK.RenderWindowControl();
            this.gbStatus = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.gbStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(21, 63);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(91, 23);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "测试按钮";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(7, 20);
            this.tbStatus.Multiline = true;
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ReadOnly = true;
            this.tbStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStatus.Size = new System.Drawing.Size(213, 137);
            this.tbStatus.TabIndex = 10;
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiFile,
            this.tmiProject,
            this.小工具ToolStripMenuItem,
            this.tmiHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1062, 25);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tmiFile
            // 
            this.tmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiImportBallsInfo,
            this.tmiSaveBallsInfo,
            this.tmiSaveCurrentRenderer,
            this.tmiGenerateImageSlices,
            this.tmiSaveSettings,
            this.tmiExitSystem});
            this.tmiFile.Name = "tmiFile";
            this.tmiFile.Size = new System.Drawing.Size(44, 21);
            this.tmiFile.Text = "文件";
            // 
            // tmiImportBallsInfo
            // 
            this.tmiImportBallsInfo.AccessibleDescription = "";
            this.tmiImportBallsInfo.Name = "tmiImportBallsInfo";
            this.tmiImportBallsInfo.Size = new System.Drawing.Size(148, 22);
            this.tmiImportBallsInfo.Text = "导入小球信息";
            this.tmiImportBallsInfo.Click += new System.EventHandler(this.tmiImportBallsInfo_Click);
            // 
            // tmiSaveBallsInfo
            // 
            this.tmiSaveBallsInfo.Name = "tmiSaveBallsInfo";
            this.tmiSaveBallsInfo.Size = new System.Drawing.Size(148, 22);
            this.tmiSaveBallsInfo.Text = "保存小球信息";
            this.tmiSaveBallsInfo.Click += new System.EventHandler(this.tmiSaveBallsInfo_Click);
            // 
            // tmiSaveCurrentRenderer
            // 
            this.tmiSaveCurrentRenderer.Name = "tmiSaveCurrentRenderer";
            this.tmiSaveCurrentRenderer.Size = new System.Drawing.Size(148, 22);
            this.tmiSaveCurrentRenderer.Text = "保存当前图像";
            this.tmiSaveCurrentRenderer.Click += new System.EventHandler(this.tmiSaveCurrentRenderer_Click);
            // 
            // tmiGenerateImageSlices
            // 
            this.tmiGenerateImageSlices.Name = "tmiGenerateImageSlices";
            this.tmiGenerateImageSlices.Size = new System.Drawing.Size(148, 22);
            this.tmiGenerateImageSlices.Text = "生成切片";
            this.tmiGenerateImageSlices.Click += new System.EventHandler(this.tmiGenerateImageSlices_Click);
            // 
            // tmiSaveSettings
            // 
            this.tmiSaveSettings.Name = "tmiSaveSettings";
            this.tmiSaveSettings.Size = new System.Drawing.Size(148, 22);
            this.tmiSaveSettings.Text = "保存配置信息";
            this.tmiSaveSettings.Click += new System.EventHandler(this.tmiSaveSettings_Click);
            // 
            // tmiExitSystem
            // 
            this.tmiExitSystem.Name = "tmiExitSystem";
            this.tmiExitSystem.Size = new System.Drawing.Size(148, 22);
            this.tmiExitSystem.Text = "退出";
            this.tmiExitSystem.Click += new System.EventHandler(this.tmiExitSystem_Click);
            // 
            // tmiProject
            // 
            this.tmiProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiSettings,
            this.tmiPreference,
            this.tmiSolveProblem,
            this.tmiStopSolveProblem});
            this.tmiProject.Name = "tmiProject";
            this.tmiProject.Size = new System.Drawing.Size(44, 21);
            this.tmiProject.Text = "项目";
            // 
            // tmiSettings
            // 
            this.tmiSettings.Name = "tmiSettings";
            this.tmiSettings.Size = new System.Drawing.Size(152, 22);
            this.tmiSettings.Text = "设置";
            this.tmiSettings.Click += new System.EventHandler(this.tmiSettings_Click);
            // 
            // tmiPreference
            // 
            this.tmiPreference.Name = "tmiPreference";
            this.tmiPreference.Size = new System.Drawing.Size(152, 22);
            this.tmiPreference.Text = "偏好";
            this.tmiPreference.Click += new System.EventHandler(this.tmiPreference_Click);
            // 
            // tmiSolveProblem
            // 
            this.tmiSolveProblem.Name = "tmiSolveProblem";
            this.tmiSolveProblem.Size = new System.Drawing.Size(152, 22);
            this.tmiSolveProblem.Text = "求解";
            this.tmiSolveProblem.Click += new System.EventHandler(this.tmiSolveProblem_Click);
            // 
            // tmiStopSolveProblem
            // 
            this.tmiStopSolveProblem.Name = "tmiStopSolveProblem";
            this.tmiStopSolveProblem.Size = new System.Drawing.Size(152, 22);
            this.tmiStopSolveProblem.Text = "停止求解";
            this.tmiStopSolveProblem.Click += new System.EventHandler(this.tmiStopSolveProblem_Click);
            // 
            // 小工具ToolStripMenuItem
            // 
            this.小工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiPlayback,
            this.tmiSimulateSinter,
            this.tmiVisualzieSlices});
            this.小工具ToolStripMenuItem.Name = "小工具ToolStripMenuItem";
            this.小工具ToolStripMenuItem.Size = new System.Drawing.Size(56, 21);
            this.小工具ToolStripMenuItem.Text = "小工具";
            // 
            // tmiPlayback
            // 
            this.tmiPlayback.Name = "tmiPlayback";
            this.tmiPlayback.Size = new System.Drawing.Size(160, 22);
            this.tmiPlayback.Text = "回放数据";
            this.tmiPlayback.Click += new System.EventHandler(this.tmiPlayback_Click);
            // 
            // tmiSimulateSinter
            // 
            this.tmiSimulateSinter.Name = "tmiSimulateSinter";
            this.tmiSimulateSinter.Size = new System.Drawing.Size(160, 22);
            this.tmiSimulateSinter.Text = "烧结过程模拟";
            this.tmiSimulateSinter.Click += new System.EventHandler(this.tmiSimulateSinter_Click);
            // 
            // tmiVisualzieSlices
            // 
            this.tmiVisualzieSlices.Name = "tmiVisualzieSlices";
            this.tmiVisualzieSlices.Size = new System.Drawing.Size(160, 22);
            this.tmiVisualzieSlices.Text = "可视化切片序列";
            this.tmiVisualzieSlices.Click += new System.EventHandler(this.tmiVisualzieSlices_Click);
            // 
            // tmiHelp
            // 
            this.tmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiUserGuide,
            this.tmiAbout});
            this.tmiHelp.Name = "tmiHelp";
            this.tmiHelp.Size = new System.Drawing.Size(44, 21);
            this.tmiHelp.Text = "帮助";
            // 
            // tmiUserGuide
            // 
            this.tmiUserGuide.Name = "tmiUserGuide";
            this.tmiUserGuide.Size = new System.Drawing.Size(152, 22);
            this.tmiUserGuide.Text = "使用说明";
            // 
            // tmiAbout
            // 
            this.tmiAbout.Name = "tmiAbout";
            this.tmiAbout.Size = new System.Drawing.Size(152, 22);
            this.tmiAbout.Text = "关于";
            // 
            // tbTestInput
            // 
            this.tbTestInput.Location = new System.Drawing.Point(78, 171);
            this.tbTestInput.Name = "tbTestInput";
            this.tbTestInput.Size = new System.Drawing.Size(127, 21);
            this.tbTestInput.TabIndex = 13;
            this.tbTestInput.Text = "3";
            // 
            // rwcVolumeDisp
            // 
            this.rwcVolumeDisp.AddTestActors = false;
            this.rwcVolumeDisp.AutoScroll = true;
            this.rwcVolumeDisp.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rwcVolumeDisp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rwcVolumeDisp.Location = new System.Drawing.Point(286, 28);
            this.rwcVolumeDisp.Name = "rwcVolumeDisp";
            this.rwcVolumeDisp.Size = new System.Drawing.Size(716, 492);
            this.rwcVolumeDisp.TabIndex = 14;
            this.rwcVolumeDisp.TestText = null;
            // 
            // gbStatus
            // 
            this.gbStatus.Controls.Add(this.label1);
            this.gbStatus.Controls.Add(this.tbStatus);
            this.gbStatus.Controls.Add(this.tbTestInput);
            this.gbStatus.Location = new System.Drawing.Point(21, 112);
            this.gbStatus.Name = "gbStatus";
            this.gbStatus.Size = new System.Drawing.Size(226, 210);
            this.gbStatus.TabIndex = 15;
            this.gbStatus.TabStop = false;
            this.gbStatus.Text = "状态信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "测试输入框";
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1062, 597);
            this.Controls.Add(this.gbStatus);
            this.Controls.Add(this.rwcVolumeDisp);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainWnd";
            this.Text = "Sphere Packing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWnd_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.MainWnd_SizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbStatus.ResumeLayout(false);
            this.gbStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ToolTip ttShowInfo;
        private System.Windows.Forms.OpenFileDialog openFiledDlg;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tmiFile;
        private System.Windows.Forms.ToolStripMenuItem tmiImportBallsInfo;
        private System.Windows.Forms.ToolStripMenuItem tmiSaveBallsInfo;
        private System.Windows.Forms.ToolStripMenuItem tmiSaveCurrentRenderer;
        private System.Windows.Forms.ToolStripMenuItem tmiGenerateImageSlices;
        private System.Windows.Forms.ToolStripMenuItem tmiExitSystem;
        private System.Windows.Forms.ToolStripMenuItem tmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tmiUserGuide;
        private System.Windows.Forms.ToolStripMenuItem tmiAbout;
        private System.Windows.Forms.ToolStripMenuItem tmiProject;
        private System.Windows.Forms.ToolStripMenuItem tmiSettings;
        private System.Windows.Forms.ToolStripMenuItem tmiPreference;
        private System.Windows.Forms.ToolStripMenuItem 小工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmiPlayback;
        private System.Windows.Forms.ToolStripMenuItem tmiSimulateSinter;
        private System.Windows.Forms.ToolStripMenuItem tmiVisualzieSlices;
        private System.Windows.Forms.TextBox tbTestInput;
        private System.Windows.Forms.ToolStripMenuItem tmiSaveSettings;
        private Kitware.VTK.RenderWindowControl rwcVolumeDisp;
        private System.Windows.Forms.ToolStripMenuItem tmiSolveProblem;
        private System.Windows.Forms.ToolStripMenuItem tmiStopSolveProblem;
        private System.Windows.Forms.GroupBox gbStatus;
        private System.Windows.Forms.Label label1;
    }
}

