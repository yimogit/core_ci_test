using System;
using System.Collections.Generic;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.IO;
using System.Drawing.Imaging;
using System.Text;

namespace core_ci_test
{
    class Program
    {

        static Dictionary<string, string> pagesConfig = new Dictionary<string, string>() { };
        static Program()
        {
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
                var saveDir = $"SaveImgs/";
                StringBuilder builder = new StringBuilder();
                foreach (var item in pagesConfig)
                {
                    try
                    {
                        Console.WriteLine($"开始前往{item.Key}首页:{item.Value}");
                        if (!Directory.Exists(saveDir))
                        {
                            Directory.CreateDirectory(saveDir);
                        }
                        driver.Navigate().GoToUrl(item.Value);
                        var saveName = $"{item.Key}.jpg";
                        var savePath= $"{ saveDir }/{ saveName}";
                        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(savePath, ScreenshotImageFormat.Jpeg);
                        builder.AppendLine($"### {item.Key}");
                        builder.AppendLine($"![图片](./{saveName})");
                        Console.WriteLine($"图片保存至：{savePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(item.Key + "" + ex.Message);
                    }
                }
                //创建MD文件
                Utils.FileHelper.WriteFile(builder.ToString(), $"{saveDir}/README.MD");
                driver.Quit();
            }
        }
    }
}
