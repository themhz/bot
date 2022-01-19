using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bot
{
    class YouTubeMp3
    {
        private WebClient downloadMp3 = new WebClient();

        public long bytesDownloaded = 0;
        public long totalBytes = 0;

        public YouTubeMp3(string youtubeUrl, string directory)
        {
            downloadMp3.DownloadProgressChanged += downloadMp3_DownloadProgressChanged;
            downloadMp3.DownloadFileCompleted += downloadMp3_DownloadFileCompleted;

            downloadMp3.DownloadFileAsync(new Uri(string.Format("http://youtubeinmp3.com/fetch/?video={0}", youtubeUrl)), directory);
        }

        private void downloadMp3_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            downloadMp3.Dispose();
        }

        private void downloadMp3_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            bytesDownloaded = e.BytesReceived;
            totalBytes = e.TotalBytesToReceive;
        }
    }
}
