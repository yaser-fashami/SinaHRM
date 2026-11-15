using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SinaHRM.Domain.ValueObjects;

namespace SinaHRM.Infrastracture.Data.Sql.Converters;

public sealed class MoneyRialConverter : ValueConverter<Money, decimal>
{
    public MoneyRialConverter()
        : base(
            money => money.AmountInRials,
            amount => Money.Create(
                amount,
                MoneyInputCurrency.IRR))
    {
    }
}
