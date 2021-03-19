using fmx_cah_host.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace fmx_cah_host.Models
{
    public class Game : IGame
    {
        private readonly object LOCK = new object();

        [JsonPropertyName("id")]
        public string Id { get; private set; }

        [JsonPropertyName("creator_id")]
        public string CreatorId { get; private set; }

        [JsonIgnore]
        public string Passcode { get; private set; }

        [JsonPropertyName("state")]
        public GameStatus State { get; private set; } = GameStatus.Created;

        [JsonPropertyName("card_zar_id")]
        public string CardZarId { get; private set; }

        [JsonPropertyName("players")]
        public List<IPlayer> Players { get; private set; } = new List<IPlayer>();

        [JsonPropertyName("current_prompt_card")]
        public ICard CurrentPromptCard { get; private set; }

        [JsonPropertyName("is_paused")]
        public bool IsPaused { get; private set; }

        [JsonPropertyName("paused_reason")]
        public PauseReason PauseReason { get; private set; }

        [JsonIgnore]
        public Dictionary<string, List<Card>> PlayerSubmittedCards = new Dictionary<string, List<Card>>();

        private List<ICard> PromptCards;
        private List<ICard> AnswerCards;

        private List<ICard> UsedPromptCards = new List<ICard>();
        private List<ICard> UsedAnswerCards = new List<ICard>();

        // Constructor
        public Game(string id, string passcode, IPlayer creator, List<ICard> promptCards, List<ICard> answerCards)
        {
            Id = id;
            CreatorId = creator.Id;
            Passcode = passcode;
            Players.Add(creator);
            PromptCards = promptCards;
            AnswerCards = answerCards;
        }

        /// <summary>
        /// Adds all the used answer cards back to the deck and shuffles the deck
        /// </summary>
        private void ShuffleAnswerCards()
        {
            foreach (var card in UsedAnswerCards)
            {
                AnswerCards.Add(card);
            }
            UsedAnswerCards.Clear();
            AnswerCards.Shuffle();
        }

        /// <summary>
        /// Adds all the used prompt cards back to the deck and shuffles the deck
        /// </summary>
        private void ShufflePromptCards()
        {
            foreach (var card in UsedPromptCards)
            {
                PromptCards.Add(card);
            }
            UsedPromptCards.Clear();
            PromptCards.Shuffle();
        }


        public List<ICard> GetPlayerCards(string playerId)
        {

            lock (LOCK)
            {
                IPlayer player;
                if (!TryGetPlayer(playerId, out player))
                    return default;

                while (player.Cards.Count < 10)
                {
                    if (AnswerCards.Count == 0)
                        ShuffleAnswerCards();
                    var card = AnswerCards.First();
                    player.Cards.Add(card);
                    AnswerCards.Remove(card);
                }

                return player.Cards;
            }
        }

        public bool ShouldPauseGame()
        {
            // checks if the game creator left the game before it started
            // if so then there are no players that can start the game
            if(State == GameStatus.Created && OnlinePlayers.Count(p=>p.IsOnline && p.Id == CreatorId) != 1)
            {
                State = GameStatus.Cancelled;
                IsPaused = true;
                PauseReason = PauseReason.NoHost;
                return true;
            }            

            // Don't run any further checks if this game is just
            // waiting for players to join
            if (State == GameStatus.Created)
                return false;

            // Check if the cardzar has gone offline
            if (OnlinePlayers.Count(p => p.Id == CardZarId && p.IsOnline) != 1)
            {
                IsPaused = true;
                PauseReason = PauseReason.CardZarLeftGame;
                return true;
            }

            // Check if there are enough players to play the game
            if (OnlineRoundPlayerCount < 3)
            {
                IsPaused = true;
                PauseReason = PauseReason.NotEnoughPlayers;
                return true;
            }

            // If no checks passed then the game should not be paused
            IsPaused = false;
            PauseReason = PauseReason.NotPaused;

            return false;
        }

        /// <summary>
        /// Get the number of players that are online and in the current round
        /// </summary>
        public int OnlineRoundPlayerCount => Players.Where(player => player.IsOnline && player.IsInRound).Count();



        /// <summary>
        /// Get the number of players that are online in the game
        /// </summary>
        public int OnlinePlayerCount => Players.Where(player => player.IsOnline).Count();

        /// <summary>
        /// A list of players that are currently online
        /// </summary>
        private List<IPlayer> OnlinePlayers { get
            {
                var onlineList = new List<IPlayer>();
                foreach (var player in Players.Where(p => p.IsOnline))
                    onlineList.Add(player);

                return onlineList;
            } 
        }

        /// <summary>
        /// Sets the online players to be in the next round
        /// </summary>
        private void SetRoundPlayers()
        {
            // Reset all players to not being in the round
            foreach (var player in Players)
                player.IsInRound = false;

            // Set only online players as in the round
            foreach (var player in OnlinePlayers)
                player.IsInRound = true;           
        }

        /// <summary>
        /// Sets the next card zar for the round
        /// </summary>
        private void SetNextCardZar()
        {
            var cardZarSet = false;
            foreach(var player in Players.Where(p=>p.IsInRound))
            {
                if (CardZarId == player.Id)
                    CardZarId = null;
                else if (string.IsNullOrEmpty(CardZarId))
                {
                    CardZarId = player.Id;
                    cardZarSet = true;
                }
            }
            if (!cardZarSet)
                CardZarId = Players.First(player => player.IsInRound).Id;
        }

        /// <summary>
        /// Sets the next prompt card for the game
        /// </summary>
        private void SetNextPromptCard()
        {
            if (PromptCards.Count == 0)
                ShufflePromptCards();

            var card = PromptCards.First();
            CurrentPromptCard = card;
            UsedPromptCards.Add(card);
            PromptCards.Remove(card);
        }

        /// <summary>
        /// Clear the previous rounds answer cards.
        /// Add the used cards to the used answer pile
        /// </summary>
        private void ClearPreviousRoundAnswerCards()
        {
            foreach(var cardList in PlayerSubmittedCards)
            {
                foreach(var card in cardList.Value)
                {
                    Players.First(p => p.Id == cardList.Key).Cards.RemoveAll(c => c.Id == card.Id);
                    UsedAnswerCards.Add(card);
                }
            }

            PlayerSubmittedCards.Clear();
        }
        
        public void StartNextRound()
        {
            ClearPreviousRoundAnswerCards();
            SetRoundPlayers();
            SetNextCardZar();
            SetNextPromptCard();
            State = GameStatus.ActiveRound;
        }

        public bool SubmitPlayerCards(string playerId, List<Card> cards)
        {
            if (PlayerSubmittedCards.ContainsKey(playerId))
                return false;

            if (cards.Count != CurrentPromptCard.PickAmount)
                return false;

            if (!TryGetPlayer(playerId, out var player) || !player.IsInRound)
                return false;

            PlayerSubmittedCards.Add(playerId, cards);
            return true;
        }

        public void SubmitWinner(string winningPlayerId)
        {
            if (!TryGetPlayer(winningPlayerId, out var player))
                return;
            
            player.Points++;            
        }

        public bool TryGetPlayer(string playerId, out IPlayer player)
        {
            var playerResult = Players.FirstOrDefault(p => p.Id == playerId);
            if (playerResult == null)
            {
                player = default;
                return false;
            }
            player = playerResult;
            return true;
        }

        public void AddPlayer(string playerId, IPlayer player)
        {
            if (Players.Count(p => p.Id == playerId) != 0)
                return;

            Players.Add(player);
        }

        /// <summary>
        /// Sets a players online status
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="isOnline"></param>
        public void SetPlayerOnlineStatus(string playerId, bool isOnline = true)
        {
            if (TryGetPlayer(playerId, out var player))
                player.IsOnline = isOnline;
        }

        /// <summary>
        /// Checks if all the players in the round have submitted cards
        /// </summary>
        public bool AllPlayersReady { get
            {
                if (Players.Count(p => p.IsInRound && p.Id != CardZarId) == PlayerSubmittedCards.Count)
                    return true;
                
                return false;
            } 
        }
    }
}
