using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utils;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace core_ci_test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");
            for (int i = 0; i < 3; i++)
            {
                Task.Run(() =>
                {
                    TestOrder();
                });

            }
            Console.ReadKey();
        }
        static void AutoTask(string _httpProxy = null)
        {
            try
            {
                PhantomJSDriverService service = PhantomJSDriverService.CreateDefaultService();

                if (!string.IsNullOrEmpty(_httpProxy))
                {
                    service.ProxyType = "http";
                    service.Proxy = _httpProxy;

                }
                using (var driver = new PhantomJSDriver(service))
                {
                    driver.Navigate().GoToUrl("http://www.cnblogs.com/");
                    Screenshot screenshot1 = ((ITakesScreenshot)driver).GetScreenshot();
                    driver.FindElementById("homepage1_HomePageDays_ctl00_DayList_TitleUrl_0").Click();
                    var title = driver.Title;
                    if (string.IsNullOrEmpty(title) == false)
                    {
                        Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                        screenshot.SaveAsFile($"F:\\imgs\\b_{_httpProxy.Replace(".", "_").Replace(":", "-")}.jpg", ScreenshotImageFormat.Jpeg);
                        Console.WriteLine($"[{_httpProxy}]请求成功：{title}");

                        driver.Navigate().GoToUrl("http://www.cnblogs.com");
                    }
                    driver.Quit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        static void TestOrder()
        {
            var listIps = GetProxyIp(100);
            Console.WriteLine($"获取代理IP：{listIps.Count}");
            GetOrderInfo(listIps);

        }
        static Random random = new Random();
        static Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,4}", RegexOptions.IgnoreCase);
        static void GetOrderInfo(List<string> listIps)
        {
            if (listIps.Count == 0)
            {
                return;
            }
            var proxyIp = listIps.FirstOrDefault();
            HttpHelper httpHelper = new HttpHelper();
            var order = random.Next(100000, 5554726).ToString() + random.Next(100000000, 632644596).ToString();
            Console.Write($"IP:{proxyIp.PadRight(25)} Order:{order.PadRight(20)}--->");

            var result = httpHelper.GetHtml(new HttpItem()
            {
                ProxyIp = proxyIp,
                Timeout = 3000,
                URL = "http://ttvp.mimidaili.com/ip/?tid=" + order + "&num=1&sortby=time&foreign=none&filter=on"
            });
            Console.Write($"Status:{result.StatusCode.ToString().PadRight(5)},代理池：{listIps.Count}");
            var checkResult = result.StatusCode == System.Net.HttpStatusCode.OK;
            if (!checkResult)
            {
                listIps.Remove(proxyIp);
            }
            if (regex.IsMatch(result.Html))
            {
                Console.Write($"有效订单:{order}");
                FileHelper.WriteFile(order, "order.txt");
            }

            Console.WriteLine();
            GetOrderInfo(listIps);
        }

        static List<string> GetProxyIpBMiMi(string proxyIp = null)
        {
            //return FileHelper.ReadFile("ips.txt").Split("\r\n").ToList();
            HttpHelper httpHelper = new HttpHelper();
            var result = httpHelper.GetHtml(new HttpItem()
            {
                URL = "http://ttvp.mimidaili.com/ip/?tid=abc&num=5&sortby=time&foreign=none&filter=on"
            });
            List<string> ips = new List<string>();
            Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,4}", RegexOptions.IgnoreCase);
            var matchs = regex.Matches(result.Html);
            for (int i = 0; i < matchs.Count; i++)
            {
                var ip = matchs[i].Value;
                if (Ping(ip.Substring(0, ip.IndexOf(":"))))
                {
                    ips.Add(ip);
                    FileHelper.WriteFile(ip, "ips.txt");
                }

            }
            return ips;
        }
        static List<string> GetProxyIpByKuaiDaiLi()
        {
            HttpHelper httpHelper = new HttpHelper();
            var result = httpHelper.GetHtml(new HttpItem()
            {
                URL = "http://www.kuaidaili.com/free/intr/"
            });
            Regex regex = new Regex(@"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})", RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(@"(<td data-title=""PORT"">)(\d{1,4})(</td>)", RegexOptions.IgnoreCase);
            var matchs = regex.Matches(result.Html);
            var matchs2 = regex2.Matches(result.Html);
            List<string> ips = new List<string>();
            for (int i = 0; i < matchs.Count; i++)
            {

                var ip = matchs[i].Value;
                if (Ping(ip))
                {
                    ips.Add(ip + ":" + matchs2[i].Groups[2].Value);
                }

            }
            return ips;
        }
        static List<string> GetProxyIp(int num = 100, string proxyIp = null)
        {
            HttpHelper httpHelper = new HttpHelper();
            var result = httpHelper.GetHtml(new HttpItem()
            {
                ProxyIp = proxyIp,
                URL = "http://www.66ip.cn/mo.php?sxb=&tqsl=" + num + "&port=&export=&ktip=&sxa=&submit=%CC%E1++%C8%A1&textarea=http%3A%2F%2Fwww.66ip.cn%2F%3Fsxb%3D%26tqsl%3D100%26ports%255B%255D2%3D%26ktip%3D%26sxa%3D%26radio%3Dradio%26submit%3D%25CC%25E1%2B%2B%25C8%25A1"
            });
            Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,4}", RegexOptions.IgnoreCase);
            var matchs = regex.Matches(result.Html);
            List<string> ips = new List<string>();
            for (int i = 0; i < matchs.Count; i++)
            {
                var ip = matchs[i].Value;
                if (Ping(ip.Substring(0, ip.IndexOf(":"))))
                {
                    ips.Add(ip);
                }

            }
            return ips;
        }
        /// <summary>;
        /// 是否能 Ping 通指定的主机
        /// </summary>
        /// <param name="ip">ip 地址或主机名或域名&lt;/param>
        /// <returns>true 通，false 不通</returns>
        public static bool Ping(string ip)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
