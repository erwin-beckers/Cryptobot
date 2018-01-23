using Binance.Net;
using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace CryptoBotApp.bot
{
    public class CryptoRobot : IDisposable
    {
        private List<IStrategy> _pairs = new List<IStrategy>();
        private BinanceClient _client;

        public CryptoRobot(string apiKey, string apiSecret, IStrategyFactory factory, TimeFrame timeframe)
        {
            Console.WriteLine("Opening connection to binance...");
            _client = new BinanceClient(apiKey, apiSecret);

            var result = _client.GetAllPrices();
            if (!result.Success)
            {
                Console.WriteLine("=== unable to get symbols ===");
                Console.WriteLine(result.Error.Message);
                _client.Dispose();
                _client = null;
                return;
            }

            Console.WriteLine($"Initializing strategy for {result.Data.Length} symbols for timeframe: {timeframe}");
            foreach (var symbol in result.Data)
            {
                _pairs.Add(factory.Create(new Symbol(_client, symbol.Symbol, timeframe)));
            }
            Console.WriteLine("bot running...");
        }

        public bool Run()
        {
            if (null == _client) return false;

            foreach (var pair in _pairs)
            {
                pair.Symbol.Refresh();
                pair.Process();
            }

            Thread.Sleep(60 * 1000);
            return true;
        }

        public void Dispose()
        {
            _client?.Dispose();
            _client = null;
        }
    }
}