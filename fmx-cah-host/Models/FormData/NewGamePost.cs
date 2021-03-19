using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models.FormData
{
    public class NewGamePost
    {
        /// <summary>
        /// The card packs the user wants to create the game with
        /// </summary>        
        [JsonPropertyName("packs")]
        [Required]
        public List<CardPack> Packs { get; set; }
    }
}
