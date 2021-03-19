using fmx_cah_host.Models;
using fmx_cah_host.Models.FormData;
using fmx_cah_host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fmx_cah_host.Controllers
{
    // Only authenticated users should be able to access this endpoint
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly CardDbService _cardService;
        private readonly GameContainerService _gameService;

        public GamesController(CardDbService cardService, GameContainerService gameService)
        {
            _cardService = cardService;
            _gameService = gameService;
        }

        // POST <GamesController>
        [HttpPost]
        public IActionResult CreateGame([FromBody] NewGamePost postData)
        {
            var gameId = Nanoid.Nanoid.Generate(size: 21);
            var code = Nanoid.Nanoid.Generate(size: 8);
            var player = new Player(User.FindFirst("id").Value, User.FindFirst(ClaimTypes.Name).Value);
            var promptCards = _cardService.GetCards(CardType.Prompt, postData.Packs);
            var answerCards = _cardService.GetCards(CardType.Answer, postData.Packs);
            // Do initial shuffle
            promptCards.Shuffle();
            answerCards.Shuffle();
                        
            var game = new Game(gameId, code, player, promptCards, answerCards);

            if (_gameService.AddGame(game))
                return Ok(new
                {
                    id = gameId,
                    code = code
                });

            return StatusCode(500, new {
                message = "Unable to create game. Try again."
            });
        }

        [HttpPost("{id}")]
        public IActionResult JoinGame([FromRoute(Name="id")] string id, [FromBody] JoinGamePost postData)
        {
            if (!_gameService.TryGetGame(postData.Id, out var game))
                return NotFound();

            if (game.Passcode != postData.Code)
                return Forbid();

            var player = new Player(User.FindFirst("id").Value, User.FindFirst(ClaimTypes.Name).Value);
            game.AddPlayer(player.Id, player);
            return Ok();
        }
    }
}
