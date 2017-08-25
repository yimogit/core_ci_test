using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Utils;
namespace core_ci_test
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("start");
            AutoTask();
            // System.Console.WriteLine("获取代理IP...");
            // var list = GetProxyIp();
            // list.ForEach(e=>Console.WriteLine(e));
            // System.Console.WriteLine($"获取代理IP:{list.Count}...");
            Console.WriteLine("end");
            Console.ReadKey();
        }
        static void AutoTask()
        {
            FileHelper.RunCmd("phantomjs", " -v");
        }

        static List<string> GetProxyIp()
        {
            HttpHelper httpHelper = new HttpHelper();
            var result = httpHelper.GetHtml(new HttpItem()
            {
                URL = "http://www.66ip.cn/mo.php?sxb=&tqsl=100&port=&export=&ktip=&sxa=&submit=%CC%E1++%C8%A1&textarea=http%3A%2F%2Fwww.66ip.cn%2F%3Fsxb%3D%26tqsl%3D100%26ports%255B%255D2%3D%26ktip%3D%26sxa%3D%26radio%3Dradio%26submit%3D%25CC%25E1%2B%2B%25C8%25A1"
            });
            Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}:\d{1,4}", RegexOptions.IgnoreCase);
            var matchs = regex.Matches(result.Html);
            List<string> ips = new List<string>();
            for (int i = 0; i < matchs.Count; i++)
            {
                ips.Add(matchs[i].Value);
            }
            return ips;
        }
    }
}
