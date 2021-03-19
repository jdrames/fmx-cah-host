using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Models
{
    /// <summary>
    /// The status of the game state
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// Default state. This is the state when the game is created and waiting to be started
        /// </summary>
        Created = 1,

        /// <summary>
        /// The game has started and is waiting for a player to join
        /// </summary>
        ActiveRound,

        /// <summary>
        /// All cards submitted and is waiting for a winner to be selected
        /// </summary>
        WaitingCardZarSelection,

        /// <summary>
        /// Displays the winning cards for the round
        /// </summary>
        DisplayWinner,

        /// <summary>
        /// A game that has been cancelled
        /// </summary>
        Cancelled
    }
}
