namespace SpherePacking.MainWindow
{
    partial class VisiualizeSlicesWnd
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
            this.rwcShowSlices = new Kitware.VTK.RenderWindowControl();
            this.btnSaveRenderer = new System.Windows.Forms.Button();
            this.tbR1 = new System.Windows.Forms.TrackBar();
            this.tbG1 = new System.Windows.Forms.TrackBar();
            this.tbB1 = new System.Windows.Forms.TrackBar();
            this.tbB2 = new System.Windows.Forms.TrackBar();
            this.tbG2 = new System.Windows.Forms.TrackBar();
            this.tbR2 = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSaveVtkFile = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tbR1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbR2)).BeginInit();
            this.SuspendLayout();
            // 
            // rwcShowSlices
            // 
            this.rwcShowSlices.AddTestActors = false;
            this.rwcShowSlices.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rwcShowSlices.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rwcShowSlices.Location = new System.Drawing.Point(12, 12);
            this.rwcShowSlices.Name = "rwcShowSlices";
            this.rwcShowSlices.Size = new System.Drawing.Size(500, 500);
            this.rwcShowSlices.TabIndex = 0;
            this.rwcShowSlices.TestText = null;
            // 
            // btnSaveRenderer
            // 
            this.btnSaveRenderer.Location = new System.Drawing.Point(531, 12);
            this.btnSaveRenderer.Name = "btnSaveRenderer";
            this.btnSaveRenderer.Size = new System.Drawing.Size(112, 23);
            this.btnSaveRenderer.TabIndex = 1;
            this.btnSaveRenderer.Text = "保存至png图像";
            this.btnSaveRenderer.UseVisualStyleBackColor = true;
            this.btnSaveRenderer.Click += new System.EventHandler(this.btnSaveRenderer_Click);
            // 
            // tbR1
            // 
            this.tbR1.Location = new System.Drawing.Point(518, 97);
            this.tbR1.Maximum = 255;
            this.tbR1.Name = "tbR1";
            this.tbR1.Size = new System.Drawing.Size(125, 45);
            this.tbR1.TabIndex = 2;
            this.tbR1.Value = 255;
            this.tbR1.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // tbG1
            // 
            this.tbG1.Location = new System.Drawing.Point(518, 138);
            this.tbG1.Maximum = 255;
            this.tbG1.Name = "tbG1";
            this.tbG1.Size = new System.Drawing.Size(125, 45);
            this.tbG1.TabIndex = 3;
            this.tbG1.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // tbB1
            // 
            this.tbB1.Location = new System.Drawing.Point(518, 189);
            this.tbB1.Maximum = 255;
            this.tbB1.Name = "tbB1";
            this.tbB1.Size = new System.Drawing.Size(125, 45);
            this.tbB1.TabIndex = 4;
            this.tbB1.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // tbB2
            // 
            this.tbB2.Location = new System.Drawing.Point(518, 393);
            this.tbB2.Maximum = 255;
            this.tbB2.Name = "tbB2";
            this.tbB2.Size = new System.Drawing.Size(125, 45);
            this.tbB2.TabIndex = 5;
            this.tbB2.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // tbG2
            // 
            this.tbG2.Location = new System.Drawing.Point(518, 331);
            this.tbG2.Maximum = 255;
            this.tbG2.Name = "tbG2";
            this.tbG2.Size = new System.Drawing.Size(125, 45);
            this.tbG2.TabIndex = 6;
            this.tbG2.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // tbR2
            // 
            this.tbR2.Location = new System.Drawing.Point(518, 280);
            this.tbR2.Maximum = 255;
            this.tbR2.Name = "tbR2";
            this.tbR2.Size = new System.Drawing.Size(125, 45);
            this.tbR2.TabIndex = 7;
            this.tbR2.Value = 255;
            this.tbR2.Scroll += new System.EventHandler(this.tbRGB_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(529, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "渐变色1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(529, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "渐变色2";
            // 
            // btnSaveVtkFile
            // 
            this.btnSaveVtkFile.Location = new System.Drawing.Point(531, 52);
            this.btnSaveVtkFile.Name = "btnSaveVtkFile";
            this.btnSaveVtkFile.Size = new System.Drawing.Size(112, 23);
            this.btnSaveVtkFile.TabIndex = 11;
            this.btnSaveVtkFile.Text = "保存至itk文件";
            this.btnSaveVtkFile.UseVisualStyleBackColor = true;
            this.btnSaveVtkFile.Click += new System.EventHandler(this.btnSaveVtkFile_Click);
            // 
            // VisiualizeSlicesWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 515);
            this.Controls.Add(this.btnSaveVtkFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbR2);
            this.Controls.Add(this.tbG2);
            this.Controls.Add(this.tbB2);
            this.Controls.Add(this.tbB1);
            this.Controls.Add(this.tbG1);
            this.Controls.Add(this.tbR1);
            this.Controls.Add(this.btnSaveRenderer);
            this.Controls.Add(this.rwcShowSlices);
            this.Name = "VisiualizeSlicesWnd";
            this.Text = "可视化切片序列窗口";
            this.Load += new System.EventHandler(this.VisiualizeSlicesWnd_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tbR1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbR2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Kitware.VTK.RenderWindowControl rwcShowSlices;
        private System.Windows.Forms.Button btnSaveRenderer;
        private System.Windows.Forms.TrackBar tbR1;
        private System.Windows.Forms.TrackBar tbG1;
        private System.Windows.Forms.TrackBar tbB1;
        private System.Windows.Forms.TrackBar tbB2;
        private System.Windows.Forms.TrackBar tbG2;
        private System.Windows.Forms.TrackBar tbR2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSaveVtkFile;
    }
}