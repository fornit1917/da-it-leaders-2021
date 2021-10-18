using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderLib
{
    public interface IFIleDownloader
    {
        void SetDegreeOfParallelism(int degreeOfParallelism);
        void AddFileToDownloadingQueue(string fileId, string url, string pathToSave);
        event Action<string> OnDownloaded;
        event Action<string, Exception> OnFailed;
    }
}
