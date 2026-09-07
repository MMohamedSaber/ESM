using ESM.Domain.Enums;
using ESM.Domain.Interfaces;

namespace ESM.Domain.Entities;

public class Invoice : BaseEntity, ISoftDeletable
{
    public Guid? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public InvoiceStatus Status { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
