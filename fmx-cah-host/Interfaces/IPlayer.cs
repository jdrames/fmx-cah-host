using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Interfaces
{
    /// <summary>
    /// A player in the game
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Unique id that corresponds to the SignalR Context.UserIdentifier
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; }

        /// <summary>
        /// The players display name used in the game
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Indicates if this player is online
        /// </summary>
        [JsonPropertyName("is_onine")]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Indicates if the player is participating in the current round
        /// </summary>
        [JsonPropertyName("is_in_round")]
        public bool IsInRound { get; set; }
        
        /// <summary>
        /// The cards currently in the players hand
        /// </summary>
        [JsonIgnore]
        public List<ICard> Cards { get; set; }

        /// <summary>
        /// The number of points the player has been awarded
        /// </summary>
        [JsonPropertyName("points")]
        public int Points { get; set; }
    }
}
