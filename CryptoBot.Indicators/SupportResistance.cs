using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoBot.Indicators
{
    public enum Details
    {
        Minium,
        MediumLow,
        Medium,
        MediumHigh,
        Maximum
    };

    //+------------------------------------------------------------------+
    internal class SRLine
    {
        public int StartBar { get; set; }
        public int EndBar { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int Touches { get; set; }
        public TimeFrame Timeframe { get; set; }
    };

    public class SupportResistance
    {
        private int _barsHistory = 3000;
        private Details _detailLevel = Details.Medium;
        private ISymbol _symbol;
        private decimal _maxDistance;
        private int _previousDay;
        private List<SRLine> _lines;

        //+------------------------------------------------------------------+
        public SupportResistance(ISymbol symbol)
        {
            _symbol = symbol;
            _previousDay = -1;
            _lines = new List<SRLine>();
        }

        //+------------------------------------------------------------------+
        private bool DoesSRLevelExists(decimal price, decimal range, out SRLine srLine)
        {
            srLine = null;
            if (_lines.Count <= 0) return false;
            foreach (var line in _lines)
            {
                var diff = Math.Abs(price - line.Price);
                if (diff <= range)
                {
                    srLine = line;
                    return true;
                }
            }
            return false;
        }

        //+------------------------------------------------------------------+
        private int GetTouches(ZigZag zigZag, int barPrice, int maxBars, ref decimal price, ref DateTime startTime, ref int startBar)
        {
            int cnt = 0;
            decimal totalPrice = price;
            decimal totalCnt = 1.0M;
            decimal lowest = price;
            decimal highest = price;

            for (int bar = barPrice + 1; bar < maxBars; bar++)
            {
                var candle = _symbol.Candles[bar];
                var arrow = zigZag.GetArrow(bar);
                if (arrow == ArrowType.None) continue;

                var lo = candle.Low;
                var hi = candle.High;

                var diffLo = Math.Abs(lo - price);
                var diffHi = Math.Abs(hi - price);
                if (diffLo < _maxDistance)
                {
                    cnt++;
                    startTime = candle.OpenTime;
                    startBar = bar;
                    totalPrice += lo;
                    totalCnt += 1.0M;
                    lowest = Math.Min(lowest, lo);
                    //var pips = diffLo / (10.0M * _symbol.Point);
                    //if (logEnable) Print("price:",price," bar:",bar, " low:",lo, " date:", startTime, " pips:",pips);
                }
                else if (diffHi <= _maxDistance)
                {
                    cnt++;
                    startTime = candle.OpenTime;
                    startBar = bar;
                    totalPrice += hi;
                    totalCnt += 1.0M;
                    highest = Math.Max(highest, hi);
                    //decimal pips = diffHi / (10.0 * Point());
                    //if (logEnable) Print("price:",price," bar:",bar, " hi:",hi,"  date:",startTime, " pips:",pips);
                }
            }

            //if (logEnable) Print("lowest:", lowest,"  highest:", highest);
            //decimal diffHi=MathAbs(highest-price);
            //decimal diffLo=MathAbs(lowest-price);
            //if (diffHi > diffLo) price=diffHi;
            //else price=diffLo;

            //price = totalPrice / totalCnt;
            return cnt;
        }

        //+------------------------------------------------------------------+
        private bool DoesLevelExists(int bar, decimal price, DateTime mostRecent)
        {
            foreach (var line in _lines)
            {
                var diff = Math.Abs(price - line.Price);
                if (diff < _maxDistance)
                {
                    if (mostRecent > line.EndDate)
                    {
                        line.EndDate = mostRecent;
                        line.EndBar = bar;
                    }
                    return true;
                }
            }
            return false;
        }

        //+------------------------------------------------------------------+
        private void Refresh()
        {
            _lines = new List<SRLine>();

            var barsAvailable = _symbol.Candles.Count;
            var bars = Math.Min(_barsHistory, barsAvailable);
            var lowestBar = _symbol.LowestBar;
            var highestBar = _symbol.HighestBar;
            var highestPrice = _symbol.Candles[highestBar].High;
            var lowestPrice = _symbol.Candles[lowestBar].Low;

            decimal priceRange = highestPrice - lowestPrice;

            decimal div = 30.0M;
            switch (_detailLevel)
            {
                case Details.Minium:
                    div = 10.0M;
                    break;

                case Details.MediumLow:
                    div = 20.0M;
                    break;

                case Details.Medium:
                    div = 30.0M;
                    break;

                case Details.MediumHigh:
                    div = 40.0M;
                    break;

                case Details.Maximum:
                    div = 50.0M;
                    break;
            }
            _maxDistance = priceRange / div;

            var zigZag = new ZigZag(12, 5, 3);
            zigZag.Refresh(_symbol);

            bool skipFirstArrow = true;
            for (int bar = 1; bar < bars; bar++)
            {
                var candle = _symbol.Candles[bar];
                var arrow = zigZag.GetArrow(bar);
                if (arrow == ArrowType.None) continue;
                if (skipFirstArrow)
                {
                    skipFirstArrow = false;
                    continue;
                }

                if (arrow == ArrowType.Buy)
                {
                    var price = candle.Low;
                    var time = candle.OpenTime;
                    var startTime = time;
                    int startBar = bar;
                    if (!DoesLevelExists(bar, price, startTime))
                    {
                        int touches = GetTouches(zigZag, bar, bars, ref price, ref startTime, ref startBar);
                        if (touches >= 0)
                        {
                            var line = new SRLine();
                            line.Price = price;
                            line.Touches = touches;
                            line.EndBar = bar;
                            line.EndDate = time;
                            line.StartDate = startTime;
                            line.StartBar = startBar;
                            _lines.Add(line);
                        }
                    }
                }
                else if (arrow == ArrowType.Sell)
                {
                    var price = candle.High;
                    var time = candle.OpenTime;
                    var startTime = time;
                    int startBar = bar;
                    if (!DoesLevelExists(bar, price, startTime))
                    {
                        int touches = GetTouches(zigZag, bar, bars, ref price, ref startTime, ref startBar);
                        if (touches >= 0)
                        {
                            var line = new SRLine();
                            line.Price = price;
                            line.Touches = touches;
                            line.EndBar = bar;
                            line.EndDate = time;
                            line.StartDate = startTime;
                            line.StartBar = startBar;
                            _lines.Add(line);
                        }
                    }
                }
            }

            // add s/r line for highest price
            var mostRecentTime = _symbol.Candles[highestBar].OpenTime;
            if (!DoesLevelExists(highestBar, highestPrice, mostRecentTime))
            {
                var line = new SRLine();
                line.Price = highestPrice;
                line.Touches = 1;
                line.StartBar = highestBar;
                line.StartDate = _symbol.Candles[highestBar].OpenTime;
                line.EndDate = _symbol.Candles[0].CloseTime;
                line.EndBar = 0;
                _lines.Add(line);
            }

            // add s/r line for lowest price
            mostRecentTime = _symbol.Candles[lowestBar].OpenTime;
            if (!DoesLevelExists(lowestBar, lowestPrice, mostRecentTime))
            {
                var line = new SRLine();
                line.Price = lowestPrice;
                line.Touches = 1;
                line.StartBar = lowestBar;
                line.StartDate = _symbol.Candles[lowestBar].OpenTime;
                line.EndDate = _symbol.Candles[0].CloseTime;
                line.EndBar = 0;
            }
        }

        //+------------------------------------------------------------------+
        private void Calculate(bool forceRefresh = false)
        {
            int day = DateTime.Now.DayOfYear;
            if (day != _previousDay || forceRefresh)
            {
                _previousDay = day;
                _maxDistance = 0;
                Refresh();

                _lines = _lines.OrderBy(e => e.Price).ToList();
            }
        }

        //+------------------------------------------------------------------+
        public bool IsAtSupportResistance(decimal price, decimal range)
        {
            Calculate();
            SRLine line;
            if (DoesSRLevelExists(price, range, out line)) return true;
            return false;
        }

        //+------------------------------------------------------------------+
        public decimal GetNextSupportLevel(decimal price, decimal range)
        {
            Calculate();
            SRLine line;
            if (!DoesSRLevelExists(price, range, out line)) return -1;
            if (line == null) return-1;

            for (int i=1; i < _lines.Count; ++i)
            {
                if (_lines[i] == line)
                {
                    return _lines[i - 1].Price;
                }
            }
            return -1;
        }

        //+------------------------------------------------------------------+
        public decimal GetNextResistanceLevel(decimal price, decimal range)
        {
            Calculate();
            SRLine line;
            if (!DoesSRLevelExists(price, range, out line)) return -1;
            if (line == null) return -1;

            for (int i = 0; i+1 < _lines.Count; ++i)
            {
                if (_lines[i] == line)
                {
                    return _lines[i + 1].Price;
                }
            }
            return -1;
        }
    }
}