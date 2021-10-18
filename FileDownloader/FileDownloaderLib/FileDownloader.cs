using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FileDownloaderLib
{
    public class FileDownloader : IFIleDownloader
    {
        public event Action<string> OnDownloaded;
        public event Action<string, Exception> OnFailed;

        private int _maxThreads = 4;

        private object _lockObject = new object();

        private ConcurrentQueue<QueueItem> _queue = new ConcurrentQueue<QueueItem>();

        private int _currentThreads = 0;

        private bool _isStarted = false;

        private HttpClient _httpClient = new HttpClient();

        public void SetDegreeOfParallelism(int degreeOfParallelism)
        {
            if (_isStarted)
            {
                throw new Exception("It's not allowed to set degree of parallelist after file added to queue");
            }
            _maxThreads = degreeOfParallelism;
        }

        public void AddFileToDownloadingQueue(string fileId, string url, string pathToSave)
        {
            _isStarted = true;
            _queue.Enqueue(new QueueItem(fileId, url, pathToSave));
            lock (_lockObject)
            {
                if (_currentThreads < _maxThreads)
                {
                    Task.Run(DownloadFiles);
                    _currentThreads++;
                }
            }
        }

        private async Task DownloadFiles()
        {
            Console.WriteLine("------- Thread was started");
            while (!_queue.IsEmpty)
            {
                if (_queue.TryDequeue(out QueueItem item))
                {
                    try
                    {
                        using Stream inputStream = await _httpClient.GetStreamAsync(item.Url);
                        using FileStream outputStream = File.OpenWrite(item.PathToSave);
                        await inputStream.CopyToAsync(outputStream);
                        OnDownloaded(item.FileId);
                    }
                    catch (Exception e)
                    {
                        OnFailed(item.FileId, e);
                    }
                }
            }

            lock (_lockObject)
            {
                _currentThreads--;
            }

            Console.WriteLine("------- Thread was finished");
        }
    }
}
