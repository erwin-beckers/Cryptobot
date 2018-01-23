using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptobot.Interfaces
{
    public interface IStrategy
    {
        ISymbol Symbol { get; }
        void Process();
    }
}
