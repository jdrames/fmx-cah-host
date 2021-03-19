using fmx_cah_host.Interfaces;
using fmx_cah_host.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fmx_cah_host.Services
{
    /// <summary>
    /// The Game Container service houses each game session.
    /// This is injected as a singleton during application startup.
    /// </summary>
    public class GameContainerService
    {
        private Dictionary<string, IGame> GamesCollection = new Dictionary<string, IGame>();

        /// <summary>
        /// Adds a new game to the Game Collection
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool AddGame(IGame game)
        {
            if (GamesCollection.ContainsKey(game.Id))
                return false;

            GamesCollection.Add(game.Id, game);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve a game from the Game Collection
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool TryGetGame(string gameId, out Game game)
        {
            IGame result;
            if (!GamesCollection.TryGetValue(gameId, out result))
            {
                game = default;
                return false;
            }

            game = (Game)result;
            return true;
        }

        /// <summary>
        /// Gets a list of gameIds that the player is in            
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public List<string> GetGamesWithPlayer(string playerId)
        {
            List<string> gameIds = new List<string>();
            foreach(var game in GamesCollection.Values)
                if(game.TryGetPlayer(playerId, out _))
                {
                    gameIds.Add(game.Id);                    
                }

            return gameIds;
        }

        /// <summary>
        /// Removes a game from the game collection
        /// </summary>
        /// <param name="gameId"></param>
        public void RemoveGame(string gameId)
        {
            GamesCollection.Remove(gameId);
        }
    }
}
