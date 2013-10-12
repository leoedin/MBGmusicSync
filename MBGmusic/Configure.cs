using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using GMusicAPI;

namespace MusicBeePlugin
{
    public partial class Configure : Form
    {
        private Plugin gmusicPlugin;
        private Plugin.MusicBeeApiInterface mbApiInterface;

        public Configure(Plugin mbPlugin)
        {
            InitializeComponent();
            gmusicPlugin = mbPlugin;
            mbApiInterface = gmusicPlugin.mbApiInterface;

            if (gmusicPlugin.SyncRunning)
            {
                syncStatusLabel.Text = "Background sync running";
                gmusicPlugin.OnSyncComplete = OnSync;
            }

            if (gmusicPlugin.SavedSettings.email != null)
            {
                // decode it from base64
                byte[] email = Convert.FromBase64String(gmusicPlugin.SavedSettings.email);
                emailTextBox.Text = Encoding.UTF8.GetString(email);
            }

            if (gmusicPlugin.SavedSettings.password != null)
            {
                byte[] passwd = Convert.FromBase64String(gmusicPlugin.SavedSettings.password);
                passwordTextBox.Text = Encoding.UTF8.GetString(passwd);
            }

            rememberCheckbox.Checked = gmusicPlugin.SavedSettings.saveCredentials;

            autoSyncCheckbox.Checked = gmusicPlugin.SavedSettings.syncOnStartup;

            PopulateLocalPlaylists();

            // tagToolsPlugin.mbForm.AddOwnedForm(this);
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            // Save the pwd and email to disc
            // NOTE although this encodes them as base64 (so they're not immediately obvious to anyone reading them) this is
            // NOT SECURITY. There is no encryption. Don't check that box unless you trust your machine!
            if (rememberCheckbox.Checked)
            {
                gmusicPlugin.SavedSettings.saveCredentials = true;
                Byte[] email = Encoding.UTF8.GetBytes(emailTextBox.Text);
                gmusicPlugin.SavedSettings.email = Convert.ToBase64String(email);
                Byte[] pwd = Encoding.UTF8.GetBytes(passwordTextBox.Text);
                gmusicPlugin.SavedSettings.password = Convert.ToBase64String(pwd);

                gmusicPlugin.SaveSettings();
            }
            else
            {
                // set them to blank and save
                gmusicPlugin.SavedSettings.email = "";
                gmusicPlugin.SavedSettings.password = "";
                gmusicPlugin.SavedSettings.saveCredentials = false;

                gmusicPlugin.SaveSettings();
            }
            gmusicPlugin.api.OnLoginComplete = new EventHandler(LoginComplete);
            gmusicPlugin.api.Login(emailTextBox.Text, passwordTextBox.Text);
            closeButton.Enabled = false;

        }

        private void LoginComplete(object sender, EventArgs e)
        {
            // Get the users songs and playlists from GMusic
            gmusicPlugin.OnFetchDataComplete = OnFetchData;
            gmusicPlugin.FetchGMusicInformation();

            this.Invoke(new MethodInvoker(delegate
            {
                string authToken = gmusicPlugin.api.AuthToken;
                if (authToken != null)
                {
                    this.loginStatusLabel.Text = "Successfully logged in.";
                    this.fetchPlaylistStatusLabel.Text = "Fetching remote playlists...";
                    // Save the auth token to file
                    gmusicPlugin.SavedSettings.authorizationToken = authToken;
                    gmusicPlugin.SaveSettings();
                }
                else
                {
                    this.loginStatusLabel.Text = "LOGIN FAILED. PLEASE TRY AGAIN";
                }
            }));
            
        }

        private void OnFetchData()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                this.closeButton.Enabled = true;
                this.syncNowButton.Enabled = true;
                this.autoSyncCheckbox.Enabled = true;
                this.fetchPlaylistStatusLabel.Text = "Song and playlist data fetched.";
                List<GMusicPlaylist> allPlaylists = gmusicPlugin.AllPlaylists;
                googleMusicPlaylistBox.Items.Clear();
                foreach (GMusicPlaylist playlist in allPlaylists)
                {
                    if (!playlist.Deleted)
                    {
                        if (gmusicPlugin.SavedSettings.gMusicPlaylistsToSync.Contains(playlist.ID))
                            googleMusicPlaylistBox.Items.Add(playlist, true);
                        else
                            googleMusicPlaylistBox.Items.Add(playlist, false);
                    }
                }
            }));
        }

        // Get local Mb playlists and place them in the checkedlistbox
        // Check the settings to see if they're currently syncable
        private void PopulateLocalPlaylists()
        {
            List<Plugin.MbPlaylistType> mbPlaylists = gmusicPlugin.GetMbPlaylists();
            foreach (Plugin.MbPlaylistType mbPlaylist in mbPlaylists)
            {
                if (gmusicPlugin.SavedSettings.mbPlaylistsToSync.Contains(mbPlaylist.mbName))
                     localPlaylistBox.Items.Add(mbPlaylist, true);
                else
                    localPlaylistBox.Items.Add(mbPlaylist, false);
            }

        }

        private void autoSyncCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            gmusicPlugin.SavedSettings.syncOnStartup = autoSyncCheckbox.Checked;
            gmusicPlugin.SaveSettings();
        }

        // Depending on user settings, either start a local sync to remote, or start a remote sync to local
        private void syncNowButton_Click(object sender, EventArgs e)
        {
            // Make sure we're logged in and have playlists, otherwise stop
            if (GoogleHTTP.AuthorizationToken == null || gmusicPlugin.AllPlaylists.Count == 0)
            {
                this.loginStatusLabel.Text = "Error, not logged in";
                return;
            }

            closeButton.Enabled = false;
            syncStatusLabel.Text = "Now synchronising. Please wait.";

            if (gmusicPlugin.SavedSettings.syncLocalToRemote)
            {
                gmusicPlugin.OnSyncComplete = OnSync;
                gmusicPlugin.SyncPlaylistsToGMusic();
            }
            else
            {
                List<GMusicPlaylist> selected = new List<GMusicPlaylist>();
                foreach (GMusicPlaylist selectedPlaylist in googleMusicPlaylistBox.CheckedItems)
                    selected.Add(selectedPlaylist);
                gmusicPlugin.OnSyncToLocalComplete = OnSync;
                gmusicPlugin.SyncPlaylistsToMusicBee(selected);
            }

        }

        // We've successfully synchronised. Fetch the playlists again to update the view
        private void OnSync()
        {
            this.Invoke(new MethodInvoker(delegate
            {
                syncStatusLabel.Text = "Synchronisation done!";
                fetchPlaylistStatusLabel.Text = "Refreshing remote playlists";
                closeButton.Enabled = false;
                this.PopulateLocalPlaylists();
                // Just refresh the playlists (don't bother fetching all the songs again)
                gmusicPlugin.OnFetchDataComplete = OnFetchData;
                gmusicPlugin.RefreshPlaylists();
            }));
        }

        private void localPlaylistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gmusicPlugin.SavedSettings.mbPlaylistsToSync.Clear();
            foreach (Plugin.MbPlaylistType playlist in localPlaylistBox.CheckedItems)
            {
                gmusicPlugin.SavedSettings.mbPlaylistsToSync.Add(playlist.mbName);
            }
            gmusicPlugin.SaveSettings();
        }

        private void googleMusicPlaylistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            gmusicPlugin.SavedSettings.gMusicPlaylistsToSync.Clear();
            foreach (GMusicPlaylist playlist in googleMusicPlaylistBox.CheckedItems)
            {
                gmusicPlugin.SavedSettings.gMusicPlaylistsToSync.Add(playlist.ID);
            }
            gmusicPlugin.SaveSettings();
        }

        

        private void allLocalPlayCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (allLocalPlayCheckbox.Checked)
                for (int i=0;i<localPlaylistBox.Items.Count;i++)
                    localPlaylistBox.SetItemChecked( i, true);
            else
                for (int i = 0; i < localPlaylistBox.Items.Count; i++)
                    localPlaylistBox.SetItemChecked(i, false);

            gmusicPlugin.SavedSettings.mbPlaylistsToSync.Clear();
            foreach (Plugin.MbPlaylistType playlist in localPlaylistBox.CheckedItems)
            {
                gmusicPlugin.SavedSettings.mbPlaylistsToSync.Add(playlist.mbName);
            }
            gmusicPlugin.SaveSettings();

        }

        private void allRemotePlayCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            
            if (allRemotePlayCheckbox.Checked)
                for (int i = 0; i < googleMusicPlaylistBox.Items.Count; i++)
                    googleMusicPlaylistBox.SetItemChecked(i, true);
            else
                for (int i = 0; i < googleMusicPlaylistBox.Items.Count; i++)
                    googleMusicPlaylistBox.SetItemChecked(i, false);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toGMusicRadiobutton_CheckedChanged(object sender, EventArgs e)
        {

            if (toGMusicRadiobutton.Checked)
            {
                fromGMusicRadioButton.Checked = false;
                gmusicPlugin.SavedSettings.syncLocalToRemote = true;
            }
            else
            {
                gmusicPlugin.SavedSettings.syncLocalToRemote = false;
            }
            gmusicPlugin.SaveSettings();
        }

        private void fromGMusicRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fromGMusicRadioButton.Checked)
                toGMusicRadiobutton.Checked = false;
        }



        
    }
}
