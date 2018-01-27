using Binance.Net;
using Binance.Net.Objects;
using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;

namespace CryptoBotApp.bot
{
    public class Symbol : ISymbol
    {
        private readonly int MAX_LIMIT_CANDLES = 3000;
        private BinanceClient _client;
        private List<ICandle> _candles;
        private DateTime _lastRefresh;
        private string _prevAlert = "";

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
        /// returns the nice name for the symbol
        /// </summary>
        public string NiceName
        {
            get
            {
                var name = Name;
                if (name.EndsWith("ETH")) name = name.Replace("ETH", "/ETH");
                if (name.EndsWith("BTC")) name = name.Replace("BTC", "/BTC");
                if (name.EndsWith("USDT")) name = name.Replace("USDT", "/USDT");
                if (name.EndsWith("BNB")) name = name.Replace("BNB", "/BNB");
                name = name.Replace("//", "/");
                return name;
            }
        }

        /// <summary>
        /// Returns the time frame
        /// </summary>
        public TimeFrame TimeFrame { get; }

        /// <summary>
        /// Returns the point value
        /// </summary>
        public decimal Point { get; private set; }

        /// <summary>
        /// Returns the digits
        /// </summary>
        public int Digits { get; private set; }

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

        /// <summary>
        /// returns the average candle size
        /// </summary>
        public decimal AverageCandleSize { get; private set; }

        public decimal Price
        {
            get
            {
                return _client.GetPrice(Name).Data.Price;
            }
        }
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

            string price = result.Data[0].Open.ToString();
            int length = price.Substring(price.IndexOf(".") + 1).Length;
            price = "0.";

            for (int i = 0; i < length; ++i)
            {
                if (i + 1 < length) price += "0";
                else price += "1";
            }
            Digits = length;
            Point = Decimal.Parse(price);

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
            for (int i = 1; i < Candles.Count; ++i)
            {
                AverageCandleSize += Candles[i].Range;
            }
            AverageCandleSize /= ((decimal)(Candles.Count));

            return true;
        }

        /// <summary>
        /// returns the candle with the lowest wick
        /// </summary>
        /// <returns>candle with lowest wick.</returns>
        /// <param name="count">period</param>
        /// <param name="startBar">Start bar.</param>
        public ICandle Lowest(int count, int startBar)
        {
            ICandle minCandle = null;
            for (int i = 0; i < count; ++i)
            {
                var candle = Candles[i + startBar];
                if (minCandle == null)
                {
                    minCandle = candle;
                }
                else if (candle.Low < minCandle.Low)
                {
                    minCandle = candle;
                }
            }
            return minCandle;
        }

        /// <summary>
        /// returns the candle with the highest wick
        /// </summary>
        /// <returns>candle with highest wick.</returns>
        /// <param name="count">period</param>
        /// <param name="startBar">Start bar.</param>
        public ICandle Highest(int count, int startBar)
        {
            ICandle maxCandle = null;
            for (int i = 0; i < count; ++i)
            {
                var candle = Candles[i + startBar];
                if (maxCandle == null)
                {
                    maxCandle = candle;
                }
                else if (candle.High > maxCandle.High)
                {
                    maxCandle = candle;
                }
            }
            return maxCandle;
        }

        /// <summary>
        /// returns the bar with the lowest price
        /// </summary>
        public int LowestBar
        {
            get
            {
                ICandle minCandle = null;
                int minIndex = 0;
                for (int i = 0; i < Candles.Count; ++i)
                {
                    var candle = Candles[i];
                    if (minCandle == null)
                    {
                        minCandle = candle;
                        minIndex = i;
                    }
                    else if (candle.Low < minCandle.Low)
                    {
                        minCandle = candle;
                        minIndex = i;
                    }
                }
                return minIndex;
            }
        }

        /// <summary>
        /// returns the bar with the highest price
        /// </summary>
        public int HighestBar
        {
            get
            {
                ICandle maxCandle = null;
                int maxIndex = 0;
                for (int i = 0; i < Candles.Count; ++i)
                {
                    var candle = Candles[i];
                    if (maxCandle == null)
                    {
                        maxCandle = candle;
                        maxIndex = i;
                    }
                    else if (candle.High > maxCandle.High)
                    {
                        maxCandle = candle;
                        maxIndex = i;
                    }
                }
                return maxIndex;
            }
        }

        public string NiceTimeFrame
        {
            get
            {
                switch (TimeFrame)
                {
                    case TimeFrame.Day:
                        return "D1";

                    case TimeFrame.FourHour:
                        return "H4";

                    case TimeFrame.Month:
                        return "MN1";

                    case TimeFrame.OneHour:
                        return "H1";
                }
                return TimeFrame.ToString();
            }
        }

        public void SendAlert(string text)
        {
            if (text == _prevAlert) return;
            _prevAlert = text;
            TelegramBot.Send(text);
        }
    }
}