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
        void PlacePiece(string gameToken, Spel localGame);
        bool AddPlayer(string gameToken, string speler2Token);
        List<GameHistory> GetGameHistory(string Gametoken, string PlayerToken);
        void AddPieceToHistory(string gameToken, string playerToken, int amount);
        void NextTurn(string gameToken, Kleur colour);
        bool FinishGame(Spel game);
        bool AcceptSurrender(string player, Spel game);
        bool BothPlayersAcceptedEnd(Spel game);
        // ... }
    }
}
