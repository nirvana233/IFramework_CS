/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public abstract class FileDownLoader : IDisposable
    {
        public event Action onCompeleted;
        protected void Compelete()
        {
            if (onCompeleted!=null)
            {
                onCompeleted();
            }
        }

        public abstract float progress { get; }
        public virtual long currentLength { get; }

        public string url { get; private set; }
        public string saveDir { get; private set; }
        public string fileExt { get; private set; }


        public bool downLoading { get; protected set; }
        public string fileNameWithoutExt { get; protected set; }
        public string saveFilePath { get; protected set; }

        public FileDownLoader(string url, string SaveDir)
        {
            this.url = url;
            this.saveDir = SaveDir;
            downLoading = false;
            fileExt = Path.GetExtension(url);
            fileNameWithoutExt = Path.GetFileNameWithoutExtension(url)
                                                    .Replace(".", "")
                                                    .Replace("=", "")
                                                    .Replace("%", "")
                                                    .Replace("&", "")
                                                    .Replace("?", "");
            saveFilePath = string.Format("{0}/{1}{2}", SaveDir, fileNameWithoutExt, fileExt);
            if (string.IsNullOrEmpty(this.url) || string.IsNullOrEmpty(SaveDir) || string.IsNullOrEmpty(saveFilePath))
                throw new Exception(GetType().Name + " Ctor  Err");
        }

        
        public abstract void DownLoad();
        public virtual void Dispose()
        {
            onCompeleted = null;
            downLoading = false;
        }

    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}