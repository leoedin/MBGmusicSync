using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestPlaylistCreation
{
    public partial class TestWindow : Form
    {
        private MusicBeePlugin.Plugin.MusicBeeApiInterface _mbApiInterface;
        public TestWindow(MusicBeePlugin.Plugin.MusicBeeApiInterface api)
        {
            InitializeComponent();
            _mbApiInterface = api;
        }

        private void CreatePlaylist_button_Click(object sender, EventArgs e)
        {
            string dir = CreatePlaylistDirectory_tb.Text;
            string name = CreatePlaylistName_tb.Text;

            string[] files = null;
            if (_mbApiInterface.Library_QueryFiles("domain=library"))
            {
                // Old (deprecated)
                //public char[] filesSeparators = { '\0' };
                //files = _mbApiInterface.Library_QueryGetAllFiles().Split(filesSeparators, StringSplitOptions.RemoveEmptyEntries);
                _mbApiInterface.Library_QueryFilesEx("domain=library", ref files);
            }

            if (dir == null)
                dir = "";

            _mbApiInterface.Playlist_CreatePlaylist(dir, name, files);
        }
    }
}
