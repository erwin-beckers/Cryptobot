using System.Collections.Generic;

namespace Cryptobot.Interfaces
{
    public interface ISymbol
    {
        /// <summary>
        /// Returns the symbol name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Returns the timeframe
        /// </summary>
        TimeFrame TimeFrame { get; }

        /// <summary>
        /// List of candles
        /// </summary>
        IList<ICandle> Candles { get; }

        /// <summary>
        /// Returns the current candle which is still open and running
        /// </summary>
        ICandle CurrentCandle { get; }

        /// <summary>
        /// Returns the previous closed candle
        /// </summary>
        ICandle PreviousCandle { get; }

        /// <summary>
        /// returns the average size of a candle
        /// </summary>
        decimal AverageCandleSize { get; }

        /// <summary>
        /// Refreshes candle sticks with exchange
        /// </summary>
        /// <returns>true when candles are refreshed, otherwise false</returns>
        bool Refresh();
    }
}