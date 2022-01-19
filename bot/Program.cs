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
            //call1();
            
            Console.WriteLine("YouTube Data API: Search");
            Console.WriteLine("========================");

            try
            {
                new Program().Run().Wait();
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
                //var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyCT8kXaxJ2l29vYg4HBdYy36H-PhAH-Teg" });
                var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = "AIzaSyCkUxp6ucqJa - 18XS19iv8q46lKHSQzfIA" });

                var searchListRequest = yt.Search.List("snippet");
                searchListRequest.Q = "boxing music"; // Replace with your search term.
                searchListRequest.MaxResults = 50;

                // Call the search.list method to retrieve results matching the specified query term.
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videos = new List<string>();
                List<string> channels = new List<string>();
                List<string> playlists = new List<string>();

                // Add each result to the appropriate list, and then display the lists of
                // matching videos, channels, and playlists.
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            //videos.Add(String.Format("{0} ({1}) url:{2}", searchResult.Snippet.Title, searchResult.Id.VideoId, "https://www.youtube.com/watch?v=" + searchResult.Id.VideoId));                            
                            videos.Add("https://www.youtube.com/watch?v=" + searchResult.Id.VideoId);
                            break;

                        case "youtube#channel":
                            channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                            break;

                        case "youtube#playlist":
                            playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                            break;
                    }
                }

                //Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
                Console.WriteLine("fetching videos.. please wait");
                var source = @"c:\Users\themhz\Music\new\";
                var youtube = YouTube.Default;
                foreach (var video in videos)
                {
                                       
                    try
                    {
                        var vid = youtube.GetVideo(video);
                        Console.WriteLine("Fetching "+ video);
                        File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                        var inputFile = new MediaFile { Filename = source + vid.FullName };
                        var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

                        using (var engine = new Engine())
                        {
                            engine.GetMetadata(inputFile);

                            engine.Convert(inputFile, outputFile);
                        }

                        Console.WriteLine("Completed ");
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("error fetching " + video + "   moving to next video..");
                        //continue;
                    }
                    
                }
                //Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
                //Console.WriteLine(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists)));            
            }
            catch (Exception e)
            {
                Console.WriteLine("Some exception occured" + e);

                
            }
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
