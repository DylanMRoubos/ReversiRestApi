using System;
using System.Collections.Generic;

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
        bool ZetMogelijk(int rijZet, int kolomZet);
        bool PlacePiece(int rijZet, int kolomZet);
    }
}
