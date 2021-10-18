namespace FileDownloaderLib
{
    class QueueItem
    {
        public string FileId { get; }
        public string Url { get; }
        public string PathToSave { get; }

        public QueueItem(string fileId, string url, string pathToSave)
        {
            FileId = fileId;
            Url = url;
            PathToSave = pathToSave;
        }
    }
}
