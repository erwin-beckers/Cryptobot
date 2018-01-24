using Cryptobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoBot.Strategy.TrendReversal
{
    public class TrendReversalStrategyFactory : IStrategyFactory
    {
        public IStrategy Create(ISymbol symbol)
        {
            return new TrendReversalStrategy(symbol);
        }
    }
}
