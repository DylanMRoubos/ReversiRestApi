using System;
namespace ReversiRestApi.Model
{
    public struct JoinGame
    {
        public string playerToken { get; set; }
        public string gameToken { get; set; }
    }
}
