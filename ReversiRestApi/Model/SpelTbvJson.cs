using System;
using Newtonsoft.Json;

namespace ReversiRestApi.Model
{
    public class SpelTbvJson
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        public string PlayerToken1 { get; set; }
        public string PlayerToken2 { get; set; }
        public string Board { get; set; }
        public Kleur CurrentPlayer { get; set; }


        public SpelTbvJson(Spel spel)
        {
            ID = spel.ID;
            Description = spel.Description;
            Token = spel.Token;
            PlayerToken1 = spel.PlayerToken1;
            PlayerToken2 = spel.Speler2Token;
            CurrentPlayer = spel.AandeBeurt;
            Board = JsonConvert.SerializeObject(spel.Bord);
        }
    }
}