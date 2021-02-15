using System;
namespace ReversiRestApi.Model
{
    public class SpelTbvJson
    {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }
        public string[] Bord { get; set; }
        public Kleur AandeBeurt { get; set; }


        public SpelTbvJson(Spel spel)
        {
            ID = spel.ID;
            Omschrijving = spel.Omschrijving;
            Token = spel.Token;
            Speler1Token = spel.Speler1Token;
            Speler2Token = spel.Speler2Token;
            Bord = CreateSerialisableBoard(spel.Bord);
            AandeBeurt = spel.AandeBeurt;
        }

        public string[] CreateSerialisableBoard(Kleur[,] board)
        {
            string[] BoardRows = new string[board.GetLength(0)];
            string ColumnColour = "";

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    ColumnColour += board[i, j];
                    if (j < board.GetLength(1) - 1) { ColumnColour += ", "; };
                }
                BoardRows[i] = ColumnColour;
                ColumnColour = "";
            }
            return BoardRows;
        }
    }
}