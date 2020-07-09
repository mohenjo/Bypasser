using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bypasser
{
    public partial class FormMain : Form
    {
        #region 사용자 추가 보조 메서드

        
        
        /// <summary> 
        /// UI 초기화
        /// </summary>
        private void InitializeUI()
        {
            this.Text = H_Globals.AppName;
            numericUpDownMtu.Value = H_Globals.CurrentTargetMtu;
            buttonSaveMtu.Enabled = false;
            H_Globals.IsProcessOn = true;
        }

        /// <summary>
        /// UI 업데이트
        /// </summary>
        private void UpdateUIAndProcess()
        {
            if (H_Globals.IsProcessOn)
            {
                buttonStart.Enabled = false;
                buttonStop.Enabled = true;
                H_NetHandler.SetMtu(H_Globals.CurrentTargetMtu);
                Debug.WriteLine("MTU 값이 변경되었습니다.");
                Debug.WriteLine(H_NetHandler.GetNetInfoString());
            }
            else
            {
                buttonStart.Enabled = true;
                buttonStop.Enabled = false;
                H_NetHandler.SetDefaultMtu();
                Debug.WriteLine("MTU 기본값이 복구되었습니다.");
                Debug.WriteLine(H_NetHandler.GetNetInfoString());
            }
        }

        #endregion

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            InitializeUI();
            UpdateUIAndProcess();
        }

        // MTU 설정 대상값 저장 (네트워크에 반영되는 것 아님. 사용자 설정에 저장됨)
        private void buttonSaveMtu_Click(object sender, EventArgs e)
        {
            H_Globals.CurrentTargetMtu = (int)numericUpDownMtu.Value;
            buttonSaveMtu.Enabled = false;
        }

        private void numericUpDownMtu_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownMtu.Value == H_Globals.CurrentTargetMtu)
            {
                buttonSaveMtu.Enabled = false;
            }
            else { buttonSaveMtu.Enabled = true; }
;
        }

          private void buttonStart_Click(object sender, EventArgs e)
        {
            H_Globals.IsProcessOn = true;
            UpdateUIAndProcess();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            H_Globals.IsProcessOn = false;
            UpdateUIAndProcess();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            H_NetHandler.SetDefaultMtu();
            Debug.WriteLine("MTU 기본값이 복구되었습니다.");
            Debug.WriteLine(H_NetHandler.GetNetInfoString());
            Application.Exit();
        }

        // 사용자가 폼을 닫기 버튼을 누를 경우
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{H_Globals.AppName} {H_Globals.Version}");
            sb.AppendLine("Copyright 2019 by Haennim Park");
            sb.AppendLine();
            sb.AppendLine("MTU 값의 임시 변경을 통해 HTTPS SNI 확인을 우회합니다.");
            sb.AppendLine("프로그램 종료(또는 예기치 못한 종료 시 재부팅) 후 원래의");
            sb.AppendLine("기본값으로 복구됩니다.");

            MessageBox.Show(sb.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void networkInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(H_NetHandler.GetNetInfoString());
            MessageBox.Show(sb.ToString(), "Network Info", MessageBoxButtons.OK, MessageBoxIcon.None);
        }
    }
}