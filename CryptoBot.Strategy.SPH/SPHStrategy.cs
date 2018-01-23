using Cryptobot.Interfaces;
using System;

namespace CryptoBot.Strategy.SPH
{
    public class SPHStrategy : IStrategy
    {
        public ISymbol Symbol { get; }

        public SPHStrategy(ISymbol symbol)
        {
            Symbol = symbol;
        }

        public void Process()
        {
            // current and previous candles should be red
            if (Symbol.CurrentCandle.IsGreen) return;
            if (Symbol.PreviousCandle.IsGreen) return;
            if (Symbol.PreviousCandle.Range == 0.0M) return;
            if (Symbol.PreviousCandle.Range < Symbol.AverageCandleSize) return;

            // previous candle should be a big candle
            decimal percentage = (Symbol.PreviousCandle.Range / Symbol.AverageCandleSize) * 100M;
            Console.WriteLine($"{Symbol.Name} previous candle: {percentage:N2}% of average candle size");
            if (Symbol.PreviousCandle.Range < Symbol.AverageCandleSize * 2) return;

            // and we should have some stability before
            int greenCandleCount = 0;
            int averageCandles = 0;
            for (int i = 0; i < 9; ++i)
            {
                var candle = Symbol.Candles[2 + i];
                if (candle.IsGreen) greenCandleCount++;
                if (candle.Range >= Symbol.AverageCandleSize * 0.8M && candle.Range <= Symbol.AverageCandleSize * 1.2M) averageCandles++;
            }

            if (greenCandleCount < 4 || greenCandleCount > 6) return;
            if (averageCandles < 6) return;

            Console.WriteLine($"{Symbol.Name} ---- SPH detected ---");
        }
    }
}