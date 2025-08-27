using ServiceAPI.Repositories.Interfaces;

namespace ServiceAPI.Authentication
{
    //using Microsoft.AspNetCore.Identity;
    public class ApplicationUser //: IdentityUser
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string ServiceCode { get; set; }
        public string Token { get; set; }
        private readonly ILogger _logger;


        public ApplicationUser(ILogger<ApplicationUser> logger)
        {
            _logger = logger;
        }
        /*public async Task<UserCompanyDto> GetUserInfo()
        {
            UserCompanyDto result = new UserCompanyDto();
            try
            {
                if (UserName != null)
                {
                    result = await _userRepo.GetUserByUsername(UserName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
                _logger.LogDebug(ex.StackTrace);
            }
            return result;
        }*/

    }
}
