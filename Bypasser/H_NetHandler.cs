using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bypasser
{
    public class H_NetHandler
    {
        /// <summary>
        /// 현재 시스템의 최적 NIC
        /// </summary>
        private static NetworkInterface BestNic = null;

        /// <summary>
        /// 시스템의 기본 최소 MTU
        /// </summary>
        public static readonly int DefaultSystemMinMtu;

        /// <summary>
        /// 현재 NIC의 기본 MTU
        /// </summary>
        public static readonly int DefaultNicMtu;

        /// <summary>
        /// 인터넷 접속 가능 여부를 확인합니다.
        /// </summary>
        /// <returns></returns>
        public static bool IsInternetConnected()
        {
            /*
             * Microsoft는 인터넷 정상 접속 여부를 체크를 위해 특별한 웹 페이지를 제공합니다.
             * Vista 이상의 Windows OS에서는 NCSI(Network Connectivity Status Indicator)라는 기능을 통해
             * 인터넷 사용 가능 여부를 체크합니다:
             * 1. HTTP GET 을 사용하여 www.msftncsi.com/ncsi.txt를 가져올 수 있는지 체크
             * 2. dns.msftncsi.com 이라는 DNS 호스트의 IP가 131.107.255.255 이 되는지 체크
             * 출처: http://www.csharpstudy.com/Tip/Tip-network-connectivity.aspx
             */

            const string NCSI_TEST_URL = "http://www.msftncsi.com/ncsi.txt";
            const string NCSI_TEST_RESULT = "Microsoft NCSI";
            const string NCSI_DNS = "dns.msftncsi.com";
            const string NCSI_DNS_IP_ADDRESS = "131.107.255.255";

            try
            {
                // 1. NCSI 링크에서 텍스트를 제대로 가져올 수 있는지 체크
                var webClient = new WebClient();
                string result = webClient.DownloadString(NCSI_TEST_URL);
                if (result != NCSI_TEST_RESULT)
                {
                    return false;
                }

                // 2. NCSI DNS 호스트 IP 체크
                var dnsHost = Dns.GetHostEntry(NCSI_DNS);
                if (dnsHost.AddressList.Count() <= 0 || dnsHost.AddressList[0].ToString() != NCSI_DNS_IP_ADDRESS)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 특정 IPv4 주소에 대해 최적의 라우트를 가진 NIC의 인덱스를 반환합니다.
        /// </summary>
        /// <param name="destAddr">32비트의 길이로 표시된 IPv4(4바이트) 주소입니다.</param>
        /// <param name="bestIfIndex">최적 NIC의 인덱스입니다.</param>
        /// <returns></returns>
        [DllImport("iphlpapi.dll", CharSet = CharSet.Auto)]
        private static extern int GetBestInterface(UInt32 destAddr, out UInt32 bestIfIndex);

        /// <summary>
        /// NIC 인덱스에 해당하는 NIC 개체 정보를 가져옵니다.
        /// </summary>
        /// <param name="index">NIC 인덱스</param>
        /// <returns></returns>
        private static NetworkInterface GetNetworkInterfaceByIndex(uint index)
        {
            // IPv4를 지원하는 모든 NIC 검색
            NetworkInterface ipv4interface =
                (from thisInterface in NetworkInterface.GetAllNetworkInterfaces()
                 where thisInterface.Supports(NetworkInterfaceComponent.IPv4)
                 let ipv4Properties = thisInterface.GetIPProperties().GetIPv4Properties()
                 where ipv4Properties != null && ipv4Properties.Index == index
                 select thisInterface).SingleOrDefault();
            if (ipv4interface != null)
            {
                return ipv4interface;
            }

            // IPv6를 지원하는 모든 NIC 검색
            NetworkInterface ipv6interface =
                (from thisInterface in NetworkInterface.GetAllNetworkInterfaces()
                 where thisInterface.Supports(NetworkInterfaceComponent.IPv6)
                 let ipv6Properties = thisInterface.GetIPProperties().GetIPv6Properties()
                 where ipv6Properties != null && ipv6Properties.Index == index
                 select thisInterface).SingleOrDefault();
            return ipv6interface;
        }

        /// <summary>
        /// 현재 인터넷 접속에 대한 최적의 NIC 오브젝트를 검색 결과와 해당 오브젝트를 반환합니다.
        /// </summary>
        /// <param name="bestNic">최적의 NIC 오브젝트</param>
        /// <returns></returns>
        public static bool GetBestNic(out NetworkInterface bestNic)
        {
            string targetUrl = "www.google.com"; // 호스트 URL
            bestNic = null;

            IPHostEntry hostInfo = null;
            try
            {
                hostInfo = Dns.GetHostEntry(targetUrl);
            }
            catch (SocketException ex)
            {
                Debug.WriteLine($"호스트 URL({targetUrl})을 확인할 때 오류가 발생했습니다.");
                Debug.WriteLine(ex.Message);
                return false;
            }

            // 호스트 URL에 대한 IPv4 주소 얻음
            IPAddress ipv4Adress =
                (from thisAddress in hostInfo.AddressList
                 where thisAddress.AddressFamily == AddressFamily.InterNetwork
                 select thisAddress).FirstOrDefault();

            // IPv4 주소 유효성 판단
            if (ipv4Adress == null)
            {
                Debug.WriteLine($"{targetUrl}에 연결된 IPv4 주소를 찾을 수 없습니다.");
                return false;
            }

            // 최적 NIC 검색
            UInt32 ipvAddressAsUInt32 = BitConverter.ToUInt32(ipv4Adress.GetAddressBytes(), 0);
            UInt32 interfaceIndex;
            int result = GetBestInterface(ipvAddressAsUInt32, out interfaceIndex);
            if (result != 0) // DWROD != NO_ERROR
            {
                Debug.WriteLine("GetBestInterface() 함수로부터 정상작으로 NIC 인덱스를 가져오지 못했습니다.");
                throw new Win32Exception(result);
            }
            bestNic = GetNetworkInterfaceByIndex(interfaceIndex);
            return true;
        }

        static H_NetHandler()
        {
            // 인터넷 연결 여부 확인
            if (!H_NetHandler.IsInternetConnected())
            {
                MessageBox.Show("인터넷에 연결되어 있지 않습니다.", H_Globals.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // NIC 검색
            if (GetBestNic(out NetworkInterface bestNic))
            {
                BestNic = bestNic;
            }
            else
            {
                Debug.WriteLine("현재 인터넷에 접속된 최적의 네트워크 인터페이스를 검색할 수 없습니다.");
                Application.Exit();
            }

            // 기본 MTU 값 저장
            DefaultSystemMinMtu = GetSystemMinMtu();
            DefaultNicMtu = GetNicMtu();
        }

        /// <summary>
        /// 현재 시스템의 최소 MTU 값을 구합니다
        /// </summary>
        /// <returns></returns>
        public static int GetSystemMinMtu()
        {
            string inputStr = H_Helper.Exec("netsh", "interface ipv4 show global", new string[] { });
            string[] findStr = new string[] { "최소 MTU", "Minimum MTU" };

            string parsedLine = (from line
                             in inputStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                 where findStr.Any(fstr => line.Contains(fstr))
                                 select line).FirstOrDefault();
            if (parsedLine == null)
            {
                Debug.WriteLine("netsh 명령에서 파싱된 문자열이 최소 MTU 값을 포함하지 않습니다.");
                throw new Exception("netsh 결과 문자열 파싱 오류");
            }

            var minMtu = int.Parse(parsedLine
                .Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)
                .Last().Trim());

            return minMtu;
        }

        /// <summary>
        /// 현재 NIC의 MTU 값을 구합니다.
        /// </summary>
        /// <returns></returns>
        public static int GetNicMtu()
        {
            string inputStr = H_Helper.Exec("netsh", "interface ipv4 show interfaces", new string[] { });

            string parsedLine = (from line
                             in inputStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                 where line.Contains(BestNic.Name)
                                 select line).ToArray()[0];
            var mtu = int.Parse(parsedLine
                .Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[2].Trim());
            return mtu;
        }

        /// <summary>
        /// MTU 값을 변경합니다. 필요시 시스템 최소 MTU 값도 변경합니다.
        /// </summary>
        /// <param name="mtuValue"></param>
        public static void SetMtu(int mtuValue)
        {
            string arg;
            if (GetSystemMinMtu() > mtuValue)
            {
                arg = $"interface ipv4 set global minmtu={mtuValue} store=active";
                H_Helper.Exec("netsh", arg, new string[] { });
            }
            arg = $"interface ipv4 set subinterface \"{BestNic.Name}\" mtu={mtuValue} store=active";
            H_Helper.Exec("netsh", arg, new string[] { });
        }

        /// <summary>
        /// MTU 값을 시스템 기본 값으로 되돌립니다.
        /// </summary>
        public static void SetDefaultMtu()
        {
            string arg;
            arg = $"interface ipv4 set subinterface \"{BestNic.Name}\" mtu={DefaultNicMtu} store=active";
            H_Helper.Exec("netsh", arg, new string[] { });
            arg = $"interface ipv4 set global minmtu={DefaultSystemMinMtu} store=active";
            H_Helper.Exec("netsh", arg, new string[] { });
        }

        /// <summary>
        /// 현재 네트워크 정보 문자열을 반환합니다.
        /// </summary>
        public static string GetNetInfoString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"시스템 최소 MTU: {GetSystemMinMtu()}");
            sb.AppendLine("최적 NIC 정보:");
            sb.AppendLine($"- ID: {BestNic.Id}");
            sb.AppendLine($"- DESCRIPTION: {BestNic.Description}");
            sb.AppendLine($"- NAME: {BestNic.Name}");
            sb.AppendLine($"- STATUS: {BestNic.OperationalStatus}");
            sb.AppendLine($"- MTU: {GetNicMtu()}");
            return sb.ToString();
        }
    }
}
