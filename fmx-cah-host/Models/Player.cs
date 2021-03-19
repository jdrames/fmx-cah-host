using fmx_cah_host.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models
{
    public class Player : IPlayer
    {
        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonPropertyName("name")]
        public string Name { get; private set; }

        [JsonPropertyName("is_online")]
        public bool IsOnline { get; set; }

        [JsonPropertyName("is_in_round")]
        public bool IsInRound { get; set; }

        [JsonIgnore]
        public List<ICard> Cards { get; set; } = new List<ICard>();

        [JsonPropertyName("points")]
        public int Points { get; set; }

        // Constructor
        public Player(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
