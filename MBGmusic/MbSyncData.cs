using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBeePlugin.Models;
using System.IO;

namespace MusicBeePlugin
{
    class MbSyncData
    {
        private Settings _settings;

        private Logger log;

        private Plugin.MusicBeeApiInterface _mbApiInterface;

        public EventHandler OnSyncComplete;

        public MbSyncData(Settings settings, Plugin.MusicBeeApiInterface mbApiInterface)
        {
            _settings = settings;

            log = Logger.Instance;

            _mbApiInterface = mbApiInterface;
        }

        //Taken from TagTools AdvanceSearchandReplace.cs #720
        // In MB you run a query and then fetch the results
        // First, we query the library for all the playlists.
        // For each playlist, we fetch all its files.
        public List<MbPlaylist> GetMbPlaylists()
        {
            List<MbPlaylist> MbPlaylists = new List<MbPlaylist>();
            _mbApiInterface.Playlist_QueryPlaylists();
            string playlist = _mbApiInterface.Playlist_QueryGetNextPlaylist();
            while (playlist != null)
            {
                string playlistName = _mbApiInterface.Playlist_GetName(playlist);
                MbPlaylist MbPlaylist = new MbPlaylist();
                MbPlaylist.mbName = playlist;
                MbPlaylist.Name = playlistName;

                MbPlaylists.Add(MbPlaylist);

                // Query the next mbPlaylist to start again
                playlist = _mbApiInterface.Playlist_QueryGetNextPlaylist();
            }

            return MbPlaylists;
        }

        public List<MbSong> GetMbSongs()
        {
            string[] files = null;
            List<MbSong> allMbSongs = new List<MbSong>();

            if (_mbApiInterface.Library_QueryFiles("domain=library"))
            {
                // Old (deprecated)
                //public char[] filesSeparators = { '\0' };
                //files = _mbApiInterface.Library_QueryGetAllFiles().Split(filesSeparators, StringSplitOptions.RemoveEmptyEntries);
                _mbApiInterface.Library_QueryFilesEx("domain=library", ref files);
            }
            else
            {
                files = new string[0];
            }

            foreach (string path in files)
            {
                MbSong thisSong = new MbSong();
                thisSong.Filename = path;
                thisSong.Artist = _mbApiInterface.Library_GetFileTag(path, Plugin.MetaDataType.Artist);
                thisSong.Title = _mbApiInterface.Library_GetFileTag(path, Plugin.MetaDataType.TrackTitle);
                allMbSongs.Add(thisSong);
            }
            return allMbSongs;
        }


        // Go through the selected playlists from GMusic,
        // delete the correspondingly named MusicBee playlist
        // Create a new playlist with the GMusic playlist contents
        public void SyncPlaylistsToMusicBee(List<GMusicPlaylist> playlists, List<GMusicSong> allGMusicSongs)
        {
            
            // The API doesn't give us a directory for playlists, 
            // We need to guess by finding the root directory of the first playlist
            /* Apparently playlistDir = "" is "root" playlist dir, so this is unneeded.
            MbPlaylist useForDir = localPlaylists.First();
            String playlistDir = new FileInfo(useForDir.mbName).DirectoryName;
            if (useForDir.Name.Contains('\\'))
            {
                String folder = useForDir.Name.Split('\\')[0];
                playlistDir = playlistDir.Replace(folder, "");
            }*/

            List<MbPlaylist> localPlaylists = GetMbPlaylists();
            List<MbSong> allMbSongs = GetMbSongs();

            // Go through each playlist we want to sync in turn
            foreach (GMusicPlaylist playlist in playlists)
            {
                // Create an empty list for this playlist's local songs
                List<MbSong> mbPlaylistSongs = new List<MbSong>();

                // For each entry in the playlist we're syncing, get the song from the GMusic library we've downloaded,
                // Get the song Title and Artist and then look it up in the list of local songs.
                // If we find it, add it to the list of local songs
                foreach (GMusicPlaylistEntry entry in playlist.Songs)
                {
                    GMusicSong thisSong = allGMusicSongs.FirstOrDefault(s => s.ID == entry.TrackID);
                    if (thisSong != null)
                    {
                        MbSong thisMbSong = allMbSongs.FirstOrDefault(s => s.Artist == thisSong.Artist && s.Title == thisSong.Title);
                        if (thisMbSong != null)
                            mbPlaylistSongs.Add(thisMbSong);
                    }
                }

                //mbAPI expects a string array of song filenames to create a playlist
                string[] mbPlaylistSongFiles = new string[mbPlaylistSongs.Count];
                int i = 0;
                foreach (MbSong song in mbPlaylistSongs)
                {
                    mbPlaylistSongFiles[i] = song.Filename;
                    i++;
                }
                // Now we need to either clear (by deleting and recreating the file) or create the playlist 
                MbPlaylist localPlaylist = localPlaylists.FirstOrDefault(p => p.Name == playlist.Name);
                if (localPlaylist != null)
                {
                    string playlistPath = localPlaylist.mbName;
                    // delete the local playlist
                    File.Delete(playlistPath);
                    // And create a new empty file in its place
                    File.Create(playlistPath).Dispose();

                    // Set all our new files into the playlist
                    _mbApiInterface.Playlist_SetFiles(localPlaylist.mbName, mbPlaylistSongFiles);
                }
                else
                {
                    // Create the playlist
                    _mbApiInterface.Playlist_CreatePlaylist("", playlist.Name, mbPlaylistSongFiles);
                    // I haven't been able to get a playlist to be created in a directory yet
                    // For now, don't give that option
                   // _mbApiInterface.Playlist_CreatePlaylist(_settings.PlaylistDirectory, playlist.Name, mbPlaylistSongFiles);
                }
            }

            // Get the local playlists again

            // Call the delegate
            if (OnSyncComplete != null)
                OnSyncComplete(this, new EventArgs());
        }


    }
}
