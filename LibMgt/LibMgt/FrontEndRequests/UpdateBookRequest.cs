namespace LibMgt.FrontEndRequests
{
    public record UpdateTransactionRequest
  (     
         Guid Id,
         string? Title,
         bool? IsDeleted,
         string? Author,
         string? ISBN,
         string? Genre,
         DateTime? PublicationDate,
         string? AvailabilityStatus,
         string? OtherDetails);
}
