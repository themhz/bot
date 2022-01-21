using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VideoLibrary;
using MediaToolkit;
using MediaToolkit.Model;

namespace bot
{

    //Reference
    //https://developers.google.com/youtube/v3/docs/search/list
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //var directory = @"c:\Users\themhz\Music\new\";
            var directory = @"C:\Users\themis\Music\";
            YouTubeMp3 ytmp3 = new YouTubeMp3("AIzaSyCkUxp6ucqJa - 18XS19iv8q46lKHSQzfIA", directory, "boxing music");            
            ytmp3.Execute();
        }
        

        static void call1()
        {
            try
            {
                //var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyCT8kXaxJ2l29vYg4HBdYy36H-PhAH-Teg" });
                var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyCkUxp6ucqJa - 18XS19iv8q46lKHSQzfIA" });

                var channelsListRequest = yt.Channels.List("contentDetails");
                channelsListRequest.ForUsername = "mipaltan";
                var channelsListResponse = channelsListRequest.Execute();
                int VideoCount = 1;
                foreach (var channel in channelsListResponse.Items)
                {
                    var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
                    var nextPageToken = "";
                    while (nextPageToken != null)
                    {
                        var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
                        playlistItemsListRequest.PlaylistId = uploadsListId;
                        playlistItemsListRequest.MaxResults = 50;
                        playlistItemsListRequest.PageToken = nextPageToken;
                        // Retrieve the list of videos uploaded to the authenticated user's channel.  
                        var playlistItemsListResponse = playlistItemsListRequest.Execute();
                        foreach (var playlistItem in playlistItemsListResponse.Items)
                        {
                            Console.WriteLine("Sl No={0}", VideoCount);
                            Console.Write("Video ID ={0} ", "https://www.youtube.com/embed/" + playlistItem.Snippet.ResourceId.VideoId);
                            //Console.Write("Video Title ={0} ", playlistItem.Snippet.Title);  
                            //Console.Write("Video Descriptions = {0}", playlistItem.Snippet.Description);  
                            //Console.WriteLine("Video ImageUrl ={0} ", playlistItem.Snippet.Thumbnails.High.Url);  
                            //Console.WriteLine("----------------------");  
                            VideoCount++;
                        }
                        nextPageToken = playlistItemsListResponse.NextPageToken;
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Some exception occured" + e);
            }
        }

    }
}
