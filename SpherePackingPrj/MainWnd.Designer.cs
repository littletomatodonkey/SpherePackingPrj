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
            this.rwcVolumeDisp = new Kitware.VTK.RenderWindowControl();
            this.btnSaveImg = new System.Windows.Forms.Button();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.btnSolveProblem = new System.Windows.Forms.Button();
            this.cbBoundType = new System.Windows.Forms.ComboBox();
            this.btnSinter = new System.Windows.Forms.Button();
            this.btnLoadSpheresJson = new System.Windows.Forms.Button();
            this.btnSaveSpheres = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.ttShowInfo = new System.Windows.Forms.ToolTip(this.components);
            this.openFiledDlg = new System.Windows.Forms.OpenFileDialog();
            this.btnGenerateSlices = new System.Windows.Forms.Button();
            this.tbStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // rwcVolumeDisp
            // 
            this.rwcVolumeDisp.AddTestActors = false;
            this.rwcVolumeDisp.AutoScroll = true;
            this.rwcVolumeDisp.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.rwcVolumeDisp.Location = new System.Drawing.Point(158, 28);
            this.rwcVolumeDisp.Name = "rwcVolumeDisp";
            this.rwcVolumeDisp.Size = new System.Drawing.Size(716, 492);
            this.rwcVolumeDisp.TabIndex = 0;
            this.rwcVolumeDisp.TestText = null;
            // 
            // btnSaveImg
            // 
            this.btnSaveImg.Location = new System.Drawing.Point(12, 124);
            this.btnSaveImg.Name = "btnSaveImg";
            this.btnSaveImg.Size = new System.Drawing.Size(92, 23);
            this.btnSaveImg.TabIndex = 1;
            this.btnSaveImg.Text = "保存当前图像";
            this.btnSaveImg.UseVisualStyleBackColor = true;
            this.btnSaveImg.Click += new System.EventHandler(this.btnSaveImg_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 1000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // btnSolveProblem
            // 
            this.btnSolveProblem.Location = new System.Drawing.Point(13, 182);
            this.btnSolveProblem.Name = "btnSolveProblem";
            this.btnSolveProblem.Size = new System.Drawing.Size(92, 23);
            this.btnSolveProblem.TabIndex = 3;
            this.btnSolveProblem.Text = "求解问题";
            this.btnSolveProblem.UseVisualStyleBackColor = true;
            this.btnSolveProblem.Click += new System.EventHandler(this.btnSolveProblem_Click);
            // 
            // cbBoundType
            // 
            this.cbBoundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBoundType.FormattingEnabled = true;
            this.cbBoundType.Items.AddRange(new object[] {
            "立方体",
            "圆柱体"});
            this.cbBoundType.Location = new System.Drawing.Point(12, 28);
            this.cbBoundType.Name = "cbBoundType";
            this.cbBoundType.Size = new System.Drawing.Size(121, 20);
            this.cbBoundType.TabIndex = 4;
            this.cbBoundType.SelectedIndexChanged += new System.EventHandler(this.cbBoundType_SelectedIndexChanged);
            // 
            // btnSinter
            // 
            this.btnSinter.Location = new System.Drawing.Point(12, 211);
            this.btnSinter.Name = "btnSinter";
            this.btnSinter.Size = new System.Drawing.Size(91, 23);
            this.btnSinter.TabIndex = 5;
            this.btnSinter.Text = "烧结过程模拟";
            this.btnSinter.UseVisualStyleBackColor = true;
            this.btnSinter.Click += new System.EventHandler(this.btnSinter_Click);
            // 
            // btnLoadSpheresJson
            // 
            this.btnLoadSpheresJson.Location = new System.Drawing.Point(12, 65);
            this.btnLoadSpheresJson.Name = "btnLoadSpheresJson";
            this.btnLoadSpheresJson.Size = new System.Drawing.Size(92, 23);
            this.btnLoadSpheresJson.TabIndex = 6;
            this.btnLoadSpheresJson.Text = "导入小球信息";
            this.btnLoadSpheresJson.UseVisualStyleBackColor = true;
            this.btnLoadSpheresJson.Click += new System.EventHandler(this.btnLoadSpheresJson_Click);
            // 
            // btnSaveSpheres
            // 
            this.btnSaveSpheres.Location = new System.Drawing.Point(13, 95);
            this.btnSaveSpheres.Name = "btnSaveSpheres";
            this.btnSaveSpheres.Size = new System.Drawing.Size(91, 23);
            this.btnSaveSpheres.TabIndex = 7;
            this.btnSaveSpheres.Text = "保存小球信息";
            this.btnSaveSpheres.UseVisualStyleBackColor = true;
            this.btnSaveSpheres.Click += new System.EventHandler(this.btnSaveSpheres_Click);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(12, 153);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(91, 23);
            this.btnTest.TabIndex = 8;
            this.btnTest.Text = "测试按钮";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // btnGenerateSlices
            // 
            this.btnGenerateSlices.Location = new System.Drawing.Point(13, 241);
            this.btnGenerateSlices.Name = "btnGenerateSlices";
            this.btnGenerateSlices.Size = new System.Drawing.Size(90, 23);
            this.btnGenerateSlices.TabIndex = 9;
            this.btnGenerateSlices.Text = "生成切片";
            this.btnGenerateSlices.UseVisualStyleBackColor = true;
            this.btnGenerateSlices.Click += new System.EventHandler(this.btnGenerateSlices_Click);
            // 
            // tbStatus
            // 
            this.tbStatus.Location = new System.Drawing.Point(12, 340);
            this.tbStatus.Name = "tbStatus";
            this.tbStatus.ReadOnly = true;
            this.tbStatus.Size = new System.Drawing.Size(100, 21);
            this.tbStatus.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 322);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "状态信息";
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(886, 532);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStatus);
            this.Controls.Add(this.btnGenerateSlices);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.btnSaveSpheres);
            this.Controls.Add(this.btnLoadSpheresJson);
            this.Controls.Add(this.btnSinter);
            this.Controls.Add(this.cbBoundType);
            this.Controls.Add(this.btnSolveProblem);
            this.Controls.Add(this.btnSaveImg);
            this.Controls.Add(this.rwcVolumeDisp);
            this.Name = "MainWnd";
            this.Text = "Sphere Packing";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWnd_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.MainWnd_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Kitware.VTK.RenderWindowControl rwcVolumeDisp;
        private System.Windows.Forms.Button btnSaveImg;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Button btnSolveProblem;
        private System.Windows.Forms.ComboBox cbBoundType;
        private System.Windows.Forms.Button btnSinter;
        private System.Windows.Forms.Button btnLoadSpheresJson;
        private System.Windows.Forms.Button btnSaveSpheres;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.ToolTip ttShowInfo;
        private System.Windows.Forms.OpenFileDialog openFiledDlg;
        private System.Windows.Forms.Button btnGenerateSlices;
        private System.Windows.Forms.TextBox tbStatus;
        private System.Windows.Forms.Label label1;
    }
}

