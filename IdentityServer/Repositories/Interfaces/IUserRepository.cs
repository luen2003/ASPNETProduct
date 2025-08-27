namespace IdentityServer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAllUsers();
        public Task<User> GetUserById(string id);
        public Task<UserCompanyDto> GetUserByUserId(string userId);

        public Task UpdateUserPassword(string userId, string newPassword);
        //public Task UpdateUserDevice(SignInRequest signInRequest);
        public Task<long> AddNew(UserAddNewParam param);
        public Task<int> ChangePassword(ChangePasswordRequest param, long userId);
        Task<int> MapUserVehicle(MapUserVehicleRequest request);
    }
}
