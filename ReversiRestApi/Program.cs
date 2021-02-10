using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReversiRestApi.Model;

namespace ReversiRestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Spel spel = new Spel();
            spel.Bord[1, 1] = Kleur.Wit;
            spel.Bord[2, 2] = Kleur.Wit;
            spel.Bord[5, 5] = Kleur.Zwart;
            //     0 1 2 3 4 5 6 7
            //     v
            // 0   2 0 0 0 0 0 0 0  <
            // 1   0 1 0 0 0 0 0 0
            // 2   0 0 1 0 0 0 0 0
            // 3   0 0 0 1 2 0 0 0
            // 4   0 0 0 2 1 0 0 0
            // 5   0 0 0 0 0 2 0 0
            // 6   0 0 0 0 0 0 0 0
            // 7   0 0 0 0 0 0 0 0 
            // Act
            spel.AandeBeurt = Kleur.Zwart;
            var actual = spel.DoeZet(0, 0);

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
