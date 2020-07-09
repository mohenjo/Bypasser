using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bypasser
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                int count = 0;
                Process[] procs = Process.GetProcesses();
                string currentProcessName = Process.GetCurrentProcess().ProcessName;
                foreach (Process p in procs)
                {
                    if (p.ProcessName.Equals(currentProcessName) == true)
                    {
                        count++;
                    }
                }

                if (count > 1)
                {
                    MessageBox.Show("이 프로그램은 중복하여 실행할 수 없습니다.", H_Globals.AppName, MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormMain());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK);
            }
        }
    }
}
