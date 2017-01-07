using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MusicBeePlugin.Models
{
    [DataContract]
    public class GMusicPlaylistEntryResponse
    {
        [DataMember(Name = "data")]
        public GPlaylistEntryData Data { get; set; }
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "nextPageToken")]
        public string NextPageToken { get; set; }
    }
}
