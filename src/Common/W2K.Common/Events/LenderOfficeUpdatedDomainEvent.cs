namespace DFI.Common.Events;

public record LenderOfficeUpdatedDomainEvent(int OfficeId, bool IsApproved) : DomainEvent;

