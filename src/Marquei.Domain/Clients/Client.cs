using Marquei.Domain.Common;
using Marquei.Domain.ValueObjects;

namespace Marquei.Domain.Clients;

public class Client : Entity
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public PhoneNumber Phone { get; private set; } = null!;
    public bool AcceptsWhatsApp { get; private set; }
    public string? Notes { get; private set; }

    private Client()
    {
    }

    public static Client Create(
        Guid tenantId,
        string name,
        PhoneNumber phone,
        bool acceptsWhatsApp)
    {
        return new Client
        {
            TenantId = tenantId,
            Name = ValidateName(name),
            Phone = phone,
            AcceptsWhatsApp = acceptsWhatsApp
        };
    }

    public void Rename(string name)
    {
        Name = ValidateName(name);
    }

    public void ChangePhone(PhoneNumber phone)
    {
        Phone = phone;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome é obrigatório.");

        var trimmed = name.Trim();

        if (trimmed.Length > 100)
            throw new DomainException("Nome muito longo.");

        return trimmed;
    }
}