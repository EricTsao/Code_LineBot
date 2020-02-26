using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LineBot.Models
{
    public class YoutubeWebApiHelper
    {
        public static SearchVideosResponse SearchVideos(string query)
        {
            /*
            GET https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=5&order=relevance&q=%E6%BA%AB%E6%9F%94&type=video&videoEmbeddable=true&key=[YOUR_API_KEY] HTTP/1.1

            Authorization: Bearer [YOUR_ACCESS_TOKEN]
            Accept: application/json
            */

            using (HttpClient client = new HttpClient())
            {
                var apiKey = "AIzaSyAdYCQF2xP6rhU9EnsIwjnbwqzdpGF1P2Y";
                var url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&maxResults=10&topicId=%2Fm%2F04rlf&order=relevance&q={query}&type=video&videoEmbeddable=true&key={apiKey}";

                try
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = client.GetAsync(url).Result;

                    //nesekmes atveju error..
                    response.EnsureSuccessStatusCode();

                    //responsas to string
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    var responseObj = JsonConvert.DeserializeObject<SearchVideosResponse>(responseBody);

                    return responseObj;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);

                    throw e;
                }
            }
        }
    }

    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class Id
    {
        public string kind { get; set; }
        public string videoId { get; set; }
    }

    public class YoutubeImgInfo
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Thumbnails
    {
        public YoutubeImgInfo @default { get; set; }
        public YoutubeImgInfo medium { get; set; }
        public YoutubeImgInfo high { get; set; }
    }

    public class Snippet
    {
        public DateTime publishedAt { get; set; }
        public string channelId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Thumbnails thumbnails { get; set; }
        public string channelTitle { get; set; }
        public string liveBroadcastContent { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public Id id { get; set; }
        public Snippet snippet { get; set; }
    }

    public class SearchVideosResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public string regionCode { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }
}
