namespace Cryptobot.Interfaces
{
    public interface IStrategyFactory
    {
        IStrategy Create(ISymbol symbol);
    }
}