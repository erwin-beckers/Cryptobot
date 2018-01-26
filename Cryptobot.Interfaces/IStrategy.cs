namespace Cryptobot.Interfaces
{
    public interface IStrategy
    {
        ISymbol Symbol { get; }

        Signal Process();

        Signal Signal { get; }
    }
}