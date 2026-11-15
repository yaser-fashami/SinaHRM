namespace SinaHRM.Domain.ValueObjects;

public enum MoneyInputCurrency
{
    /// <summary>
    /// Rial
    /// </summary>
    IRR,
    /// <summary>
    /// Toman
    /// </summary>
    IRT
}


public sealed record Money
{
    public decimal AmountInRials { get; }

    private Money(decimal amountInRials)
    {
        AmountInRials = amountInRials;
    }

    public static Money Create(decimal amount, MoneyInputCurrency currency = MoneyInputCurrency.IRR)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Money amount cannot be negative.");
        }

        var wholeAmount = decimal.Floor(amount);

        var amountInRials = currency switch
        {
            MoneyInputCurrency.IRR => wholeAmount,
            MoneyInputCurrency.IRT => wholeAmount * 10m,
            _ => throw new ArgumentOutOfRangeException(
                nameof(currency))
        };

        return new Money(amountInRials);
    }

    public static Money Zero() => new(0m);

    public Money Add(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return new Money(AmountInRials + other.AmountInRials);
    }

    public Money Subtract(Money other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (other.AmountInRials > AmountInRials)
        {
            throw new InvalidOperationException(
                "Money amount cannot become negative.");
        }

        return new Money(
            AmountInRials - other.AmountInRials);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(factor),
                "Factor cannot be negative.");
        }

        return new Money(
            decimal.Floor(AmountInRials * factor));
    }
}
