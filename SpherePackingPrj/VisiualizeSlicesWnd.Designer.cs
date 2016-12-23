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
            this.SuspendLayout();
            // 
            // rwcShowSlices
            // 
            this.rwcShowSlices.AddTestActors = false;
            this.rwcShowSlices.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.rwcShowSlices.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.rwcShowSlices.Location = new System.Drawing.Point(12, 12);
            this.rwcShowSlices.Name = "rwcShowSlices";
            this.rwcShowSlices.Size = new System.Drawing.Size(769, 521);
            this.rwcShowSlices.TabIndex = 0;
            this.rwcShowSlices.TestText = null;
            // 
            // VisiualizeSlicesWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 545);
            this.Controls.Add(this.rwcShowSlices);
            this.Name = "VisiualizeSlicesWnd";
            this.Text = "可视化切片序列窗口";
            this.Load += new System.EventHandler(this.VisiualizeSlicesWnd_Load);
            this.SizeChanged += new System.EventHandler(this.VisiualizeSlicesWnd_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private Kitware.VTK.RenderWindowControl rwcShowSlices;
    }
}