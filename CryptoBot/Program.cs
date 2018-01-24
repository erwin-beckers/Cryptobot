using Cryptobot.Interfaces;
using CryptoBot.Strategy.SPH;
using CryptoBot.Strategy.TrendReversal;
using CryptoBotApp.bot;
using System;
using System.Configuration;

namespace CryptoBotApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var apiKey = ConfigurationManager.AppSettings["apikey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("api key is empty in app.config");
                return;
            }

            var apiSecret = ConfigurationManager.AppSettings["apisecret"];
            if (string.IsNullOrEmpty(apiSecret))
            {
                Console.WriteLine("api secret is empty in app.config");
                return;
            }
             
            using (var bot = new CryptoRobot(apiKey, apiSecret, new TrendReversalStrategyFactory(), TimeFrame.OneHour))
            {
                while (bot.Run())
                {
                }
            }
        }
    }
}