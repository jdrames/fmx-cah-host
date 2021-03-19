using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Services
{
    /// <summary>
    /// Tells SignalR to use the ID from the JWT as the UserIdentifier
    /// </summary>
    public class UserIdProviderService : IUserIdProvider
    {
        // Get the User ID from the token value
        public string GetUserId(HubConnectionContext connection) => connection.User?.FindFirst("id")?.Value;        
    }
}
