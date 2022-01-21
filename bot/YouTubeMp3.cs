using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.IO;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;
using Google.Apis.Util;
using Google.Apis.YouTube.v3.Data;
using static Google.Apis.YouTube.v3.SearchResource;

namespace bot
{
    class YouTubeMp3
    {
        public String url { get; set; }

        public String youTubeApiKey { get; set; }

        public String filePathToSaveMp3 { get; set; }

        public String search { get; set; }

        public YouTubeService yt { get; set; }

        public ListRequest searchListRequest { get; set; }

        public SearchListResponse searchListResponse { get; set; }

        public List<string> videoResults { get; set; }

        public YouTubeMp3(String youTubeApiKey, String filePathToSaveMp3, String search, int maxResults = 3)
        {            
            this.youTubeApiKey = youTubeApiKey;
            this.filePathToSaveMp3 = filePathToSaveMp3;
            this.search = search;          
            this.yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = this.youTubeApiKey });
            this.searchListRequest = this.yt.Search.List("snippet");
            this.searchListRequest.Q = search;
            this.searchListRequest.MaxResults = maxResults;
            this.videoResults = new List<string> { };
        }

        public void Execute()
        {           
            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            try
            {
                    this.Run().Wait();
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
        
       

        async Task Run()
        {
            try
            {
                await this.startTheSearch();                
                this.getResult();
                this.copyVideosOnDisk();
                this.convertToMp3();
                               
            }
            catch (Exception e)
            {
                Console.WriteLine("Some exception occured" + e);


            }
        }

        public YouTubeVideo getVideo(String video)
        {
            return YouTube.Default.GetVideo(video);
        }
        public void convertToMp3()
        {            
            foreach (var video in this.videoResults)
            {

                try
                {
                    Console.WriteLine("converting to mp3,  " + video);
                    var vid = this.getVideo(video);
                    
                    var inputFile = new MediaFile { Filename = this.filePathToSaveMp3 + vid.FullName };
                    var outputFile = new MediaFile { Filename = $"{this.filePathToSaveMp3 + vid.FullName}.mp3" };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);

                        engine.Convert(inputFile, outputFile);
                    }

                    Console.WriteLine("Completed ");
                }
                catch (Exception e)
                {
                    Console.WriteLine("error fetching " + video + "   moving to next video..");                    
                }

            }
        }
        public void copyVideosOnDisk()
        {
            foreach (var video in this.videoResults)
            {

                try
                {
                    Console.WriteLine("Fetching " + video);
                    var vid = this.getVideo(video);                    
                    File.WriteAllBytes(this.filePathToSaveMp3 + vid.FullName, vid.GetBytes());                    

                    Console.WriteLine("Completed ");
                }
                catch (Exception e)
                {
                    Console.WriteLine("error fetching " + video + "   moving to next video..");
                    //continue;
                }

            }
        }
        public List<string> getResult()
        {
            Console.WriteLine("fetching videos.. please wait");
            foreach (var searchResult in this.searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    this.videoResults.Add("https://www.youtube.com/watch?v=" + searchResult.Id.VideoId);
                }
            }

            return this.videoResults;
        }

        async Task startTheSearch()
        {
            // Call the search.list method to retrieve results matching the specified query term.
            this.searchListResponse = await searchListRequest.ExecuteAsync();
        }

    }
}
