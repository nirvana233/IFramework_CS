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
using System.Net;
namespace IFramework.Net
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
    public class FileDownLoader_Http : FileDownLoader
    {
        private FileStream fileStream;
        private Stream stream;
        private HttpWebResponse response;
        private string tempFileExt = ".temp";
        /// 临时文件全路径
        private string tempSaveFilePath;

        private long _currentLength;
        public override long currentLength { get { return _currentLength; } }
        public virtual long FileLength { get; private set; }
        public override float progress
        {
            get
            {
                if (FileLength > 0)
                    return Math.Max(Math.Min(1, (float)currentLength / FileLength), 0);
                return 0;
            }
        }


        public FileDownLoader_Http(string url, string SaveDir) : base(url, SaveDir)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", SaveDir, fileNameWithoutExt, tempFileExt);
        }
        public override void DownLoad()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            if (File.Exists(tempSaveFilePath))
            {
                //若之前已下载了一部分，继续下载
                fileStream = File.OpenWrite(tempSaveFilePath);
                _currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current);
                //设置下载的文件读取的起始位置
                request.AddRange((int)currentLength);
            }
            else
            {
                //第一次下载
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                _currentLength = 0;
            }

            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
           
            //总的文件大小=当前需要下载的+已下载的
            FileLength = response.ContentLength + currentLength;

            downLoading = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < FileLength)
            {
                byte[] buffer = new byte[bufferMaxLength];
                if (!stream.CanRead) break;
                //读写操作
                lengthOnce = stream.Read(buffer, 0, buffer.Length);
                _currentLength += lengthOnce;
                fileStream.Write(buffer, 0, lengthOnce);
            }
            downLoading = false;
            response.Close();
            stream.Close();
            fileStream.Close();
            //临时文件转为最终的下载文件
            if (File.Exists(saveFilePath))
                File.Delete(saveFilePath);
            File.Move(tempSaveFilePath, saveFilePath);
            Compelete();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (response!=null) response.Close();
            if (stream!=null) stream.Close();
            if (fileStream!=null) fileStream.Close();
        }
    }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
}