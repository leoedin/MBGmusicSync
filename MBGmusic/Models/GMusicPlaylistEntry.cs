using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Models
{


    [DataContract]
    public class GMusicPlaylistEntry
    {
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "playlistId")]
        public string PlaylistID { get; set; }
        [DataMember(Name = "absolutePosition")]
        public string AbsolutePosition { get; set; }
        [DataMember(Name = "trackId")]
        public string TrackID { get; set; }
        [DataMember(Name = "creationTimestamp")]
        public string CreationTimestamp { get; set; }
        [DataMember(Name = "lastModifiedTimestamp")]
        public string LastModifiedTimestamp { get; set; }
        [DataMember(Name = "deleted")]
        public Boolean Deleted { get; set; }
        [DataMember(Name = "source")]
        public string Source { get; set; }
    }

}
