using System.Text.RegularExpressions;
using Marquei.Domain.Common;

namespace Marquei.Domain.Tenants;

public class Tenant : Entity
{
    private static readonly string[] ReservedSlugs =
        { "admin", "api", "app", "login", "www" };

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public decimal DepositPercent { get; private set; }
    public int PaymentWindowMinutes { get; private set; }
    public bool IsActive { get; private set; }

    private Tenant()
    {
    }

    public static Tenant Create(string name, string slug)
    {
        return new Tenant
        {
            Name = ValidateName(name),
            Slug = ValidateSlug(slug),
            DepositPercent = 50,
            PaymentWindowMinutes = 15,
            IsActive = true
        };
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangeSlug(string slug)
    {
        Slug = ValidateSlug(slug);
    }

    public void ChangeDepositPercent(decimal percent)
    {
        if (percent is not (50 or 100))
            throw new DomainException("O sinal deve ser de 50% ou 100%.");

        DepositPercent = percent;
    }

    public void ChangePaymentWindow(int minutes)
    {
        if (minutes is < 5 or > 60)
            throw new DomainException("O prazo do Pix deve ficar entre 5 e 60 minutos.");

        PaymentWindowMinutes = minutes;
    }

    public void Suspend()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do negócio é obrigatório.");

        var trimmed = name.Trim();

        if (trimmed.Length > 100)
            throw new DomainException("Nome do negócio muito longo.");

        return trimmed;
    }

    private static string ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("O link é obrigatório.");

        var normalized = slug.Trim().ToLowerInvariant();

        if (normalized.Length is < 3 or > 40)
            throw new DomainException("O link deve ter entre 3 e 40 caracteres.");

        if (!Regex.IsMatch(normalized, "^[a-z0-9]+(-[a-z0-9]+)*$"))
            throw new DomainException("O link só pode ter letras minúsculas, números e hífen.");

        if (ReservedSlugs.Contains(normalized))
            throw new DomainException("Este link não está disponível.");

        return normalized;
    }
}