namespace IdentityServer.Domains.Dto
{
    public class UserCompanyDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public int PasswordExpireDay { get; set; }
        public long LastPasswordChanged { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public long PosId { get; set; }
        public string Birthday { get; set; }
        public int Gender { get; set; }
        public string Avatar { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string TaxCode { get; set; }
        public string DeviceId { get; set; }
        public string TokenDevice { get; set; }
        public string UserAgent { get; set; }
        public string SysLoginInfo { get; set; }
        public long UserCreate { get; set; }
        public long UserUpdate { get; set; }
        public int Version { get; set; }
        public int SysS { get; set; }
        public string Vehicle { get; set; }
        public long Balance { get; set; }
        public string MapToCompany { get; set; }
        public string MapToBoss { get; set; }
    }
}
