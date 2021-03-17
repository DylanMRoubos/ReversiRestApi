using ReversiRestApi.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
                    spel.Bord[Convert.ToInt32(rdr["Col"]), Convert.ToInt32(rdr["Row"])] = (Kleur)Convert.ToInt32(rdr["Kleur"]);
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
            }
            return spelList;
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
    }
}