using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text;
using TestPlaylistCreation;

namespace MusicBeePlugin
{
    public partial class Plugin
    {

        public MusicBeeApiInterface mbApiInterface;

        private PluginInfo about = new PluginInfo();

        #region Plugin Exported Methods

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "MB Test API Plugin";
            about.Description = "Quickly test API functionality";
            about.Author = "Leo Rampen";
            about.TargetApplication = "None";   // current only applies to artwork, lyrics or instant messenger name that appears in the provider drop down selector or target Instant Messenger
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 1;
            about.Revision = 1;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // not implemented yet: height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function

            
            // Comment this out to disable logging
           // Logger.Instance.LogFile = System.IO.Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), "gMusicPlaylistSync.log.txt");

            createMenu();

            return about;
        }

        private TestWindow _testWindow; 

        public bool Configure(IntPtr panelHandle)
        {

            int backColor = mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault,
                                                                               ElementComponent.ComponentBackground);
            int foreColor = mbApiInterface.Setting_GetSkinElementColour(SkinElement.SkinInputControl, ElementState.ElementStateDefault,
                                                                                ElementComponent.ComponentForeground);

            return false;
        }

        private void createMenu()
        {
            mbApiInterface.MB_AddMenuItem("mnuTools/Test MB API Plugin", "", onMenuItemClick);
        }

        private void onMenuItemClick(object sender, EventArgs e)
        {
            if (_testWindow != null)
            {
                _testWindow.Dispose();
                _testWindow = null;
            }

            _testWindow = new TestWindow(mbApiInterface);

            _testWindow.Show();
        }

        // called by MusicBee when the user clicks Apply or Save in the MusicBee Preferences screen.
        // its up to you to figure out whether anything has changed and needs updating
        public void SaveSettings()
        {
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
                    /*
                    if (SavedSettings.syncOnStartup)
                    {
                        OnFetchDataComplete = SyncPlaylists;
                        LoginToGMusic();
                    }*/

                    break;
            }
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

        #endregion

    }
}