using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Payment : BaseEntity, ISoftDeletable
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public PaymentMethod Method { get; set; }
    public PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
