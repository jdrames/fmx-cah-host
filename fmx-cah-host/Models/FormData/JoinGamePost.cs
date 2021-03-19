using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models.FormData
{
    public class JoinGamePost
    {
        [JsonPropertyName("id")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9-_]{21}$", ErrorMessage = "Game ID can only contain letters, number, hypens and underscores")]
        public string Id { get; set; }

        [JsonPropertyName("code")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9-_]{8}$", ErrorMessage = "Access code can only contain letters, number, hypens and underscores.")]
        public string Code { get; set; }
    }
}
