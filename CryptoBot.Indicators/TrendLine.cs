using System;
using Cryptobot.Interfaces;

namespace CryptoBot.Indicators
{
    public class TrendLine
    {
        private decimal[] _trendNone;
        private decimal[] _trendRed;
        private decimal _filterNumber;
        private decimal _period;

        public TrendLine(decimal filterNumber = 3, decimal period = 10)
        {
            _filterNumber = filterNumber;
            _period = period;
        }

        //--------------------------------------------------------------------
        private decimal TrendMA(ISymbol symbol, int startBar, int period)
        {
            return MovingAverage.Get(symbol, startBar, period, MaMethod.LWMA, AppliedPrice.Close);
        }

        //--------------------------------------------------------------------
        bool IsGreen(int bar)
        {
            if (bar < 0 || bar >= _trendRed.Length) return false;
            var x0 = bar > 0 ? _trendRed[bar - 1] : 0;
            var x1 = _trendRed[bar];
            var x2 = _trendRed[bar + 1];
            if (x0 >= x1) return true;
            if (x0 >= x2) return true;
            return false;
        }

        //--------------------------------------------------------------------
        bool IsRed(int bar)
        {
            if (bar < 0 || bar >= _trendRed.Length) return false;
            var x0 = bar > 0 ? _trendRed[bar - 1] : 0;
            var x1 = _trendRed[bar];
            var x2 = _trendRed[bar + 1];
            if (x0 <= x1) return true;
            if (x0 <= x2) return true;
            return false;
        }

        //--------------------------------------------------------------------
        void Refresh(ISymbol symbol)
        {
            int limit = 500;
            _trendNone = new decimal[symbol.Candles.Count];
            _trendRed = new decimal[symbol.Candles.Count];

            for (int i = 0; i < limit; i++)
            {
                _trendNone[i] = 2 * TrendMA(symbol, i, (int)Math.Round(_period / _filterNumber)) - TrendMA(symbol, i, (int)_period);
            }

            int maPeriod = (int)Math.Round(Math.Sqrt((double)_period));
            for (int i = 0; i < limit; i++)
            {
                // _trendRed[i] = iMAOnArray(_trendNone, 0, maPeriod, 0, ma_method, i);
                _trendRed[i] = MovingAverage.OnArray(_trendNone, 0, maPeriod, 0, MaMethod.LWMA, i);
            }

        }
    }
}
