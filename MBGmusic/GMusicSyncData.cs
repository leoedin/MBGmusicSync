using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MusicBeePlugin.GMusicAPI;
using MusicBeePlugin.Models;


namespace MusicBeePlugin
{
    class GMusicSyncData
    {
        private Settings _settings;

        private Logger log;

        private Plugin.MusicBeeApiInterface _mbApiInterface;

        public GMusicSyncData(Settings settings, Plugin.MusicBeeApiInterface mbApiInterface)
        {
            _allPlaylists = new List<GMusicPlaylist>();
            _allSongs = new List<GMusicSong>();

            _settings = settings;

            _loggedIn = false;
            _syncRunning = false;
            
            log = Logger.Instance;

            _mbApiInterface = mbApiInterface;
        }

        public EventHandler OnLoginComplete;
        public EventHandler OnFetchDataComplete;
        public EventHandler OnSyncComplete;

        private List<GMusicPlaylist> _allPlaylists;
        public List<GMusicPlaylist> AllPlaylists { get { return _allPlaylists; } }

        private List<GMusicSong> _allSongs;
        public List<GMusicSong> AllSongs { get { return _allSongs; } }

        

        private Boolean _syncRunning;
        public Boolean SyncRunning { get { return _syncRunning; } }

        private Boolean _dataFetched;
        public Boolean DataFetched { get { return _dataFetched; } }

        private Boolean _loggedIn;
        public Boolean LoggedIn { get { return (api.AuthToken != null && api.AuthToken != ""); } }

        #region Logging in

        public void LoginToGMusic(string email, string password)
        {

            // The GoogleMusicAPI comes from GoogleMusicAPI.NET
            // https://github.com/taylorfinnell/GoogleMusicAPI.NET/
            //login to GMusic

            if (_settings.AuthorizationToken != null && _settings.AuthorizationToken != "")
            {
                api.Login(_settings.AuthorizationToken);
                if (OnLoginComplete != null)
                    OnLoginComplete(this, new EventArgs());
                _loggedIn = true;
                FetchGMusicInformation();
            }

            else
            {
                api.OnLoginComplete = new EventHandler(LoginComplete);
                api.Login(email, password);
            }

            // If neither is true, then we do nothing

        }

        private void LoginComplete(object sender, EventArgs e)
        {
            _settings.AuthorizationToken = api.AuthToken;
            _settings.Save();

            _loggedIn = true;
            if (OnLoginComplete != null)
                OnLoginComplete(this, e);
            FetchGMusicInformation();
        }

        #endregion

        #region Fetch GMusic Information

        // The global-ish stuff we need to sync with Google Music
        private API api = new API();

        public void FetchGMusicInformation()
        {
            _syncRunning = true;
            api.OnGetAllSongsComplete = onGetAllSongs;
            api.GetAllSongs();
        }

        private void onGetAllSongs(List<GMusicSong> songs)
        {
            
            _allSongs = songs;
            UpdatePlaylists();
        }

        // Just update the playlists
        public void UpdatePlaylists()
        {
            _syncRunning = true;
            api.OnGetAllPlaylistsComplete = onGetAllPlaylists;
            api.GetAllPlaylists();
        }

        private void onGetAllPlaylists(List<GMusicPlaylist> playlists)
        {
            _allPlaylists = playlists;
            _syncRunning = false;
            _dataFetched = true;

            if (OnFetchDataComplete != null)
               OnFetchDataComplete(this, new EventArgs());
        }

        #endregion

        #region Sync to GMusic

        // Synchronise the playlists defined in the settings file to Google Music
        public void SyncPlaylistsToGMusic(List<MbPlaylist> mbPlaylistsToSync)
        {
            _syncRunning = true;
            AutoResetEvent waitForEvent = new AutoResetEvent(false);


            if (_dataFetched)
            {
                // Get the MusicBee playlists
                foreach (MbPlaylist playlist in mbPlaylistsToSync)
                {
                    // Use LINQ to check for a playlist with the same name
                    // If there is one, clear it's contents, otherwise create one
                    // Unless it's been deleted, in which case pretend it doesn't exist.
                    // I'm not sure how to undelete a playlist, or even if you can
                    GMusicPlaylist thisPlaylist = _allPlaylists.FirstOrDefault(p => p.Name == playlist.Name && p.Deleted == false);
                    String thisPlaylistID = "";
                    if (thisPlaylist != null)
                    {
                        List<GMusicPlaylistEntry> allPlsSongs = thisPlaylist.Songs;
                        // This simply signals the wait handle when done
                        api.OnDeleteFromPlaylistComplete = delegate(MutatePlaylistResponse response)
                        {
                            waitForEvent.Set();
                        };
                        api.DeleteFromPlaylist(allPlsSongs);

                        // Wait until the deletion is done
                        waitForEvent.WaitOne();
                        thisPlaylistID = thisPlaylist.ID;
                    }
                    else
                    {
                        // Set the callback
                        api.OnCreatePlaylistComplete = delegate(MutateResponse response)
                        {
                            thisPlaylistID = response.ID;
                            waitForEvent.Set();
                        };
                        // Create the playlist
                        api.CreatePlaylist(playlist.Name);
                        // Wait until creation is done
                        waitForEvent.WaitOne();
                    }

                    // Create a list of files based on the MB Playlist
                    string[] playlistFiles = null;
                    if (_mbApiInterface.Playlist_QueryFiles(playlist.mbName))
                    {
                        // Old method:
                        //  playlistFiles = _mbApiInterface.Playlist_QueryGetAllFiles().Split(filesSeparators, StringSplitOptions.RemoveEmptyEntries);

                        bool success = _mbApiInterface.Playlist_QueryFilesEx(playlist.mbName, ref playlistFiles);
                        if (!success)
                            throw new Exception("Couldn't get playlist files");
                    }
                    else
                    {
                        playlistFiles = new string[0];
                    }

                    List<GMusicSong> songsToAdd = new List<GMusicSong>();
                    // And get the title and artist of each file, and add it to the GMusic playlist
                    foreach (string file in playlistFiles)
                    {
                        string title = _mbApiInterface.Library_GetFileTag(file, Plugin.MetaDataType.TrackTitle);
                        string artist = _mbApiInterface.Library_GetFileTag(file, Plugin.MetaDataType.Artist);
                        GMusicSong gSong = _allSongs.FirstOrDefault(item => (item.Artist == artist && item.Title == title));
                        if (gSong != null)
                            songsToAdd.Add(gSong);
                    }

                    api.OnAddToPlaylistComplete = delegate(MutatePlaylistResponse response)
                    {
                        waitForEvent.Set();
                    };
                    api.AddToPlaylist(thisPlaylistID, songsToAdd);
                    waitForEvent.WaitOne();

                }

                _syncRunning = false;
                // Signal to anyone calling that we're done
                if (OnSyncComplete != null)
                    OnSyncComplete(this, new EventArgs());

            }
            else
            {
                throw new Exception("Not fetched data yet");
            }


        }

        #endregion
    }
}
