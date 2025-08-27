namespace IdentityServer.Domains.Dto
{
    public class SMSPINRequest
    {
        public string? requestId { get; set; }
        public string? requestDate { get; set; }
        public string? phoneNumber { get; set; }
        public string? plxId { get; set; }
        public string? pin { get; set; }
        public string? checkSum { get; set; }
    }
}
