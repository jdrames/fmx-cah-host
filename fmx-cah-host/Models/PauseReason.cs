using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Models
{
    /// <summary>
    /// The reason a game has been paused
    /// </summary>
    public enum PauseReason
    {
        /// <summary>
        /// Indicates that the game is not currently paused
        /// </summary>
        NotPaused,

        /// <summary>
        /// Indicates the game was paused because there were not enough players to continue
        /// </summary>
        NotEnoughPlayers,

        /// <summary>
        /// Indicates the game was paused because the cardzar left the game
        /// </summary>
        CardZarLeftGame,

        /// <summary>
        /// Indicates the host left the game before it started
        /// </summary>
        NoHost
    }
}
