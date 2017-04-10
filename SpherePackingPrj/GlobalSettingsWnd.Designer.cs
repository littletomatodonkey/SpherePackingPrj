namespace SpherePacking.MainWindow
{
    partial class GlobalSettingsWnd
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
            this.gbDirSettings = new System.Windows.Forms.GroupBox();
            this.btnLogFolder = new System.Windows.Forms.Button();
            this.tbLogDir = new System.Windows.Forms.TextBox();
            this.btnResultsFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbResultsDir = new System.Windows.Forms.TextBox();
            this.gbModelInfo = new System.Windows.Forms.GroupBox();
            this.tbResolution = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbStepLength = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cbParticleSizeType = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tbZRate = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbBallsNumber = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbIterNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbParaCompute = new System.Windows.Forms.CheckBox();
            this.cbVisualize = new System.Windows.Forms.CheckBox();
            this.tbHeight = new System.Windows.Forms.TextBox();
            this.tbRadius = new System.Windows.Forms.TextBox();
            this.tbCubeLength = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbBoundType = new System.Windows.Forms.ComboBox();
            this.btnSaveInfo = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbDirSettings.SuspendLayout();
            this.gbModelInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDirSettings
            // 
            this.gbDirSettings.Controls.Add(this.btnLogFolder);
            this.gbDirSettings.Controls.Add(this.tbLogDir);
            this.gbDirSettings.Controls.Add(this.btnResultsFolder);
            this.gbDirSettings.Controls.Add(this.label2);
            this.gbDirSettings.Controls.Add(this.label1);
            this.gbDirSettings.Controls.Add(this.tbResultsDir);
            this.gbDirSettings.Location = new System.Drawing.Point(12, 201);
            this.gbDirSettings.Name = "gbDirSettings";
            this.gbDirSettings.Size = new System.Drawing.Size(540, 106);
            this.gbDirSettings.TabIndex = 0;
            this.gbDirSettings.TabStop = false;
            this.gbDirSettings.Text = "文件夹设置";
            // 
            // btnLogFolder
            // 
            this.btnLogFolder.Location = new System.Drawing.Point(345, 64);
            this.btnLogFolder.Name = "btnLogFolder";
            this.btnLogFolder.Size = new System.Drawing.Size(31, 21);
            this.btnLogFolder.TabIndex = 8;
            this.btnLogFolder.Text = "...";
            this.btnLogFolder.UseVisualStyleBackColor = true;
            this.btnLogFolder.Click += new System.EventHandler(this.btnLogFolder_Click);
            // 
            // tbLogDir
            // 
            this.tbLogDir.Location = new System.Drawing.Point(134, 64);
            this.tbLogDir.Name = "tbLogDir";
            this.tbLogDir.ReadOnly = true;
            this.tbLogDir.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbLogDir.Size = new System.Drawing.Size(213, 21);
            this.tbLogDir.TabIndex = 7;
            // 
            // btnResultsFolder
            // 
            this.btnResultsFolder.Location = new System.Drawing.Point(345, 28);
            this.btnResultsFolder.Name = "btnResultsFolder";
            this.btnResultsFolder.Size = new System.Drawing.Size(31, 21);
            this.btnResultsFolder.TabIndex = 6;
            this.btnResultsFolder.Text = "...";
            this.btnResultsFolder.UseVisualStyleBackColor = true;
            this.btnResultsFolder.Click += new System.EventHandler(this.btnResultsFolder_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(23, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 14);
            this.label2.TabIndex = 3;
            this.label2.Text = "日志存放文件夹";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(23, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "运行结果文件夹";
            // 
            // tbResultsDir
            // 
            this.tbResultsDir.Location = new System.Drawing.Point(134, 28);
            this.tbResultsDir.Name = "tbResultsDir";
            this.tbResultsDir.ReadOnly = true;
            this.tbResultsDir.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbResultsDir.Size = new System.Drawing.Size(213, 21);
            this.tbResultsDir.TabIndex = 0;
            // 
            // gbModelInfo
            // 
            this.gbModelInfo.Controls.Add(this.tbResolution);
            this.gbModelInfo.Controls.Add(this.label13);
            this.gbModelInfo.Controls.Add(this.label12);
            this.gbModelInfo.Controls.Add(this.tbStepLength);
            this.gbModelInfo.Controls.Add(this.label11);
            this.gbModelInfo.Controls.Add(this.cbParticleSizeType);
            this.gbModelInfo.Controls.Add(this.label10);
            this.gbModelInfo.Controls.Add(this.tbZRate);
            this.gbModelInfo.Controls.Add(this.label9);
            this.gbModelInfo.Controls.Add(this.tbBallsNumber);
            this.gbModelInfo.Controls.Add(this.label8);
            this.gbModelInfo.Controls.Add(this.tbIterNumber);
            this.gbModelInfo.Controls.Add(this.label7);
            this.gbModelInfo.Controls.Add(this.cbParaCompute);
            this.gbModelInfo.Controls.Add(this.cbVisualize);
            this.gbModelInfo.Controls.Add(this.tbHeight);
            this.gbModelInfo.Controls.Add(this.tbRadius);
            this.gbModelInfo.Controls.Add(this.tbCubeLength);
            this.gbModelInfo.Controls.Add(this.label6);
            this.gbModelInfo.Controls.Add(this.label5);
            this.gbModelInfo.Controls.Add(this.label4);
            this.gbModelInfo.Controls.Add(this.label3);
            this.gbModelInfo.Controls.Add(this.cbBoundType);
            this.gbModelInfo.Location = new System.Drawing.Point(12, 25);
            this.gbModelInfo.Name = "gbModelInfo";
            this.gbModelInfo.Size = new System.Drawing.Size(540, 157);
            this.gbModelInfo.TabIndex = 1;
            this.gbModelInfo.TabStop = false;
            this.gbModelInfo.Text = "模型信息设置";
            // 
            // tbResolution
            // 
            this.tbResolution.Location = new System.Drawing.Point(87, 127);
            this.tbResolution.Name = "tbResolution";
            this.tbResolution.ReadOnly = true;
            this.tbResolution.Size = new System.Drawing.Size(146, 21);
            this.tbResolution.TabIndex = 30;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(233, 134);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(287, 14);
            this.label13.TabIndex = 29;
            this.label13.Text = "将半径与位置乘以该值得到的结果以um为单位";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(18, 128);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 14);
            this.label12.TabIndex = 29;
            this.label12.Text = "小球精度";
            // 
            // tbStepLength
            // 
            this.tbStepLength.Location = new System.Drawing.Point(406, 101);
            this.tbStepLength.Name = "tbStepLength";
            this.tbStepLength.Size = new System.Drawing.Size(72, 21);
            this.tbStepLength.TabIndex = 28;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(328, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 14);
            this.label11.TabIndex = 27;
            this.label11.Text = "步长";
            // 
            // cbParticleSizeType
            // 
            this.cbParticleSizeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParticleSizeType.FormattingEnabled = true;
            this.cbParticleSizeType.Items.AddRange(new object[] {
            "30~50um",
            "50~70um",
            "70~100um"});
            this.cbParticleSizeType.Location = new System.Drawing.Point(118, 102);
            this.cbParticleSizeType.Name = "cbParticleSizeType";
            this.cbParticleSizeType.Size = new System.Drawing.Size(189, 20);
            this.cbParticleSizeType.TabIndex = 26;
            this.cbParticleSizeType.SelectedIndexChanged += new System.EventHandler(this.cbParticleSizeType_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(20, 102);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 14);
            this.label10.TabIndex = 25;
            this.label10.Text = "小球粒径类型";
            // 
            // tbZRate
            // 
            this.tbZRate.Location = new System.Drawing.Point(406, 19);
            this.tbZRate.Name = "tbZRate";
            this.tbZRate.Size = new System.Drawing.Size(72, 21);
            this.tbZRate.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(328, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 14);
            this.label9.TabIndex = 23;
            this.label9.Text = "Z方向比例";
            // 
            // tbBallsNumber
            // 
            this.tbBallsNumber.Location = new System.Drawing.Point(245, 19);
            this.tbBallsNumber.Name = "tbBallsNumber";
            this.tbBallsNumber.ReadOnly = true;
            this.tbBallsNumber.Size = new System.Drawing.Size(62, 21);
            this.tbBallsNumber.TabIndex = 22;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(170, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 14);
            this.label8.TabIndex = 21;
            this.label8.Text = "小球个数";
            // 
            // tbIterNumber
            // 
            this.tbIterNumber.Location = new System.Drawing.Point(87, 74);
            this.tbIterNumber.Name = "tbIterNumber";
            this.tbIterNumber.Size = new System.Drawing.Size(61, 21);
            this.tbIterNumber.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(18, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 19;
            this.label7.Text = "迭代次数";
            // 
            // cbParaCompute
            // 
            this.cbParaCompute.AutoSize = true;
            this.cbParaCompute.Location = new System.Drawing.Point(331, 74);
            this.cbParaCompute.Name = "cbParaCompute";
            this.cbParaCompute.Size = new System.Drawing.Size(168, 16);
            this.cbParaCompute.TabIndex = 18;
            this.cbParaCompute.Text = "是否在迭代时进行并行计算";
            this.cbParaCompute.UseVisualStyleBackColor = true;
            // 
            // cbVisualize
            // 
            this.cbVisualize.AutoSize = true;
            this.cbVisualize.Location = new System.Drawing.Point(173, 74);
            this.cbVisualize.Name = "cbVisualize";
            this.cbVisualize.Size = new System.Drawing.Size(108, 16);
            this.cbVisualize.TabIndex = 17;
            this.cbVisualize.Text = "是否进行可视化";
            this.cbVisualize.UseVisualStyleBackColor = true;
            // 
            // tbHeight
            // 
            this.tbHeight.Location = new System.Drawing.Point(406, 47);
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.Size = new System.Drawing.Size(72, 21);
            this.tbHeight.TabIndex = 15;
            // 
            // tbRadius
            // 
            this.tbRadius.Location = new System.Drawing.Point(245, 47);
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.Size = new System.Drawing.Size(62, 21);
            this.tbRadius.TabIndex = 14;
            // 
            // tbCubeLength
            // 
            this.tbCubeLength.Location = new System.Drawing.Point(87, 47);
            this.tbCubeLength.Name = "tbCubeLength";
            this.tbCubeLength.Size = new System.Drawing.Size(61, 21);
            this.tbCubeLength.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(6, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 12;
            this.label6.Text = "立方体边长";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(328, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 14);
            this.label5.TabIndex = 11;
            this.label5.Text = "圆柱体高度";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(170, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 14);
            this.label4.TabIndex = 10;
            this.label4.Text = "圆柱体半径";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("SimSun", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(20, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 14);
            this.label3.TabIndex = 9;
            this.label3.Text = "容器类型";
            // 
            // cbBoundType
            // 
            this.cbBoundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoundType.FormattingEnabled = true;
            this.cbBoundType.Items.AddRange(new object[] {
            "长方体",
            "圆柱体"});
            this.cbBoundType.Location = new System.Drawing.Point(87, 20);
            this.cbBoundType.Name = "cbBoundType";
            this.cbBoundType.Size = new System.Drawing.Size(61, 20);
            this.cbBoundType.TabIndex = 5;
            // 
            // btnSaveInfo
            // 
            this.btnSaveInfo.Location = new System.Drawing.Point(136, 313);
            this.btnSaveInfo.Name = "btnSaveInfo";
            this.btnSaveInfo.Size = new System.Drawing.Size(75, 23);
            this.btnSaveInfo.TabIndex = 2;
            this.btnSaveInfo.Text = "保存";
            this.btnSaveInfo.UseVisualStyleBackColor = true;
            this.btnSaveInfo.Click += new System.EventHandler(this.btnSaveInfo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(257, 313);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // GlobalSettingsWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 377);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveInfo);
            this.Controls.Add(this.gbModelInfo);
            this.Controls.Add(this.gbDirSettings);
            this.Name = "GlobalSettingsWnd";
            this.Text = "设置";
            this.gbDirSettings.ResumeLayout(false);
            this.gbDirSettings.PerformLayout();
            this.gbModelInfo.ResumeLayout(false);
            this.gbModelInfo.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDirSettings;
        private System.Windows.Forms.GroupBox gbModelInfo;
        private System.Windows.Forms.Button btnSaveInfo;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnResultsFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbResultsDir;
        private System.Windows.Forms.Button btnLogFolder;
        private System.Windows.Forms.TextBox tbLogDir;
        private System.Windows.Forms.ComboBox cbBoundType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbVisualize;
        private System.Windows.Forms.TextBox tbHeight;
        private System.Windows.Forms.TextBox tbRadius;
        private System.Windows.Forms.TextBox tbCubeLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cbParaCompute;
        private System.Windows.Forms.TextBox tbIterNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbBallsNumber;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbZRate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbParticleSizeType;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tbStepLength;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbResolution;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
    }
}