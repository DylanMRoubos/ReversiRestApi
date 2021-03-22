using System;
using System.Collections.Generic;

namespace ReversiRestApi.Model
{
    public interface ISpelRepository
    {
        void AddSpel(Spel spel);
        public List<Spel> GetSpellen();
        Spel GetSpel(string spelToken);
        public void RemoveGame(string spelToken);
        void PlacePiece(string gameToken, int aandeBeurt, int x, int y, Spel localGame);
        bool AddPlayer(string gameToken, string speler2Token);
        // ... }
    }
}
