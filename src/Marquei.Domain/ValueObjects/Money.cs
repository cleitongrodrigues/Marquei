using Marquei.Domain.Common;

namespace Marquei.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public static Money Zero => new(0m);

    public static Money Create(decimal amount)
    {
        if (amount < 0)
            throw new DomainException("O valor não pode ser negativo.");

        return new Money(Math.Round(amount, 2, MidpointRounding.AwayFromZero));
    }

    public Money Add(Money other) => Create(Amount + other.Amount);

    public Money Percentage(decimal percent)
    {
        if (percent is < 0 or > 100)
            throw new DomainException("Percentual inválido.");

        return Create(Amount * percent / 100);
    }
    public Money Subtract(Money other)
    {
        if (other.Amount > Amount)
            throw new DomainException("O valor a subtrair é maior que o valor total.");

        return Create(Amount - other.Amount);
    }
}