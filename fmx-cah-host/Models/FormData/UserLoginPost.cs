using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models.FormData
{
    public class UserLoginPost
    {
        [JsonPropertyName("name")]        
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [MaxLength(15, ErrorMessage = "Name cannot be longer that 15 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9\\_]{3,15}$", ErrorMessage = "Username can only contain letters, number and underscore.")]
        public string Name { get; set; }
    }
}
