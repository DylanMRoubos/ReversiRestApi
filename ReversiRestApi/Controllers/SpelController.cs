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
                 select value.Omschrijving)
                .ToList());
        }

        // GET api/spel
        [HttpGet("{gameToken}")]
        public ActionResult<Spel> GetGame(string gameToken)
        {
             var spel = (
                  (from value in iRepository.GetSpellen()
                  where value.Token.Equals(gameToken)
                  select value).First());

            return new ObjectResult(new SpelTbvJson(spel));
        }

        // GET api/SpelSpeler/<spelertoken>
        [HttpGet("Speler/{playerToken}")]
        public ActionResult<Spel> GetGamePlayer(string playerToken)
        {
            if(!string.IsNullOrWhiteSpace(playerToken))
            {
                var spel = (
                (from value in iRepository.GetSpellen()
                 where value.Speler1Token.Equals(playerToken) ||
                 value.Speler2Token.Equals(playerToken)
                 select value).First());

                return new ObjectResult(new SpelTbvJson(spel));
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
        public ObjectResult CreateGame(string playerToken, string omschrijving)
        {
            Spel spel = new Spel
            {
                Speler1Token = playerToken,
                Omschrijving = omschrijving,
                Token = Guid.NewGuid().ToString()
            };

            iRepository.AddSpel(spel);

            return StatusCode(201, spel.Token);
        }
    }
}
