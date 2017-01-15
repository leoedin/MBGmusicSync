using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MusicBeePlugin;
using MusicBeePlugin.GMusicAPI;
using MusicBeePlugin.Models;

namespace TestGMusicAPI
{
    public partial class Form1 : Form
    {
        private List<GMusicPlaylist> AllPlaylists = new List<GMusicPlaylist>();
        private List<GMusicSong> AllSongs = new List<GMusicSong>();
        private List<GMusicPlaylistEntry> AllEntries = new List<GMusicPlaylistEntry>();

        API api = new API();
        APIOAuth apiOauth = new APIOAuth();

        public Form1()
        {
            InitializeComponent();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            bool loggedIn = await apiOauth.LoginAsync(Secret.USERNAME, Secret.PASSWORD);
            this.statusLabel.Text = String.Format("Login Status: {0}", loggedIn);
        }

        private async void getTracksButton_Click(object sender, EventArgs e)
        {
            List<GMusicSong> library = await apiOauth.GetLibraryAsync();
            AllSongs = library;
            foreach(GMusicSong song in library)
            {
                songListBox.Items.Add(song);
            }

            songTotalLabel.Text = String.Format("Total songs: {0}", library.Count);
        }

        #region Get playlists
        // Get all the playlists tied to this account and display them

        private async void getPlaylistsButton_Click(object sender, EventArgs e)
        {
            List<GMusicPlaylist> playlists = await apiOauth.GetPlaylistsWithEntriesAsync();
            AllPlaylists = playlists;
            playlistListBox.Items.Clear();
            foreach (GMusicPlaylist playlist in playlists)
            {
                playlistListBox.Items.Add(playlist);
            }
        }

        #endregion

        #region Create playlists
        private async void createPlaylistButton_Click(object sender, EventArgs e)
        {
            await apiOauth.CreatePlaylistAsync(createPlaylistName.Text);
            getPlaylistsButton.PerformClick();
        }

        #endregion

        #region Delete playlist
        // Deletes the playlist currently selected by the user
        private async void deletePlaylistButton_Click(object sender, EventArgs e)
        {
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            await apiOauth.DeletePlaylistAsync(selectedPlaylist.ID);
            getPlaylistsButton.PerformClick();
        }

        #endregion

        #region Modify playlists
        private async void addSongsButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection songsSelected = songListBox.SelectedItems;
            List<GMusicSong> songsList = new List<GMusicSong>();
            foreach (GMusicSong song in songsSelected)
                songsList.Add(song);

            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;

            await apiOauth.AddToPlaylistAsync(selectedPlaylist.ID, songsList);
            getPlaylistsButton.PerformClick();

        }

        // This could delete multiple selections at once, but it doesn't
        private async void deleteSong_Click(object sender, EventArgs e)
        {
            GMusicSong selectedSong = (GMusicSong)playlistSongsBox.SelectedItem;
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            GMusicPlaylistEntry songEntry = selectedPlaylist.Songs.First(s => s.TrackID == selectedSong.ID);
            await apiOauth.RemoveFromPlaylistAsync(new List<GMusicPlaylistEntry> { songEntry });
        }

        #endregion

        #region Update playlist songs
        private void playlistListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            playlistSongsBox.Items.Clear();
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            foreach (GMusicPlaylistEntry song in selectedPlaylist.Songs)
            {
                if (!song.Deleted)
                {
                    GMusicSong thisSong = AllSongs.FirstOrDefault(s => s.ID == song.TrackID);
                    if (thisSong != null)
                    {
                        playlistSongsBox.Items.Add(thisSong);
                    }
                }
            }
        }

        #endregion

        private async void renameButton_Click(object sender, EventArgs e)
        {
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            await apiOauth.UpdatePlaylistAsync(selectedPlaylist.ID, renamePlaylistTextBox.Text, description:"test test new description test");
            getPlaylistsButton.PerformClick();
        }

      
    }
}
