namespace LibMgt.FrontEndRequests
{
    public record TransactionCreateRequest
     (
     
         string Title ,
         string Author ,
         string ISBN ,
         string Genre ,
         DateTime PublicationDate ,
         string AvailabilityStatus ,
         string OtherDetails 
    );
}
