using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReversiRestApi.DAL;
using ReversiRestApi.Model;

namespace ReversiRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {


            //Arrange
           //Spel spel = new Spel();
           // spel.Bord[2, 2] = Kleur.Wit;
           // spel.Bord[2, 3] = Kleur.Wit;
           // spel.Bord[2, 4] = Kleur.Wit;
           // spel.Bord[3, 4] = Kleur.Wit;
           // spel.Bord[4, 4] = Kleur.Wit;
           // spel.Bord[3, 2] = Kleur.Zwart;
           // spel.Bord[3, 3] = Kleur.Zwart;
           // spel.Bord[4, 3] = Kleur.Zwart;
           // //     0 1 2 3 4 5 6 7
           // //               \/
           // // 0   0 0 0 0 0 0 0 0  
           // // 1   0 0 0 0 0 0 0 0  <
           // // 2   0 0 1 1 1 0 0 0
           // // 3   0 0 2 2 1 0 0 0
           // // 4   0 0 0 2 1 0 0 0
           // // 5   0 0 0 0 0 0 0 0
           // // 6   0 0 0 0 0 0 0 0
           // // 7   0 0 0 0 0 0 0 0

           // // Act
           // spel.AandeBeurt = Kleur.Zwart;
           // var actual = spel.PlacePiece(1, 5);

           // spel.PrintBoard();
           // Console.WriteLine("-=-=-");
           // spel.PrintBoard();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
