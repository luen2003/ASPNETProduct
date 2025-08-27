namespace IdentityServer.Domains.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Password { get; set; }
        public string TaxCode { get; set; }
        public int Estatus { get; set; }
        public int UserType { get; set; }
        public int Status { get; set; }
        public int PasswordExpireDay { get; set; }
        public long LastPasswordChanged { get; set; }
        public long CompanyId { get; set; }
        public long PosId { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ChatId { get; set; }
        public string SysLoginInfo { get; set; }
        public long SysU { get; set; }
        public long SysD { get; set; }
        public int SysV { get; set; }
        public int SysS { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExp { get; set; }
        public string CompanyName { get; set; }
    }
}
