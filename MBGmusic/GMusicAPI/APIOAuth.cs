using Mono.Web;
using MusicBeePlugin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        // old client id 565126123933-f27ojtmm7veeb51f8floos7s9vk80i5k
        // Client id from gmusicapi
        private static string ClientId = "565126123933-f27ojtmm7veeb51f8floos7s9vk80i5k";

        private static string SJ_URL = "https://mclients.googleapis.com/sj/v2.5/";
        private static string SJ_URL_TRACKS = "https://mclients.googleapis.com/sj/v2.5/trackfeed";
        private static string STREAM_URL = "https://android.clients.google.com/music/mplay";
        private static string SJ_STREAM_URL = "https://mclients.googleapis.com/music/";
        private static string SJ_URL_PLAYLISTS_FEED = "https://mclients.googleapis.com/sj/v2.5/playlistfeed";
        private static string SJ_URL_PLAYLISTS_BATCH = "https://mclients.googleapis.com/sj/v2.5/playlistbatch";
        private static string SJ_URL_PLAYLISTS_ENTRY_FEED = "https://mclients.googleapis.com/sj/v2.5/plentryfeed";
        private static string SJ_URL_PLAYLIST_ENTRIES_BATCH = "https://mclients.googleapis.com/sj/v2.5/plentriesbatch";

        private static string BASE_URL = "https://play.google.com/music/";
        private static string SERVICE_URL = BASE_URL + "services/";
        private static string CREATE_PLAYLIST_SERVICE_URL= SERVICE_URL + "createplaylist";

        public enum ShareState { PUBLIC, PRIVATE}

        public APIOAuth()
        {
            gpsoauth = new GPSOAuth();
        }

        #region Public Helper functions

        public Boolean LoggedIn()
        {
            return !String.IsNullOrEmpty(gpsoauth.OAuthToken);
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
            string nextPageToken = null;
            do
            {
                JObject requestData = new JObject()
                {
                    { "max-results", "20000" },
                    { "start_token", nextPageToken }
                };

                string responseAsJson = await SendPostAsync(SJ_URL_TRACKS, requestData);
                GMusicSongs response = JsonConvert.DeserializeObject<GMusicSongs>(responseAsJson);
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
                else
                {
                    nextPageToken = response.NextPageToken;
                }
            }
            while (nextPageToken != null);

            return lib;
        }

        /// <summary>
        /// Gets all playlists
        /// </summary>
        /// <param name="playlistsToGet">Number of playlists to get, default is all of them</param>
        /// <returns>List of all playlists in the library</returns>
        public async Task<List<GMusicPlaylist>> GetPlaylistsAsync(int playlistsToGet = 0)
        {
            List<GMusicPlaylist> playlists = new List<GMusicPlaylist>();
            int totalPlaylists = 0;
            string nextPageToken = null;
            do
            {
                JObject requestData = new JObject()
                {
                    { "max-results", "20000" },
                    { "start_token", nextPageToken }
                };

                string responseAsJson = await SendPostAsync(SJ_URL_PLAYLISTS_FEED, requestData);
                GMusicPlaylists response = JsonConvert.DeserializeObject<GMusicPlaylists>(responseAsJson);
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
                else
                {
                    nextPageToken = response.NextPageToken;
                }
            } while (nextPageToken != null);

            return playlists;
        }

        /// <summary>
        /// Gets all playlist entries
        /// </summary>
        /// <param name="entriesToGet">Numer of playlist entries to get, default is all of them</param>
        /// <returns></returns>
        public async Task<List<GMusicPlaylistEntry>> GetPlaylistEntriesAsync(int entriesToGet = 0)
        {
            List<GMusicPlaylistEntry> playlists = new List<GMusicPlaylistEntry>();
            int totalPlaylists = 0;
            string nextPageToken = "";
            do
            {
                JObject requestData = new JObject()
                { { "data" , new JArray()
                    { new JObject()
                        {
                            { "max_results", "5000" },
                            {"start_token", nextPageToken }
                        }
                    }
                } };

                string responseAsJson = await SendPostAsync(SJ_URL_PLAYLISTS_ENTRY_FEED, requestData);
                GMusicPlaylistEntryResponse response = JsonConvert.DeserializeObject<GMusicPlaylistEntryResponse>(responseAsJson);
                playlists.AddRange(response.Data.AllPlaylistEntries);
                totalPlaylists += response.Data.AllPlaylistEntries.Count;

                if (entriesToGet != 0)
                {
                    if (totalPlaylists < entriesToGet)
                    {
                        nextPageToken = response.NextPageToken;
                    }
                    else
                    {
                        return playlists.Take(entriesToGet).ToList();
                    }
                }
            } while (nextPageToken != "");

            return playlists;
        }
      
        /// <summary>
        /// Gets all playlists and entries and matches the playlists.songs with the entries
        /// </summary>
        /// <returns></returns>
        public async Task<List<GMusicPlaylist>> GetPlaylistsWithEntriesAsync()
        {
            List<GMusicPlaylist> playlists = await GetPlaylistsAsync();
            List<GMusicPlaylistEntry> entries = await GetPlaylistEntriesAsync();

            foreach (GMusicPlaylist playlist in playlists)
            {
                playlist.Songs = entries.Where(s => s.PlaylistID == playlist.ID).ToList();
                playlist.Songs.OrderBy(s => s.AbsolutePosition);
            }
            return playlists;
        }

        /// <summary>
        /// Creates a playlist
        /// </summary>
        /// <param name="name">Name of the playlist to be created</param>
        /// <param name="description">Description of the playlist to be created</param>
        /// <param name="shareState">If the playlist will be PUBLIC or PRIVATE</param>
        /// <returns></returns>
        public async Task<MutatePlaylistResponse> CreatePlaylistAsync(string name, string description = null, ShareState shareState = ShareState.PRIVATE)
        {
            JObject requestData = new JObject()
            { { "mutations" , new JArray()
                { new JObject()
                    { { "create" , new JObject()
                        {
                            {"name", name},
                            {"deleted", false},
                            {"creationTimestamp", "-1"},
                            {"lastModifiedTimestamp","0"},
                            {"type", "USER_GENERATED"},
                            {"shareState", shareState == ShareState.PRIVATE ? "PRIVATE" : "PUBLIC" },
                            {"description", description}
                        }
                    } }
                }
            } };

            return await PerformMutatePlaylistRequest(SJ_URL_PLAYLISTS_BATCH, requestData);
        }

        /// <summary>
        /// Deletes a playlist
        /// </summary>
        /// <param name="playlistId">ID of the playlist</param>
        /// <returns></returns>
        public async Task<MutateResponse> DeletePlaylistAsync(string playlistId)
        {
            JObject requestData = new JObject()
            { { "mutations" , new JArray()
                {
                    new JObject() { { "delete", playlistId} }
                }
            } };

            return await PerformMutateRequest(SJ_URL_PLAYLISTS_BATCH, requestData);
        }

        /// <summary>
        /// Renames a playlist
        /// </summary>
        /// <param name="playlistId">ID of the playlist to be modified</param>
        /// <param name="name">New name for the playlist (or null to make no changes)</param>
        /// <param name="description">New description for the playlist (or null to make no changes)</param>
        /// <param name="shareState">New share state (PUBLIC or PRIVATE) (or null to make no changes)</param>
        /// <returns></returns>
        public async Task<MutateResponse> UpdatePlaylistAsync(string playlistId, string name, string description = null, ShareState? shareState = null)
        {
            JObject requestData = new JObject()
            { { "mutations" , new JArray()
                { new JObject()
                    { { "update" , new JObject()
                        {
                            {"name", name},
                            {"id", playlistId},
                            {"description", description},
                            {"sharestate", shareState == null ? null : shareState.ToString() }
                        }
                    } }
                }
            } };

            return await PerformMutateRequest(SJ_URL_PLAYLISTS_BATCH, requestData);
        }

        /// <summary>
        /// Adds the provided songs to the playlist
        /// </summary>
        /// <param name="playlistId">ID of the playlist to add songs to</param>
        /// <param name="songs">List of songs to add</param>
        /// <returns></returns>
        public async Task<MutatePlaylistResponse> AddToPlaylistAsync(string playlistId, List<GMusicSong> songs)
        {
            Guid prev_uid = Guid.NewGuid();
            Guid current_uid = Guid.NewGuid();
            Guid next_uid = Guid.NewGuid();

            // This function is taken more or less completely from def build_plentry_adds() in
            // the unofficial google music API
            JArray songsToAdd = new JArray();

            int i = 0;
            foreach (GMusicSong song in songs)
            {
                JObject songJObject = new JObject
                    {
                    { "clientId", current_uid.ToString() },
                    { "creationTimestamp", -1 },
                    { "deleted", false },
                    { "lastModifiedTimestamp", 0},
                    { "playlistId", playlistId },
                    { "source", 1 },
                    {"trackId", song.ID }
                    };

                if (song.ID.First() == 'T')
                    songJObject["source"] = 2;

                if (i > 0)
                    songJObject["precedingEntryId"] = prev_uid;

                if (i < songs.Count - 1)
                    songJObject["followingEntryId"] = next_uid;

                JObject createJObject = new JObject { { "create", songJObject } };

                songsToAdd.Add(createJObject);
                prev_uid = current_uid;
                current_uid = next_uid;
                next_uid = Guid.NewGuid();
                i++;
            }

            JObject requestData = new JObject
            {{
                 "mutations", songsToAdd
             }};

            return await PerformMutatePlaylistRequest(SJ_URL_PLAYLIST_ENTRIES_BATCH, requestData);
        }

        /// <summary>
        /// Removes the list of playlist entries from their playlist(s)
        /// </summary>
        /// <param name="songs">List of entries to remove</param>
        /// <returns></returns>
        public async Task<MutatePlaylistResponse> RemoveFromPlaylistAsync(List<GMusicPlaylistEntry> songs)
        {
            JArray songsToDelete = new JArray();
            foreach (GMusicPlaylistEntry entry in songs)
            {
                songsToDelete.Add(new JObject
                {
                {"delete", entry.ID}
                });
            }

            JObject requestData = new JObject
            {{
                 "mutations", songsToDelete
             }};

            return await PerformMutatePlaylistRequest(SJ_URL_PLAYLIST_ENTRIES_BATCH, requestData);
        }

        #endregion

        #region Helper Functions

        private async Task<MutatePlaylistResponse> PerformMutatePlaylistRequest(string url, JObject data)
        {
            string responseAsJson = await SendPostAsync(url, data);
            MutatePlaylistResponse response = JsonConvert.DeserializeObject<MutatePlaylistResponse>(responseAsJson);
            return response;
        }

        private async Task<MutateResponse> PerformMutateRequest(string url, JObject data)
        {
            string responseAsJson = await SendPostAsync(url, data);
            MutateResponse response = JsonConvert.DeserializeObject<MutateResponse>(responseAsJson);
            return response;
        }

        private async Task<string> SendPostAsync(string url, JObject data)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue
                .Parse(String.Format("GoogleLogin auth={0}", gpsoauth.OAuthToken));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var builder = new UriBuilder(url);
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["alt"] = "json";
            query["dv"] = "0";
            query["hl"] = "en_US";
            query["tier"] = "aa";
            builder.Query = query.ToString();

            string combinedUrl = builder.ToString();
            string temp = data.ToString(); 
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

        #endregion
    }
}
