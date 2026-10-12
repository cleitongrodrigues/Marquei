using Marquei.Domain.Common;
using Marquei.Domain.ValueObjects;

namespace Marquei.Domain.Services;

public class Service : Entity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public TimeSpan Duration { get; private set; }
    public Money Price { get; private set; } = null!;
    public bool IsPriceFrom { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsArchived { get; private set; }

    private Service()
    {
    }

    public static Service Create(
        Guid tenantId,
        string name,
        string? category,
        TimeSpan duration,
        Money price,
        bool isPriceFrom,
        int displayOrder = 0)
    {
        return new Service
        {
            TenantId = tenantId,
            Name = ValidateName(name),
            Category = NormalizeCategory(category),
            Duration = ValidateDuration(duration),
            Price = price,
            IsPriceFrom = isPriceFrom,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Update(
        string name,
        string? category,
        TimeSpan duration,
        Money price,
        bool isPriceFrom)
    {
        EnsureNotArchived();

        Name = ValidateName(name);
        Category = NormalizeCategory(category);
        Duration = ValidateDuration(duration);
        Price = price;
        IsPriceFrom = isPriceFrom;
    }

    public void Activate()
    {
        EnsureNotArchived();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Archive()
    {
        IsArchived = true;
        IsActive = false;
    }

    private void EnsureNotArchived()
    {
        if (IsArchived)
            throw new DomainException("Serviço arquivado não pode ser alterado.");
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do serviço é obrigatório.");

        var trimmed = name.Trim();

        if (trimmed.Length > 100)
            throw new DomainException("Nome do serviço muito longo.");

        return trimmed;
    }

    private static string? NormalizeCategory(string? category)
    {
        return string.IsNullOrWhiteSpace(category) ? null : category.Trim();
    }

    private static TimeSpan ValidateDuration(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            throw new DomainException("A duração precisa ser maior que zero.");

        return duration;
    }
}