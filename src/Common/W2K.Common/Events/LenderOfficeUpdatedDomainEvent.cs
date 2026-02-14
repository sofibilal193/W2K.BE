namespace W2K.Common.Events;

public record LenderOfficeUpdatedDomainEvent(int OfficeId, bool IsApproved) : DomainEvent;

