using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptobot.Interfaces
{
    public interface ICandle
    {
        decimal Open { get; }
        decimal Close { get; }

        decimal High { get; }
        decimal Low { get; }
        DateTime OpenTime { get; }
        DateTime CloseTime { get; }
        
        decimal UpperWick { get; }
        decimal LowerWick { get; }

        decimal Range { get; }
        decimal Body { get; }

        bool IsRed { get; }
        bool IsGreen { get; }
    }
}
