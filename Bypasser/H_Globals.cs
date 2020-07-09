using System.Diagnostics;
using System.Reflection;

namespace Bypasser
{
    public class H_Globals
    {
        public static string AppName = "Bypasser";

        public static string Version = Assembly.GetExecutingAssembly()
            .GetName().Version.ToString();

        public static bool IsProcessOn;

        public static int CurrentTargetMtu
        {
            get
            {
                return Properties.Settings.Default.MtuValue;
            }
            set
            {
                if (value >= 352 && value <=1500)
                {
                    Properties.Settings.Default.MtuValue = (int)value;
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Debug.WriteLine($"MTU 값이 정상범위 [352, 1500]을 벗어납니다. 기존의 값 {CurrentTargetMtu}을 유지합니다.");
                }
            }
        }
    }
}