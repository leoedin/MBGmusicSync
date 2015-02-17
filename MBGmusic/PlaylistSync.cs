using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicBeePlugin.Models;
using MusicBeePlugin.GMusicAPI;
using System.Threading;

namespace MusicBeePlugin
{
    class PlaylistSync
    {
        private Logger log;

        private Settings _settings;

        private Plugin.MusicBeeApiInterface _mbApiInterface;

        public PlaylistSync(Settings settings, Plugin.MusicBeeApiInterface mbApiInterface)
        {
            _settings = settings;

            _mbApiInterface = mbApiInterface;

            log = Logger.Instance;

            _gMusic = new GMusicSyncData(settings, mbApiInterface);
            _mbSync = new MbSyncData(settings, mbApiInterface);
        }
               
        private GMusicSyncData _gMusic;
        public GMusicSyncData GMusic { get { return _gMusic; } }

        private MbSyncData _mbSync;
        public MbSyncData MBSync { get { return _mbSync; } }

        /// <summary>
        /// This is blocking, so run it on a thread
        /// </summary>
        public void SyncPlaylists()
        {
            if (!_gMusic.LoggedIn || _gMusic.SyncRunning)
                return;

            if (_settings.SyncLocalToRemote)
            {
                List<MbPlaylist> playlists = new List<MbPlaylist>();
                List<MbPlaylist> allPlaylists = _mbSync.GetMbPlaylists();
                // Only sync the playlists that the settings say we should
                // Surely there's a nicer LINQ query for this?
                foreach (MbPlaylist pls in allPlaylists)
                {
                    if (_settings.MBPlaylistsToSync.Contains(pls.mbName))
                    {
                        playlists.Add(pls);
                    }
                }
                 _gMusic.SyncPlaylistsToGMusic(playlists);   
            }
            else
            {
                if (_gMusic.DataFetched)
                {
                    List<GMusicPlaylist> playlists = new List<GMusicPlaylist>();
                    foreach (string id in _settings.GMusicPlaylistsToSync)
                    {
                        GMusicPlaylist pls = _gMusic.AllPlaylists.FirstOrDefault(p => p.ID == id);
                        if (pls != null)
                            playlists.Add(pls);
                    }
                    _mbSync.SyncPlaylistsToMusicBee(playlists, _gMusic.AllSongs);
                    return;
                } else {
                    return;
                }
            }

        }

    }
}
