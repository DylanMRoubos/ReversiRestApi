using System;
namespace ReversiRestApi.Model
{
    public struct PlacePiece
    {
        public int x { get; set; }
        public int y { get; set; }
        public string playerToken { get; set; }
        public string gameToken { get; set; }
        public bool pass { get; set; }

    }
}
