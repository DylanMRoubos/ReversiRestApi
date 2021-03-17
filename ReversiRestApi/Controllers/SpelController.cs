using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Model;

namespace ReversiRestApi.Controllers
{
   
    [ApiController]
    [Route("api/Spel")]
    public class SpelController : ControllerBase
    {
        private readonly ISpelRepository iRepository;

        public SpelController(ISpelRepository repository)
        {
            iRepository = repository;
        }
        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler()
        {
            //Check if player2 token is set
            return new ObjectResult(
                (from value
                 in iRepository.GetSpellen()
                 where string.IsNullOrWhiteSpace(value.Speler2Token)
                 select new {
                     value.PlayerToken1,
                     value.Description,
                     value.Token
                 })
                .ToList());
        }
        // Put api/Spel/
        [HttpDelete("{gameToken}")]
        public ActionResult<bool> Delete(string gameToken)
        {
            iRepository.RemoveGame(gameToken);

            iRepository.GetSpel(gameToken);

            return StatusCode(204, true);

        }

        // GET api/spel{gametoken}
        [HttpGet("{gameToken}")]
        public ActionResult<Spel> GetGame(string gameToken)
        {
            var spel = iRepository.GetSpel(gameToken);

            return new ObjectResult(new SpelTbvJson(spel));
        }

        // GET api/Speler/<spelertoken>
        [HttpGet("Speler/{playerToken}")]
        public ActionResult<Spel> GetGamePlayer(string playerToken)
        {
            if(!string.IsNullOrWhiteSpace(playerToken))
            {
                try
                {
                    var spel = (
               (from value in iRepository.GetSpellen()
                where (value.PlayerToken1.Equals(playerToken) ||
                value.Speler2Token.Equals(playerToken)) && value.Finished != true
                select value).First());

                    return new ObjectResult(new SpelTbvJson(spel));
                }
                catch (Exception e)
                {
                    return null;
                }
               
            }
            return null;
           
        }

        // GET api/Beurt/<spelertoken>
        [HttpGet("Beurt/{gameToken}")]
        public ActionResult<Kleur> GetGameTurn(string gameToken)
        {
            if (!string.IsNullOrWhiteSpace(gameToken))
            {
                var turn = (
                (from value in iRepository.GetSpellen()
                 where value.Token.Equals(gameToken)
                 select value.AandeBeurt).First());

                return new ObjectResult(turn);
            }
            return null;

        }

        // Put api/Spel/Zet
        [HttpPut("Zet")]
        public ActionResult<bool> PlacePiece([FromBody]PlacePiece data)
        {
            var game = (from value in iRepository.GetSpellen()
            where value.Token.Equals(data.gameToken)
            select value).First();

            //Check if the playerToken is on its turn
            if (game.GetPlayerColour(data.playerToken) != game.AandeBeurt)
            {
                return StatusCode(403);
            }

            //Check if player wants to pass its turn
            if(!data.pass)
            {
                return game.DoeZet(data.y, data.x);
            } else
            {
                return game.Pas();
            }

        }

        // Put api/Spel/Opgeven
        [HttpPut("Opgeven")]
        public ActionResult<bool> Surrender([FromBody] Surrender data)
        {

            var game = (from value in iRepository.GetSpellen()
                        where value.Token.Equals(data.gameToken)
                        select value).First();

            return game.Surrender(data.playerToken);

        }

        // POST api/CreateGame
        [HttpPost]
        public ObjectResult CreateGame([FromBody] CreateGame data)
        {
            Spel spel = new Spel
            {
                PlayerToken1 = data.PlayerToken1,
                Description = data.Description,
                Token = Guid.NewGuid().ToString()
            };

            iRepository.AddSpel(spel);

            return StatusCode(201, spel.Token);
        }
    }
}
