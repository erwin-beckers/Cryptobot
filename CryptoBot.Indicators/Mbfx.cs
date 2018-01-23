using System;
using Cryptobot.Interfaces;

namespace CryptoBot.Indicators
{
    public class Mbfx
    {
        private decimal[] _mbfxYellow;
        private decimal[] _mbfxGreen;
        private decimal[] _mbfxRed;
        int _len;
        decimal _filter;

        public Mbfx(int len = 7, decimal filter = 0.0M)
        {
            _len = len;
            _filter = filter;
        }

        public bool IsGreen(int bar)
        {
            if (bar <0 || bar >= _mbfxGreen.Length) return false;
            return (_mbfxGreen[bar] != decimal.MaxValue);
        }
        
        public decimal GreenValue(int bar)
        {
          if (bar <0 || bar >= _mbfxGreen.Length) return decimal.MaxValue;
          return _mbfxGreen[bar];
        } 
       
        public decimal RedValue(int bar)
        {
           if (bar <0 || bar >= _mbfxGreen.Length) return decimal.MaxValue;
           return _mbfxRed[bar];
        }
       
        public void Refresh(ISymbol symbol)
        {
            _mbfxYellow = new decimal[symbol.Candles.Count];
            _mbfxRed = new decimal[symbol.Candles.Count];
            _mbfxGreen = new decimal[symbol.Candles.Count];
            decimal ld_0 = 0M;
            decimal ld_8 = 0M;
            decimal ld_16 = 0M;
            decimal ld_24 = 0M;
            decimal ld_32 = 0M;
            decimal ld_40 = 0M;
            decimal ld_48 = 0M;
            decimal ld_56 = 0M;
            decimal ld_64 = 0M;
            decimal ld_72 = 0M;
            decimal ld_80 = 0M;
            decimal ld_88 = 0M;
            decimal ld_96 = 0M;
            decimal ld_104 = 0M;
            decimal ld_112 = 0M;
            decimal ld_120 = 0M;
            decimal ld_128 = 0M;
            decimal ld_136 = 0M;
            decimal ld_144 = 0M;
            decimal ld_152 = 0M;
            decimal ld_160 = 0M;
            decimal ld_168 = 0M;
            decimal ld_176 = 0M;
            decimal ld_184 = 0M;
            decimal ld_192 = 0M;
            decimal ld_200 = 0M;
            decimal ld_208 = 0M;

            int barLimit = (symbol.Candles.Count-1) - _len - 1;
            for (int bar = barLimit; bar >= 0; bar--)
            {
                var candle = symbol.Candles[bar];
                if (ld_8 == 0.0M)
                {
                    ld_8 = 1.0M;
                    ld_16 = 0.0M;
                    if (_len - 1M >= 5M) ld_0 = _len - 1.0M;
                    else ld_0 = 5.0M;
                    ld_80 = 100.0M * ( (candle.High + candle.Low + candle.Close) / 3.0M);
                    ld_96 = 3.0M / (_len + 2.0M);
                    ld_104 = 1.0M - ld_96;
                }
                else
                {
                    if (ld_0 <= ld_8) ld_8 = ld_0 + 1.0M;
                    else ld_8 += 1.0M;
                    ld_88 = ld_80;
                    ld_80 = 100.0M * ( (candle.High + candle.Low + candle.Close) / 3.0M);
                    ld_32 = ld_80 - ld_88;
                    ld_112 = ld_104 * ld_112 + ld_96 * ld_32;
                    ld_120 = ld_96 * ld_112 + ld_104 * ld_120;
                    ld_40 = 1.5M * ld_112 - ld_120 / 2.0M;
                    ld_128 = ld_104 * ld_128 + ld_96 * ld_40;
                    ld_208 = ld_96 * ld_128 + ld_104 * ld_208;
                    ld_48 = 1.5M * ld_128 - ld_208 / 2.0M;
                    ld_136 = ld_104 * ld_136 + ld_96 * ld_48;
                    ld_152 = ld_96 * ld_136 + ld_104 * ld_152;
                    ld_56 = 1.5M * ld_136 - ld_152 / 2.0M;
                    ld_160 = ld_104 * ld_160 + ld_96 * Math.Abs(ld_32);
                    ld_168 = ld_96 * ld_160 + ld_104 * ld_168;
                    ld_64 = 1.5M * ld_160 - ld_168 / 2.0M;
                    ld_176 = ld_104 * ld_176 + ld_96 * ld_64;
                    ld_184 = ld_96 * ld_176 + ld_104 * ld_184;
                    ld_144 = 1.5M * ld_176 - ld_184 / 2.0M;
                    ld_192 = ld_104 * ld_192 + ld_96 * ld_144;
                    ld_200 = ld_96 * ld_192 + ld_104 * ld_200;
                    ld_72 = 1.5M * ld_192 - ld_200 / 2.0M;
                    if (ld_0 >= ld_8 && ld_80 != ld_88) ld_16 = 1.0M;
                    if (ld_0 == ld_8 && ld_16 == 0.0M) ld_8 = 0.0M;
                }
                if (ld_0 < ld_8 && ld_72 > 0.0000000001M)
                {
                    ld_24 = 50.0M * (ld_56 / ld_72 + 1.0M);
                    if (ld_24 > 100.0M) ld_24 = 100.0M;
                    if (ld_24 < 0.0M) ld_24 = 0.0M;
                }
                else ld_24 = 50.0M;

                _mbfxYellow[bar] = ld_24;
                _mbfxGreen[bar] = ld_24;
                _mbfxRed[bar] = ld_24;
                if (_mbfxYellow[bar] > _mbfxYellow[bar + 1] - _filter) _mbfxRed[bar] = decimal.MaxValue;
                else
                {
                    if (_mbfxYellow[bar] < _mbfxYellow[bar + 1] + _filter) _mbfxGreen[bar] = decimal.MaxValue;
                    else
                    {
                        if (_mbfxYellow[bar] == _mbfxYellow[bar + 1] + _filter)
                        {
                            _mbfxGreen[bar] = decimal.MaxValue;
                            _mbfxRed[bar] = decimal.MaxValue;
                        }
                    }
                }
            }
        }
    }
}
