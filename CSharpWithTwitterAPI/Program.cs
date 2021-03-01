using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tweetinvi.Models;
using System.Text;
using Tweetinvi.Parameters;
using Tweetinvi;
using Newtonsoft.Json;

namespace CSharpWithTwitterAPI
{
    class Program
    {
        private static string ApiKey = "apikey";
        private static string ApiSecret = "apisecret";
        private static string AccessToken = "accesstoken";
        private static string AccessSecret = "accesssecret";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("********| TwitterAPI Kullanımı |********");
            Console.WriteLine("Resimli veya resimsiz tweet atmak için seçiniz. (Resimli - Resimsiz) : ");
            string withPicture = Console.ReadLine();
            if(withPicture.ToLower() == "resimli")
            {
                Console.Write("Atılıcak Tweeti Giriniz : ");
                string tweet = Console.ReadLine();
                Console.Write("Paylaşılacak Resmi Giriniz : ");
                string path = Console.ReadLine();
                TweetWithImage(tweet, path);
            }
            if(withPicture.ToLower() == "resimsiz")
            {
                Console.Write("Atılıcak Tweeti Giriniz : ");
                string tweet = Console.ReadLine();
                TweetText(tweet);
            }
            
            
        }

        static async Task TweetWithImage(string text, string path)
        {
            byte[] ImageBytes = File.ReadAllBytes(path);
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            IMedia ImageIMedia = await client.Upload.UploadTweetImageAsync(ImageBytes);
            ITweet tweet = await client.Tweets.PublishTweetAsync(new PublishTweetParameters(text) { Medias = { ImageIMedia } });
        }
        static async Task TweetText(string text)
        {
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            ITweet tweet = await client.Tweets.PublishTweetAsync(text);
        }
    }

        
}
