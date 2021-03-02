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
            Console.WriteLine("***************| TwitterAPI Kullanımı |***************");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("1 | Tweet Ara");
            Console.WriteLine("2 | Tweet Paylaş");
            Console.WriteLine("3 | Anasayfa Tweetlerini Görüntüle");
            Console.WriteLine("4 | Bir Kullanıcı'nın Tweetlerini Listele");
            Console.WriteLine("5 | Bir Kullanıcı'nın Takip Ettiği Kişileri Listele");
            Console.WriteLine("6 | Bir Kullanıcı'nın Takipçilerini Listele");
            Console.WriteLine("------------------------------------------------------");
            Console.Write("Lütfen Bir İşlem Seçiniz (1 - 6) : ");
            int methodSelector = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("\n \n");
            if (methodSelector == 1)
            {
                Console.Write("Aramak İstediğiniz Tweet Parametresini Giriniz :  ");
                string searchParam = Console.ReadLine();
                await GetTweetsWithKeyword(searchParam);
            }
            else if(methodSelector == 2)
            {
                Console.Write("Resimli veya resimsiz tweet atmak için seçiniz. (Resimli - Resimsiz) : ");
                string withPicture = Console.ReadLine();
                if (withPicture.ToLower() == "resimli")
                {
                    Console.Write("Atılıcak Tweeti Giriniz : ");
                    string tweet = Console.ReadLine();
                    Console.Write("Paylaşılacak Resmi Giriniz : ");
                    string path = Console.ReadLine();
                    await TweetWithImage(tweet, path);
                }
                else if (withPicture.ToLower() == "resimsiz")
                {
                    Console.Write("Atılıcak Tweeti Giriniz : ");
                    string tweet = Console.ReadLine();
                    await TweetText(tweet);
                }
                else
                {
                    Console.WriteLine("Geçerli Bir Komut Giriniz!");
                }
            }
            else if(methodSelector == 3)
            {
                Console.WriteLine("Anasayfa Tweetleri Gösteriliyor");
                string timelineUser = Console.ReadLine();
                
                await GetTimeline();
            }
            else if(methodSelector == 4)
            {
                Console.Write("Tweetlerini Listelemek İstediğiniz Kullanıcının Kullanıcı Adı'nı Giriniz : ");
                string timelineUser = Console.ReadLine();

                await GetUserTimeline(timelineUser);
            }
            else if(methodSelector == 5)
            {
                Console.Write("Takip Ettiği Kişileri Gösterilecek Kullanıcı'nın Kullanıcı Adı : ");
                string listfriends = Console.ReadLine();

                await ListFriends(listfriends);
            }
            else if(methodSelector == 6)
            {
                Console.Write("Kullanıcı'yı Takip Edenleri Görmek istediğiniz kullanıcı'nın Kullanıcı Adı : ");
                string userfollowers = Console.ReadLine();

                await UserFollowers(userfollowers);
            }
            else
            {
                Console.WriteLine("Geçerli Bir Komut Giriniz!");
            }

            Console.ReadKey();

        }

        static async Task UserFollowers(string username)
        {
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            var followerIds = await client.Users.GetFollowerIdsAsync(username);
            foreach (var follower in followerIds)
            {
                var userResponse = await client.UsersV2.GetUserByIdAsync(follower);
                Console.WriteLine(userResponse.User.Name + " " + userResponse.User.Url);
            }
        }

        static async Task ListFriends(string username)
        {
            TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
            var friendIds = await client.Users.GetFriendIdsAsync(username);
            foreach(var friend in friendIds)
            {
                var userResponse = await client.UsersV2.GetUserByIdAsync(friend);
                Console.WriteLine(userResponse.User.Name + " " + userResponse.User.Url);
            }

        }

        static async Task GetUserTimeline(string username)
        {
            TwitterClient userClient = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);

            var homeTimelineTweets = await userClient.Timelines.GetUserTimelineAsync(username);
            foreach (var tweets in homeTimelineTweets)
            {

                Console.WriteLine("Tweet : " + tweets.Text);
            }
        }
        static async Task GetTimeline()
        {
            TwitterClient userClient = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);

            var homeTimelineTweets = await userClient.Timelines.GetHomeTimelineAsync();
            foreach(var tweets in homeTimelineTweets)
            {
                Console.WriteLine("Kullanıcı : "+tweets.CreatedBy.ToString());
                Console.Write("Tweet : " + tweets.Text + "\n");
            }
        }

        static async Task TweetWithImage(string text, string path)
        {
            try
            {
                byte[] ImageBytes = File.ReadAllBytes(path);
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                IMedia ImageIMedia = await client.Upload.UploadTweetImageAsync(ImageBytes);
                ITweet tweet = await client.Tweets.PublishTweetAsync(new PublishTweetParameters(text) { Medias = { ImageIMedia } });
                Console.WriteLine("Başarıyla Tweetlendi.");
            }
            catch
            {
                Console.WriteLine("Tweet Paylaşılamadı.");
            }

        }
        static async Task TweetText(string text)
        {
            try
            {
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                ITweet tweet = await client.Tweets.PublishTweetAsync(text);
                Console.WriteLine("Başarıyla Tweetlendi.");
            }
            catch
            {
                Console.WriteLine("Tweet Paylaşılamadı.");
            }

        }

        public static async Task GetTweetsWithKeyword(string keyword)
        {
            try
            {
                TwitterClient client = new TwitterClient(ApiKey, ApiSecret, AccessToken, AccessSecret);
                var searchResults = await client.SearchV2.SearchTweetsAsync(keyword);
                var tweets = searchResults.Tweets;
                Console.Write("Bilgiler JSON Dosyasına Yazılsın mı? (Evet - Hayır) : ");
                string jsonWrite = Console.ReadLine();
                if(jsonWrite.ToLower() == "evet")
                {
                    try
                    {
                        string convertJson = JsonConvert.SerializeObject(tweets);
                        File.WriteAllText("files/TweetDetails.json", convertJson);
                        Console.WriteLine("Bilgiler başarıyla JSON dosyasına aktarıldı.");
                    }
                    catch
                    {
                        Console.WriteLine("Hata!");
                    }
                }
                else if(jsonWrite.ToLower() == "hayır" || jsonWrite.ToLower() == "hayir")
                {
                    foreach (var tweet in tweets)
                    {
                        Console.WriteLine("Kullanıcı Id : " + tweet.AuthorId.ToString());
                        Console.WriteLine("Tweet : " + tweet.Text.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Evet veya Hayır parametrelerinden birini girmelisiniz!");
                }
                
            }
            catch
            {
                Console.Write("Hata!");
            }
            
        }
    }


}
