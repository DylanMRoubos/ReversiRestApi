using System;
using System.Collections.Generic;
using ReversiRestApi.Model;

namespace ReversiRestApi
{
    public interface ISpel
    {
        int ID { get; set; }
        string Description { get; set; }
        string Token { get; set; }
        string PlayerToken1 { get; set; }
        string Speler2Token { get; set; }

        Kleur[,] Bord { get; set; }
        Kleur AandeBeurt { get; set; }
        
        bool Pas();
        bool Afgelopen();
        Kleur OverwegendeKleur();
        List<Direction> ZetMogelijk(int rijZet, int kolomZet);
        bool PlacePiece(int rijZet, int kolomZet);
        void PlacePiecesInDirections(List<Direction> directions, Kleur colour, int x, int y);
    }
}
