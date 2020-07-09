using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bypasser
{
    public class H_Helper
    {
        /// <summary>
        /// 외부 명령을 실행시키고, 출력 결과 문자열을 가져옵니다.
        /// <para>
        /// 외부 명령 실행 시 관리자 권한이 필요한 경우 app.manifest 파일을 생성 (프로젝트-추가-새항목-응용프로그램 매니페스트 파일)하여 관리자 권한을 변경합니다.
        /// </para>
        /// </summary>
        /// <param name="filaName">실행할 명령어(파일명)</param>
        /// <param name="arguments">실행 인수</param>
        /// <param name="inputCommands">실행 후 전달할 명령 문자열의 배열</param>
        /// <returns></returns>
        public static string Exec(string filaName, string arguments, string[] inputCommands)
        {
            // 프로세스 정보 및 설정
            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = filaName;
            if (arguments != "") { pi.Arguments = arguments; }
            pi.WindowStyle = ProcessWindowStyle.Hidden;
            pi.CreateNoWindow = true;
            pi.UseShellExecute = false;
            pi.RedirectStandardInput = true;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;

            // 프로세스 시작
            Process process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo = pi;
            process.Start();
            // 프로세스 실행 후 명령 문자열 입력
            foreach (var input in inputCommands)
            {
                process.StandardInput.WriteLine(input);
            }
            process.StandardInput.Close();

            // 출력 결과 및 에러 수집
            string resultOutput = process.StandardOutput.ReadToEnd().ToString();
            string resultError = process.StandardError.ReadToEnd().ToString();

            // 프로세스 종료
            process.WaitForExit();
            process.Close();

            // 외부 프로그램 오류 발생 시 처리
            if (resultError != string.Empty)
            {
                Debug.WriteLine("외부 명령 실행 시 오류가 발생했습니다.");
                Debug.Indent();
                Debug.WriteLine("오류: 실행 결과가 StandardError 스트림으로 출력되거나 실행에 실패하였습니다:");
                Debug.WriteLine($"파일이름: {pi.FileName}");
                Debug.WriteLine($"인수: {pi.Arguments}");
                Debug.Unindent();
                throw new Exception(resultError);
            }

            return resultOutput;
        }
    }
}
