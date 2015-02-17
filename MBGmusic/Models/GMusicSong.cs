using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Models
{

    [DataContract]
    public class GMusicSong
    {
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Artist + " - " + Title;
        }

        [DataMember(Name = "album")]
        public string Album { get; set; }
        [DataMember(Name = "albumArtist")]
        public string AlbumArtist { get; set; }
        [DataMember(Name = "albumId")]
        public string AlbumId { get; set; }
        [DataMember(Name = "artist")]
        public string Artist { get; set; }
        [DataMember(Name = "beatsPerMinute")]
        public string BeatsPerMinute { get; set; }
        [DataMember(Name = "clientId")]
        public string ClientId { get; set; }
        [DataMember(Name = "comment")]
        public string Comment { get; set; }
        [DataMember(Name = "composer")]
        public string Composer { get; set; }
        [DataMember(Name = "creationTimestamp")]
        public string CreationTimestamp { get; set; }
        [DataMember(Name = "deleted")]
        public Boolean Deleted { get; set; }
        [DataMember(Name = "discNumber")]
        public string DiscNumber { get; set; }
        [DataMember(Name = "durationMillis")]
        public string DurationMillis { get; set; }
        [DataMember(Name = "estimatedSize")]
        public string EstimatedSize { get; set; }
        [DataMember(Name = "genre")]
        public string Genre { get; set; }
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "lastModifiedTimestamp")]
        public string LastModifiedTimestamp { get; set; }
        [DataMember(Name = "nid")]
        public string NID { get; set; }
        [DataMember(Name = "playCount")]
        public string PlayCount { get; set; }
        [DataMember(Name = "rating")]
        public string Rating { get; set; }
        [DataMember(Name = "recentTimestamp")]
        public string RecentTimestamp { get; set; }
        [DataMember(Name = "storeId")]
        public string StoreID { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
        [DataMember(Name = "totalDiscCount")]
        public string TotalDiscCount { get; set; }
        [DataMember(Name = "totalTrackCount")]
        public string TotalTrackCount { get; set; }
        [DataMember(Name = "trackNumber")]
        public string TrackNumber { get; set; }
        [DataMember(Name = "trackType")]
        public string TrackType { get; set; }
        [DataMember(Name = "year")]
        public string Year { get; set; }

    }
}
