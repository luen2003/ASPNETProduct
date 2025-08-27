using IdentityServer.Models.Account;

namespace IdentityServer.Domains.Param
{
    public class UserAddNewParam
    {
        public UserAddNewParam()
        {
        }

        public UserAddNewParam(SignUpRequest request)
        {
            UserName = request.Username;
            FullName = request.Name;
            Password = request.Password;
            if (request.Email != null)
            {
                Email = request.Email.ToLower();
            }
            DeviceId = request.DeviceId;
        }
        //public long Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public int PasswordExpireDay { get; set; }
        public long LastPasswordChanged { get; set; }
        public long CompanyId { get; set; }
        public long PosId { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string DeviceId { get; set; }
        public string SysLoginInfo { get; set; }
        public long UserCreate { get; set; }
        public long UserUpdate { get; set; }
        public int Version { get; set; }
        public int SysS { get; set; }

    }
}
