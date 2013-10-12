using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GMusicAPI;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        public MusicBeeApiInterface mbApiInterface;

        // The global-ish stuff we need to sync with Google Music
        public API api = new API();
        private List<GMusicPlaylist> allPlaylists = new List<GMusicPlaylist>();
        private List<GMusicSong> allSongs = new List<GMusicSong>();

        public List<GMusicPlaylist> AllPlaylists
        {
            get { return allPlaylists; }
        }

        public List<GMusicSong> AllSongs
        {
            get { return allSongs; }
        }

        public Boolean SyncRunning = false;

        public delegate void _OnFetchDataCompleteDelegate();
        public _OnFetchDataCompleteDelegate OnFetchDataComplete;

        public delegate void _OnSyncCompleteDelegate();
        public _OnSyncCompleteDelegate OnSyncComplete;

        public delegate void _OnSyncToLocalCompleteDelegate();
        public _OnSyncToLocalCompleteDelegate OnSyncToLocalComplete;

        public struct SavedSettingsType
        {
            public Boolean syncOnStartup;
            public String authorizationToken;
            public String password;
            public String email;
            public Boolean saveCredentials;
            public Boolean syncLocalToRemote;

            public List<String> mbPlaylistsToSync;
            public List<String> gMusicPlaylistsToSync;
        }

        public SavedSettingsType SavedSettings;
        public String ConfigFilePath;

        public class MbPlaylistType
        {
            public String mbName;
            public String Name;
            public override string ToString()
            {
                return Name;
            }
        }

        public class MbSongType
        {
            public String Filename;
            public String Artist;
            public String Title;

            public override string ToString()
            {
                return Artist + " - " + Title;
            }

        }

        // This exists to let us wait for callbacks when inside loops or similar
        AutoResetEvent stopWaitHandle = new AutoResetEvent(false);
        String lastPlaylistID;

        private PluginInfo about = new PluginInfo();
        public char[] filesSeparators = { '\0' };

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "Google Music Sync";
            about.Description = "Sync your playlists to Google Play Music.";
            about.Author = "Leo Rampen";
            about.TargetApplication = "None";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 0;  // your plugin version
            about.VersionMinor = 1;
            about.Revision = 1;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // not implemented yet: height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

            SavedSettings = new SavedSettingsType();
            SavedSettings.mbPlaylistsToSync = new List<String>();
            SavedSettings.gMusicPlaylistsToSync = new List<String>();


            // load defaults
            SavedSettings.syncOnStartup = false;
            SavedSettings.syncLocalToRemote = true;

            // Process taken from tag tools
            //Lets try to read defaults for controls from settings file
            ConfigFilePath = System.IO.Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), "gmusicStream.Settings.xml");

            System.IO.FileStream stream = System.IO.File.Open(ConfigFilePath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Read, System.IO.FileShare.None);
            System.IO.StreamReader file = new System.IO.StreamReader(stream, Encoding.UTF8);

            System.Xml.Serialization.XmlSerializer controlsDefaultsSerializer = null;
            try
            {
                controlsDefaultsSerializer = new System.Xml.Serialization.XmlSerializer(typeof(SavedSettingsType));
            }
            catch (Exception e)
            {
                MessageBox.Show("" + e.Message);
            }

            try
            {
                SavedSettings = (SavedSettingsType)controlsDefaultsSerializer.Deserialize(file);
            }
            catch
            {
                //Ignore...
            };

            file.Close();
            
            return about;
        }

        public bool Configure(IntPtr panelHandle)
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();

            Configure configureForm = new Configure(this);
            configureForm.Show();

            return false;
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
            
            using (FileStream cfgFile = File.Open(ConfigFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                StreamWriter file = new System.IO.StreamWriter(cfgFile, Encoding.UTF8);
                System.Xml.Serialization.XmlSerializer controlsDefaultsSerializer = new System.Xml.Serialization.XmlSerializer(typeof(SavedSettingsType));
                controlsDefaultsSerializer.Serialize(file, SavedSettings);
                file.Close();
            }

        }

        // MusicBee is closing the plugin (plugin is being disabled by user or MusicBee is shutting down)
        public void Close(PluginCloseReason reason)
        {
        }

        // uninstall this plugin - clean up any persisted files
        public void Uninstall()
        {
        }

        // receive event notifications from MusicBee
        // you need to set about.ReceiveNotificationFlags = PlayerEvents to receive all notifications, and not just the startup event
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            // perform some action depending on the notification type
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // Do a sync straight after we get the playlists
                    if (SavedSettings.syncOnStartup)
                    {
                        OnFetchDataComplete = SyncPlaylists;
                        LoginToGMusic();
                    }
                    
                    break;
            }
        }



        private void LoginToGMusic()
        {

            // The GoogleMusicAPI comes from GoogleMusicAPI.NET
            // https://github.com/taylorfinnell/GoogleMusicAPI.NET/
            //read the password from a file so we don't commit it to version control (consider using a GUI to set this later?)
            //login to GMusic


            if (SavedSettings.saveCredentials)
            {
                // decode it from base64
                byte[] email = Convert.FromBase64String(SavedSettings.email);
                byte[] passwd = Convert.FromBase64String(SavedSettings.password);
                api.OnLoginComplete = new EventHandler(LoginComplete);
                api.Login(Encoding.UTF8.GetString(email), Encoding.UTF8.GetString(passwd));
            }
            else if (SavedSettings.authorizationToken != "")
            {
                api.Login(SavedSettings.authorizationToken);
                FetchGMusicInformation();
            }

            // If neither is true, then we do nothing

        }

        private void LoginComplete(object sender, EventArgs e)
        {
            FetchGMusicInformation();  
        }


        public void FetchGMusicInformation()
        {
            SyncRunning = true;
            api.OnGetAllSongsComplete = OnGetAllSongs;
            api.GetAllSongs();
        }

        private void OnGetAllSongs(List<GMusicSong> songs)
        {
            allSongs = songs;
            api.OnGetAllPlaylistSongsComplete = OnGetAllPlaylists;
            api.GetAllPlaylists();
        }

        // Just update the playlists
        public void RefreshPlaylists()
        {
            api.OnGetAllPlaylistSongsComplete = OnGetAllPlaylists;
            api.GetAllPlaylists();
        }

        private void OnGetAllPlaylists(List<GMusicPlaylist> playlists)
        {
            allPlaylists = playlists;
            SyncRunning = false;
            if (OnFetchDataComplete != null)
                OnFetchDataComplete();
        }

        //Taken from TagTools AdvanceSearchandReplace.cs #720
        // In MB you run a query and then fetch the results
        // First, we query the library for all the playlists.
        // For each playlist, we fetch all its files.
        public List<MbPlaylistType> GetMbPlaylists()
        {
            List<MbPlaylistType> MbPlaylists = new List<MbPlaylistType>();
            mbApiInterface.Playlist_QueryPlaylists();
            string playlist = mbApiInterface.Playlist_QueryGetNextPlaylist();
            while (playlist != null)
            {
                string playlistName = mbApiInterface.Playlist_GetName(playlist);
                MbPlaylistType MbPlaylist = new MbPlaylistType();
                MbPlaylist.mbName = playlist;
                MbPlaylist.Name = playlistName;

                MbPlaylists.Add(MbPlaylist);
                
                // Query the next mbPlaylist to start again
                playlist = mbApiInterface.Playlist_QueryGetNextPlaylist();
            }

            return MbPlaylists;
        }

        public List<MbSongType> GetMbSongs()
        {
            string[] files;
            List<MbSongType> allMbSongs = new List<MbSongType>();

            if (mbApiInterface.Library_QueryFiles("domain=library"))
                files = mbApiInterface.Library_QueryGetAllFiles().Split(filesSeparators, StringSplitOptions.RemoveEmptyEntries);
            else
                files = new string[0];

            foreach (string path in files)
            {
                MbSongType thisSong = new MbSongType();
                thisSong.Filename = path;
                thisSong.Artist = mbApiInterface.Library_GetFileTag(path, MetaDataType.Artist);
                thisSong.Title = mbApiInterface.Library_GetFileTag(path, MetaDataType.TrackTitle);
                allMbSongs.Add(thisSong);
            }
            return allMbSongs;
        }

        private void SyncPlaylists()
        {
            if (SavedSettings.syncLocalToRemote)
            {
                SyncPlaylistsToGMusic();
            }
            else
            {
                List<GMusicPlaylist> playlists = new List<GMusicPlaylist>();
                foreach (string id in SavedSettings.gMusicPlaylistsToSync)
                {
                    GMusicPlaylist pls = allPlaylists.FirstOrDefault(p => p.ID == id);
                    if (pls != null)
                        playlists.Add(pls);
                }
                SyncPlaylistsToMusicBee(playlists);
            }

        }

        // Synchronise the playlists defined in the settings file to Google Music
        public void SyncPlaylistsToGMusic()
        {
            SyncRunning = true;

            // Get the MusicBee playlists
            List<MbPlaylistType> MbPlaylists = GetMbPlaylists();
            foreach (MbPlaylistType playlist in MbPlaylists)
            {
                // Only synchronise the playlists that settings say we should
                if (SavedSettings.mbPlaylistsToSync.Contains(playlist.mbName))
                {
                    // Use LINQ to check for a playlist with the same name
                    // If there is one, clear it's contents, otherwise create one
                    // Unless it's been deleted, in which case pretend it doesn't exist.
                    // I'm not sure how to undelete a playlist, or even if you can
                    GMusicPlaylist thisPlaylist = allPlaylists.FirstOrDefault(p => p.Name == playlist.Name && p.Deleted == false);
                    String thisPlaylistID;
                    if (thisPlaylist != null)
                    {
                        List<GMusicPlaylistEntry> allPlsSongs = thisPlaylist.Songs;
                        // This simply signals the wait handle when done
                        api.OnDeleteFromPlaylistComplete = OnModifyPlaylist;
                        api.DeleteFromPlaylist(allPlsSongs);

                        // Wait until the deletion is done
                        stopWaitHandle.WaitOne();
                        thisPlaylistID = thisPlaylist.ID;
                    }
                    else
                    {
                        // Set the callback
                        api.OnCreatePlaylistComplete = OnCreatePlaylist;
                        // Create the playlist
                        api.CreatePlaylist(playlist.Name);
                        // Wait until creation is done
                        stopWaitHandle.WaitOne();
                        thisPlaylistID = lastPlaylistID;
                    }

                    // Create a list of files based on the MB Playlist
                    string[] playlistFiles;
                    if (mbApiInterface.Playlist_QueryFiles(playlist.mbName))
                    {
                        playlistFiles = mbApiInterface.Playlist_QueryGetAllFiles().Split(filesSeparators, StringSplitOptions.RemoveEmptyEntries);

                    }
                    else
                    {
                        playlistFiles = new string[0];
                    }

                    List<GMusicSong> songsToAdd = new List<GMusicSong>();
                    // And get the title and artist of each file, and add it to the GMusic playlist
                    foreach (string file in playlistFiles)
                    {
                        string title = mbApiInterface.Library_GetFileTag(file, MetaDataType.TrackTitle);
                        string artist = mbApiInterface.Library_GetFileTag(file, MetaDataType.Artist);
                        GMusicSong gSong = allSongs.FirstOrDefault(item => (item.Artist == artist && item.Title == title));
                        if (gSong != null)
                            songsToAdd.Add(gSong);
                    }

                    api.OnAddToPlaylistComplete = OnModifyPlaylist;
                    api.AddToPlaylist(thisPlaylistID, songsToAdd);
                    stopWaitHandle.WaitOne();
                }

            }

            SyncRunning = false;
            // Signal to anyone calling that we're done
            if (OnSyncComplete != null)
                OnSyncComplete();

        }

        // Go through the selected playlists from GMusic,
        // delete the correspondingly named MusicBee playlist
        // Create a new playlist with the GMusic playlist contents
        public void SyncPlaylistsToMusicBee(List<GMusicPlaylist> playlists)
        {
            List<MbPlaylistType> localPlaylists = GetMbPlaylists();
            // The API doesn't give us a directory for playlists, 
            // We need to guess by finding the root directory of the first playlist
            MbPlaylistType useForDir = localPlaylists.First();
            String playlistDir = new FileInfo(useForDir.mbName).DirectoryName;
            if (useForDir.Name.Contains('\\'))
            {
                String folder = useForDir.Name.Split('\\')[0];
                playlistDir = playlistDir.Replace(folder, "");
            }
            
            
            List<MbSongType> allMbSongs = GetMbSongs();

            // Go through each playlist we want to sync in turn
            foreach (GMusicPlaylist playlist in playlists)
            {
                // Create an empty list for this playlist's local songs
                List<MbSongType> mbPlaylistSongs = new List<MbSongType>();
                
                // For each entry in the playlist we're syncing, get the song from the GMusic library we've downloaded,
                // Get the song Title and Artist and then look it up in the list of local songs.
                // If we find it, add it to the list of local songs
                foreach (GMusicPlaylistEntry entry in playlist.Songs)
                {
                    GMusicSong thisSong = allSongs.FirstOrDefault(s => s.ID == entry.TrackID);
                    if (thisSong != null)
                    {
                        MbSongType thisMbSong = allMbSongs.FirstOrDefault(s => s.Artist == thisSong.Artist && s.Title == thisSong.Title);
                        if (thisMbSong != null)
                            mbPlaylistSongs.Add(thisMbSong);
                    }
                }

                //mbAPI expects a string array of song filenames to create a playlist
                string[] mbPlaylistSongFiles = new string[mbPlaylistSongs.Count];
                int i=0;
                foreach (MbSongType song in mbPlaylistSongs)
                {
                    mbPlaylistSongFiles[i] = song.Filename;
                    i++;
                }
                // Now we need to either clear (by deleting and recreating the file) or create the playlist 
                MbPlaylistType localPlaylist = localPlaylists.FirstOrDefault(p => p.Name == playlist.Name);
                if (localPlaylist != null)
                {
                    string playlistPath = localPlaylist.mbName;
                    // delete the local playlist
                    File.Delete(playlistPath);
                    // And create a new empty file in its place
                    File.Create(playlistPath).Dispose();

                    // Set all our new files into the playlist
                    mbApiInterface.Playlist_SetFiles(localPlaylist.mbName, mbPlaylistSongFiles);
                }
                else
                {
                    // Create the playlist
                    mbApiInterface.Playlist_CreatePlaylist(playlistDir, playlist.Name, mbPlaylistSongFiles);
                }


            }

            // Call the delegate
            if (OnSyncToLocalComplete != null)
                OnSyncToLocalComplete();
        }

        private void OnCreatePlaylist(MutateResponse response)
        {
            lastPlaylistID = response.ID;
            stopWaitHandle.Set();
        }

        private void OnModifyPlaylist(MutatePlaylistResponse response)
        {
            // signal the wait handle
            stopWaitHandle.Set();
        }

        // return an array of lyric or artwork provider names this plugin supports
        // the providers will be iterated through one by one and passed to the RetrieveLyrics/ RetrieveArtwork function in order set by the user in the MusicBee Tags(2) preferences screen until a match is found
        public string[] GetProviders()
        {
            return null;
        }

        // return lyrics for the requested artist/title from the requested provider
        // only required if PluginType = LyricsRetrieval
        // return null if no lyrics are found
        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        {
            return null;
        }

        // return Base64 string representation of the artwork binary data from the requested provider
        // only required if PluginType = ArtworkRetrieval
        // return null if no artwork is found
        public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        {
            //Return Convert.ToBase64String(artworkBinaryData)
            return null;
        }

        #region " Storage Plugin "
        // user initiated refresh (eg. pressing F5) - reconnect/ clear cache as appropriate
        public void Refresh()
        {
        }

        // is the server ready
        // you can initially return false and then use MB_SendNotification when the storage is ready (or failed)
        public bool IsReady()
        {
            return false;
        }

        // return a 16x16 bitmap for the storage icon
        public Bitmap GetIcon()
        {
            return new Bitmap(16, 16);
        }

        public bool FolderExists(string path)
        {
            return true;
        }

        // return the full path of folders in a folder
        public string[] GetFolders(string path)
        {
            return new string[] { };
        }

        // this function returns an array of files in the specified folder
        // each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
        // a tag for FilePropertyType.Url must be included
        // you can initially return null and then use MB_SendNotification when the file data is ready (on receiving the notification MB will call GetFiles(path) again)
        public KeyValuePair<byte, string>[][] GetFiles(string path)
        {
            return null;
        }

        public bool FileExists(string url)
        {
            return true;
        }

        //  each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
        // a tag for FilePropertyType.Url must be included
        public KeyValuePair<byte, string>[] GetFile(string url)
        {
            return null;
        }

        // return an array of bytes for the raw picture data
        public byte[] GetFileArtwork(string url)
        {
            return null;
        }

        // return an array of playlist identifiers
        // where each playlist identifier is a KeyValuePair(id, name)
        public KeyValuePair<string, string>[] GetPlaylists()
        {
            return null;
        }

        // return an array of files in a playlist - a playlist being identified by the id parameter returned by GetPlaylists()
        // each file is represented as a array of tags - each tag being a KeyValuePair(Of Byte, String), where Byte is a FilePropertyType or MetaDataType enum value and String is the value
        // a tag for FilePropertyType.Url must be included
        public KeyValuePair<byte, string>[][] GetPlaylistFiles(string id)
        {
            return null;
        }

        // return a stream that can read through the raw (undecoded) bytes of a music file
        public System.IO.Stream GetStream(string url)
        {
            return null;
        }

        // return the last error that occurred
        public Exception GetError()
        {
            return null;
        }

        #endregion

    }
}