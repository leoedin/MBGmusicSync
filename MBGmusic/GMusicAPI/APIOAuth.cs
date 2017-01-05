using MusicBeePlugin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin.GMusicAPI
{
    public class APIOAuth
    {
        private GPSOAuth gpsoauth;

        private static string ClientId = "565126123933-f27ojtmm7veeb51f8floos7s9vk80i5k";

        private string SJ_URL = "https://mclients.googleapis.com/sj/v1.11/";
        private string SJ_URL_TRACKS = "https://mclients.googleapis.com/sj/v1.11/trackfeed";
        private string STREAM_URL = "https://android.clients.google.com/music/mplay";
        private string SJ_URL_PLAYLISTS = "https://mclients.googleapis.com/sj/v1.11/playlistfeed";

        public APIOAuth()
        {
            gpsoauth = new GPSOAuth();
        }

        #region Sync API functions

        public bool Login(string email, string password)
        {
            bool loggedIn = false;
            Task.Run(async () =>
            {
                loggedIn = await LoginAsync(email, password);
            }).Wait();
            return loggedIn;
        }

        public List<GMusicSong> GetLibrary(int tracksToGet = 0)
        {
            List<GMusicSong> lib = new List<GMusicSong>();
            Task.Run(async () =>
            {
                lib = await GetLibraryAsync(tracksToGet);
            }).Wait();
            return lib;
        }

        public List<GMusicPlaylist> GetPlaylists(int playlistsToGet = 0)
        {
            List<GMusicPlaylist> playlists = new List<GMusicPlaylist>();
            Task.Run(async () =>
            {
                playlists = await GetPlaylistsAsync(playlistsToGet);
            }).Wait();
            return playlists;
        }
       

        #endregion

        #region Async API functions
        /// <summary>
        /// Log in to Google Play Music using OAuth
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="password">password</param>
        /// <returns>boolean indicating success or failure</returns>
        public async Task<bool> LoginAsync(string email, string password)
        {
            Dictionary<String, String> response = await gpsoauth.PerformMasterLogin(email, password);
            if (!response.ContainsKey("Token"))
            {
                Console.WriteLine("Master auth failed");
                return false;
            }

            gpsoauth.MasterToken = response["Token"];

            Dictionary<String, String> oauthResponse = await gpsoauth.PerformOAuth(email, gpsoauth.MasterToken, "sj",
                    "com.google.android.music", ClientId);
            if (!oauthResponse.ContainsKey("Auth"))
            {
                Console.WriteLine("Oauth login failed");
                return false;
            }

            gpsoauth.OAuthToken = oauthResponse["Auth"];

            return true;
        }

        /// <summary>
        /// Gets all tracks in the library
        /// </summary>
        /// <param name="tracksToGet">Number of tracks to fetch</param>
        /// <returns>List of all the songs in the library</returns>
        public async Task<List<GMusicSong>> GetLibraryAsync(int tracksToGet = 0)
        {
            List<GMusicSong> lib = new List<GMusicSong>();
            int totalTracks = 0;
            string nextPageToken = "";
            do
            {
                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>()
                { new KeyValuePair<string, string>("max_results", "5000"),
                    new KeyValuePair<string, string>("start_token", nextPageToken)};


                string responseAsJson = await SendPostAsync(SJ_URL_TRACKS, data);
                GMusicSongs response = ParseTrackObjectsFromJsonList(responseAsJson);
                lib.AddRange(response.Data.AllSongs);
                totalTracks += response.Data.AllSongs.Count;

                if (tracksToGet != 0)
                {
                    if (totalTracks < tracksToGet)
                    {
                        nextPageToken = response.NextPageToken;
                    }
                    else
                    {
                        return lib.Take(tracksToGet).ToList();
                    }
                }

            }
            while (nextPageToken != "");

            return lib;
        }

        /// <summary>
        /// Gets all playlists
        /// </summary>
        /// <param name="playlistsToGet">Number of playlists to get, default is all of them</param>
        /// <returns></returns>
        public async Task<List<GMusicPlaylist>> GetPlaylistsAsync(int playlistsToGet = 0)
        {
            List<GMusicPlaylist> playlists = new List<GMusicPlaylist>();
            int totalPlaylists = 0;
            string nextPageToken = "";
            do
            {
                List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>()
                { new KeyValuePair<string, string>("max_results", "5000"),
                    new KeyValuePair<string, string>("start_token", nextPageToken)};

                string responseAsJson = await SendPostAsync(SJ_URL_PLAYLISTS, data);
                GMusicPlaylists response = ParsePlaylistObjectFromJsonList(responseAsJson);
                playlists.AddRange(response.Data.AllPlaylists);
                totalPlaylists += response.Data.AllPlaylists.Count;
                if (playlistsToGet != 0)
                {
                    if (totalPlaylists < playlistsToGet)
                    {
                        nextPageToken = response.NextPageToken;
                    }
                    else
                    {
                        return playlists.Take(playlistsToGet).ToList();
                    }
                }
            } while (nextPageToken != "");

            return playlists;
        }

        #endregion

        #region Helper Functions

        private async Task<string> SendPostAsync(string url, List<KeyValuePair<string, string>> data)
        {

            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue
                .Parse(String.Format("GoogleLogin auth={0}", gpsoauth.OAuthToken));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string combinedUrl = url;//+ "?alt=json&include-tracks=true&updated-min=0";
            string temp = KeyValuePairsToJson(data); // this string doesnt seem to affect the results...
            HttpContent content = new StringContent(temp, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(combinedUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        private async Task<string> SendGetAsync(string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue
                .Parse(String.Format("GoogleLogin auth={0}", gpsoauth.OAuthToken));

            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }

        private GMusicSongs ParseTrackObjectsFromJsonList(string json)
        {
            return JsonConvert.DeserializeObject<GMusicSongs>(json);
        }

        private GMusicPlaylists ParsePlaylistObjectFromJsonList(string json)
        {
            return JsonConvert.DeserializeObject<GMusicPlaylists>(json);
        }

        private string KeyValuePairsToJson(List<KeyValuePair<string, string>> list)
        {
            StringWriter sw = new StringWriter(new StringBuilder());
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("data");
            writer.WriteStartArray();
            foreach (KeyValuePair<string, string> item in list)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(item.Key);
                writer.WriteValue(item.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();

            return sw.ToString();
        }

        #endregion
    }
}
