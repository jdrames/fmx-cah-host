using fmx_cah_host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Interfaces
{
    /// <summary>
    /// Represents a game of Cards Against Humanity
    /// </summary>
    public interface IGame
    {
        // fields

        /// <summary>
        /// Unique ID for the game
        /// </summary>
        [JsonPropertyName("id")]
        string Id { get; }

        /// <summary>
        /// ID of the user(player) who created the game
        /// </summary>
        [JsonPropertyName("creator_id")]
        string CreatorId { get; }

        /// <summary>
        /// Code required to join the game
        /// </summary>
        [JsonIgnore]
        string Passcode { get; }

        /// <summary>
        /// The current state of the game
        /// </summary>
        [JsonPropertyName("state")]
        GameStatus State { get; }

        /// <summary>
        /// ID of the player who is assigned CardZar role
        /// </summary>
        [JsonPropertyName("card_zar_id")]
        public string CardZarId { get; }

        /// <summary>
        /// The players that are in the game
        /// </summary>
        [JsonPropertyName("players")]
        public List<Player> Players { get; }

        /// <summary>
        /// The current prompt(black) card for the round
        /// </summary>
        [JsonPropertyName("current_prompt_card")]
        public Card CurrentPromptCard { get; }

        /// <summary>
        /// Indicates if the game is paused
        /// </summary>
        [JsonPropertyName("is_paused")]
        public bool IsPaused { get; }

        /// <summary>
        /// Indicates the reason the game is paused
        /// </summary>
        [JsonPropertyName("paused_reason")]
        public PauseReason PauseReason { get; }

        // methods

        /// <summary>
        /// Retrieve a player from the game. Useful for checking the player is in the game
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool TryGetPlayer(string playerId, out Player player);

        /// <summary>
        /// Adds a player to the game.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public void AddPlayer(string playerId, Player player);

        /// <summary>
        /// Gets a list of the players cards
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<Card> GetPlayerCards(string playerId);

        /// <summary>
        /// Submits the players cards to the current game
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public bool SubmitPlayerCards(string playerId, List<Card> cards);

        /// <summary>
        /// Give a point to the winner of a round
        /// </summary>
        /// <param name="winningPlayerId"></param>
        public void SubmitWinner(string winningPlayerId);

        /// <summary>
        /// Starts the next round
        /// </summary>
        public void StartNextRound();

        /// <summary>
        /// Checks to see if a game should be paused and pauses it if true
        /// </summary>
        /// <returns></returns>
        public bool ShouldPauseGame();

    }
}
