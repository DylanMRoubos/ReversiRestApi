using ReversiRestApi.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;

namespace ReversiRestApi.DAL
{
    public class SpelAccessLayer : ISpelRepository
    {
        private const string ConnectionString = @"Server=localhost;Database=ReversiDbRestApi;User Id=SA;Password=S7N4b8RsVCgvKqEx;";

        //SQL querys to allow insertation into db
        private const string IdOn = "SET IDENTITY_INSERT [dbo].[Games] ON";
        private const string IdOff = "SET IDENTITY_INSERT [dbo].[Games] Off";

        //Insert game to database table & insert all cell values into cell table with the game token
        public void AddSpel(Spel spel)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCmdOn = new SqlCommand(IdOn, sqlCon);
                SqlCommand sqlCmdOff = new SqlCommand(IdOff, sqlCon);

                sqlCon.Open();
                sqlCmdOff.ExecuteNonQuery();
                //INSET querys for game and bord
                string addSpelQuery = "INSERT INTO Games (Token, Speler1Token, Omschrijving, AandeBeurt) VALUES(@Token, @Speler1Token, @Omschrijving, @AandeBeurt)";
                string addBordQuery = "INSERT INTO Cell (Token, Row, Col, Kleur) VALUES (@Token, @Row, @Col, @Kleur)";
                
                SqlCommand sqlCmd = new SqlCommand(addSpelQuery, sqlCon);
                sqlCmd.Parameters.AddWithValue("@Token", spel.Token);
                sqlCmd.Parameters.AddWithValue("@Speler1Token", spel.PlayerToken1);
                sqlCmd.Parameters.AddWithValue("@Omschrijving", spel.Description);
                sqlCmd.Parameters.AddWithValue("@AandeBeurt", spel.AandeBeurt);               
                
                sqlCmd.ExecuteNonQuery();

                //Loops over game bord and adds all cells with x and y locations and the colour that occupies the space and the game token
                for (int i = 0; i < spel.Bord.GetLength(0); i++)
                {
                    for (int j = 0; j < spel.Bord.GetLength(1); j++)
                    {
                        SqlCommand sqlCmdAddBord = new SqlCommand(addBordQuery, sqlCon);
                        sqlCmdAddBord.Parameters.AddWithValue("@Token", spel.Token);
                        sqlCmdAddBord.Parameters.AddWithValue("@Row", i);
                        sqlCmdAddBord.Parameters.AddWithValue("@Col", j);
                        sqlCmdAddBord.Parameters.AddWithValue("@Kleur", spel.Bord[i, j]);
                        sqlCmdAddBord.ExecuteNonQuery();
                    }
                }

                sqlCmdOff.ExecuteNonQuery();
                sqlCon.Close();

            }
        }

        public void PlacePiece(string spelToken, Spel localGame)
        {

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                Console.WriteLine(localGame.Bord);
                sqlCon.Open();
                string addBordQuery = "UPDATE Cell SET kleur = @Kleur WHERE Token = @Token AND Row = @Row AND Col = @Col";
                //Loops over game bord and adds all cells with x and y locations and the colour that occupies the space and the game token
                for (int i = 0; i < localGame.Bord.GetLength(0); i++)
                {
                    for (int j = 0; j < localGame.Bord.GetLength(1); j++)
                    {
                        SqlCommand sqlCmdAddBord = new SqlCommand(addBordQuery, sqlCon);
                        sqlCmdAddBord.Parameters.AddWithValue("@Token", localGame.Token);
                        sqlCmdAddBord.Parameters.AddWithValue("@Row", i);
                        sqlCmdAddBord.Parameters.AddWithValue("@Col", j);
                        sqlCmdAddBord.Parameters.AddWithValue("@Kleur", localGame.Bord[i, j]);

                        Console.WriteLine(DateTime.Now.ToString() + i + ", " + j + localGame.Bord[i, j] + Thread.CurrentThread.ManagedThreadId);
                        sqlCmdAddBord.ExecuteNonQuery();
                        
                    }
                }
                //Check if game is finished & if it is update
                if (localGame.Finished == true)
                {
                    string addWinner = "UPDATE Games SET Afgelopen = 1, Winnaar = @Winner WHERE Token = @Token";
                    SqlCommand sqlCmd = new SqlCommand(addWinner, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@Winner", localGame.Winner);
                    sqlCmd.Parameters.AddWithValue("@Token", spelToken);

                    sqlCmd.ExecuteNonQuery();
                }
                sqlCon.Close();

            }
            //Adds te amount of piecies from the other player
            AddPieceToHistory(spelToken, localGame.PlayerToken1, localGame.getPieceAmount(Kleur.Wit));
            AddPieceToHistory(spelToken, localGame.Speler2Token, localGame.getPieceAmount(Kleur.Zwart));
            NextTurn(spelToken, localGame.AandeBeurt);
        }

        public Spel GetSpel(string spelToken)
        {
            var spel = new Spel();
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                string query = "SELECT * FROM Games JOIN Cell ON Games.Token = Cell.Token WHERE Games.Token = '" + spelToken + "'";
                sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                SqlDataReader rdr = sqlCmd.ExecuteReader();
                while (rdr.Read())
                {
                    spel.ID = Convert.ToInt32(rdr["ID"]);
                    spel.Token = Convert.ToString(rdr["Token"]);
                    spel.PlayerToken1 = Convert.ToString(rdr["Speler1Token"]);
                    spel.Speler2Token = Convert.ToString(rdr["Speler2Token"]);
                    spel.Description = Convert.ToString(rdr["Omschrijving"]);
                    spel.AandeBeurt = (Kleur)Convert.ToInt32(rdr["AandeBeurt"]);
                    spel.Finished = Convert.ToBoolean(rdr["Afgelopen"]);
                    spel.Winner = Convert.ToString(rdr["Winnaar"]);
                    spel.Bord[Convert.ToInt32(rdr["Row"]), Convert.ToInt32(rdr["Col"])] = (Kleur)Convert.ToInt32(rdr["Kleur"]);                    
                }
                sqlCon.Close();
            }

            return spel;
        }

        public List<Spel> GetSpellen()
        {
            var spelList = new List<Spel>();
            string sqlQuery = "SELECT Token FROM Games";

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlCon);
                SqlDataReader rdr = sqlCmd.ExecuteReader();

                while(rdr.Read())
                {
                    spelList.Add(GetSpel(Convert.ToString(rdr["Token"])));
                }
                sqlCon.Close();
            }
            return spelList;
        }
        public void AddPieceToHistory(string gameToken, string playerToken, int amount)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                string addHistory = "INSERT INTO PieceHistory(GameToken, PlayerToken, Amount) VALUES(@gameToken, @playerToken, @amount)";

                SqlCommand sqlCmd = new SqlCommand(addHistory, sqlCon);
                sqlCmd.Parameters.AddWithValue("@gameToken", gameToken);
                sqlCmd.Parameters.AddWithValue("@playerToken", playerToken);
                sqlCmd.Parameters.AddWithValue("@amount", amount);

                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        public List<GameHistory> GetGameHistory(string Gametoken, string PlayerToken)
        {
            var gameHistory = new List<GameHistory>();
            string sqlQuery = "SELECT GameToken, PlayerToken, Amount, CreateData FROM PieceHistory WHERE Gametoken = @GameToken AND PlayerToken = @PlayerToken ORDER BY CreateData ASC";

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlCon);
                sqlCmd.Parameters.AddWithValue("@GameToken", Gametoken);
                sqlCmd.Parameters.AddWithValue("@PlayerToken", PlayerToken);
                SqlDataReader rdr = sqlCmd.ExecuteReader();

                if(rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        gameHistory.Add(new GameHistory()
                        {
                            GameToken = Convert.ToString(rdr["GameToken"]),
                            PlayerToken = Convert.ToString(rdr["PlayerToken"]),
                            Amount = (int)Convert.ToInt64(rdr["Amount"]),
                            CreateData = Convert.ToString(rdr["CreateData"])
                        });
                    }
                }

               
                sqlCon.Close();
            }
            return gameHistory;
        }

        public void RemoveBord(string gameToken)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {

                sqlCon.Open();
                var sqlCmd = new SqlCommand("DELETE FROM Cell WHERE Token = @Token", sqlCon);

                sqlCmd.Parameters.AddWithValue("@Token", gameToken);

                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        public void RemoveGame(string spelToken)
        {
                using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {

                    sqlCon.Open();
                    var sqlCmd = new SqlCommand("DELETE FROM Games WHERE Token = @Token", sqlCon);

                    sqlCmd.Parameters.AddWithValue("@Token", spelToken);

                    sqlCmd.ExecuteNonQuery();
                }
        }

        //public void Surrender(string gameToken, Spel game)
        //{
        //    using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
        //    {

        //        sqlCon.Open();
        //        var sqlCmd = new SqlCommand("DELETE FROM Games WHERE Token = @Token", sqlCon);

        //        sqlCmd.Parameters.AddWithValue("@Token", spelToken);

        //        sqlCmd.ExecuteNonQuery();
        //    }
        //}
        public void NextTurn(string gameToken, Kleur colour)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                    sqlCon.Open();
                    var sqlCmd = new SqlCommand("UPDATE Games SET AandeBeurt = @AandeBeurt WHERE Token = @gameToken", sqlCon);

                    sqlCmd.Parameters.AddWithValue("@AandeBeurt", (int)colour);
                    sqlCmd.Parameters.AddWithValue("@gameToken", gameToken);
                sqlCmd.ExecuteNonQuery();
                sqlCon.Close();
            }
        }

        public bool AddPlayer(string gameToken, string playerToken)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                try
                {
                    sqlCon.Open();
                    var sqlCmd = new SqlCommand("UPDATE Games SET Speler2Token = @player2Token WHERE Token = @gameToken", sqlCon);

                    sqlCmd.Parameters.AddWithValue("@player2Token", playerToken);
                    sqlCmd.Parameters.AddWithValue("@gameToken", gameToken);

                    var result = sqlCmd.ExecuteNonQuery();
                    sqlCon.Close();
                    if (result != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                } catch(Exception e)
                {
                    return false;
                }                               
            }
        }
    }
}