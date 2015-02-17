using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MusicBeePlugin.Models
{
    [DataContract]
    public class GMusicPlaylist
    {
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }

        public GMusicPlaylist()
        {
            Songs = new List<GMusicPlaylistEntry>();
        }

        [DataMember(Name = "accessControlled")]
        public string AccessControlled { get; set; }
        [DataMember(Name = "creationTimestamp")]
        public string CreationTimestamp { get; set; }
        [DataMember(Name = "deleted")]
        public Boolean Deleted { get; set; }
        [DataMember(Name = "id")]
        public string ID { get; set; }
        [DataMember(Name = "kind")]
        public string Kind { get; set; }
        [DataMember(Name = "lastModifiedTimestamp")]
        public string lastModifiedTimestamp { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "ownerName")]
        public string OwnerName { get; set; }
        [DataMember(Name = "ownerProfilePhotoUrl")]
        public string OwnerProfilePhotoURL { get; set; }
        [DataMember(Name = "recentTimestamp")]
        public string RecentTimestamp { get; set; }
        [DataMember(Name = "shareToken")]
        public string ShareToken { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }

        public List<GMusicPlaylistEntry> Songs { get; set; }
    }
}
