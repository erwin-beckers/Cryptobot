using Cryptobot.Interfaces;

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