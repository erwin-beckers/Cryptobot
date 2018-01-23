using Cryptobot.Interfaces;
using System;

namespace CryptoBotApp.bot
{
    public class Candle : ICandle
    {
        public decimal Open { get; set; }

        public decimal Close { get; set; }

        public decimal High { get; set; }

        public decimal Low { get; set; }

        public DateTime OpenTime { get; set; }

        public DateTime CloseTime { get; set; }

        public decimal Range => Math.Abs(High - Low);

        public decimal Body => Math.Abs(Open - Close);

        public decimal UpperWick => High - Math.Max(Open, Close);

        public decimal LowerWick => Math.Min(Open, Close) - Low;

        public bool IsRed => Open > Close;

        public bool IsGreen => Close > Open;
    }
}