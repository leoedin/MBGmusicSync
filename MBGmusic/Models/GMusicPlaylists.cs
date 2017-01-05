using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Models
{
    [DataContract]
    public class GMusicPlaylists
    {
        [DataMember(Name = "data")]
        public GPlaylistData Data { get; set; }
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "nextPageToken")]
        public string NextPageToken { get; set; }
    }

}
