using Marquei.Domain.Common;

namespace Marquei.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }
    private PhoneNumber(string value)
    {
        Value = value;
    }
    public PhoneNumber Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new DomainException("O Telefone é obrigatório.");
        }

        var digits = new string(input.Where(char.IsDigit).ToArray());

        if (digits.StartsWith("55") && digits.Length > 11)
            digits = digits[2..];

        if (digits.Length is not (10 or 11))
            throw new DomainException("Telefone inválido.");

        return new PhoneNumber("+55" + digits); 
    }
}