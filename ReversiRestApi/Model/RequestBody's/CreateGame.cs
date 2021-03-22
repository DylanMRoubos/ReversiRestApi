using System;
namespace ReversiRestApi.Model
{
    public struct CreateGame
    {
        public string PlayerToken1 { get; set; }
        public string Description { get; set; }
    }
}
