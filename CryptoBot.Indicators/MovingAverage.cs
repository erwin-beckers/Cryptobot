using System;
using Cryptobot.Interfaces;

namespace CryptoBot.Indicators
{
    public enum AppliedPrice
    {
        Close,
        Open,
        High, 
        Low
    }

    public enum MaMethod
    {
        Sma,
        Ema,
        SSMA,
        LWMA
    }

    // todo : MaMethod is not used atm...

    public static class MovingAverage
    { 
        public static decimal Get(ISymbol symbol, int bar, int period, MaMethod method = MaMethod.Sma, AppliedPrice appliedPrice = AppliedPrice.Close)
        {
            decimal price = 0M;
            for (int i = 0; i < period; ++i)
            {
                switch (appliedPrice)
                {
                    case AppliedPrice.Close:
                        price += symbol.Candles[bar + i].Close;
                        break;

                    case AppliedPrice.Open:
                        price += symbol.Candles[bar + i].Open;
                        break;

                    case AppliedPrice.High:
                        price += symbol.Candles[bar + i].High;
                        break;

                    case AppliedPrice.Low:
                        price += symbol.Candles[bar + i].Low;
                        break;
                } 
            }
            return price / ((decimal)period);
        }

        // todo: total, ma_shift and maMethod are not used atm...
        public static decimal OnArray(decimal[] array, int total, int maPeriod, int ma_shift, MaMethod maMethod, int shift)
        {
            decimal price = 0M;
            for (int i = 0; i < maPeriod; ++i)
            {
                price += array[shift + i];
            }
            return price / ((decimal)maPeriod);
        }
    }
}
