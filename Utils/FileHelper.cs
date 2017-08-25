using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Utils
{
    public class FileHelper
    {
        public static string ExcuteBatFile(string batPath, ref string errMsg)
        {
            if (errMsg == null) throw new ArgumentNullException("errMsg");
            string output;
            using (Process process = new Process())
            {
                FileInfo file = new FileInfo(batPath);
                if (file.Directory != null)
                {
                    process.StartInfo.WorkingDirectory = file.Directory.FullName;
                }
                process.StartInfo.FileName = batPath;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                //process.WaitForExit();
                output = process.StandardOutput.ReadToEnd();
                errMsg = process.StandardError.ReadToEnd();
            }
            return output;
        }
        /// <summary>
        /// 运行cmd命令
        /// 会显示命令窗口
        /// </summary>
        /// <param name="cmdExe">指定应用程序的完整路径</param>
        /// <param name="cmdStr">执行命令行参数</param>
        public static void RunCmd(string cmdExe, string cmdStr)
        {
            try
            {
                using (Process myPro = new Process())
                {
                    //指定启动进程是调用的应用程序和命令行参数
                    ProcessStartInfo psi = new ProcessStartInfo(cmdExe, cmdStr);
                    myPro.StartInfo = psi;
                    myPro.Start();
                    myPro.WaitForExit();
                }
            }
            catch { }
        }
        public static string WriteFile(string input, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "写入的文件名不能为空";
            }
            try
            {
                var savePath = fileName;
                System.IO.File.WriteAllText(savePath, input, System.Text.Encoding.UTF8);
            }
            catch
            {
                return null;
            }
            return string.Empty;
        }
        public static string ReadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }
            try
            {
                var result = string.Empty;
                var finfo = new FileInfo(fileName);
                using (var fs = finfo.OpenRead())
                {
                    using (var r = new StreamReader(fs))
                    {
                        result = r.ReadToEnd();
                    }
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}