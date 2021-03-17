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

        //TODO: Remove this if works 
        //public string[] CreateSerialisableBoard(Kleur[,] board)
        //{
        //    string[] BoardRows = new string[64];


        //    string[] row = new string[8];

        //    for (int i = 0; i < board.GetLength(0); i++)
        //    {
        //        for(int j = 0; j < board.GetLength(1); j++)
        //        {
        //            row[j] = board[i, j].ToString();
        //        }
        //        BoardRows[i] = row;
        //        ColumnColour = "";
        //    }
        //    return BoardRows;
        //}
    }
}