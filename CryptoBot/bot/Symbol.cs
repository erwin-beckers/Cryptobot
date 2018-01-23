﻿using Binance.Net;
using Binance.Net.Objects;
using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;

namespace CryptoBotApp.bot
{
    public class Symbol : ISymbol
    {
        private readonly int MAX_LIMIT_CANDLES = 200;
        private BinanceClient _client;
        private List<ICandle> _candles;
        private DateTime _lastRefresh;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="client">client handle to exchange</param>
        /// <param name="name">name of the symbol</param>
        /// <param name="timeFrame">time frame of current chart</param>
        public Symbol(BinanceClient client, string name, TimeFrame timeFrame)
        {
            _client = client;
            Name = name;
            TimeFrame = timeFrame;
        }

        /// <summary>
        /// Returns the symbol name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns the time frame
        /// </summary>
        public TimeFrame TimeFrame { get; }

        /// <summary>
        /// List of candles
        /// </summary>
        public IList<ICandle> Candles => _candles;

        /// <summary>
        /// Returns the current candle which is still open and running
        /// </summary>
        public ICandle CurrentCandle => Candles?[0];

        /// <summary>
        /// Returns the previous closed candle
        /// </summary>
        public ICandle PreviousCandle => Candles?[1];

        public decimal AverageCandleSize { get; private set; }

        /// <summary>
        /// Refreshes candle sticks with exchange
        /// </summary>
        public bool Refresh()
        {
            // refresh candles every 30 mins.
            if (PreviousCandle != null)
            {
                var timeSpan = DateTime.Now - _lastRefresh;
                if (timeSpan.TotalMinutes < 30) return true;
            }

            var interval = KlineInterval.OneHour;
            switch (TimeFrame)
            {
                case TimeFrame.Month:
                    interval = KlineInterval.OneMonth;
                    break;

                case TimeFrame.Week:
                    interval = KlineInterval.OneWeek;
                    break;

                case TimeFrame.Day:
                    interval = KlineInterval.OneDay;
                    break;

                case TimeFrame.FourHour:
                    interval = KlineInterval.FourHour;
                    break;

                case TimeFrame.OneHour:
                    interval = KlineInterval.OneHour;
                    break;
            }
            Console.WriteLine($"{Name} refresh...");
            var result = _client.GetKlines(Name, interval, null, null, MAX_LIMIT_CANDLES);
            if (!result.Success)
            {
                Console.WriteLine($"{Name} unable to refresh candle sticks");
                Console.WriteLine(result.Error);
                return false;
            }

            _candles = new List<ICandle>();
            for (int i = result.Data.Length - 1; i >= 0; i--)
            {
                var kline = result.Data[i];
                _candles.Add(new Candle()
                {
                    Open = kline.Open,
                    Close = kline.Close,
                    High = kline.High,
                    Low = kline.Low,
                    OpenTime = kline.OpenTime,
                    CloseTime = kline.CloseTime
                });
            }

            _lastRefresh = DateTime.Now;

            AverageCandleSize = 0;

            /// calculate average candle size
            for (int i=1; i < Candles.Count; ++i)
            {
                AverageCandleSize += Candles[i].Range;
            }
            AverageCandleSize /= ((decimal)(Candles.Count - 1));

            return true;
        }
    }
}