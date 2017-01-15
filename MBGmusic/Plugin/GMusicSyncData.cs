using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MusicBeePlugin.Models;
using System.Threading.Tasks;
using GooglePlayMusicAPI;

namespace MusicBeePlugin
{
    class GMusicSyncData
    {
        private Settings _settings;

        private Logger log;

        private Plugin.MusicBeeApiInterface _mbApiInterface;

        public GMusicSyncData(Settings settings, Plugin.MusicBeeApiInterface mbApiInterface)
        {
            _allPlaylists = new List<Playlist>();
            _allSongs = new List<Track>();

            _settings = settings;

            _syncRunning = false;
            
            log = Logger.Instance;

            _mbApiInterface = mbApiInterface;
        }

        private List<Playlist> _allPlaylists;
        public List<Playlist> AllPlaylists { get { return _allPlaylists; } }

        private List<Track> _allSongs;
        public List<Track> AllSongs { get { return _allSongs; } }

        

        private Boolean _syncRunning;
        public Boolean SyncRunning { get { return _syncRunning; } }

        private Boolean _dataFetched;
        public Boolean DataFetched { get { return _dataFetched; } }

        public Boolean LoggedIn { get { return api.LoggedIn(); } }

        #region Logging in

        public async Task<bool> LoginToGMusic(string email, string password)
        {
            bool result = await api.LoginAsync(email, password);
            FetchLibraryAndPlaylists();
            return result;
        }

        #endregion

        #region Fetch GMusic Information

        // The global-ish stuff we need to sync with Google Music
        private GooglePlayMusicClient api = new GooglePlayMusicClient();

        public async void FetchLibraryAndPlaylists()
        {
            _allSongs = await api.GetLibraryAsync();
            _allPlaylists = await api.GetPlaylistsWithEntriesAsync();
            _dataFetched = true;
        }

        public async Task<List<Track>> FetchLibrary()
        {
            _allSongs = await api.GetLibraryAsync();
            return _allSongs;
        }

        public async Task<List<Playlist>> FetchPlaylists()
        {
            _allPlaylists = await api.GetPlaylistsWithEntriesAsync();
            return _allPlaylists;
        }

        #endregion

        #region Sync to GMusic

        // Synchronise the playlists defined in the settings file to Google Music
        public async void SyncPlaylistsToGMusic(List<MbPlaylist> mbPlaylistsToSync)
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
                    Playlist thisPlaylist = _allPlaylists.FirstOrDefault(p => p.Name == playlist.Name && p.Deleted == false);
                    String thisPlaylistID = "";
                    if (thisPlaylist != null)
                    {
                        List<PlaylistEntry> allPlsSongs = thisPlaylist.Songs;

                        if (allPlsSongs.Count > 0)
                        {
                            MutatePlaylistResponse response = await api.RemoveFromPlaylistAsync(allPlsSongs);
                        }
                        thisPlaylistID = thisPlaylist.ID;
                    }
                    else
                    {
                        MutatePlaylistResponse response = await api.CreatePlaylistAsync(playlist.Name);
                        thisPlaylistID = response.MutateResponses.First().ID;
                    }

                    // Create a list of files based on the MB Playlist
                    string[] playlistFiles = null;
                    if (_mbApiInterface.Playlist_QueryFiles(playlist.mbName))
                    {
                        bool success = _mbApiInterface.Playlist_QueryFilesEx(playlist.mbName, ref playlistFiles);
                        if (!success)
                            throw new Exception("Couldn't get playlist files");
                    }
                    else
                    {
                        playlistFiles = new string[0];
                    }

                    List<Track> songsToAdd = new List<Track>();
                    // And get the title and artist of each file, and add it to the GMusic playlist
                    foreach (string file in playlistFiles)
                    {
                        string title = _mbApiInterface.Library_GetFileTag(file, Plugin.MetaDataType.TrackTitle);
                        string artist = _mbApiInterface.Library_GetFileTag(file, Plugin.MetaDataType.Artist);
                        Track gSong = _allSongs.FirstOrDefault(item => (item.Artist == artist && item.Title == title));
                        if (gSong != null)
                            songsToAdd.Add(gSong);
                    }

                    await api.AddToPlaylistAsync(thisPlaylistID, songsToAdd);
                }

                _syncRunning = false;

            }
            else
            {
                throw new Exception("Not fetched data yet");
            }


        }

        #endregion
    }
}
