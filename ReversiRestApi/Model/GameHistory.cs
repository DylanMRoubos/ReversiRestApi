using System;
namespace ReversiRestApi.Model
{
    public class GameHistory
    {
        public string GameToken { get; set; }
        public string PlayerToken { get; set; }
        public int Amount { get; set; }
        public string CreateData { get; set; }
    }
}
