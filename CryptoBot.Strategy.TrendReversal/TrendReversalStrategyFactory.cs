using Cryptobot.Interfaces;

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