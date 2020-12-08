using System;
using System.Diagnostics;

namespace IFramework.Utility
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public static class ProcessUtil
    {
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
        public static void OpenFloder(string path)
        {
            Process.Start("explorer.exe", path.Replace("/", "\\"));
        }
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
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

}
