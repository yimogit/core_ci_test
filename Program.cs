using System;
using System.Collections.Generic;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.IO;
using System.Drawing.Imaging;

namespace core_ci_test
{
    class Program
    {

        static Dictionary<string, string> pagesConfig = new Dictionary<string, string>() { };
        static Program()
        {
            pagesConfig.Add("cnblogs", "http://www.cnblogs.com/");
            pagesConfig.Add("baidu", "http://www.baidu.com/");
            pagesConfig.Add("segmentfault", "https://segmentfault.com");
            pagesConfig.Add("github", "https://github.com/");
            pagesConfig.Add("google", "https://www.google.com");
            pagesConfig.Add("facebook", "http://www.facebook.com");
            pagesConfig.Add("youtube", "http://www.youtube.com/");
        }
        static void Main(string[] args)
        {
            using (var driver = new PhantomJSDriver())
            {
                driver.Manage().Window.Maximize();
                foreach (var item in pagesConfig)
                {
                    try
                    {
                        Console.WriteLine($"开始前往{item.Key}首页:{item.Value}");
                        var saveDir = $"SaveImgs/{item.Key}";
                        var savePath = $"{saveDir}/{DateTime.Now.ToString("yyyy_MM_dd")}.jpg";
                        if (!Directory.Exists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }
                        driver.Navigate().GoToUrl(item.Value);
                        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(savePath, ScreenshotImageFormat.Jpeg);
                        Console.WriteLine($"图片保存至：{savePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Key + "" + ex.Message);
                    }
                }
                driver.Quit();
            }
        }
    }
}
