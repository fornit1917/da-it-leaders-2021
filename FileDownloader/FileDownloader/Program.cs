using FileDownloaderLib;
using System;
using System.IO;
using System.Threading;

namespace FileDownloaderApp
{
    class Program
    {
        private static object _lockObject = new object();

        private static int _totalFiles = 0;
        private static int _downloadedFiles = 0;
        private static int _failedFiles = 0;

        static void Main(string[] args)
        {
            IFIleDownloader fileDownloader = new FileDownloader();
            fileDownloader.OnDownloaded += OnDownloaded;
            fileDownloader.OnFailed += OnFailed;

            string pathToList = "c:\\tmp\\fileDownloader\\list.txt";
            using (StreamReader reader = new StreamReader(pathToList))
            {
                string url;
                int i = 0;
                while ((url = reader.ReadLine()) != null)
                {
                    _totalFiles++;
                    i++;
                    string fileId = i.ToString();
                    string pathToSave = $"c:\\tmp\\fileDownloader\\files\\{i.ToString("D2")}.jpg";

                    fileDownloader.AddFileToDownloadingQueue(fileId, url, pathToSave);

                    if (i == 20)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }

            Console.ReadLine();
        }

        private static void OnDownloaded(string fileId)
        {
            lock (_lockObject)
            {
                _downloadedFiles++;
                Console.WriteLine($"Downloaded {_downloadedFiles} / {_totalFiles}");
                if (_downloadedFiles + _failedFiles == _totalFiles)
                {
                    Console.WriteLine("Dowloading if finished");
                }
            }
        }

        private static void OnFailed(string fileId, Exception e)
        {
            lock (_lockObject)
            {
                _failedFiles++;
                Console.WriteLine($"Error, file {fileId} not downloaded: ${e.Message}");
                if (_downloadedFiles + _failedFiles == _totalFiles)
                {
                    Console.WriteLine("Dowloading if finished");
                }
            }
        }
    }
}
