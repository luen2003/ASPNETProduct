namespace IdentityServer.Domains.Entities
{
    public class UserPin
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Pin { get; set; }
        public string? PlxId { get; set; }
        public int? State { get; set; }
        public long? LockDate { get; set; }
        public long? OtpPhone { get; set; }
    }
}
