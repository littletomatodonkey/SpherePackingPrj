﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpherePacking.MainWindow
{
    public partial class GlobalSettingsWnd : Form
    {
        /// <summary>
        /// 重新加载设置
        /// 需要对主界面中的RenderWindowControl进行重绘等，因此需要委托事件通知mainwnd
        /// </summary>
        public delegate void ReloadSystemSettingHandler();

        public ReloadSystemSettingHandler ReloadSystemSetting;

        public GlobalSettingsWnd()
        {
            InitializeComponent();
            ImportSettings();
        }

        /// <summary>
        /// 根据系统设置填充界面
        /// </summary>
        /// <returns></returns>
        private bool ImportSettings()
        {
            bool res = true;
            
            cbBoundType.SelectedIndex = (PackingSystemSetting.SystemBoundType == BoundType.CubeType) ? 0 : 1;
            tbCubeLength.Text = PackingSystemSetting.CubeLength.ToString();
            tbRadius.Text = PackingSystemSetting.Radius.ToString();
            tbHeight.Text = PackingSystemSetting.Height.ToString();
            tbZRate.Text = PackingSystemSetting.ZRate.ToString();
            tbIterNumber.Text = PackingSystemSetting.IterationNum.ToString();
            cbParaCompute.Checked = PackingSystemSetting.IsParaCompute;
            cbVisualize.Checked = PackingSystemSetting.IsVisualize;
            tbResultsDir.Text = PackingSystemSetting.ResultDir;
            tbLogDir.Text = PackingSystemSetting.LogDir;

            tbStepLength.Text = PackingSystemSetting.StepLength.ToString();
            tbBallsNumber.Text = PackingSystemSetting.BallsNumber.ToString();
            cbParticleSizeType.SelectedIndex = (int)PackingSystemSetting.ParticleSizeType;
            return res;
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        /// <returns></returns>
        private bool SaveSettings()
        {
            bool res = true;
            try
            {
                Convert.ToInt32(tbIterNumber.Text);
                Convert.ToDouble(tbCubeLength.Text);
                Convert.ToDouble(tbRadius.Text);
                Convert.ToDouble(tbHeight.Text);
                Convert.ToDouble( tbZRate.Text );
                Convert.ToDouble( tbStepLength.Text );

                PackingSystemSetting.SystemBoundType = (cbBoundType.SelectedIndex == 0) ? BoundType.CubeType : BoundType.CylinderType;
                PackingSystemSetting.IterationNum = Convert.ToInt32(tbIterNumber.Text);
                PackingSystemSetting.ZRate = Convert.ToDouble( tbZRate.Text );
                PackingSystemSetting.Height = Convert.ToDouble(tbHeight.Text);
                PackingSystemSetting.IsParaCompute = cbParaCompute.Checked;
                PackingSystemSetting.IsVisualize = cbVisualize.Checked;
                PackingSystemSetting.ResultDir = tbResultsDir.Text;
                PackingSystemSetting.LogDir = tbLogDir.Text;
                PackingSystemSetting.ParticleSizeType = (ActualSampleType)cbParticleSizeType.SelectedIndex;
                PackingSystemSetting.StepLength = Convert.ToDouble( tbStepLength.Text );
                switch (PackingSystemSetting.SystemBoundType)
                {
                    case BoundType.CubeType:
                        PackingSystemSetting.CubeLength = Convert.ToDouble(tbCubeLength.Text);
                        break;
                    case BoundType.CylinderType:
                        PackingSystemSetting.Radius = Convert.ToDouble(tbRadius.Text);
                        break;
                    default:
                        break;
                }

            }
            catch
            {
                res = false;
            }
            
            return res;
        }

        private void btnResultsFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.UserProfile,
                Description = "请选择保存结果的文件夹(建议：英文路径)",
            };
            if( dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                tbResultsDir.Text = dialog.SelectedPath;
            }
        }

        private void btnLogFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                RootFolder = Environment.SpecialFolder.UserProfile,
                Description = "请选择保存日志的文件夹(建议：英文路径)",
            };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbLogDir.Text = dialog.SelectedPath;
            }
        }

        private void btnSaveInfo_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                DataReadWriteHelper.SaveObjAsJsonFile(new PackSysSettingForSave(), PackingSystemSetting.SettingFilename);
                ReloadSystemSetting();
                this.Close();
            }
            else
            {
                MessageBox.Show("填充的信息有误，请检查其合法性！", "warning");
            }
            
        }

        /// <summary>
        /// 按esc键可以退出该界面
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            int WM_KEYDOWN = 256;
            int WM_SYSKEYDOWN = 260;
            if (msg.Msg == WM_KEYDOWN | msg.Msg == WM_SYSKEYDOWN)
            {
                switch (keyData)
                {
                    case Keys.Escape:
                        this.Close();//esc关闭窗体
                        break;
                    default:
                        break;
                }
            }
            return false;
        }

        private void cbParticleSizeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            double reso = ActualSampleParameter.ActualSampleParaDict[(ActualSampleType)(cbParticleSizeType.SelectedIndex)].MaxDiameter / 
                    (2 * PackingSystemSetting.MaxBallRadius) / 1e-6;

            tbResolution.Text = reso.ToString();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
