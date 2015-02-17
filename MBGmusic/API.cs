﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GMusicAPI
{
    public class API
    {
        #region Events

        public EventHandler OnLoginComplete;

        public delegate void _Error(Exception e);
        public _Error OnError;

        public delegate void _GetAllSongs(List<GMusicSong> songList);
        public _GetAllSongs OnGetAllSongsComplete;

        // public delegate void _GetAllPlaylists(List<GMusicPlaylist> playlists);
        // public _GetAllPlaylists OnGetAllPlaylistsComplete;

        public delegate void _GetAllPlaylistSongs(List<GMusicPlaylist> playlists);
        public _GetAllPlaylistSongs OnGetAllPlaylistSongsComplete;

        public delegate void _DeletePlaylist(MutateResponse response);
        public _DeletePlaylist OnDeletePlaylistComplete;

        public delegate void _CreatePlaylist(MutateResponse response);
        public _CreatePlaylist OnCreatePlaylistComplete;

        public delegate void _RenamePlaylist(MutateResponse response);
        public _CreatePlaylist OnRenamePlaylistComplete;

        public delegate void _AddToPlaylist(MutatePlaylistResponse response);
        public _AddToPlaylist OnAddToPlaylistComplete;

        public delegate void _DeleteFromPlaylist(MutatePlaylistResponse response);
        public _DeleteFromPlaylist OnDeleteFromPlaylistComplete;

        #endregion

        private GoogleHTTP client;

        private List<GMusicSong> allSongs;
        private List<GMusicPlaylist> allPlaylists;

        #region Constructor
        public API()
        {
            client = new GoogleHTTP();
            // JSON is parsed into the tracksReceived class

            allSongs = new List<GMusicSong>();
            allPlaylists = new List<GMusicPlaylist>();
        }

        #endregion

        public String AuthToken
        {
            get { return GoogleHTTP.AuthorizationToken; }
        }

        // Request a login using Google ClientLogin.
        // The parameters could be nicer (eg the FormBuilder class used in other implementations) but this is quick, dirty and works
        // If it turns out we *are* giving lots of post parameters, then perhaps do a proper post-request builder.
        public void Login(String email, String password)
        {
            string parameters = "service=sj&Email=" + email + "&Passwd=" + password;
            byte[] data = Encoding.UTF8.GetBytes(parameters);
            client.UploadDataAsync(new Uri("https://www.google.com/accounts/ClientLogin"), data, GetAuthTokenComplete);


        }

        // If we've got a saved auth token then use that
        public void Login(String authToken)
        {
            GoogleHTTP.AuthorizationToken = authToken;
        }

        // Take the returned data and parse it for the auth token (ignore everything else)
        // Afterwards, fire the OnLoginComplete delegate if it exists
        private void GetAuthTokenComplete(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            if (error != null && OnError != null)
            {
                OnError(error);
                return;
            }


            string CountTemplate = @"Auth=(?<AUTH>(.*?))$";
            Regex CountRegex = new Regex(CountTemplate, RegexOptions.IgnoreCase);
            string auth = CountRegex.Match(jsonData).Groups["AUTH"].ToString();

            if (!(auth == ""))
                GoogleHTTP.AuthorizationToken = auth;

            if (OnLoginComplete != null)
                OnLoginComplete(this, EventArgs.Empty);
        }

        // Get all the songs at once. The "max-results: 20000" should do it in one request.
        // Next page either provided as data (http://stackoverflow.com/questions/16638356/google-music-api-how-to-request-the-next-1000-songs)
        // or provide "maxresults: 20000" for all songs
        public void GetAllSongs()
        {
            // Clear the list of anything old
            allSongs = new List<GMusicSong>();
            GetAllSongsRequest();
        }

        private void GetAllSongsRequest(String nextPageToken = "")
        {
            String dataString = "";
            if (nextPageToken != "")
                dataString = "{'start-token':'" + nextPageToken + "'}";

            byte[] data = Encoding.UTF8.GetBytes(dataString);
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/trackfeed?alt=json&updated-min=0&include-tracks=true"), data, TrackListReceived);
        }

        // Take the received results and parse them into the GMusicSongs class. This is a bit hacky - the songs are then copied out of the class into a list
        // There's probably a way of doing this that doesn't require defining an entire class (and extra wrapper class), but this works.
        private void TrackListReceived(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            if (error != null && OnError != null)
            {
                OnError(error);
                return;
            }

            GMusicSongs tracksReceived = new GMusicSongs();
            tracksReceived = JsonConvert.DeserializeObject<GMusicSongs>(jsonData);

            foreach (GMusicSong song in tracksReceived.Data.AllSongs)
                allSongs.Add(song);


            // Check for a nextPageToken, and if there is one go round the loop again
            JObject songJson = JObject.Parse(jsonData);
            string nextPageToken = "";
            try { nextPageToken = songJson["nextPageToken"].ToString(); }
            catch { }

            if (nextPageToken != "")
            {
                GetAllSongsRequest(nextPageToken);
                return;
            }

            if (OnGetAllSongsComplete != null)
                OnGetAllSongsComplete(allSongs);

        }

        public void GetAllPlaylists()
        {
            allPlaylists = new List<GMusicPlaylist>();
            GetAllPlaylistRequest();
        }

        // Fetch the full list of playlists
        private void GetAllPlaylistRequest(string nextPageToken = "")
        {
            String dataString = "";
            if (nextPageToken != "")
                dataString = "{'start-token':'" + nextPageToken + "'}";
            byte[] data = Encoding.UTF8.GetBytes(dataString);

            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/playlistfeed?alt=json&updated-min=0&include-tracks=true"), data, PlaylistsReceived);

        }

        private void PlaylistsReceived(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {

            GMusicPlaylists playlistsReceived = new GMusicPlaylists();
            try
            {
                playlistsReceived = JsonConvert.DeserializeObject<GMusicPlaylists>(jsonData);
            }
            catch (Exception e)
            {
                if (OnError != null)
                {
                    OnError(e);
                    return;
                }
            }



            foreach (GMusicPlaylist playlist in playlistsReceived.Data.AllPlaylists)
                allPlaylists.Add(playlist);

            // Check for a nextPageToken, and if there is one go round the loop again
            JObject playlistJson = JObject.Parse(jsonData);
            string nextPageToken = "";
            try { nextPageToken = playlistJson["nextPageToken"].ToString(); }
            catch { }

            if (nextPageToken != "")
            {
                GetAllPlaylistRequest(nextPageToken);
                return;
            }

            GetAllPlaylistSongs();

            // My implementation will always get the playlist's songs
            /*
            if (OnGetAllPlaylistsComplete != null)
                OnGetAllPlaylistsComplete(allPlaylists);
             */
        }

        // We have to get a full list of songs which are in playlists, and then populate the list of playlists with them
        public void GetAllPlaylistSongs(string nextPageToken = "")
        {
            String dataString = "";
            if (nextPageToken != "")
                dataString = "{'start-token':'" + nextPageToken + "'}";
            byte[] data = Encoding.UTF8.GetBytes(dataString);
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/plentryfeed?alt=json&updated-min=0&include-tracks=true"), data, PlaylistSongsReceived);
        }

        // This feels awfully hacky. (and potentially very slow).
        // For each element in the received JSON, fetch the song object from our previously created "allSongs" list
        // then fetch the playlist from our "allPlaylists" list
        // finally, remove the playlist from the list, update it to have the song object attached, and then re-add it to the list (this will change the list order, but that's not particuarly important)
        // A song which has been "deleted" from a playlist won't be added to it.
        private void PlaylistSongsReceived(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            JObject allSongsReceived = JObject.Parse(jsonData);
            foreach (JObject song in allSongsReceived["data"]["items"])
            {
                GMusicPlaylistEntry thisSong = JsonConvert.DeserializeObject<GMusicPlaylistEntry>(song.ToString());
                GMusicPlaylist thisPlaylist = allPlaylists.FirstOrDefault(p => p.ID == thisSong.PlaylistID);
                if (thisPlaylist != null)
                {
                    // I think as the list contains a reference to the object, it will be updated
                    thisPlaylist.Songs.Add(thisSong);
                }
            }

            // Check to see if there's a nextpage token
            // This try/catch approach is pretty awful and done because of laziness. I assume
            // Json.Net has some sort of "does token exist" method?
            string nextPageToken = "";
            try { nextPageToken = allSongsReceived["nextPageToken"].ToString(); }
            catch { }

            if (nextPageToken != "")
            {
                GetAllPlaylistSongs(nextPageToken);
                return;
            }

            if (OnGetAllPlaylistSongsComplete != null)
                OnGetAllPlaylistSongsComplete(allPlaylists);
        }

        public void DeletePlaylist(String playlistID)
        {
            JObject requestData = new JObject
            {{"mutations", new JArray
            {new JObject 
                {{"delete", playlistID}}
            }
            }};

            byte[] data = Encoding.UTF8.GetBytes(requestData.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/playlistbatch?alt=json"), data, PlaylistDeleted);
        }

        private void PlaylistDeleted(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            MutatePlaylistResponse mutateResponse = JsonConvert.DeserializeObject<MutatePlaylistResponse>(jsonData);
            MutateResponse responseObject = mutateResponse.MutateResponses.FirstOrDefault();

            if (OnDeletePlaylistComplete != null)
                OnDeletePlaylistComplete(responseObject);
        }

        // Don't bother with this for now
        /*
        public void DeletePlaylist(List<GMusicPlaylist> playlists)
        {
            JArray deletions = new JArray();
            foreach (GMusicPlaylist playlist in playlists)
                deletions.Add(new JObject {{"delete", playlist.ID}});

            JObject create = new JObject
            {{
                 "mutations", deletions
             }};

            byte[] data = Encoding.UTF8.GetBytes(create.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/playlistbatch?alt=json"), data, PlaylistCreated);
        }
         * */

        // can't do
        public void CreatePlaylist(String playlistTitle)
        {
            JObject requestData = new JObject
            {{"mutations", new JArray
            {new JObject 
                {{"create", new JObject 
                    {
                    {"creationTimestamp", -1},
                    {"deleted", false},
                    {"lastModifiedTimestamp",0},
                    {"name", playlistTitle},
                    {"type", "USER_GENERATED"}
                    }
                }}
            }
            }};

            byte[] data = Encoding.UTF8.GetBytes(requestData.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/playlistbatch?alt=json"), data, PlaylistCreated);
        }

        private void PlaylistCreated(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            MutatePlaylistResponse mutateResponse = JsonConvert.DeserializeObject<MutatePlaylistResponse>(jsonData);
            MutateResponse responseObject = mutateResponse.MutateResponses.FirstOrDefault();

            if (OnCreatePlaylistComplete != null)
                OnCreatePlaylistComplete(responseObject);
        }

        // Could iterate over a list of playlists for batch update
        // (see for instance adding songs to a playlist)
        public void RenamePlaylist(String playlistID, String newName)
        {
            JObject requestData = new JObject
            {{"mutations", new JArray
            {new JObject 
                {{"update", new JObject 
                    {
                    {"id", playlistID},
                    {"name", newName}
                    }
                }}
            }
            }};

            byte[] data = Encoding.UTF8.GetBytes(requestData.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/playlistbatch?alt=json"), data, PlaylistUpdated);
        }

        private void PlaylistUpdated(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            MutatePlaylistResponse mutateResponse = JsonConvert.DeserializeObject<MutatePlaylistResponse>(jsonData);
            MutateResponse responseObject = mutateResponse.MutateResponses.FirstOrDefault();
            // Add the callback
            if (OnRenamePlaylistComplete != null)
                OnRenamePlaylistComplete(responseObject);
        }

        // Add a list of songs to a playlist
        public void AddToPlaylist(String playlistID, List<GMusicSong> songs)
        {
            // Unique ID required to place each song in the list
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
                    { "playlistId", playlistID },
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

            byte[] data = Encoding.UTF8.GetBytes(requestData.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/plentriesbatch?alt=json"), data, PlaylistSongsAdded);
        }

        private void PlaylistSongsAdded(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            MutatePlaylistResponse mutateResponse = JsonConvert.DeserializeObject<MutatePlaylistResponse>(jsonData);

            // Add the callback
            if (OnAddToPlaylistComplete != null)
                OnAddToPlaylistComplete(mutateResponse);
        }

        // Deletes the songs defined in the list playlistSongIDs from the playlist they're part of.
        // Note that the caller needs to determine the song IDs from the Playlist's Songs list
        public void DeleteFromPlaylist(List<GMusicPlaylistEntry> playlistEntries)
        {

            JArray songsToDelete = new JArray();
            foreach (GMusicPlaylistEntry entry in playlistEntries)
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

            byte[] data = Encoding.UTF8.GetBytes(requestData.ToString());
            client.UploadDataAsync(new Uri("https://www.googleapis.com/sj/v1.1/plentriesbatch?alt=json"), data, PlaylistSongsDeleted);

        }

        private void PlaylistSongsDeleted(HttpWebRequest request, HttpWebResponse response, String jsonData, Exception error)
        {
            MutatePlaylistResponse mutateResponse = JsonConvert.DeserializeObject<MutatePlaylistResponse>(jsonData);

            // Add the callback
            if (OnDeleteFromPlaylistComplete != null)
                OnDeleteFromPlaylistComplete(mutateResponse);
        }

    }
}