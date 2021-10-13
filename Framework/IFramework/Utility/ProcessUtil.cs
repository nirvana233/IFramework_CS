using System;
using System.Diagnostics;

namespace IFramework.Utility
{
    /// <summary>
    /// 进程工具
    /// </summary>
    public static class ProcessUtil
    {
        /// <summary>
        /// 创建进程
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="args"></param>
        /// <param name="workingDir"></param>
        /// <returns></returns>
        public static Process CreateProcess(string cmd, string args, string workingDir = "")
        {
            var pStartInfo = new ProcessStartInfo(cmd);
            pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = false;
            pStartInfo.UseShellExecute = true;
            pStartInfo.RedirectStandardError = false;
            pStartInfo.RedirectStandardInput = false;
            pStartInfo.RedirectStandardOutput = false;
            if (!string.IsNullOrEmpty(workingDir))
            {
                pStartInfo.WorkingDirectory = workingDir;
            }
            return Process.Start(pStartInfo);
        }
        /// <summary>
        /// 进程输出Log
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isThrowExcpetion"></param>
        public static void OuputLog(this Process p, bool isThrowExcpetion)
        {
            string standardError = string.Empty;
            p.BeginErrorReadLine();

            p.ErrorDataReceived += (sender, outLine) =>
            {
                standardError += outLine.Data;
            };

            string standardOutput = string.Empty;
            p.BeginOutputReadLine();
            p.OutputDataReceived += (sender, outLine) =>
            {
                standardOutput += outLine.Data;
            };

            p.WaitForExit();
            p.Close();

            Log.L(standardOutput);
            if (standardError.Length > 0)
            {
                if (isThrowExcpetion)
                {
                    Log.E(standardError);
                    throw new Exception(standardError);
                }

                Log.E(standardError);
            }
        }
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFloder(string path)
        {
            Process.Start("explorer.exe", path.Replace("/", "\\"));
        }
    }

}
