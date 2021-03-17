using System;
using System.Collections.Generic;

namespace ReversiRestApi.Model
{
    public class Spel : ISpel
    {
        public Spel()
        {
            Bord = new Kleur[8, 8];
            AandeBeurt = Kleur.Zwart;
            ResetBoard();
            PrintBoard();
        }

        private Dictionary<string, Kleur> PlayerMapping = new Dictionary<string, Kleur>();
        public int ID { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
        private string _speler1Token;
        public string PlayerToken1
        {
            get => _speler1Token;
            set
            {
                _speler1Token = value;
                if (Speler2Token != null)
                {
                    PlayerMapping.TryAdd(PlayerToken1, Kleur.Zwart);

                }
            }
        }
        private string _speler2Token;
        public string Speler2Token
        {
            get => _speler2Token;
            set
            {
                _speler2Token = value;
                if(Speler2Token != null)
                {
                    PlayerMapping.TryAdd(Speler2Token, Kleur.Wit);
                }
                
            }            
        }
        public Kleur[,] Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        public bool Finished { get; set; }
        public string Winner { get; set; }
        
        public Kleur GetPlayerColour(string playerToken)
        {
            try
            {
                Kleur playerColour;

                PlayerMapping.TryGetValue(playerToken, out playerColour);

                return playerColour;
            }
            catch (Exception _)
            {
                return Kleur.Geen;
            }
            
        }

        public bool Afgelopen()
        {
            //Check if all squares are placed
            for (int i = 0; i < Bord.GetLength(1); i++)
            {
                for (int j = 0; j < Bord.GetLength(0); j++)
                {
                    //Check if piece can be placed if square is empty
                    if(Bord[i, j] == Kleur.Geen)
                    {
                        if(ZetMogelijk(i, j))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool DoeZet(int rijZet, int kolomZet)
        {
            if(ZetMogelijk(rijZet, kolomZet)) {
                Bord[rijZet, kolomZet] = AandeBeurt;
                return Pas();
            }

            return false;
        }

        //Method to surrender game
        public bool Surrender(string playerID)
        {
            if(playerID.Equals(PlayerToken1))
            {
                Winner = Speler2Token;
                Finished = true;
                return true;

            }
            else if (playerID.Equals(Speler2Token))
            {
                Winner = PlayerToken1;
                Finished = true;
                return true;
            }
            else
            {
                //Coulndt find player to surrender
                return false;
            }
        }

        //Check which colour has more pieces
        public Kleur OverwegendeKleur()
        {
            //Create count variables for both colours
            int whiteCount = 0;
            int blackCount = 0;

            //Loop over all coordinates
            for (int i = 0; i < Bord.GetLength(1); i++)
            {
                for (int j = 0; j < Bord.GetLength(0); j++)
                {
                    //Check to who the coordinate belongs & add to the count variable
                    if (Bord[i, j] == Kleur.Wit) whiteCount++;
                    else if (Bord[i, j] == Kleur.Zwart) blackCount++;
                }
            }
            //Check the highest count & return corresponding colour
            if (whiteCount > blackCount) return Kleur.Wit;
            else if (blackCount > whiteCount) return Kleur.Zwart;
            else return Kleur.Geen;
        }

        public bool Pas()
        {
            if(AandeBeurt == Kleur.Wit)
            {
                AandeBeurt = Kleur.Zwart;
                return true;
            }
            else if (AandeBeurt == Kleur.Zwart)
            {
                AandeBeurt = Kleur.Wit;
                return true;
            }
            return false;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            //return false if piece is placed outside of board
            if (rijZet > 7 || kolomZet > 7 || kolomZet < 0 || rijZet < 0) return false;

            //check if no other piece is in place
            if (Bord[rijZet, kolomZet] != Kleur.Geen) return false;

            //Check if there is a piece of the different color connected
            Kleur[,] surroundingPieces = new Kleur[3, 3];

            int x = rijZet - 1;
            int y = kolomZet - 1;
            int direction = 0;
            for (int i = 0; i < 3; i++)
            {
                if(x <= 7 && x >= 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (y <= 7 && y >= 0)
                        {
                            if ((AandeBeurt == Kleur.Wit) && (Bord[x, y] == Kleur.Zwart))
                            {
                                if (CheckForPieceInDirection((Direction)direction, Kleur.Wit, x, y))
                                {

                                    PlacePieceInDirection((Direction)direction, Kleur.Wit, x, y);
                                    return true;
                                }
                            }
                            else if ((AandeBeurt == Kleur.Zwart) && (Bord[x, y] == Kleur.Wit))
                            {
                                //check if there is a black piece in direction of the black piece
                                if (CheckForPieceInDirection((Direction)direction, Kleur.Zwart, x, y))
                                {
                                    PlacePieceInDirection((Direction)direction, Kleur.Zwart, x, y);
                                    return true;
                                }
                            }
                            surroundingPieces[i, j] = Bord[x, y];
                            y++;
                            direction++;
                        } else
                        {
                            direction++;
                            y++;
                        }
                       
                    }
                    y = y - 3;
                } else
                {
                    direction = direction + 3;
                }                
                x++;
                
            }

            //PrintSurrounding(surroundingPieces);
            return false;
        }

        public void PrintBoard()
        {
            for (int i = 0; i < Bord.GetLength(0); i++)
            {
                for (int j = 0; j < Bord.GetLength(1); j++)
                {
                    Console.Write(Bord[i, j]);
                }
                Console.WriteLine();
            }
        }

        //public void PrintSurrounding(Kleur[,] surroundingPieces)
        //{
        //    for (int i = 0; i < surroundingPieces.GetLength(1); i++)
        //    {
        //        for (int j = 0; j < surroundingPieces.GetLength(0); j++)
        //        {
        //            Console.Write(surroundingPieces[i, j]);
        //        }
        //        Console.WriteLine();
        //    }
        //}

        public void ResetBoard()
        {
            for (int i = 0; i < Bord.GetLength(1); i++)
            {
                for (int j = 0; j < Bord.GetLength(0); j++)
                {
                    if ((i == 3 && j == 3) || (i == 4 && j == 4))
                    {
                        Bord[i, j] = Kleur.Wit;
                    }
                    else if ((i == 3 && j == 4) || (i == 4 && j == 3))
                    {
                        Bord[i, j] = Kleur.Zwart;
                    }
                    else
                    {
                        Bord[i, j] = Kleur.Geen;
                    }

                }
            }
        }

        //TODO: add method for diagnals && add comments
        public bool CheckForPieceInDirection(Direction direction, Kleur colour, int row, int col)
        {
            bool result = false;

            switch (direction)
            {
                case Direction.North:
                    for (int i = row; i >= 0; i--) { if (Bord[i, col] == colour) { result = true; break; } if (Bord[i, col] == Kleur.Geen) { break; } }
                    break;
                case Direction.South:
                    for (int i = row; i < 8; i++) { if (Bord[i, col] == colour) { result = true; break; } if (Bord[i, col] == Kleur.Geen) { break; } }
                    break;
                case Direction.West:
                    for (int i = col; i >= 0; i--) { if (Bord[row, i] == colour) { result = true; break; } if (Bord[row, i] == Kleur.Geen) { break; } }
                    break;
                case Direction.East:
                    for (int i = col; i < 8; i++) { if (Bord[row, i] == colour) { result = true; break; } if (Bord[row, i] == Kleur.Geen) { break; } }
                    break;
                case Direction.NorthEast:
                    for (int i = row, j = col, k = 0; k <= Math.Min(row, 8 - col); i--, j++, k++) { if (Bord[i, j] == colour) { result = true; break; } if (Bord[i, j] == Kleur.Geen) { break; } }
                    break;
                case Direction.SouthWest:
                    for (int i = row, j = col, k = 0; k <= Math.Min(8 - row, col); i++, j--, k++) { if (Bord[i, j] == colour) { result = true; break; } if (Bord[i, j] == Kleur.Geen) { break; } }
                    break;
                case Direction.NorthWest:
                    for (int i = row, j = col, k = Math.Min(row, col); k >= 0; i--, j--, k--) { if (Bord[i, j] == colour) { result = true; break; } if (Bord[i, j] == Kleur.Geen) { break; } }
                    break;
                case Direction.SouthEast:
                    for (int i = row, j = col, k = Math.Max(row, col); k < 8; i++, j++, k++) { if (Bord[i, j] == colour) { result = true; break; } if (Bord[i, j] == Kleur.Geen) { break; } }
                    break;
            }
            return result;
        }
        public void PlacePieceInDirection(Direction direction, Kleur colour, int row, int col)
        {
            switch (direction)
            {
                case Direction.North:
                    for (int i = row; i >= 0; i--) { if (Bord[i, col] != Kleur.Geen) { Bord[i, col] = colour; } else { break; } }
                    break;
                case Direction.South:
                    for (int i = row; i < 8; i++) { if (Bord[i, col] != Kleur.Geen) { Bord[i, col] = colour; } else { break; } }
                    break;
                case Direction.West:
                    for (int i = col; i >= 0; i--) { if (Bord[row, i] != Kleur.Geen) { Bord[row, i] = colour; } else { break; } }
                    break;
                case Direction.East:
                    for (int i = col; i < 8; i++) { if (Bord[row, i] != Kleur.Geen) { Bord[row, i] = colour; } else { break; } }
                    break;
                case Direction.NorthEast:
                    for (int i = row, j = col, k = 0; k <= Math.Min(row, 8 - col); i--, j++, k++) { if (Bord[i, j] != Kleur.Geen) { Bord[j, i] = colour; } else { break; } }
                    break;
                case Direction.SouthWest:
                    for (int i = row, j = col, k = 0; k <= Math.Min(8 - row, col); i++, j--, k++) { if (Bord[i, j] != Kleur.Geen) { Bord[j, i] = colour; } else { break; } }
                    break;
                case Direction.NorthWest:
                    for (int i = row, j = col, k = Math.Min(row, col); k >= 0; i--, j--, k--) { if (Bord[i, j] != Kleur.Geen) { Bord[j, i] = colour; } else { break; } }
                    break;
                case Direction.SouthEast:
                    for (int i = row, j = col, k = Math.Max(row, col); k < 8; i++, j++, k++) { if (Bord[i, j] != Kleur.Geen) { Bord[j, i] = colour; } else { break; } }
                    break;
            }
        }
    }
}
