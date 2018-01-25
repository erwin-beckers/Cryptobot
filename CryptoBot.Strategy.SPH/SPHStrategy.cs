using Cryptobot.Interfaces;
using System;

namespace CryptoBot.Strategy.SPH
{
    public class SPHStrategy : IStrategy
    {
        private Signal _signal;
        public ISymbol Symbol { get; }
        public Signal Signal => _signal;

        public SPHStrategy(ISymbol symbol)
        {
            Symbol = symbol;
            _signal = new Signal();
        }

        public Signal Process()
        {
            // current and previous candles should be red
            if (Symbol.CurrentCandle.IsGreen) return null;
            if (Symbol.PreviousCandle.IsGreen) return null;
            if (Symbol.PreviousCandle.Range == 0.0M) return null;
            if (Symbol.PreviousCandle.Range < Symbol.AverageCandleSize) return null;

            // previous candle should be a big candle
            decimal percentage = (Symbol.PreviousCandle.Range / Symbol.AverageCandleSize) * 100M;
            Console.WriteLine($"{Symbol.Name} previous candle: {percentage:N2}% of average candle size");
            if (Symbol.PreviousCandle.Range < Symbol.AverageCandleSize * 2) return null;

            // and we should have some stability before
            int greenCandleCount = 0;
            int averageCandles = 0;
            for (int i = 0; i < 9; ++i)
            {
                var candle = Symbol.Candles[2 + i];
                if (candle.IsGreen) greenCandleCount++;
                if (candle.Range >= Symbol.AverageCandleSize * 0.8M && candle.Range <= Symbol.AverageCandleSize * 1.2M) averageCandles++;
            }

            if (greenCandleCount < 4 || greenCandleCount > 6) return null;
            if (averageCandles < 6) return null;

            Console.WriteLine($"{Symbol.Name} ---- SPH detected ---");
            return null;
        }
    }
}