using Cryptobot.Interfaces;

namespace CryptoBot.Indicators
{
    public enum ArrowType
    {
        None,
        Buy,
        Sell
    };

    public class ZigZag
    {
        private decimal[] _zigZagBufferBuy;
        private decimal[] _zigZagBufferSell;
        private int _extDepth;
        private decimal _extDeviation;
        private int _extBackstep;

        public ZigZag(int extDepth = 60, decimal extDeviation = 5.0M, int extBackstep = 3)
        {
            _extDepth = extDepth;
            _extDeviation = extDeviation;
            _extBackstep = extBackstep;
        }

        public void Refresh(ISymbol symbol, int startBar = 0)
        {
            int shift, back, lasthighpos, lastlowpos;
            decimal val, res;
            decimal curlow, curhigh, lasthigh, lastlow;
            lastlow = 0;
            lasthigh = 0;

            var maxBars = symbol.Candles.Count - _extBackstep;
            _zigZagBufferBuy = new decimal[symbol.Candles.Count];
            _zigZagBufferSell = new decimal[symbol.Candles.Count];
            decimal symbolPoint = symbol.Point;

            for (shift = maxBars - _extDepth; shift >= 0; shift--)
            {
                var candle = symbol.Candles[shift + startBar];
                var lowestCandle = symbol.Lowest(_extDepth, shift + startBar);
                val = lowestCandle.Low;
                if (val == lastlow)
                {
                    val = 0.0M;
                }
                else
                {
                    lastlow = val;
                    if ((candle.Low - val) > (_extDeviation * symbolPoint))
                    {
                        val = 0.0M;
                    }
                    else
                    {
                        for (back = 1; back <= _extBackstep; back++)
                        {
                            res = _zigZagBufferBuy[shift + back];
                            if ((res != 0M) && (res > val)) _zigZagBufferBuy[shift + back] = 0.0M;
                        }
                    }
                }
                _zigZagBufferBuy[shift] = val;

                //--- high
                var highestCandle = symbol.Highest(_extDepth, shift + startBar);
                val = highestCandle.High;
                if (val == lasthigh)
                {
                    val = 0.0M;
                }
                else
                {
                    lasthigh = val;
                    if ((val - candle.High) > (_extDeviation * symbolPoint))
                    {
                        val = 0.0M;
                    }
                    else
                    {
                        for (back = 1; back <= _extBackstep; back++)
                        {
                            res = _zigZagBufferSell[shift + back];
                            if ((res != 0M) && (res < val)) _zigZagBufferSell[shift + back] = 0.0M;
                        }
                    }
                }
                _zigZagBufferSell[shift] = val;
            }

            // final cutting
            lasthigh = -1;
            lasthighpos = -1;
            lastlow = -1;
            lastlowpos = -1;

            for (shift = maxBars - _extDepth; shift >= 0; shift--)
            {
                curlow = _zigZagBufferBuy[shift];
                curhigh = _zigZagBufferSell[shift];
                if ((curlow == 0M) && (curhigh == 0M)) continue;

                if (curhigh != 0M)
                {
                    if (lasthigh > 0M)
                    {
                        if (lasthigh < curhigh) _zigZagBufferSell[lasthighpos] = 0M;
                        else _zigZagBufferSell[shift] = 0M;
                    }
                    if (lasthigh < curhigh || lasthigh < 0M)
                    {
                        lasthigh = curhigh;
                        lasthighpos = shift;
                    }
                    lastlow = -1M;
                }

                if (curlow != 0M)
                {
                    if (lastlow > 0M)
                    {
                        if (lastlow > curlow) _zigZagBufferBuy[lastlowpos] = 0M;
                        else _zigZagBufferBuy[shift] = 0M;
                    }
                    if ((curlow < lastlow) || (lastlow < 0M))
                    {
                        lastlow = curlow;
                        lastlowpos = shift;
                    }
                    lasthigh = -1M;
                }
            }

            for (shift = maxBars - 1; shift >= 0; shift--)
            {
                if (shift >= maxBars - _extDepth)
                {
                    _zigZagBufferBuy[shift] = 0.0M;
                }
                else
                {
                    res = _zigZagBufferSell[shift];
                    if (res != 0.0M) _zigZagBufferSell[shift] = res;
                }
            }
        }

        public ArrowType GetArrow(int bar)
        {
            if (_zigZagBufferBuy == null) return ArrowType.None;
            if (bar < 0 || bar >= _zigZagBufferBuy.Length) return ArrowType.None;
            if (_zigZagBufferBuy[bar] != 0) return ArrowType.Buy;
            if (_zigZagBufferSell[bar] != 0) return ArrowType.Sell;
            return ArrowType.None;
        }
    }
}