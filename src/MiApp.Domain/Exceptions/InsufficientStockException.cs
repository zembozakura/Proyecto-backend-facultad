namespace MiApp.Domain.Exceptions;

public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(int requested, int available)
        : base($"Cannot reserve {requested} units — only {available} available") { }
}
