using fmx_cah_host.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models
{
    /// <summary>
    /// A Cards Against Humanity card
    /// </summary>
    public class Card : ICard
    {
        /// <summary>
        /// Unique ID for the card
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Text on the card
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The type of card represented
        /// </summary>
        [JsonPropertyName("type")]
        public CardType Type { get; set; }

        /// <summary>
        /// The pack the card is from
        /// </summary>
        [JsonPropertyName("pack")]
        public CardPack Pack { get; set; }

        /// <summary>
        /// If a prompt card, the number of answer cards required to answer
        /// </summary>
        [JsonPropertyName("pick_amount")]
        public int? PickAmount { get; set; }
    }
    
}
