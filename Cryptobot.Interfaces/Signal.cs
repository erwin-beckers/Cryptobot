using System.Collections.Generic;
using System.Linq;

namespace Cryptobot.Interfaces
{
    public enum SignalType
    {
        None,
        Buy,
        Sell
    }

    public class Indicator
    {
        public Indicator(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public bool IsValid { get; set; }
    }

    public class Signal
    {
        public ISymbol Symbol { get; set; }
        public SignalType Type { get; set; }
        public List<Indicator> Indicators { get; set; }

        public int Count
        {
            get
            {
                return Indicators.Count(e => e.IsValid);
            }
        }

        public Signal()
        {
            Indicators = new List<Indicator>();
        }

        public void Reset(ISymbol symbol)
        {
            Type = SignalType.None;
            foreach (var indicator in Indicators)
            {
                indicator.IsValid = false;
            }
        }
    }
}