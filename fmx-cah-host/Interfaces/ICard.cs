using fmx_cah_host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Interfaces
{
    /// <summary>
    /// A Cards Against Humanity card
    /// </summary>
    public interface ICard
    {
        /// <summary>
        /// Id of the card
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Text that appears on the card
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// The type of card this is Prompt(black) or Answer(white)
        /// </summary>
        [JsonPropertyName("type")]
        public CardType Type { get; set; }

        /// <summary>
        /// The pack the card comes from
        /// </summary>
        [JsonPropertyName("pack")]
        public CardPack Pack { get; set; }

        /// <summary>
        /// If prompt card, the number of answer cards needed to answer the prompt
        /// </summary>
        [JsonPropertyName("pick_amount")]
        public int? PickAmount { get; set; }
    }
}
