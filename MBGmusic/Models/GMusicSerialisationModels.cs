using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Models
{
    // This wrapper class is here to make serializing the JSON easier.
    [DataContract]
    public class GPlaylistData
    {
        [DataMember(Name = "items")]
        public List<GMusicPlaylist> AllPlaylists { get; set; }
    }

    [DataContract]
    public class GSongData
    {
        [DataMember(Name = "items")]
        public List<GMusicSong> AllSongs { get; set; }
    }

    [DataContract]
    public class GPlaylistEntryData
    {
        [DataMember(Name = "items")]
        public List<GMusicPlaylistEntry> AllPlaylistEntries { get; set; }
    }

    [DataContract]
    public class MutateResponse
    {
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "client_id")]
        public string client_id { get; set; }
        [DataMember(Name = "response_code")]
        public string response_code { get; set; }
    }

    [DataContract]
    public class MutatePlaylistResponse
    {
        [DataMember(Name = "mutate_response")]
        public List<MutateResponse> MutateResponses { get; set; }
    }

}
