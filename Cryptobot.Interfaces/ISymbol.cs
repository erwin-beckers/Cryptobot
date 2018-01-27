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

        /// <summary>
        /// returns the candle with the lowest wick
        /// </summary>
        /// <returns>candle with lowest wick.</returns>
        /// <param name="count">period</param>
        /// <param name="startBar">Start bar.</param>
        ICandle Lowest(int count, int startBar);

        /// <summary>
        /// returns the candle with the highest wick
        /// </summary>
        /// <returns>candle with highest wick.</returns>
        /// <param name="count">period</param>
        /// <param name="startBar">Start bar.</param>
        ICandle Highest(int count, int startBar);

        /// <summary>
        /// returns point value of this symbol.
        /// </summary>
        /// <value>The point value</value>
        decimal Point { get; }

        /// <summary>
        /// return the # of digits of this symbol.
        /// </summary>
        int Digits { get; }

        /// <summary>
        /// returns the nice name for this symbol
        /// </summary>
        string NiceName { get; }

        /// <summary>
        /// returns index to candle with the lowest price
        /// </summary>
        int LowestBar { get; }

        /// <summary>
        /// returns index to candle with the highest price
        /// </summary>
        /// 
        int HighestBar { get; }

        /// <summary>
        /// sends an alert to telegram
        /// </summary>
        /// <param name="text">the alert</param>
        void SendAlert(string text);

        string NiceTimeFrame { get; }

        decimal Price { get; }
    }
}