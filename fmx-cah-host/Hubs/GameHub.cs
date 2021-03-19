using fmx_cah_host.Interfaces;
using fmx_cah_host.Models;
using fmx_cah_host.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Hubs
{
    // SignalR Hub for games


    public class GameHub : Hub
    {
        private readonly GameContainerService _gameService;        

        public GameHub(GameContainerService gameService)
        {
            _gameService = gameService;
        }

        /// <summary>
        /// Handle player disconnections
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var clientGames = _gameService.GetGamesWithPlayer(Context.UserIdentifier);
            foreach(var gameId in clientGames)
            {
                if(_gameService.TryGetGame(gameId, out var game))
                {
                    game.SetPlayerOnlineStatus(Context.UserIdentifier, false);
                    await Clients.Group(game.Id).SendAsync("player_left", Context.UserIdentifier);
                    if (game.ShouldPauseGame())
                    {
                        await Clients.Group(game.Id).SendAsync("pause_game", game.PauseReason);
                    }

                    if(game.State == GameStatus.Cancelled)
                    {
                        await Clients.Group(game.Id).SendAsync("game_cancelled");
                    }

                    if(game.OnlinePlayerCount == 0)
                    {
                        _gameService.RemoveGame(game.Id);
                    }
                }
            }
            

            await base.OnDisconnectedAsync(exception);
        }

        // actions executed from clients

        /// <summary>
        /// This is a response from the client 
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task<Game> GetGame(string gameId)
        {
            if (!_gameService.TryGetGame(gameId, out var game))
                return default;

            if(!game.TryGetPlayer(Context.UserIdentifier, out var player))
                return default;
            
            game.SetPlayerOnlineStatus(Context.UserIdentifier, true);
            await Groups.AddToGroupAsync(Context.ConnectionId, game.Id);
            await Clients.Group(game.Id).SendAsync("player_joined", player);

            // check to see if the game was pause, if so see if it can be resumed
            if (game.IsPaused && !game.ShouldPauseGame())
                await StartNextRound(game);

            return (Game)game;
        }

        /// <summary>
        /// Triggers the start of a game
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public async Task StartGame(string gameId)
        {
            if (!_gameService.TryGetGame(gameId, out var game))
                return;

            if (game.CreatorId != Context.UserIdentifier)
                return;

            if (game.OnlinePlayerCount < 3)
                return;

            await StartNextRound(game);
        }

        /// <summary>
        /// This triggers the next round cycle and sends the info to all players
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        private async Task StartNextRound(Game game)
        {
            game.StartNextRound();
            await Clients.Group(game.Id).SendAsync("start_round", game);
        }

        /// <summary>
        /// Gets a list of cards for the current player
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public List<Card> GetPlayerCards(string gameId)
        {
            if (!_gameService.TryGetGame(gameId, out var game))
                return default;

            if (!game.TryGetPlayer(Context.UserIdentifier, out _))
                return default;

            return game.GetPlayerCards(Context.UserIdentifier);
        }

        /// <summary>
        /// Submits the players cards for the current round
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="cards"></param>
        /// <returns></returns>
        public async Task<bool> SubmitPlayerCards(string gameId, List<Card> cards)
        {
            Console.WriteLine("spc 1");
            if (!_gameService.TryGetGame(gameId, out var game))
                return false;

            Console.WriteLine("spc 2");
            if (!game.TryGetPlayer(Context.UserIdentifier, out var player))
                return false;

            if (!player.IsInRound)
                return false;

            Console.WriteLine("Submitting player cards");
            var result = game.SubmitPlayerCards(Context.UserIdentifier, cards);

            Console.WriteLine("checking if all players are availaible.");

            if (game.AllPlayersReady)
                await Clients.Group(game.Id).SendAsync("all_players_ready", game.PlayerSubmittedCards);

            return result;
        }

        /// <summary>
        /// Submits the winner for a round
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public async Task<bool> SubmitWinner(string gameId, string playerId)
        {
            if (!_gameService.TryGetGame(gameId, out var game))
                return false;

            if (game.CardZarId != Context.UserIdentifier)
                return false;

            game.SubmitWinner(playerId);

            await Clients.Group(game.Id).SendAsync("round_winner", playerId);

            await Task.Delay(10 * 1000);

            await StartNextRound(game);

            return true;
        }

    }
}
