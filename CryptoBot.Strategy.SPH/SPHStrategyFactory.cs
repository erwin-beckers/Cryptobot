using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.Strategy.SPH
{
    public class SPHStrategyFactory : IStrategyFactory
    {
        public IStrategy Create(ISymbol symbol)
        {
            return new SPHStrategy(symbol);
        }
    }
}
