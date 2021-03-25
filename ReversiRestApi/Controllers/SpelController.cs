using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        // Delete api/Spel/
        [HttpDelete("{gameToken}")]
        public ActionResult<bool> Delete(string gameToken)
        {
            iRepository.RemoveGame(gameToken);

            //iRepository.GetSpel(gameToken);

            return StatusCode(204, true);

        }

        //Get a single game based on Token
        // GET api/spel{gametoken}
        [HttpGet("{gameToken}")]
        public ActionResult<Spel> GetGame(string gameToken)
        {
            var spel = iRepository.GetSpel(gameToken);            
            return new ObjectResult(new SpelTbvJson(spel));
        }

        // PUT api/spel
        [HttpPut]
        public ActionResult<bool> JoinGame([FromBody] JoinGame data)
        {
            if(data.gameToken == "" || data.playerToken == "")
            {
                return BadRequest("Value must be passed in the request body.");
            }

            var game = iRepository.GetSpellen().FirstOrDefault(g => g.Token == data.gameToken);
            if(game == null)
            {
                return BadRequest("Game does not exist");
            }
            else if (game.Speler2Token != "")
            {
                return BadRequest("Can't join a game when there are already two players");
            }
            else if (game.PlayerToken1 == data.playerToken)
            {
                return BadRequest("Can't join a game you're already a particapant of");
            }

            if(iRepository.AddPlayer(data.gameToken, data.playerToken))
            {
                return StatusCode(204, data.gameToken);
            } else
            {
                return StatusCode(500, "500 Internal Server Error");
            }
            
            

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

        // GET api/Amount/<gametoken>
        [HttpGet("Amount/{gameToken}")]
        public ActionResult GetPieceAmount(string gameToken)
        {
            if (!string.IsNullOrWhiteSpace(gameToken))
            {
                List<List<GameHistory>> gameHistory = new List<List<GameHistory>>();

                var game = (
                (from value in iRepository.GetSpellen()
                 where value.Token.Equals(gameToken)
                 select value).First());


                var Player1History = iRepository.GetGameHistory(game.Token, game.PlayerToken1);
                var Player2History = iRepository.GetGameHistory(game.Token, game.Speler2Token);

                gameHistory.Add(Player1History);
                gameHistory.Add(Player2History);

                return new ObjectResult(JsonConvert.SerializeObject(gameHistory));
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
            if (game.GetPlayerColourOnToken(data.playerToken) != game.AandeBeurt)
            {
                return StatusCode(403);
            }

            //Check if player wants to pass its turn
            if (!data.pass)
            {
                 if (game.PlacePiece(data.y, data.x))
                {
                    Console.WriteLine(game.Bord);
                    //add zet to db
                    iRepository.PlacePiece(data.gameToken, game);                    
                    return true;
                }
                return false;
            } else
            {
                return true;
            }

        }

        // Put api/Spel/Opgeven
        [HttpPut("Opgeven")]
        public ActionResult<bool> Surrender([FromBody] Surrender data)
        {

            var game = (from value in iRepository.GetSpellen()
                        where value.Token.Equals(data.gameToken)
                        select value).First();

            game.Surrender(data.playerToken);

            //iRepository.Surrender(data.gameToken, game);

            return true;
        }

        // Put api/Spel/Skip
        [HttpPut("Skip")]
        public ActionResult<bool> SkipTurn([FromBody] Skip data)
        {

            var game = (from value in iRepository.GetSpellen()
                        where value.Token.Equals(data.gameToken)
                        select value).First();

            game.Pas();
            iRepository.NextTurn(data.gameToken, game.AandeBeurt);
            return true;
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
