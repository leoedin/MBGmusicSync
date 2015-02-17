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

        API api = new API();
        public Form1()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            string[] credentials = System.IO.File.ReadAllLines("C:\\\\Temp\\gmusic.txt");
            api.OnLoginComplete = new EventHandler(delegate(object send2, EventArgs ee)
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    this.statusLabel.Text = String.Format("Login complete, auth: {0}", api.AuthToken);
                }));

            });
            api.Login(credentials[0], credentials[1]);
        }

        private void getTracksButton_Click(object sender, EventArgs e)
        {
            api.OnGetAllSongsComplete = OnGetSongsComplete;
            api.GetAllSongs();
        }

        private void OnGetSongsComplete(List<GMusicSong> songs)
        {
            AllSongs = songs;
            this.Invoke(new MethodInvoker(delegate
            {
                foreach (GMusicSong song in songs)
                {
                    songListBox.Items.Add(song);
                    
                }
                songTotalLabel.Text = "Total songs: " + Convert.ToString(songs.Count());
            }));
        }

        #region Get playlists
        // Get all the playlists tied to this account and display them

        private void getPlaylistsButton_Click(object sender, EventArgs e)
        {
            api.OnGetAllPlaylistsComplete = OnGetAllPlaylistSongs;
            api.GetAllPlaylists();
        }

        private void OnGetAllPlaylistSongs(List<GMusicPlaylist> playlists)
        {
            AllPlaylists = playlists;
            this.Invoke(new MethodInvoker(delegate
            {
                int selectedPlaylist = playlistListBox.SelectedIndex;
                playlistListBox.Items.Clear();
                foreach (GMusicPlaylist playlist in playlists)
                    if (!playlist.Deleted)
                        playlistListBox.Items.Add(playlist);
                playlistListBox.SelectedIndex = selectedPlaylist;
            }));
        }

        #endregion

        #region Create playlists
        private void createPlaylistButton_Click(object sender, EventArgs e)
        {
            api.OnCreatePlaylistComplete = CreatePlaylistDone;
            api.CreatePlaylist(createPlaylistName.Text);
        }

        private void CreatePlaylistDone(MutateResponse response)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                // Get the playlists again
                getPlaylistsButton.PerformClick();
            }));
        }

        #endregion

        #region Delete playlist
        // Deletes the playlist currently selected by the user
        private void deletePlaylistButton_Click(object sender, EventArgs e)
        {
            api.OnDeletePlaylistComplete = DeletePlaylistDone;
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            api.DeletePlaylist(selectedPlaylist.ID);
        }

        private void DeletePlaylistDone(MutateResponse response)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                statusLabel.Text += "Deleted: " + response.ID;
                // Get the playlists again
                getPlaylistsButton.PerformClick();
            }));
        }

        #endregion

        #region Modify playlists
        private void addSongsButton_Click(object sender, EventArgs e)
        {
            api.OnAddToPlaylistComplete = OnChangePlaylist;
            ListBox.SelectedObjectCollection songsSelected = songListBox.SelectedItems;
            List<GMusicSong> songsList = new List<GMusicSong>();
            foreach (GMusicSong song in songsSelected)
                songsList.Add(song);

            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;

            api.AddToPlaylist(selectedPlaylist.ID, songsList);

        }

        // This could delete multiple selections at once, but it doesn't
        private void deleteSong_Click(object sender, EventArgs e)
        {
            api.OnDeleteFromPlaylistComplete = OnChangePlaylist;
            GMusicSong selectedSong = (GMusicSong)playlistSongsBox.SelectedItem;
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            GMusicPlaylistEntry songEntry = selectedPlaylist.Songs.First(s => s.TrackID == selectedSong.ID);
            api.DeleteFromPlaylist(new List<GMusicPlaylistEntry>{songEntry});
        }

        private void OnChangePlaylist(MutatePlaylistResponse response)
        {
            this.Invoke(new MethodInvoker(delegate
                {
                    getPlaylistsButton.PerformClick();
                }));
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
                    playlistSongsBox.Items.Add(thisSong);
                }
            }
        }

        #endregion

        private void renameButton_Click(object sender, EventArgs e)
        {
            api.OnRenamePlaylistComplete = OnRenamePlaylist;
            GMusicPlaylist selectedPlaylist = (GMusicPlaylist)playlistListBox.SelectedItem;
            api.RenamePlaylist(selectedPlaylist.ID, renamePlaylistTextBox.Text);
        }

        private void OnRenamePlaylist(MutateResponse response)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                getPlaylistsButton.PerformClick();
            }));
        }

      
    }
}
