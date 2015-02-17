using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicBeePlugin.Models
{
    public class MbSong
    {
        public String Filename;
        public String Artist;
        public String Title;

        public override string ToString()
        {
            return Artist + " - " + Title;
        }

    }
}
