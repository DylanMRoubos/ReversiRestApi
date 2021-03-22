using System;
namespace ReversiRestApi.Model
{
    public struct Surrender
    {
        public string gameToken { get; set; }
        public string playerToken { get; set; }
    }
}
