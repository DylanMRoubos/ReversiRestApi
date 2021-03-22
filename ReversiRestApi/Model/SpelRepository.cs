using System;
using System.Collections.Generic;
using System.Linq;

namespace ReversiRestApi.Model
{
    public class SpelRepository : ISpelRepository
    {
        // Lijst met tijdelijke spellen
        public List<Spel> Spellen { get; set; }
        public SpelRepository()
        {
            Spel spel1 = new Spel();
            Spel spel2 = new Spel();
            Spel spel3 = new Spel();

            spel1.Token = "joejoe";
            spel1.PlayerToken1 = "abcdef";
            spel1.Speler2Token = "abcdefg";
            spel1.Description = "Potje snel reveri, dus niet lang nadenken";
            spel2.Token = "joe";
            spel2.PlayerToken1 = "ghijkl";
            spel2.Speler2Token = "mnopqr";
            spel2.Description = "Ik zoek een gevorderde tegenspeler!";
            spel3.Token = "jo";
            spel3.PlayerToken1 = "stuvwx";
            spel3.Description = "Na dit spel wil ik er nog een paar spelen tegen zelfde tegenstander";

            Spellen = new List<Spel> { spel1, spel2, spel3 };
        }

        public void AddSpel(Spel spel)
        {
            Spellen.Add(spel);
        }

        public List<Spel> GetSpellen()
        {
            return Spellen;
        }

        public Spel GetSpel(string spelToken)
        {
            return (Spel)(from value in Spellen where spelToken == value.PlayerToken1 select value);
        }

        public void RemoveGame(string spelToken)
        {
            throw new NotImplementedException();
        }

        public bool PlacePiece(string gameToken, int aandeBeurt, int x, int y)
        {
            throw new NotImplementedException();
        }
        public void AddPlayer(string gameToken, string playerToken) {
            throw new NotImplementedException();
        }

        void ISpelRepository.PlacePiece(string gameToken, int aandeBeurt, int x, int y, Spel localGame)
        {
            throw new NotImplementedException();
        }

        bool ISpelRepository.AddPlayer(string gameToken, string speler2Token)
        {
            throw new NotImplementedException();
        }
    }
}

