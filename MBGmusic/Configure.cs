﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using MusicBeePlugin.Models;
using System.Threading.Tasks;
using GooglePlayMusicAPI;
using System.Security;

namespace MusicBeePlugin
{
    partial class Configure : Form, IDisposable
    {

        private PlaylistSync _playlistSync;

        private Settings _settings;

        private Logger log;

        private void updateSyncStatus(string text)
        {
            syncStatusLabel.Text = text;
        }

        private void updateLoginStatus(string text)
        {
            loginStatusLabel.Text = text;
        }

        public Configure(PlaylistSync playlistSync, Settings settings, Plugin.MusicBeeApiInterface mbApiInterface)
        {
            InitializeComponent();

            foreach (Control control in this.Controls)
            {
                control.ForeColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                    MusicBeePlugin.Plugin.SkinElement.SkinInputPanel,
                    MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                    MusicBeePlugin.Plugin.ElementComponent.ComponentForeground));
                control.BackColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                    MusicBeePlugin.Plugin.SkinElement.SkinInputControl,
                    MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                    MusicBeePlugin.Plugin.ElementComponent.ComponentBackground));

                if (control.Controls.Count > 0)
                {
                    foreach (Control child in control.Controls)
                    {
                        child.ForeColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                            MusicBeePlugin.Plugin.SkinElement.SkinInputPanel,
                            MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                            MusicBeePlugin.Plugin.ElementComponent.ComponentForeground));
                        child.BackColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                            MusicBeePlugin.Plugin.SkinElement.SkinInputControl,
                            MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                            MusicBeePlugin.Plugin.ElementComponent.ComponentBackground));
                    }
                }
            }

            this.ForeColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                MusicBeePlugin.Plugin.SkinElement.SkinInputPanel,
                MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                MusicBeePlugin.Plugin.ElementComponent.ComponentForeground));
            this.BackColor = Color.FromArgb(mbApiInterface.Setting_GetSkinElementColour(
                MusicBeePlugin.Plugin.SkinElement.SkinInputControl,
                MusicBeePlugin.Plugin.ElementState.ElementStateDefault,
                MusicBeePlugin.Plugin.ElementComponent.ComponentBackground));



            log = Logger.Instance;
            log.OnLogUpdated = new EventHandler(log_OnLogUpdated);

            _settings = settings;
            _playlistSync = playlistSync;

            // Register event handlers
            _playlistSync.MBSync.OnSyncComplete = new EventHandler(MBSync_OnSyncComplete);

            if (_playlistSync.GMusic.SyncRunning)
            {
                updateSyncStatus("Background sync running");
            }

            if (_settings.Email != null)
            {
                // decode it from base64
                byte[] email = Convert.FromBase64String(_settings.Email);
                emailTextBox.Text = Encoding.UTF8.GetString(email);
            }

            if (_settings.Password != null)
            {
                byte[] passwd = Convert.FromBase64String(_settings.Password);
                passwordTextBox.Text = Encoding.UTF8.GetString(passwd);
            }

            rememberCheckbox.Checked = _settings.SaveCredentials;

           // autoSyncCheckbox.Checked = _settings.SyncOnStartup;

            populateLocalPlaylists();

            // tagToolsPlugin.mbForm.AddOwnedForm(this);
        }

        void MBSync_OnSyncComplete(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                syncStatusLabel.Text = "Synchronisation done!";
                closeButton.Enabled = false;
                populateLocalPlaylists();
            }));
        }

        void GMusic_OnSyncComplete(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                syncStatusLabel.Text = "Synchronisation done!";
                closeButton.Enabled = false;
                // Just refresh the playlists (don't bother fetching all the songs again)
                _playlistSync.GMusic.FetchPlaylists();
            }));
        }

        void log_OnLogUpdated(object sender, EventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                fetchPlaylistStatusLabel.Text = log.LastLog;
            }));
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            // Save the pwd and email to disc
            // NOTE although this encodes them as base64 (so they're not immediately obvious to anyone reading them) this is
            // NOT SECURITY. There is no encryption. Don't check that box unless you trust your machine!
            if (rememberCheckbox.Checked)
            {
                _settings.SaveCredentials = true;
                Byte[] email = Encoding.UTF8.GetBytes(emailTextBox.Text);
                _settings.Email = Convert.ToBase64String(email);
                Byte[] pwd = Encoding.UTF8.GetBytes(passwordTextBox.Text);
                _settings.Password = Convert.ToBase64String(pwd);
                _settings.Save();
            }
            else
            {
                _settings.SaveCredentials = false;
                _settings.Email = "";
                _settings.Password = "";
                _settings.Save();
            }

            bool loggedIn = await _playlistSync.GMusic.LoginToGMusic(emailTextBox.Text, passwordTextBox.Text);

            if (loggedIn)
            {
                updateLoginStatus("Successfully logged in.");
            }
            else
            {
                updateLoginStatus("LOGIN FAILED. PLEASE TRY AGAIN");
            }

            this.closeButton.Enabled = true;
            this.syncNowButton.Enabled = true;
            //this.autoSyncCheckbox.Enabled = true;
            List<Playlist> allPlaylists = await _playlistSync.GMusic.FetchPlaylists();
            googleMusicPlaylistBox.Items.Clear();
            foreach (Playlist playlist in allPlaylists)
            {
                if (!playlist.Deleted)
                {
                    if (_settings.GMusicPlaylistsToSync.Contains(playlist.ID))
                        googleMusicPlaylistBox.Items.Add(playlist, true);
                    else
                        googleMusicPlaylistBox.Items.Add(playlist, false);
                }
            }


        }

        // Get local Mb playlists and place them in the checkedlistbox
        // Check the settings to see if they're currently syncable
        private void populateLocalPlaylists()
        {
            List<MbPlaylist> mbPlaylists = _playlistSync.MBSync.GetMbPlaylists();
            localPlaylistBox.Items.Clear();
            foreach (MbPlaylist mbPlaylist in mbPlaylists)
            {
                if (_settings.MBPlaylistsToSync.Contains(mbPlaylist.mbName))
                     localPlaylistBox.Items.Add(mbPlaylist, true);
                else
                    localPlaylistBox.Items.Add(mbPlaylist, false);
            }

        }

        /*
        private void autoSyncCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _settings.SyncOnStartup = autoSyncCheckbox.Checked;
            _settings.Save();
        }*/

        // Depending on user settings, either start a local sync to remote, or start a remote sync to local
        private async void syncNowButton_Click(object sender, EventArgs e)
        {
            // Make sure we're logged in and have playlists, otherwise stop
            if (!_playlistSync.GMusic.LoggedIn)
            {
                updateSyncStatus("Error: NOT LOGGED IN");
                return;
            }

            if (!_playlistSync.GMusic.DataFetched)
            {
                updateSyncStatus("Please fetch data before attempting to sync.");
                return;
            }

            closeButton.Enabled = false;
            updateSyncStatus("Now synchronising. Please wait.");


            _playlistSync.SyncPlaylists();

            updateSyncStatus("Done Synching.");

            //                List<GMusicPlaylist> selected = new List<GMusicPlaylist>();
            //foreach (GMusicPlaylist selectedPlaylist in googleMusicPlaylistBox.CheckedItems)
             //   selected.Add(selectedPlaylist);

        }

        private void toGMusicRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            if (toGMusicRadiobutton.Checked)
                fromGMusicRadioButton.Checked = false;

            _settings.SyncLocalToRemote = toGMusicRadiobutton.Checked;
            _settings.Save();
        }

        private void fromGMusicRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fromGMusicRadioButton.Checked)
                toGMusicRadiobutton.Checked = false;
        }
        

        private void allLocalPlayCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (allLocalPlayCheckbox.Checked)
            {
                for (int i = 0; i < localPlaylistBox.Items.Count; i++)
                {
                    localPlaylistBox.SetItemChecked(i, true);
                }
            }
            else
            {
                for (int i = 0; i < localPlaylistBox.Items.Count; i++)
                {
                    localPlaylistBox.SetItemChecked(i, false);
                }
            }

            saveLocalPlaylistSettings();
        }


        private void allRemotePlayCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            
            if (allRemotePlayCheckbox.Checked)
                for (int i = 0; i < googleMusicPlaylistBox.Items.Count; i++)
                    googleMusicPlaylistBox.SetItemChecked(i, true);
            else
                for (int i = 0; i < googleMusicPlaylistBox.Items.Count; i++)
                    googleMusicPlaylistBox.SetItemChecked(i, false);

            saveRemotePlaylistSettings();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //SelectedIndexChanged NOT CheckedItemsChanged because the latter is called before the CheckedItems collection has actually updated,
        // making our eventual list miss the most recently selected item...
        private void localPlaylistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveLocalPlaylistSettings();
        }

        private void googleMusicPlaylistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            saveRemotePlaylistSettings();
        }



        private void saveLocalPlaylistSettings()
        {
            _settings.MBPlaylistsToSync.Clear();
            foreach (MbPlaylist playlist in localPlaylistBox.CheckedItems)
            {
                _settings.MBPlaylistsToSync.Add(playlist.mbName);
            }
            _settings.Save();
        }

        private void saveRemotePlaylistSettings()
        {
            _settings.GMusicPlaylistsToSync.Clear();
            foreach (Playlist playlist in googleMusicPlaylistBox.CheckedItems)
            {
                _settings.GMusicPlaylistsToSync.Add(playlist.ID);
            }
            _settings.Save();
        }

        private void unsubscribeEvents()
        {
            log.OnLogUpdated = null;

        }

        public new void Dispose()
        {
            this.unsubscribeEvents();
            base.Dispose();
        }

        private void Configure_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.unsubscribeEvents();
        }

        
    }
}
